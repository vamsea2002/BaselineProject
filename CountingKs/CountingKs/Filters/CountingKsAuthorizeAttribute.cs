using CountingKs.Data;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using CountingKs.Data.Entities;
using Ninject;
using WebMatrix.WebData;

namespace CountingKs.Filters
{
    public class CountingKsAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private static bool _perUser;

        

        public CountingKsAuthorizeAttribute(bool perUser = true)
        {
            _perUser = perUser;
        }
        [Inject]
        public CountingKsRepository TheRepository { get; set; }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if(ReadApiKeyAndTokenFromContext(actionContext))
                return;
            HandleUnauthorized(actionContext);
        }

        private bool ReadApiKeyAndTokenFromContext(HttpActionContext actionContext)
        {
            const string APIKEYNAME = "apikey";
            const string TOKENNAME = "token";

            //parsing is removing + from string need to address the issue
            var query = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
            if(!string.IsNullOrWhiteSpace(query[APIKEYNAME]) && !string.IsNullOrWhiteSpace(TOKENNAME))
            {
                var apikey = query[APIKEYNAME];
                var token = "mvMydqj5QqzdzN+1tdjtkZdPCAj9SUA5z46MpXvCo9U=";  //query[TOKENNAME]; 
                var authToken = TheRepository.GetAuthToken(token);
                if(IsAuthenticatedCheck(actionContext, authToken, apikey))
                    return true;
            }
            return false;
        }

        private bool IsAuthenticatedCheck(HttpActionContext actionContext, AuthToken authToken, string apikey)
        {
            if(authToken != null && authToken.ApiUser.AppId == apikey && authToken.Expiration > DateTime.UtcNow)
            {
                if(_perUser)
                {
                    if(Authenticate(actionContext))
                        return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Authenticate(HttpActionContext actionContext)
        {
            if(Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                return true;
            }
            var authHeader = actionContext.Request.Headers.Authorization;
            if(authHeader != null)
            {
                if(authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                   !string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    if(GetCredentialsFromHeader(authHeader))
                        return true;
                }
            }
            return false;
        }

        private static bool GetCredentialsFromHeader(AuthenticationHeaderValue authHeader)
        {
            var rawCredentials = authHeader.Parameter;
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var credentilas = encoding.GetString(Convert.FromBase64String(rawCredentials));
            var split = credentilas.Split(':');
            var username = split[0];
            var password = split[1];
            if(InitilizeWebSecurity(username, password))
                return true;
            return false;
        }

        private static bool InitilizeWebSecurity(string username, string password)
        {
            if(!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection",
                                                         "UserProfile",
                                                         "UserId",
                                                         "UserName",
                                                         autoCreateTables: true);
            }
            if(!WebSecurity.Login(username, password))
            {
                return false;
            }
            var principal = new GenericPrincipal(new GenericIdentity(username), null);
            Thread.CurrentPrincipal = principal;
            return true;
        }

        private static void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            if(_perUser)
            {
                actionContext.Response.Headers.Add("WWW-Authenticate",
                                                   "Basic Scheme='CountingKs' location='http://localhost:8901/account/login'");
            }
        }
    }
}