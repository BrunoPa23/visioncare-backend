using Microsoft.EntityFrameworkCore.Storage;

namespace VisionCareCore.Shared.Domain.Repositories;

public interface IUnitOfWork
{

    Task CompleteAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();

}