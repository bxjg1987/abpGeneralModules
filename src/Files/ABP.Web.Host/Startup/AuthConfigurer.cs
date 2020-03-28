using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Abp.Runtime.Security;
using BXJG.WeChat.MiniProgram;
using Microsoft.AspNetCore.Authentication;
using ZLJ.Authentication.WeChatMiniProgram;

namespace ZLJ.Web.Host.Startup
{
    public static class AuthConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var authBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            });

            if (bool.Parse(configuration["Authentication:JwtBearer:IsEnabled"]))
            {
                authBuilder.AddJwtBearer("JwtBearer", options =>
                {
                    options.Audience = configuration["Authentication:JwtBearer:Audience"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"])),

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = configuration["Authentication:JwtBearer:Audience"],

                        // Validate the token expiry
                        ValidateLifetime = true,

                        // If you want to allow a certain amount of clock drift, set that here
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = QueryStringTokenResolver
                    };
                });
            }

            if (bool.Parse(configuration["Authentication:WeChartMiniProgram:IsEnabled"]))
            {
                authBuilder.AddWeChartMiniProgram<WeChatMiniProgramLoginHandler>(opt =>
                {
                    opt.AppId = configuration["Authentication:WeChartMiniProgram:AppId"];
                    opt.Secret = configuration["Authentication:WeChartMiniProgram:Secret"];

                    //登录时不考虑前端传，而是在controller中单独提供一个api 来更新用户信息，包括手机号的处理

                    //opt.ClaimActions.MapJsonKey("nickName", "nickName");
                    //opt.ClaimActions.MapJsonKey("avatarUrl", "avatarUrl");
                    //opt.ClaimActions.MapJsonKey("gender", "gender");
                    //opt.ClaimActions.MapJsonKey("country", "country");
                    //opt.ClaimActions.MapJsonKey("province", "province");
                    //opt.ClaimActions.MapJsonKey("city", "city");
                    //opt.ClaimActions.MapJsonKey("language", "language");
                });
            }
        }

        /* This method is needed to authorize SignalR javascript client.
         * SignalR can not send authorization header. So, we are getting it from query string as an encrypted text. */
        private static Task QueryStringTokenResolver(MessageReceivedContext context)
        {
            if (!context.HttpContext.Request.Path.HasValue ||
                !context.HttpContext.Request.Path.Value.StartsWith("/signalr"))
            {
                // We are just looking for signalr clients
                return Task.CompletedTask;
            }

            var qsAuthToken = context.HttpContext.Request.Query["enc_auth_token"].FirstOrDefault();
            if (qsAuthToken == null)
            {
                // Cookie value does not matches to querystring value
                return Task.CompletedTask;
            }

            // Set auth token from cookie
            context.Token = SimpleStringCipher.Instance.Decrypt(qsAuthToken, AppConsts.DefaultPassPhrase);
            return Task.CompletedTask;
        }
    }
}
