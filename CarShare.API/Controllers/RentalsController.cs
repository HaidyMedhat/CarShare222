using CarShare.BLL.DTOs.Rental;
using CarShare.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace CarShare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RentalsController : BaseController
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [Authorize(Roles = "Renter")]
        [HttpPost("proposals")]
        public async Task<IActionResult> CreateProposal([FromForm] RentalProposalDTO proposalDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid token: missing user ID.");

            var renterId = Guid.Parse(userIdClaim);

            var result = await _rentalService.CreateProposalAsync(proposalDTO, renterId);
            return CreatedAtAction(nameof(GetProposal), new { id = result.ProposalId }, result);
        }

        [Authorize(Roles = "CarOwner")]
        [HttpPatch("proposals/{id}/approve")]
        public async Task<IActionResult> ApproveProposal(Guid id)
        {
            //var subClaim = User.FindFirst("sub")?.Value;

            //if (string.IsNullOrWhiteSpace(subClaim) || !Guid.TryParse(subClaim, out Guid ownerId))
            //    return Unauthorized("Invalid or missing user ID in token.");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid ownerId))
                return Unauthorized("Invalid or missing user ID in token.");


            await _rentalService.ApproveProposalAsync(id, ownerId);
            return NoContent();
        }

        [HttpGet("proposals/{id}")]
        public async Task<IActionResult> GetProposal(Guid id)
        {
            var proposal = await _rentalService.GetProposalByIdAsync(id);
            if (proposal == null)
                return NotFound();

            return Ok(proposal);
        }


        [Authorize(Roles = "CarOwner")]
        [HttpPatch("proposals/{id}/reject")]
        public async Task<IActionResult> RejectProposal(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid ownerId))
                return Unauthorized("Invalid or missing user ID in token.");

            await _rentalService.RejectProposalAsync(id, ownerId);
            return NoContent();
        }


        [Authorize(Roles = "CarOwner")]
       [HttpGet("proposals/owner")]
       public async Task<IActionResult> GetProposalsForOwner()
       {
          var ownerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
          var proposals = await _rentalService.GetProposalsForOwnerAsync(ownerId);
          return Ok(proposals);
     }


        [Authorize(Roles = "Renter")]
        [HttpPost("reviews")]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDTO reviewDTO)
        {
            // 1. Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("User ID not found in token");

            if (!Guid.TryParse(userIdClaim, out Guid renterId))
                return Unauthorized("Invalid user ID format");

            // 2. Process review
            try
            {
                var result = await _rentalService.AddCarReviewAsync(reviewDTO, renterId);
                return CreatedAtAction(
                    actionName: nameof(GetCarReviews),
                    routeValues: new { carId = reviewDTO.CarId },
                    value: result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

        }

        [HttpGet("cars/{carId}/reviews")]
        public async Task<IActionResult> GetCarReviews(Guid carId)
        {
            var reviews = await _rentalService.GetReviewsForCarAsync(carId);
            return Ok(reviews);
        }
        
    }
}