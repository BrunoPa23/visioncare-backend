using NSubstitute;
using VisionCareCore.HealthCare.Application.Internal.CommandServices;
using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.HealthCare.Domain.Model.Commands;
using VisionCareCore.HealthCare.Domain.Repositories;
using VisionCareCore.Shared.Domain.Repositories;

namespace VisionCareCore.Tests.HealthCare;

public class MedicineCommandServiceTests
{
    private readonly IMedicineRepository _repositorio = Substitute.For<IMedicineRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MedicineCommandService _servicio;

    public MedicineCommandServiceTests()
    {
        _servicio = new MedicineCommandService(_repositorio, _unitOfWork);
    }

    [Fact]
    public async Task Crear_ComandoValido_GuardaElMedicamentoDelUsuarioYDevuelveSuId()
    {
        var userId = Guid.NewGuid();
        var comando = new CreateMedicineCommand("Paracetamol", "Analgesico", null, null, userId, "Cada 8 horas", "2027-01-01");

        var id = await _servicio.Handle(comando);

        Assert.NotEqual(Guid.Empty, id);
        await _repositorio.Received(1).AddAsync(Arg.Is<Medicine>(m => m.Id == id && m.UserId == userId && m.Nombre == "Paracetamol"));
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Crear_ComandoNulo_LanzaArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _servicio.Handle((CreateMedicineCommand)null!));
        await _unitOfWork.DidNotReceive().CompleteAsync();
    }

    [Fact]
    public async Task Eliminar_MedicamentoExistente_LoEliminaYGuarda()
    {
        var medicamento = new Medicine("Ibuprofeno", "", "", "", Guid.NewGuid(), null, null);
        _repositorio.GetByIdAsync(medicamento.Id).Returns(medicamento);

        await _servicio.Handle(new DeleteMedicineCommand(medicamento.Id));

        _repositorio.Received(1).Remove(medicamento);
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Eliminar_MedicamentoInexistente_NoEliminaNiGuarda()
    {
        _repositorio.GetByIdAsync(Arg.Any<Guid>()).Returns((Medicine?)null);

        await _servicio.Handle(new DeleteMedicineCommand(Guid.NewGuid()));

        _repositorio.DidNotReceive().Remove(Arg.Any<Medicine>());
        await _unitOfWork.DidNotReceive().CompleteAsync();
    }
}
