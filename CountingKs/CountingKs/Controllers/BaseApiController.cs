using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private ModelFactory _modelFactory;

        protected BaseApiController(ICountingKsRepository repository)
        {
            TheRepository = repository;
        }

        protected ICountingKsRepository TheRepository { get; }
        protected ModelFactory TheModelFactory
        {
            get
            {
                if(_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(Request, TheRepository);
                }
                return _modelFactory;
            }
        }
    }
}