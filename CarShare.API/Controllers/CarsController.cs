using CarShare.BLL.DTOs.Car;
using CarShare.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarShare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : BaseController
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAvailable()
        {
            var result = await _carService.GetAllAvailableAsync();

            // HandleResult will return the list of cars including the owner info if present in DTO
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _carService.GetByIdAsync(id);
            return HandleResult(result);
        }

        [Authorize(Roles = "CarOwner")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CarCreateDTO carDTO)
        {
            // Optional: Debugging claims
            var allClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}");
            Console.WriteLine(string.Join("\n", allClaims)); // Debugging line

            var ownerId = Guid.Parse(
                User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
            );

            var result = await _carService.CreateAsync(carDTO, ownerId);
            return CreatedAtAction(nameof(GetById), new { id = result.CarId }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> ApproveCar(Guid id)
        {
            await _carService.ApproveCarAsync(id);
            return NoContent();
        }

        // ✅ Endpoint جديد عشان يجيب العربيات اللي لسه مستنيا الموافقة
        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingCars()
        {
            var result = await _carService.GetPendingCarsAsync();
            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/reject")]
        public async Task<IActionResult> RejectCar(Guid id)
        {
            await _carService.RejectCarAsync(id);
            return NoContent();
        }
        // Get my cars (owner only)
        [Authorize(Roles = "CarOwner")]
        [HttpGet("my-cars")]
        public async Task<IActionResult> GetMyCars()
        {
            var ownerId = GetCurrentUserId();
            var result = await _carService.GetByOwnerIdAsync(ownerId);
            return HandleResult(result);
        }

        // Update car (owner only)
        [Authorize(Roles = "CarOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CarUpdateDTO carDTO)
        {
            var ownerId = GetCurrentUserId();
            carDTO.CarId = id; // Ensure ID matches route
            var result = await _carService.UpdateAsync(id, carDTO, ownerId);
            return HandleResult(result);
        }


        // Delete car (owner only) - with rental check
        [Authorize(Roles = "CarOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ownerId = GetCurrentUserId();
            var result = await _carService.DeleteAsync(id, ownerId);
            return HandleResult(result);
        }
        private Guid GetCurrentUserId()
        {
            return Guid.Parse(
                User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
            );
        }


    }
}
