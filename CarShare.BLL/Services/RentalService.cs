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
                CarTitle = p.Car.Title,
                RenterName = p.Renter.FirstName + p.Renter.LastName ,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status.ToString()
            });
        }




    }
}