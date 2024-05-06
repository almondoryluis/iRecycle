using Ardalis.Specification;

namespace Uagrm.Sw1.iRecycle.Bff.SharedKernel.Interfaces
{
    public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
    {
    }
}
