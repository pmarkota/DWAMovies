using AutoMapper;
using DAL.BLModels;
using DAL.DALModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Services;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : Controller
    {
        private readonly DwaMoviesContext _db;
        private readonly IMapper _mapper;

        public TagsController(DwaMoviesContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            var tags = await _db.Tags
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var tagsResponse = _mapper.Map<List<BLTag>>(tags);

            var response = new DAL.PagedApiResponse<BLTag>
            {
                Items = tagsResponse,
                TotalItems = await _db.Tags.CountAsync()
            };

            return Ok(response);
        }

        [HttpGet/*("all")*/]
        public async Task<IActionResult> Get()
        {
            var tags = await _db.Tags.ToListAsync();

            var tagsResponse = _mapper.Map<List<BLTag>>(tags);

            return Ok(tagsResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tag = await _db.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            var tagResponse = _mapper.Map<BLTag>(tag);

            return Ok(tagResponse);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BLTag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newTag = _mapper.Map<Tag>(tag);

            _db.Tags.Add(newTag);
            await _db.SaveChangesAsync();

            var tagResponse = _mapper.Map<BLTag>(newTag);

            return CreatedAtAction(nameof(Get), new { id = tagResponse.Id }, tagResponse);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BLTag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tagToUpdate = await _db.Tags.FindAsync(id);

            if (tagToUpdate == null)
            {
                return NotFound();
            }

            tagToUpdate.Name = tag.Name;

            await _db.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _db.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();

            return NoContent();
        }

    }
}
