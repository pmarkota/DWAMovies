using AutoMapper;
using DAL.DALModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly DwaMoviesContext _db;
        private readonly IMapper _mapper;

        public ImagesController(DwaMoviesContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var images = await _db.Images.ToListAsync();
            var respImages = _mapper.Map<List<DAL.BLModels.BLImage>>(images);
            return Ok(respImages);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _db.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            var respImage = _mapper.Map<DAL.BLModels.BLImage>(image);
            return Ok(respImage);
        }
        [HttpPost]
        public async Task<IActionResult> PostImage([FromBody] DAL.BLModels.BLImage image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbImage = _mapper.Map<DAL.DALModels.Image>(image);
            _db.Images.Add(dbImage);
            await _db.SaveChangesAsync();
            return CreatedAtAction("GetImage", new { id = dbImage.Id }, image);
        }
    }
}
