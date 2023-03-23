using GpuTracker.Database;
using GpuTracker.GpuModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GpuTracker.Backend.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpusController : ControllerBase
    {
        private readonly IRepository<DbGpu, int> repository;
        public GpusController(IRepository<DbGpu, int> repository)
        {
            this.repository = repository;
        }

        // GET: api/<GpusController>
        [HttpGet]
        public IEnumerable<DbGpu> Get()
        {
            return this.repository.Get();
        }

        // GET api/<GpusController>/5
        [HttpGet("{id}")]
        public DbGpu Get(int id)
        {
            return this.repository.Get(id);
        }

        // POST api/<GpusController>
        [HttpPost]
        public void Post([FromBody] DbGpu value)
        {
            this.repository.Create(value);
        }

        // PUT api/<GpusController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] DbGpu value)
        {
            if(id != value.Id)
            {
                throw new Exception($"Bad Request");
            }

            var checkIfExists = this.repository.Get(id);
            if(checkIfExists == null)
            {
                throw new Exception($"Gpu with id {id} does not exist!");
            }
            
            this.repository.Update(value);
        }

        // DELETE api/<GpusController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var checkIfExists = this.repository.Get(id);
            if (checkIfExists == null)
            {
                throw new Exception($"Gpu with id {id} does not exist!");
            }

            this.repository.Delete(id);
        }
    }
}
