using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    /// <summary>
    /// Generic repository interface updated for asynchronous operations and pagination.
    /// </summary>
    public interface IRepository<K, T> where T : class
    {
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        // --- CHANGE IS HERE ---
        // The return type is now nullable (T?) to indicate that null can be returned if the entity is not found.
        Task<T?> Delete(K key);
        Task<T?> GetById(K key);
        IQueryable<T> GetAll();
    }
}
