using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Filters;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    [CountingKsAuthorize(false)]
    public class FoodsController : BaseApiController
    {
        public FoodsController(ICountingKsRepository repository) : base(repository) { }
        private const int PAGE_SIZE = 10;
        public object Get(bool includeMeasures = true, int page = 0)
        {
            IQueryable<Food> query;
            if (includeMeasures)
            {
                query = TheRepository.GetAllFoodsWithMeasures();
            }
            else
            {
                query = TheRepository.GetAllFoods();
            }
            var baseQuery = query.OrderBy(f => f.Description);
            var totalCount = baseQuery.Count();

            var totalPages = Math.Ceiling((double)totalCount / PAGE_SIZE);

            var helper = new UrlHelper(Request);
            var prevPageUrl = page>0?helper.Link("Food", new {page = page - 1}):"";
            var nextPageUrl = page < totalPages-1? helper.Link("Food", new { page = page + 1 }):"";
            var result = baseQuery
                .Skip(PAGE_SIZE * page)
                .Take(PAGE_SIZE).ToList()
                .Select(m => TheModelFactory.Create(m));
            return new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PrevPageUrl=prevPageUrl,
                NextPageUrl=nextPageUrl,
                Results = result
            };
        }

        public FoodModel Get(int foodid)
        {
            return TheModelFactory.Create(TheRepository.GetFood(foodid));
        }
    }
}

//var result =
//        _repository.GetAllFoodsWithMeasures()
//                   .OrderBy(f => f.Description)
//                   .Take(25)
//                   .ToList()
//                   .Select(
//                           m =>
//                               new FoodModel()
//                               {
//                                   Description = m.Description,
//                                   Measures =
//                                   m.Measures.Select(k => new MeasureModel() {Description = k.Description, Calories = k.Calories})
//                               });

//var result = _repository.GetAllFoodsWithMeasures().OrderBy(f => f.Description).Take(25).ToList();
//var result = _repository.GetAllFoods().OrderBy(f => f.Description).Take(25).ToList();