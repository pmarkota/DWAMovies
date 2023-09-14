using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.BLModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.DALModels;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class PutVideo
    {
        [Key]
        public int Id { get; set; }

        [StringLength(256)]
        public string? Name { get; set; } = null!;

        [StringLength(1024)]
        public string? Description { get; set; }

        public int? GenreId { get; set; }

        public int? TotalSeconds { get; set; }

        [StringLength(256)]
        public string? StreamingUrl { get; set; }

        public int? ImageId { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "IsAllowedPolicy")]
    public class VideosController : ControllerBase
    {
        private readonly DwaMoviesContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;




        public VideosController(DwaMoviesContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetVideos([FromQuery]string? videoName = null, [FromQuery] string? genreName = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            IQueryable<Video> query = _db.Videos;

            if (!string.IsNullOrEmpty(videoName))
            {
                query = query.Where(v => v.Name.Contains(videoName));
            }

            if (!string.IsNullOrEmpty(genreName))
            {
                query = query.Where(v => v.Genre.Name.Contains(genreName));
            }

            int totalItems = await query.CountAsync();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var videos = await query.ToListAsync();

            var blVideos = _mapper.Map<List<BLVideo>>(videos);

            var result = new
            {
                TotalItems = totalItems,
                Videos = blVideos
            };

            return Ok(result);
        }




        [HttpGet]
        public async Task<IActionResult> GetVideos()
        {
          if (_db.Videos == null)
          {
              return NotFound();
          }
          var videos = await _db.Videos.ToListAsync();
            return Ok(videos);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
          if (_db.Videos == null)
          {
              return NotFound();
          }
            var video = await _db.Videos.FindAsync(id);

            if (video == null)
            {
                return NotFound($"Video with id {id} not found");
            }
            var blVideo = _mapper.Map<DAL.BLModels.BLVideo>(video);
            return Ok(blVideo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, BLVideo blVideo)
        {
            var dbVideo = await _db.Videos.FindAsync(id);
            if (dbVideo == null)
            {
                return NotFound($"Video with id {id} not found");
            }

            // Assign values from blVideo
            dbVideo.Name = blVideo.Name ?? dbVideo.Name;
            dbVideo.Description = blVideo.Description ?? dbVideo.Description;
            dbVideo.StreamingUrl = blVideo.StreamingUrl ?? dbVideo.StreamingUrl;

            dbVideo.GenreId = blVideo.GenreId;
            dbVideo.TotalSeconds = blVideo.TotalSeconds;
            dbVideo.ImageId = blVideo.ImageId;

            try
            {
                await _db.SaveChangesAsync();
                return Ok(blVideo);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + e.InnerException.Message);
                    Console.WriteLine("Inner Exception Stack Trace: " + e.InnerException.StackTrace);
                }

                return Problem(e.Message);
            }
        }




        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo(BLVideo video)
        {
          if (_db.Videos == null)
          {
              return Problem("Los problemmos");
          }
          var dbVideo = _mapper.Map<Video>(video);
          dbVideo.CreatedAt = DateTime.Now;
            _db.Videos.Add(dbVideo);
            await _db.SaveChangesAsync();
            return Ok(video);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            if (_db.Videos == null)
            {
                return NotFound();
            }
            var video = await _db.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound($"Video with id {id} not found");
            }
            var respVideo = _mapper.Map<BLVideo>(video);
            _db.VideoTags.RemoveRange(video.VideoTags); 
            _db.Videos.Remove(video);
            await _db.SaveChangesAsync();

            return Ok(respVideo);
        }
        
    }
}
