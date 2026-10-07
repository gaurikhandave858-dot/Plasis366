using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _service.GetAllAsync();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var customer = await _service.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            var createdCustomer =
                await _service.CreateAsync(customer);

            return Ok(createdCustomer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            Customer customer)
        {
            if (id != customer.CustomerId)
                return BadRequest("ID mismatch.");

            var updatedCustomer =
                await _service.UpdateAsync(customer);

            return Ok(updatedCustomer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Customer deleted successfully.");
        }
    }
}
