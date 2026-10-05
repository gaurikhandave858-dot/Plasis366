using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenants = await _tenantService.GetAllAsync();

            return Ok(tenants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);

            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tenant tenant)
        {
            var createdTenant = await _tenantService.CreateAsync(tenant);

            return Ok(createdTenant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Tenant tenant)
        {
            if (id != tenant.TenantId)
                return BadRequest("Tenant ID mismatch.");

            var existingTenant = await _tenantService.GetByIdAsync(id);

            if (existingTenant == null)
                return NotFound();

            var updatedTenant = await _tenantService.UpdateAsync(tenant);

            return Ok(updatedTenant);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _tenantService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Tenant deleted successfully.");
        }
    }

}
