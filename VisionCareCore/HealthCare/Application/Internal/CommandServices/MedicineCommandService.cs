using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.HealthCare.Domain.Model.Commands;
using VisionCareCore.HealthCare.Domain.Repositories;
using VisionCareCore.HealthCare.Domain.Services;
using VisionCareCore.Shared.Domain.Repositories;

namespace VisionCareCore.HealthCare.Application.Internal.CommandServices;

public class MedicineCommandService : IMedicineCommandService
{
    private readonly IMedicineRepository _medicineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MedicineCommandService(IMedicineRepository medicineRepository, IUnitOfWork unitOfWork)
    {
        _medicineRepository = medicineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMedicineCommand command)
    {
        if (command == null)
            throw new ArgumentException("Invalid medicine data.");

        var medicine = new Medicine(
            command.Nombre,
            command.Description,
            command.SideEffects,
            command.Warnings,
            command.UserId,
            command.Instruccions,
            command.ExpirationDate
        );

        await _medicineRepository.AddAsync(medicine);
        await _unitOfWork.CompleteAsync();

        return medicine.Id;
    }
    public async Task Handle(DeleteMedicineCommand command)
    {
        var medicine = await _medicineRepository.GetByIdAsync(command.MedicineId);

        if (medicine == null)
            return;

        _medicineRepository.Delete(medicine);
        await _unitOfWork.CompleteAsync();
    }

}