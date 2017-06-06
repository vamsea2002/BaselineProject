using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CountingKs.Filters
{

    public class RequireHttpsAttribute:AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            if(request.RequestUri.Scheme!=Uri.UriSchemeHttps)
            {
                const string html = "<p>Http is Required</p>";

                var controllerFilters = actionContext.ControllerContext.ControllerDescriptor.GetFilters();
                var actionFilters = actionContext.ActionDescriptor.GetFilters();


                if(( controllerFilters != null &&
                     controllerFilters.Select(t => t.GetType() == typeof(RequireHttpsAttribute)).Any() ) ||
                   ( actionFilters != null &&
                     actionFilters.Select(t => t.GetType() == typeof(RequireHttpsAttribute)).Any() ))
                {
                    actionContext.Response = request.CreateResponse(HttpStatusCode.Found);
                    actionContext.Response.Content = new StringContent(html, Encoding.UTF8, "text/html");
                    var uriBulder = new UriBuilder(request.RequestUri);
                    uriBulder.Scheme = Uri.UriSchemeHttps;
                    uriBulder.Port = 443;

                    actionContext.Response.Headers.Location = uriBulder.Uri;
                }


                //if (request.Method.Method == "GET")
                //{
                //    actionContext.Response = request.CreateResponse(HttpStatusCode.Found);
                //    actionContext.Response.Content = new StringContent(html, Encoding.UTF8,"text/html");
                //    var uriBulder = new UriBuilder(request.RequestUri);
                //    uriBulder.Scheme = Uri.UriSchemeHttps;
                //    uriBulder.Port = 443;

                //    actionContext.Response.Headers.Location = uriBulder.Uri;
                //}
                else
                {
                    actionContext.Response = request.CreateResponse(HttpStatusCode.NotFound);
                    actionContext.Response.Content = new StringContent(html,Encoding.UTF8);

                }
            }
        }
    }
}