using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebAPIServerClient.API.Infra
{
    public class MyOAuthServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var user = "email@test.com";
            var pass = "123@qwe";

            if (context.UserName != user || context.Password != pass)
            {
                context.SetError("invalid_grant", "Usuário ou senha inválidos");
                return;
            }


            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Email, user));

            var principal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = principal;

            context.Validated(identity);
        }
    }
}