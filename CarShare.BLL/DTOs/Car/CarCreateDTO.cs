using CarShare.DAL.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CarShare.BLL.DTOs.Car
{
    public class CarCreateDTO
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public CarType Type { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public TransmissionType Transmission { get; set; }

        [Required]
        public decimal PricePerDay { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        // ✅ الجديد
        public List<IFormFile>? Images { get; set; }
    }
}