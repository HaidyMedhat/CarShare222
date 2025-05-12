using AutoMapper;
using CarShare.BLL.DTOs.Car;
using CarShare.BLL.Interfaces;
using CarShare.DAL.Enums;
using CarShare.DAL.Interfaces;
using CarShare.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CarShare.BLL.Services
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CarService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<CarResponseDTO> CreateAsync(CarCreateDTO carDTO, Guid ownerId)
        {
            await _userService.VerifyCarOwnerAsync(ownerId);

            var car = _mapper.Map<Car>(carDTO);
            car.OwnerId = ownerId;
            car.IsApproved = false; // Still requires admin approval
            car.RentalStatus = RentalStatus.Available; // Ensure it's set to available by default

            await _unitOfWork.Cars.AddAsync(car);
            // ✅ لو فيه صور، خزنها
            if (carDTO.Images != null && carDTO.Images.Count > 0)
            {
                foreach (var image in carDTO.Images)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine("wwwroot/images/cars", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Just in case

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var carImage = new CarImage
                    {
                        CarId = car.CarId,
                        ImageUrl = $"/images/cars/{fileName}",
                        IsMain = false
                    };

                    await _unitOfWork.Context.CarImages.AddAsync(carImage);
                }
            }

            await _unitOfWork.CommitAsync();

            return _mapper.Map<CarResponseDTO>(car);
        }

        public async Task<IEnumerable<CarResponseDTO>> GetAllAvailableAsync()
        {
            var cars = await _unitOfWork.Context.Cars
                .Include(c => c.Owner)
                .Include(c => c.CarImages)   
                .Where(c => c.IsApproved && c.RentalStatus == RentalStatus.Available)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CarResponseDTO>>(cars);
        }



        public async Task<CarResponseDTO> GetByIdAsync(Guid carId)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            return _mapper.Map<CarResponseDTO>(car);
        }

        public async Task ApproveCarAsync(Guid carId)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            car.IsApproved = true;
            await _unitOfWork.CommitAsync();
        }

        // view pending cars
        public async Task<IEnumerable<CarResponseDTO>> GetPendingCarsAsync()
        {
            var pendingCars = await _unitOfWork.Context.Cars
                .Include(c => c.Owner)
                .Where(c => !c.IsApproved)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CarResponseDTO>>(pendingCars);
        }

        // reject car posts
        public async Task RejectCarAsync(Guid carId)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null)
                throw new Exception("Car not found");

            _unitOfWork.Cars.Remove(car);
            await _unitOfWork.CommitAsync();
        }

        // New: Get all cars owned by a specific user
        public async Task<IEnumerable<CarResponseDTO>> GetByOwnerIdAsync(Guid ownerId)
        {
            var cars = await _unitOfWork.Context.Cars
                .Include(c => c.Owner)
                .Include(c => c.CarImages)   // NEW: Load images!
                .Where(c => c.OwnerId == ownerId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CarResponseDTO>>(cars);
        }

        public async Task<CarResponseDTO> UpdateAsync(Guid carId, CarUpdateDTO carDTO, Guid ownerId)
        {
            var car = await _unitOfWork.Context.Cars
                .FirstOrDefaultAsync(c => c.CarId == carId);

            if (car == null) throw new Exception("Car not found");
            if (car.OwnerId != ownerId) throw new UnauthorizedAccessException("Not your car!");

            // Only update fields that were EXPLICITLY provided AND have meaningful values
            if (!string.IsNullOrEmpty(carDTO.Title)) car.Title = carDTO.Title;
            if (!string.IsNullOrEmpty(carDTO.Description)) car.Description = carDTO.Description;
            if (carDTO.CarType.HasValue && carDTO.CarType.Value != 0) car.CarType = carDTO.CarType.Value;
            if (!string.IsNullOrEmpty(carDTO.Brand)) car.Brand = carDTO.Brand;
            if (!string.IsNullOrEmpty(carDTO.Model)) car.Model = carDTO.Model;
            if (carDTO.Year.HasValue && carDTO.Year.Value != 0) car.Year = carDTO.Year.Value;
            if (carDTO.Transmission.HasValue && carDTO.Transmission.Value != 0) car.Transmission = carDTO.Transmission.Value;
            if (!string.IsNullOrEmpty(carDTO.Location)) car.Location = carDTO.Location;
            if (!string.IsNullOrEmpty(carDTO.LicensePlate)) car.LicensePlate = carDTO.LicensePlate;
            if (carDTO.PricePerDay.HasValue && carDTO.PricePerDay.Value != 0) car.PricePerDay = carDTO.PricePerDay.Value;

            car.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CarResponseDTO>(car);
        }

        // New: Delete car (only if no active rentals)
        public async Task<bool> DeleteAsync(Guid carId, Guid ownerId)
        {
            var car = await _unitOfWork.Context.Cars
                .Include(c => c.RentalProposals)
                .FirstOrDefaultAsync(c => c.CarId == carId);

            if (car == null) throw new Exception("Car not found.");
            if (car.OwnerId != ownerId) throw new UnauthorizedAccessException("Not your car!");
            if (car.RentalStatus == RentalStatus.Rented)
                throw new InvalidOperationException("Cannot delete: Car is currently rented!");

            _unitOfWork.Cars.Remove(car);
            await _unitOfWork.CommitAsync();
            return true;
        }


    }
}
