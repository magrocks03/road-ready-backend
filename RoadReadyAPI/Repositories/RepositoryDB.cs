using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    /// <summary>
    /// Abstract base repository providing shared async CRUD logic and a queryable source.
    /// </summary>
    public abstract class RepositoryDB<K, T> : IRepository<K, T> where T : class
    {
        protected readonly RoadReadyContext _context;

        public RepositoryDB(RoadReadyContext context)
        {
            _context = context;
        }

        public async Task<T> Add(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // --- CHANGE IS HERE ---
        // The return type is now nullable (Task<T?>) to match the interface.
        public async Task<T?> Delete(K key)
        {
            var entity = await GetById(key);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }

        public abstract Task<T?> GetById(K key);

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public async Task<T> Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
