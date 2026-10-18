using NSubstitute;
using VisionCareCore.HealthCare.Application.Internal.QueryServices;
using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.HealthCare.Domain.Queries;
using VisionCareCore.HealthCare.Domain.Repositories;

namespace VisionCareCore.Tests.HealthCare;

public class MedicineQueryServiceTests
{
    private readonly IMedicineRepository _repositorio = Substitute.For<IMedicineRepository>();

    [Fact]
    public async Task ObtenerPorUsuario_DevuelveLosMedicamentosDelRepositorio()
    {
        var userId = Guid.NewGuid();
        var medicamentos = new List<Medicine> { new("A", "", "", "", userId, null, null), new("B", "", "", "", userId, null, null) };
        _repositorio.GetAllByUserIdAsync(userId).Returns(medicamentos);

        var resultado = await new MedicineQueryService(_repositorio).Handle(new GetAllMedicinesByUserIdQuery(userId));

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task ObtenerPorId_Inexistente_DevuelveNull()
    {
        _repositorio.GetByIdAsync(Arg.Any<Guid>()).Returns((Medicine?)null);

        var resultado = await new MedicineQueryService(_repositorio).Handle(new GetMedicineByIdQuery(Guid.NewGuid()));

        Assert.Null(resultado);
    }
}
