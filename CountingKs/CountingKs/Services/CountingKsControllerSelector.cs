using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CountingKs.Services
{
    public class CountingKsControllerSelector : DefaultHttpControllerSelector
    {
        private HttpConfiguration _configuration;

        public CountingKsControllerSelector(HttpConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllers = GetControllerMapping();
            var routeData = request.GetRouteData();
            var contollerName = (string)routeData.Values["controller"];
            HttpControllerDescriptor descriptor;
            if (controllers.TryGetValue(contollerName, out descriptor))
            {
                var version = GetVersionFromMediaType(request);
                    //GetVersionFromAcceptHeaderVersion(request);
                    //GetVersionFromHeader(request);
                    //GetVersionFromQueryString(request);
                var newName = string.Concat(contollerName, "V", version);
                HttpControllerDescriptor versionDescriptor;
                if (controllers.TryGetValue(newName, out versionDescriptor))
                {
                    return versionDescriptor;
                }
            }
            return descriptor;
        }

        private static string GetVersionFromMediaType(HttpRequestMessage request)
        {
            var accept = request.Headers.Accept;
            var ex = new Regex(@"application\/vnd\.countingks\.([a-z]+)\.v([0-9]+)\+json", RegexOptions.IgnoreCase);

            foreach(var mime in accept)
            {
                var match = ex.Match(mime.MediaType);
                if(match != null)
                {
                    return match.Groups[2].Value;
                }

            }
            return "1";
        }

        private string GetVersionFromAcceptHeaderVersion(HttpRequestMessage request)
        {
            var accept = request.Headers.Accept;
            foreach(var mime in accept)
            {
                if(mime.MediaType =="application/json")
                {
                    var value = mime.Parameters.FirstOrDefault(v => v.Name.Equals("version",StringComparison.OrdinalIgnoreCase));
                    return value.Value;
                }
            }
            return "1";
        }

        private string GetVersionFromHeader(HttpRequestMessage request)
        {
            const string HEADER_NAME = "X-CountingKs-Version";
            if(request.Headers.Contains(HEADER_NAME))
            {
                var header = request.Headers.GetValues(HEADER_NAME).FirstOrDefault();
                if(header != null)
                {
                    return header;
                }
            }
            return "1";
        }

        private string GetVersionFromQueryString(HttpRequestMessage request)
        {
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var version = query["v"];
            if(version != null)
            {
                return version;
            }

            return "1";
        }
    }
}