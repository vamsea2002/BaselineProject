using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using CountingKs.Data;

namespace CountingKs.Controllers
{
    [RoutePrefix("api/stats")]
    [EnableCors("*","*","GET")]
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repository) : base(repository)
        {

        }

      [Route("")]
        public HttpResponseMessage Get()
        {
            var results =
                    new
                    {
                        NumFood = TheRepository.GetAllFoods().Count(),
                        NumUsers = TheRepository.GetApiUsers().Count()
                    };

            return Request.CreateResponse(results);
        }
        [Route("~/api/stat/{id:int}")]
        public HttpResponseMessage Get(int id)
        {
            if(id == 1)
            {
                return Request.CreateResponse(new {Food = TheRepository.GetFood(id)});
            }
            if(id == 2)
            {
                return Request.CreateResponse(new { User = TheRepository.GetApiUsers().FirstOrDefault(u => u.Id==id) });
            }




            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [Route("~/api/stat/{name:alpha}")]
        public HttpResponseMessage Get(string s)
        {

            if(s=="food")
            {
                return Request.CreateResponse(HttpStatusCode.Found, new {Foods = TheRepository.GetAllFoods()});
            }
            if (s == "user")
            {
                return Request.CreateResponse(HttpStatusCode.Found, new { Foods = TheRepository.GetApiUsers() });
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

    }
}