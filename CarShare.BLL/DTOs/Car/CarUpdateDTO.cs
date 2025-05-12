using CarShare.DAL.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CarShare.BLL.DTOs.Car
{
    public class CarUpdateDTO
    {
        [Required]
        public Guid CarId { get; set; }

        // Make all fields nullable and remove defaults
        public string? Title { get; set; } = null; // Explicit null default
        public string? Description { get; set; } = null;
        public CarType? CarType { get; set; } = null;
        public string? Brand { get; set; } = null;
        public string? Model { get; set; } = null;
        public int? Year { get; set; } = null;
        public TransmissionType? Transmission { get; set; } = null;
        public string? Location { get; set; } = null;
        public string? LicensePlate { get; set; } = null;
        public decimal? PricePerDay { get; set; } = null;
    }
}