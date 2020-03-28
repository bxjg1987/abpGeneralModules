using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using BXJG.WeChat.MiniProgram;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZLJ.Authentication.External;
using ZLJ.Authentication.JwtBearer;
using ZLJ.Authorization;
using ZLJ.Authorization.Users;
using ZLJ.Models.TokenAuth;

namespace ZLJ.Authentication.WeChatMiniProgram
{
    public class WeChatMiniProgramLoginHandler : IWeChatMiniProgramLoginHandler
    {
        private readonly IAbpSession AbpSession;
        private readonly ITenantCache _tenantCache;
        private readonly LogInManager _logInManager;
        private readonly UserManager userManager;
        private readonly TokenAuthConfiguration _configuration;
        private readonly UserRegistrationManager _userRegistrationManager;
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        private HttpContext httpContext;
        private HttpResponse httpResponse;
        string tenancyName=null;
        public WeChatMiniProgramLoginHandler(IAbpSession abpSession,
            LogInManager logInManager,
            ITenantCache tenantCache,
            UserRegistrationManager userRegistrationManager,
            TokenAuthConfiguration configuration,
            UserManager userManager)
        {
            _logInManager = logInManager;
            AbpSession = abpSession;
            _tenantCache = tenantCache;
            this.userManager = userManager;
            _configuration = configuration;
            _userRegistrationManager = userRegistrationManager;
        }

        [UnitOfWork]
        public async Task<bool> ExcuteAsync(WeChatMiniProgramLoginContext ct)
        {
            
            this.httpContext = ct.HttpContext;
            this.httpResponse = httpContext.Response;
            if (AbpSession.TenantId.HasValue)
                this.tenancyName = _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;

            //尝试做第三发登录（内部通过openid找到本地账号做登录），
            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(MiniProgramConsts.AuthenticationScheme, ct.WeChatMiniProgramUser.openid, MiniProgramConsts.AuthenticationSchemeDisplayName), tenancyName);
            //根据登录结果，若成功则直接返回jwtToken 或者自动注册后返回
            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        //var tttt = ct.WeChatMiniProgramUser.Input.EnumerateArray();
                        ////更新微信用户信息
                        //foreach (var item in tttt)
                        //{
                        //    await userManager.ReplaceClaimAsync(loginResult.User, new Claim(item., ""), item);
                        //}

                        //返回jwtToken
                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

                      
                        await WriteJsonAsync(new
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        });
                        return true;
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        //若未找到关联的本地账号则自动注册，再返回jwtToken
                        var newUser = await RegisterExternalUserAsync(new ExternalAuthUserInfo
                        {
                            Provider = MiniProgramConsts.AuthenticationScheme,
                            ProviderKey = ct.WeChatMiniProgramUser.openid,
                            Name = Guid.NewGuid().ToString("N"),
                            EmailAddress = Guid.NewGuid().ToString("N") + "@mp.com",
                            Surname = "a"
                        });
                        //if (!newUser.IsActive)
                        //{
                        //    return new ExternalAuthenticateResultModel
                        //    {
                        //        WaitingForActivation = true
                        //    };
                        //}

                        // Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(MiniProgramConsts.AuthenticationScheme, ct.WeChatMiniProgramUser. openid, MiniProgramConsts.AuthenticationSchemeDisplayName), tenancyName);
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            //throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            //    loginResult.Result,
                            //    openid,
                            //    tenancyName
                            //);
                            await WriteJsonAsync(new { msg = "注册失败" });

                        }
                        //保存微信用户信息（排出openid，因为它存储在userlogins里）
                        // await userManager.AddClaimsAsync(loginResult.User, t.Principal.Claims.Where(c => c.Type != ClaimTypes.NameIdentifier));

                        else
                        {
                            await WriteJsonAsync(new
                            {
                                AccessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
                                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                            });

                        }
                        return true;
                    }
                default:
                    {
                        await WriteJsonAsync(new { msg = "登录失败！" });
                        //throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        //    loginResult.Result,
                        //    openid,
                        //    tenancyName
                        //);

                    }
                    return true;
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }
        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                Authorization.Users.User.CreateRandomPassword(),
                true
            );

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalUser.Provider,
                    ProviderKey = externalUser.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            await UnitOfWorkManager.Current.SaveChangesAsync();

            return user;
        }

        public Task WriteJsonAsync(object o)
        {
            return this.httpResponse.WriteAsync(JsonExtensions.SerializeToJson(o), this.httpContext.RequestAborted);
        }
    }
}
