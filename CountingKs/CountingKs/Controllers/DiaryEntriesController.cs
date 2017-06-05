using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using CountingKs.Services;

namespace CountingKs.Controllers
{
    public class DiaryEntriesController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repository, ICountingKsIdentityService identityService)
            : base(repository)
        {
            _identityService = identityService;
        }


        public IEnumerable<DiaryEntryModel> Get(DateTime diaryId)
        {
            var results =
                    TheRepository.GetDiaryEntries(_identityService.CurrentUser, diaryId.Date)
                                 .ToList()
                                 .Select(e => TheModelFactory.Create(e));

            return results;
        }

        public HttpResponseMessage Get(DateTime diaryId, int id)
        {
            var result = TheRepository.GetDiaryEntry(_identityService.CurrentUser, diaryId.Date, id);

            if(result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
        }


        public HttpResponseMessage Post(DateTime diaryId, [FromBody] DiaryEntryModel model)
        {

            try
            {
                var entity = TheModelFactory.Parse(model);
                if(entity == null)
                {
                    Request.CreateResponse(HttpStatusCode.BadRequest, "Could Not Read Diary Entry ");
                }

                var diary = TheRepository.GetDiary(_identityService.CurrentUser, diaryId);

                if(diary == null)
                {
                    Request.CreateResponse(HttpStatusCode.NotFound);
                }

                diary.Entries.Add(entity);

                if(diary.Entries.Any(e => e.Measure.Id == entity.Measure.Id))
                {
                    Request.CreateResponse(HttpStatusCode.BadRequest, "Duplicate Measure Not Allowed");
                }


                if(TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(entity));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Could Not save to the DB");
                }


            }
            catch(Exception)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Somthing Bad Happened");
            }

        }

        public HttpResponseMessage Delete(DateTime diaryId, int id)
        {
            try
            {
                if(TheRepository.GetDiaryEntries(_identityService.CurrentUser, diaryId).Any(e => e.Id == id))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if(TheRepository.DeleteDiary(id) && TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {


                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }



            catch(Exception)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        [HttpPatch][HttpPut]
        public HttpResponseMessage Patch(DateTime diaryId, int id, [FromBody] DiaryEntryModel model)
        {

            try
            {
                var entity = TheRepository.GetDiaryEntry(_identityService.CurrentUser, diaryId, id);
                if(entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                var parsedValue = TheModelFactory.Parse(model);

                if(parsedValue==null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                if(entity.Quantity!= parsedValue.Quantity)
                {
                    entity.Quantity = parsedValue.Quantity;

                    if(TheRepository.SaveAll())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            }
            catch(Exception)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

    }


}

