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

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly DwaMoviesContext _db;
        private readonly IMapper _mapper;

        public GenresController(DwaMoviesContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetGenresPaged(int page = 1, int pageSize = 10)
        {
            if (_db.Genres == null)
            {
                return NotFound();
            }
            var genres = await _db.Genres.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalItems = _db.Genres.Count();
            var respGenres = _mapper.Map<List<BLGenre>>(genres);
            var resp = new DAL.PagedApiResponse<BLGenre>
            {
                Items = respGenres,
                TotalItems = totalItems
            };
            return Ok(resp);
        }
        [HttpGet]
        public async Task<IActionResult> GetGenres()
        {
            if (_db.Genres == null)
            {
                return NotFound();
            }
            var genres = await _db.Genres.ToListAsync();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenre(int id)
        {
            if (_db.Genres == null)
            {
                return NotFound();
            }
            var genre = await _db.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound($"Genre with provided id ({id}) not found!");
            }
            var respGenre = _mapper.Map<BLGenre>(genre);
            return Ok(respGenre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, BLGenre genre)
        {
            var dbGenre = await _db.Genres.FindAsync(id);
            if (dbGenre == null)
            {
                return NotFound($"Genre with provided id ({id}) not found!");
            }

            _mapper.Map(genre, dbGenre);

            try
            {
                _db.Update(dbGenre);

                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            var respGenre = _mapper.Map<BLGenre>(dbGenre);
            return Ok(respGenre);
        }

        [HttpPost]
        public async Task<ActionResult<Genre>> PostGenre(BLGenre genre)
        {
            if (_db.Genres == null)
            {
                return Problem("Los problemmos");
            }
            var dbGenre = _mapper.Map<Genre>(genre);
            _db.Genres.Add(dbGenre);
            await _db.SaveChangesAsync();

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            if (_db.Genres == null)
            {
                return NotFound();
            }
            var genre = await _db.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound($"Genre with provided id ({id}) not found!");
            }

            _db.Genres.Remove(genre);
            await _db.SaveChangesAsync();

            return Ok(genre);
        }

        private bool GenreExists(int id)
        {
            return (_db.Genres?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }


}

