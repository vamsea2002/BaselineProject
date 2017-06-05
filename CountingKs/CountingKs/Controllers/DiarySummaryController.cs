using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;

namespace CountingKs.Controllers
{
    public class DiarySummaryController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiarySummaryController(ICountingKsRepository repository, ICountingKsIdentityService identityService)
            : base(repository)
        {
            _identityService = identityService;
        }


        public HttpResponseMessage Get(DateTime diaryId)
        {
            try
            {
                var dairy = TheRepository.GetDiary(_identityService.CurrentUser, diaryId);

                if(dairy == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK,TheModelFactory.CreateSummary(dairy));

            }
            catch(Exception)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, "");
            }
            
        }
    }
}
