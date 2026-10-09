using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plasis366.Infrastructure;

namespace Plasis366.API.Controllers
{
    // Small lists for the dropdowns: each level is filtered by the one above it
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MasterDataController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MasterDataController(ApplicationDbContext db)
        {
            _db = db;
        }

        public record Option(long Id, string Name);

        [HttpGet("regions")]
        public async Task<IActionResult> Regions() =>
            Ok(await _db.Regions
                .Where(x => x.IsActive)
                .OrderBy(x => x.RegionName)
                .Select(x => new Option(x.RegionId, x.RegionName))
                .ToListAsync());

        [HttpGet("countries")]
        public async Task<IActionResult> Countries([FromQuery] long regionId) =>
            Ok(await _db.Countries
                .Where(x => x.IsActive && x.RegionId == regionId)
                .OrderBy(x => x.CountryName)
                .Select(x => new Option(x.CountryId, x.CountryName))
                .ToListAsync());

        [HttpGet("states")]
        public async Task<IActionResult> States([FromQuery] long countryId) =>
            Ok(await _db.States
                .Where(x => x.IsActive && x.CountryId == countryId)
                .OrderBy(x => x.StateName)
                .Select(x => new Option(x.StateId, x.StateName))
                .ToListAsync());

        [HttpGet("districts")]
        public async Task<IActionResult> Districts([FromQuery] long stateId) =>
            Ok(await _db.Districts
                .Where(x => x.IsActive && x.StateId == stateId)
                .OrderBy(x => x.DistrictName)
                .Select(x => new Option(x.DistrictId, x.DistrictName))
                .ToListAsync());

        [HttpGet("talukas")]
        public async Task<IActionResult> Talukas([FromQuery] long districtId) =>
            Ok(await _db.Talukas
                .Where(x => x.IsActive && x.DistrictId == districtId)
                .OrderBy(x => x.TalukaName)
                .Select(x => new Option(x.TalukaId, x.TalukaName))
                .ToListAsync());

        [HttpGet("cities")]
        public async Task<IActionResult> Cities([FromQuery] long talukaId) =>
            Ok(await _db.Cities
                .Where(x => x.IsActive && x.TalukaId == talukaId)
                .OrderBy(x => x.CityName)
                .Select(x => new Option(x.CityId, x.CityName))
                .ToListAsync());
    }
}