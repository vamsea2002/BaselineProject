using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using CacheCow.Server;
using CacheCow.Server.EntityTagStore.SqlServer;
using CountingKs.Converters;
using CountingKs.Filters;
using CountingKs.Models;
using CountingKs.Services;
using Newtonsoft.Json;
using WebApiContrib.Formatting.Jsonp;

namespace CountingKs
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           config.MapHttpAttributeRoutes(); 

            config.Routes.MapHttpRoute(name: "Food",
                                       routeTemplate: "api/nutrition/foods/{foodid}",
                                       defaults: new {controller = "Foods", foodid = RouteParameter.Optional});
            config.Routes.MapHttpRoute(name: "Measures",
                                       routeTemplate: "api/nutrition/foods/{foodid}/measures/{id}",
                                       defaults: new {controller = "measures", id = RouteParameter.Optional});
            config.Routes.MapHttpRoute(name: "Diaries",
                                       routeTemplate: "api/user/diaries/{diaryId}",
                                       defaults: new { controller = "diaries", diaryId = RouteParameter.Optional });
            config.Routes.MapHttpRoute(name: "DiaryEntries",
                                       routeTemplate: "api/user/diaries/{diaryId}/entries/{id}",
                                       defaults: new {controller = "diaryentries", id = RouteParameter.Optional});
            config.Routes.MapHttpRoute(name: "DiarySummary",
                                                   routeTemplate: "api/user/diaries/{diaryId}/summary",
                                                   defaults: new { controller = "diarysummary"});

            config.Routes.MapHttpRoute(name: "Token",
                                                   routeTemplate: "api/token",
                                                   defaults: new { controller = "token" });
            config.Routes.MapHttpRoute(name: "Stats",
                                                  routeTemplate: "api/stats",
                                                  defaults: new { controller = "stats" });

            //config.Routes.MapHttpRoute(name: "Measures2",
            //                           routeTemplate: "api/v2/nutrition/foods/{foodid}/measures/{id}",
            //                           defaults: new { controller = "measuresv2", id = RouteParameter.Optional });



            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.Converters.Add(new LinkModelConverter());
            CreateMediaTypes(jsonFormatter);
            
            //support for jsonp
            //var formatter = new JsonpMediaTypeFormatter(jsonFormatter,"cb");
            //config.Formatters.Insert(0,formatter);

            // Uncomment this to support only https for the application
            //config.Filters.Add(new RequireHttpsAttribute());

            //Replace the Controller Congiguration
            config.Services.Replace(typeof(IHttpControllerSelector), new CountingKsControllerSelector(config));

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            //Configure Cacheing 
            
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            var etagStore = new SqlServerEntityTagStore(connectionString);
            var cacheHandler = new CachingHandler(config, etagStore) {AddLastModifiedHeader = false};

            config.MessageHandlers.Add(cacheHandler);
            var attr = new EnableCorsAttribute("*","*","GET");
           config.EnableCors(attr);
        }

        private static void CreateMediaTypes(JsonMediaTypeFormatter jsonFormatter)
        {
            var mediaTypes = new string[]
                             {
                                 "application/vnd.countingks.food.v1+json",
                                 "application/vnd.countingks.measure.v1+json",
                                 "application/vnd.countingks.measure.v2+json",
                                 "application/vnd.countingks.diary.v1+json",
                                 "application/vnd.countingks.diaryEntry.v1+json",

                             };

            foreach(var mediaType in mediaTypes)
            {
                jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }
        }
    }
}