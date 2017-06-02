using System.Collections.Generic;
using System.Linq;
using CountingKs.Data;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public class MeasuresController : BaseApiController
    {
        public MeasuresController(ICountingKsRepository repository) : base(repository) {}

        public IEnumerable<MeasureModel> Get(int foodid)
        {
            var results = TheRepository.GetMeasuresForFood(foodid).ToList().Select(m => TheModelFactory.Create(m));
            return results;
        }

        public MeasureModel Get(int foodid, int id)
        {
            var results = TheRepository.GetMeasure(id);
            if(results.Food.Id == foodid)
            {
                return TheModelFactory.Create(results);
            }
            return null;
        }

    }
}