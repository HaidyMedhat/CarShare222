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




    }
}
