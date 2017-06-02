using System.Collections.Generic;
using System.Linq;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public class FoodsController : BaseApiController
    {
        public FoodsController(ICountingKsRepository repository) : base(repository) {}

        public IEnumerable<FoodModel> Get(bool includeMeasures = true)
        {
            IQueryable<Food> query;
            if(includeMeasures)
            {
                query = TheRepository.GetAllFoodsWithMeasures();
            }
            else
            {
                query = TheRepository.GetAllFoods();
            }
            var result = query.OrderBy(f => f.Description).Take(25).ToList().Select(m => TheModelFactory.Create(m));
            return result;
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