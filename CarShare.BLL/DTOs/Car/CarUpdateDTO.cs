using CarShare.DAL.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarShare.BLL.DTOs.Car
{
    public class CarUpdateDTO
    {
        [Required]
        public Guid CarId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public CarType CarType { get; set; }

        [Required, MaxLength(50)]
        public string Brand { get; set; }

        [Required, MaxLength(50)]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public TransmissionType Transmission { get; set; }

        [Required, MaxLength(100)]
        public string Location { get; set; }

        [Required, MaxLength(20)]
        public string LicensePlate { get; set; }

        [Required]
        public decimal PricePerDay { get; set; }
    }
}
