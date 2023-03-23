using GpuTracker.Database;
using GpuTracker.GpuModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GpuTracker.Backend.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {

        private readonly IRepository<DbGpu, int> repository;
        public SearchController(IRepository<DbGpu, int> repository)
        {
            this.repository = repository;
        }

        // GET: api/<GpusController>
        [HttpGet]
        public IEnumerable<DbGpu> Get([FromQuery] string query)
        {
            return this.repository.GetByQuery(query);
        }
    }
}
