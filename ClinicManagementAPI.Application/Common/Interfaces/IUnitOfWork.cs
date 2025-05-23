namespace ClinicManagementAPI.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : Domain.Common.BaseEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 