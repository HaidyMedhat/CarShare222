using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShare.BLL.DTOs.Rental
{
    public class ReviewResponseDTO
    {
        public Guid ReviewId { get; set; }
        public Guid CarId { get; set; }
        public string CarTitle { get; set; }
        public string RenterName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
     
    }
}
