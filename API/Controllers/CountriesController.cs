using AutoMapper;
using DAL.DALModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly DwaMoviesContext _db;
        private readonly IMapper _mapper;

        public CountriesController(DwaMoviesContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetCountriesPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var countries = await _db.Countries
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return Ok(countries);
        }
    }
}
