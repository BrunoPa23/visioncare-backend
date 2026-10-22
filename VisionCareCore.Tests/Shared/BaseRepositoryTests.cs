using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.HealthCare.Infrastructure.Persistence.EFC.Repositories;
using VisionCareCore.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace VisionCareCore.Tests.Shared;

/// <summary>
/// Prueba los metodos de BaseRepository con claves Guid sobre SQLite en memoria,
/// usando MedicineRepository como repositorio concreto.
/// </summary>
public sealed class BaseRepositoryTests : IDisposable
{
    private readonly SqliteConnection _conexion = new("DataSource=:memory:");
    private readonly AppDbContext _context;
    private readonly MedicineRepository _repositorio;

    public BaseRepositoryTests()
    {
        _conexion.Open();
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_conexion).Options);
        _context.Database.EnsureCreated();
        _repositorio = new MedicineRepository(_context);
    }

    private static Medicine NuevoMedicamento(string nombre) =>
        new(nombre, "descripcion", "efectos", "advertencias", Guid.NewGuid(), null, null);

    [Fact]
    public async Task FindByIdAsync_ConGuidExistente_DevuelveLaEntidad()
    {
        var medicamento = NuevoMedicamento("Paracetamol");
        await _repositorio.AddAsync(medicamento);
        _context.ChangeTracker.Clear();

        var encontrado = await _repositorio.FindByIdAsync(medicamento.Id);

        Assert.NotNull(encontrado);
        Assert.Equal("Paracetamol", encontrado.Nombre);
    }

    [Fact]
    public async Task FindByIdAsync_ConGuidInexistente_DevuelveNull()
    {
        Assert.Null(await _repositorio.FindByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_ConGuid_EliminaSoloEsaEntidad()
    {
        var aBorrar = NuevoMedicamento("Ibuprofeno");
        var aConservar = NuevoMedicamento("Amoxicilina");
        await _repositorio.AddAsync(aBorrar);
        await _repositorio.AddAsync(aConservar);

        await _repositorio.DeleteAsync(aBorrar.Id);

        var restantes = await _repositorio.ListAsync();
        Assert.Equal(aConservar.Id, Assert.Single(restantes).Id);
    }

    public void Dispose()
    {
        _context.Dispose();
        _conexion.Dispose();
    }
}
