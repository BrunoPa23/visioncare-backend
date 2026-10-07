using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisionCareCore.HealthCare.Domain.Queries;
using VisionCareCore.HealthCare.Domain.Services;
using VisionCareCore.HealthCare.Interfaces.Resources;
using VisionCareCore.HealthCare.Interfaces.Transform;
using VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Extensions;

namespace VisionCareCore.HealthCare.Interfaces.Controllers;

[Authorize]
[ApiController]
[Route("api/medicine-time")]
public class MedicineTimeController : ControllerBase
{
    private readonly IMedicineTimeCommandService _commandService;
    private readonly IMedicineTimeQueryService _queryService;
    private readonly IMedicineQueryService _medicineQueryService;

    public MedicineTimeController(
        IMedicineTimeCommandService commandService,
        IMedicineTimeQueryService queryService,
        IMedicineQueryService medicineQueryService)
    {
        _commandService = commandService;
        _queryService = queryService;
        _medicineQueryService = medicineQueryService;
    }

    // Indica si el medicamento existe y pertenece al usuario autenticado
    private async Task<bool> EsMedicamentoDelUsuario(Guid medicineId)
    {
        var userId = User.GetUserId();
        if (userId is null) return false;

        var medicine = await _medicineQueryService.Handle(new GetMedicineByIdQuery(medicineId));
        return medicine is not null && medicine.UserId == userId;
    }

    /// <summary>
    /// Reemplaza todos los MedicineTime de una medicina por los nuevos enviados.
    /// </summary>
    [HttpPost("update-all")]
    public async Task<IActionResult> UpdateAll([FromBody] UpdateMedicineTimesRequest request)
    {
        if (!await EsMedicamentoDelUsuario(request.MedicineId)) return NotFound();

        // Eliminar todos los MedicineTime existentes para la medicina
        var existing = await _queryService.GetAllByMedicineIdAsync(request.MedicineId);
        foreach (var mt in existing)
        {
            await _commandService.SoftDelete(mt.Id);
        }

        // Crear los nuevos MedicineTime
        foreach (var resource in request.MedicineTimes)
        {
            // Forzar el MedicineId correcto
            resource.MedicineId = request.MedicineId;
            var command = CreateMedicineTimeTransform.ToCommand(resource);
            await _commandService.Handle(command);
        }
        var result = await _queryService.GetAllByMedicineIdAsync(request.MedicineId);
        //devolver el nuevo
        return Ok(result);
    }

   
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicineTimeResource resource)
    {
        if (!await EsMedicamentoDelUsuario(resource.MedicineId)) return NotFound();

        var command = CreateMedicineTimeTransform.ToCommand(resource);
        var id = await _commandService.Handle(command);
        return Ok(new { id });
    }

    
    [HttpGet("by-medicine/{medicineId}")]
    public async Task<IActionResult> GetByMedicineId(Guid medicineId)
    {
        if (!await EsMedicamentoDelUsuario(medicineId)) return NotFound();

        var result = await _queryService.GetAllByMedicineIdAsync(medicineId);
        return Ok(result);
    }

   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _queryService.GetByIdAsync(id);
        if (result == null || !await EsMedicamentoDelUsuario(result.MedicineId)) return NotFound();
        return Ok(result);
    }

 
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var medicineTime = await _queryService.GetByIdAsync(id);
        if (medicineTime == null || !await EsMedicamentoDelUsuario(medicineTime.MedicineId)) return NotFound();

        await _commandService.SoftDelete(id);
        return NoContent();
    }
}