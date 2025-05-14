using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShare.BLL.DTOs.Rental
{
    public class ReviewCreateDTO
    {
        [Required]
        public Guid CarId { get; set; }
        [Required]
        public Guid RentalId { get; set; }

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
