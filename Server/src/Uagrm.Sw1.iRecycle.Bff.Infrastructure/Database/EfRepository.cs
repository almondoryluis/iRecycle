using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Uagrm.Sw1.iRecycle.Bff.SharedKernel.Interfaces;

namespace Uagrm.Sw1.iRecycle.Bff.Infrastructure.Database;

public class EfRepository<TEntity>(AppDbContext dbContext) : RepositoryBase<TEntity>(dbContext), IReadRepositoryBase<TEntity>, IRepository<TEntity> where TEntity : class, IAggregateRoot
{
}
