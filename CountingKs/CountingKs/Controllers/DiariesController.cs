using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CountingKs.Filters;

namespace CountingKs.Controllers
{
    [CountingKsAuthorize]   
    public class DiariesController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiariesController(ICountingKsRepository repository, ICountingKsIdentityService identIndentityService)
            : base(repository)
        {
            _identityService = identIndentityService;
        }


        public IEnumerable<DiaryModel> Get()
        {
            var userName = _identityService.CurrentUser;
            var results =
                    TheRepository.GetDiaries(userName)
                                 .OrderByDescending(d => d.CurrentDate)
                                 .ToList()
                                 .Select(e => TheModelFactory.Create(e));
            return results;
        }

        public HttpResponseMessage Get(DateTime diaryId, int id)
        {
            var userName = _identityService.CurrentUser;
            var result = TheRepository.GetDiaryEntry(userName, diaryId, id);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
        }
    }
}