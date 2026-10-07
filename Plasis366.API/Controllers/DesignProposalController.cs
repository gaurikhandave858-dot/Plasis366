using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignProposalController : ControllerBase
    {
        private readonly IDesignProposalService _designProposalService;

        public DesignProposalController(
            IDesignProposalService designProposalService)
        {
            _designProposalService = designProposalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proposals = await _designProposalService.GetAllAsync();
            return Ok(proposals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var proposal = await _designProposalService.GetByIdAsync(id);

            if (proposal == null)
                return NotFound();

            return Ok(proposal);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] DesignProposal designProposal)
        {
            var createdProposal =
                await _designProposalService.CreateAsync(designProposal);

            return Ok(createdProposal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            [FromBody] DesignProposal designProposal)
        {
            if (id != designProposal.DesignProposalId)
                return BadRequest(
                    "Design Proposal ID in URL and body must match.");

            var existingProposal =
                await _designProposalService.GetByIdAsync(id);

            if (existingProposal == null)
                return NotFound();

            var updatedProposal =
                await _designProposalService.UpdateAsync(designProposal);

            return Ok(updatedProposal);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _designProposalService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Design proposal deleted successfully.");
        }
    }
}
