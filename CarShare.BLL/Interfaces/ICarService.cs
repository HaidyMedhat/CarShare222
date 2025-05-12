using CarShare.BLL.DTOs.Car;
using CarShare.DAL.Models;

namespace CarShare.BLL.Interfaces
{
    public interface ICarService
    {
        Task<CarResponseDTO> CreateAsync(CarCreateDTO carDTO, Guid ownerId);
        Task<IEnumerable<CarResponseDTO>> GetAllAvailableAsync();
        Task<IEnumerable<CarResponseDTO>> GetPendingCarsAsync();
        Task<CarResponseDTO> GetByIdAsync(Guid carId);
        Task ApproveCarAsync(Guid carId);
        Task RejectCarAsync(Guid carId);

        // New methods for Car Owner CRUD
        Task<IEnumerable<CarResponseDTO>> GetByOwnerIdAsync(Guid ownerId);
        Task<CarResponseDTO> UpdateAsync(Guid carId, CarUpdateDTO carDTO, Guid ownerId);
        Task<bool> DeleteAsync(Guid carId, Guid ownerId);


    }
}