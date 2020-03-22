using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace BXJG.WeChart.MiniProgram
{
    //可以参考TwitterHandler和OAuthHandler的设计

    public class MiniProgramAuthenticationHandler : RemoteAuthenticationHandler<MiniProgramAuthenticationOptions>
    {
        private HttpClient Backchannel => Options.Backchannel;
        public MiniProgramAuthenticationHandler(IOptionsMonitor<MiniProgramAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            //Request.BodyReader
            //Utf8JsonReader sdf = new Utf8JsonReader();

            var input = await new StreamReader(Request.Body).ReadToEndAsync();
            var jd = JsonDocument.Parse(input);

            var requestUrl = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string>
            {
                { "appid", Options.AppId },
                { "secret", Options.Secret },
                { "js_code",jd.RootElement.GetString("code") },
                { "grant_type", "authorization_code" },
            });

            var response = await Backchannel.GetAsync(requestUrl, Context.RequestAborted);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"An error occurred when retrieving wechar mini program user information ({response.StatusCode}). Please check if the authentication information is correct and the corresponding Microsoft Account API is enabled.");

            //正式处理
            var rt = JsonSerializer.Deserialize<MiniProgramToken>(await response.Content.ReadAsStringAsync());
            
            //调试
            //var rt = new MiniProgramToken
            //{
            //    errcode = 0,
            //    errmsg = "",
            //    openid = "1111111",
            //    session_key = "222",
            //    unionid = ""
            //};

            if (rt.errcode != 0)
                throw new HttpRequestException($"errcode:{rt.errcode}, errmsg:{rt.errmsg}");


            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, rt.openid),
                new Claim(nameof(rt.session_key), rt.session_key),
                new Claim(nameof(rt.unionid), rt.unionid)
            }, ClaimsIssuer);

            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = base.Clock.UtcNow.AddMinutes(5),
                IsPersistent = true,
                IssuedUtc = Clock.UtcNow
            };

            var ticket = await CreateTicketAsync(identity, authenticationProperties, rt, jd.RootElement);

            return HandleRequestResult.Success(ticket);
        }
        protected virtual async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, MiniProgramToken miniProgramToken, JsonElement user)
        {
            var context = new MiniProgramCreatingTicketContext(Context, Scheme, Options, new ClaimsPrincipal(identity), properties, miniProgramToken.openid, miniProgramToken.session_key, miniProgramToken.unionid, user);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        protected new MiniProgramAuthenticationEvent Events
        {
            get { return (MiniProgramAuthenticationEvent)base.Events; }
            set { base.Events = value; }
        }
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new MiniProgramAuthenticationEvent());

        //此逻辑 父类已实现
        //protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        //{
        //    var result = await Context.AuthenticateAsync(SignInScheme);
        //    return await base.HandleAuthenticateAsync();
        //}
    }
}
