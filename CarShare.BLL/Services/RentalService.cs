using AutoMapper;
using CarShare.BLL.DTOs.Rental;
using CarShare.BLL.Interfaces;
using CarShare.DAL.Enums;
using CarShare.DAL.Interfaces;
using CarShare.DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace CarShare.BLL.Services
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public RentalService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<RentalResponseDTO> CreateProposalAsync(RentalProposalDTO proposalDTO, Guid renterId)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(proposalDTO.CarId);
            if (car == null) throw new Exception("Car not found");
            if (!car.IsApproved) throw new Exception("Car not approved for rental");

            var proposal = _mapper.Map<RentalProposal>(proposalDTO);
            proposal.RenterId = renterId;
            proposal.Status = ProposalStatus.Pending;

            // Save license image
            if (proposalDTO.LicenseVerificationUrl != null && proposalDTO.LicenseVerificationUrl.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(proposalDTO.LicenseVerificationUrl.FileName)}";
                var folderPath = Path.Combine("images", "licenses");
                var fullPath = Path.Combine(_environment.WebRootPath, folderPath);

                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                var filePath = Path.Combine(fullPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await proposalDTO.LicenseVerificationUrl.CopyToAsync(stream);
                }

                proposal.LicenseVerificationUrl = $"/{folderPath}/{fileName}".Replace("\\", "/");
            }

            await _unitOfWork.RentalProposals.AddAsync(proposal);
            await _unitOfWork.CommitAsync();

            // Load renter for response mapping
            var fullProposal = await _unitOfWork.Context.RentalProposals
                .Include(p => p.Renter)
                .Include(p => p.Car)
                .FirstOrDefaultAsync(p => p.ProposalId == proposal.ProposalId);


            return _mapper.Map<RentalResponseDTO>(proposal);
        }

        //public async Task ApproveProposalAsync(Guid proposalId, Guid ownerId)
        //{
        //    var proposal = await _unitOfWork.Context.RentalProposals
        //        .Include(p => p.Car)
        //        .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

        //    if (proposal == null || proposal.Car.OwnerId != ownerId)
        //        throw new Exception("Proposal not found or unauthorized");

        //    // Update Proposal Status
        //    proposal.Status = ProposalStatus.Accepted;

        //    // Update Car Rental Status
        //    proposal.Car.RentalStatus = RentalStatus.Rented;

        //    await _unitOfWork.CommitAsync();
        //}
        public async Task ApproveProposalAsync(Guid proposalId, Guid ownerId)
        {
            var proposal = await _unitOfWork.Context.RentalProposals
                .Include(p => p.Car)
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (proposal == null || proposal.Car.OwnerId != ownerId)
                throw new Exception("Proposal not found or unauthorized");

            // Update Proposal Status
            proposal.Status = ProposalStatus.Accepted;

            // Update Car Rental Status
            proposal.Car.RentalStatus = RentalStatus.Rented;

            // 🔽 Create Rental record (if it doesn't already exist)
            var existingRental = await _unitOfWork.Context.Rentals
                .FirstOrDefaultAsync(r => r.ProposalId == proposalId);

            if (existingRental == null)
            {
                var rental = new Rental
                {
                    ProposalId = proposal.ProposalId,
                    Status = RentalState.Completed // or .Pending if you prefer
                };

                await _unitOfWork.Rentals.AddAsync(rental);
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task RejectProposalAsync(Guid proposalId, Guid ownerId)
        {
            // get from db
            var proposal = await _unitOfWork.Context.RentalProposals
                .Include(p => p.Car)
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (proposal == null || proposal.Car.OwnerId != ownerId)
                throw new Exception("Proposal not found or unauthorized");

            // update status to Rejected
            proposal.Status = ProposalStatus.Rejected;

            await _unitOfWork.CommitAsync();
        }



        public async Task<RentalResponseDTO?> GetProposalByIdAsync(Guid proposalId)
        {
            var proposal = await _unitOfWork.RentalProposals
                .GetByIdWithIncludesAsync(p => p.ProposalId == proposalId,
                                          p => p.Car,
                                          p => p.Renter);

            if (proposal == null)
                throw new Exception("Proposal not found");

            return _mapper.Map<RentalResponseDTO>(proposal);

        }
        public async Task<IEnumerable<RentalResponseDTO>> GetProposalsForOwnerAsync(Guid ownerId)
        {
            var proposals = await _unitOfWork.Context.RentalProposals
                .Include(p => p.Car)
                .Include(p => p.Renter)
                .Where(p => p.Car.OwnerId == ownerId)
                .ToListAsync();

            return proposals.Select(p => new RentalResponseDTO
            {
                ProposalId = p.ProposalId,
                CarId = p.Car.CarId,
                CarTitle = p.Car.Title,
                RenterName = p.Renter.FirstName + p.Renter.LastName ,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status.ToString()
            });
        }
        public async Task<ReviewResponseDTO> AddCarReviewAsync(ReviewCreateDTO reviewDTO, Guid renterId)
        {
            // 1. Verify rental with more flexible time check
            var rental = await _unitOfWork.Context.Rentals
                        .Include(r => r.Proposal)
                           .ThenInclude(p => p.Car)
                        .Include(r => r.Proposal.Renter)
                        .FirstOrDefaultAsync(r =>
                            r.RentalId == reviewDTO.RentalId &&
                            r.Proposal.CarId == reviewDTO.CarId &&
                            r.Proposal.RenterId == renterId &&
                            r.Proposal.Status == ProposalStatus.Accepted);


            if (rental == null)
                throw new Exception("Rental not found or invalid");

            // 2. Allow reviews if rental was EVER active (remove time check)
            // if (rental.EndDate < DateTime.UtcNow) // Removed this check

            // 3. Check for existing review
            if (await _unitOfWork.Context.Reviews
                .AnyAsync(r => r.RentalId == reviewDTO.RentalId))
            {
                throw new Exception("You've already reviewed this rental");
            }

            // 4. Create and save review
            var review = new Review
            {
                CarId = reviewDTO.CarId,
                RenterId = renterId,
                RentalId = reviewDTO.RentalId,
                Rating = reviewDTO.Rating,
                Comment = reviewDTO.Comment,
                ReviewDate = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ReviewResponseDTO>(review);
        }

        private async Task UpdateCarRating(Guid carId)
        {
            // 1. Get the car with tracking to update it
            var car = await _unitOfWork.Context.Cars
                .FirstOrDefaultAsync(c => c.CarId == carId);

            if (car == null) return;

            // 2. Calculate new average rating
            car.AverageRating = await _unitOfWork.Context.Reviews
                .Where(r => r.CarId == carId)
                .AverageAsync(r => (decimal?)r.Rating) ?? 0; // Handle no reviews case

            // 3. Explicitly mark as modified (optional but clear)
            _unitOfWork.Context.Entry(car).Property(x => x.AverageRating).IsModified = true;

            // 4. Save changes
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ReviewResponseDTO>> GetReviewsForCarAsync(Guid carId)
        {
            return await _unitOfWork.Context.Reviews
                .Where(r => r.CarId == carId)
                .Include(r => r.Renter)
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new ReviewResponseDTO
                {
                    ReviewId = r.ReviewId,
                    RenterName = $"{r.Renter.FirstName} {r.Renter.LastName}",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate,
                })
                .ToListAsync();
        }

    }
}