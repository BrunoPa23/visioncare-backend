using NSubstitute;
using VisionCareCore.HealthCare.Application.Internal.CommandServices;
using VisionCareCore.HealthCare.Domain.Model.Commands;
using VisionCareCore.HealthCare.Domain.Model.Entities;
using VisionCareCore.HealthCare.Domain.Model.ValueObjects;
using VisionCareCore.HealthCare.Domain.Repositories;
using VisionCareCore.Shared.Domain.Repositories;

namespace VisionCareCore.Tests.HealthCare;

public class MedicineTimeCommandServiceTests
{
    private readonly IMedicineTimeRepository _repositorio = Substitute.For<IMedicineTimeRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MedicineTimeCommandService _servicio;

    public MedicineTimeCommandServiceTests()
    {
        _servicio = new MedicineTimeCommandService(_repositorio, _unitOfWork);
    }

    [Fact]
    public async Task Crear_HoraEspecifica_GuardaElHorarioConLosDatosDelComando()
    {
        var medicineId = Guid.NewGuid();
        var hora = new TimeOnly(8, 30);
        var comando = new CreateMedicineTimeCommand(medicineId, "Lunes", Type_Remember.specific_time, null, hora, null);

        var id = await _servicio.Handle(comando);

        Assert.NotEqual(Guid.Empty, id);
        await _repositorio.Received(1).AddAsync(Arg.Is<MedicineTime>(t =>
            t.Id == id && t.MedicineId == medicineId && t.Day == "Lunes" &&
            t.TypeRemember == Type_Remember.specific_time && t.SpecificTime == hora));
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Crear_PorIntervalo_GuardaElIntervalo()
    {
        var comando = new CreateMedicineTimeCommand(Guid.NewGuid(), "Martes", Type_Remember.interval, null, null, Interval.Every8Hours);

        await _servicio.Handle(comando);

        await _repositorio.Received(1).AddAsync(Arg.Is<MedicineTime>(t => t.Interval == Interval.Every8Hours && t.SpecificTime == null));
    }

    [Fact]
    public async Task Eliminar_HorarioExistente_LoEliminaYGuarda()
    {
        var horario = new MedicineTime { Id = Guid.NewGuid(), MedicineId = Guid.NewGuid(), Day = "Lunes" };
        _repositorio.FindByIdAsync(horario.Id).Returns(horario);

        await _servicio.SoftDelete(horario.Id);

        _repositorio.Received(1).Remove(horario);
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Eliminar_HorarioInexistente_LanzaExcepcionSinGuardar()
    {
        _repositorio.FindByIdAsync(Arg.Any<Guid>()).Returns((MedicineTime?)null);

        await Assert.ThrowsAsync<Exception>(() => _servicio.SoftDelete(Guid.NewGuid()));
        await _unitOfWork.DidNotReceive().CompleteAsync();
    }
}
