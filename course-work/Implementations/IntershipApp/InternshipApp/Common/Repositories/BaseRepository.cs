using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Repositories
{
    public class BaseRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _items;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _items = context.Set<T>();
        }

        // Новый метод, возвращающий IQueryable для запросов
        public IQueryable<T> GetAllQueryable(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _items.AsNoTracking();
            if (filter != null)
                query = query.Where(filter);
            return query;
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return GetAllQueryable(filter).ToList();
        }

        public T GetById(int id) => _items.Find(id);

        public T FirstOrDefault(Expression<Func<T, bool>> filter)
            => _items.FirstOrDefault(filter);

        public void Add(T item)
        {
            _items.Add(item);
            _context.SaveChanges();
        }

        public void Update(T item)
        {
            _items.Update(item);
            _context.SaveChanges();
        }

        public void Delete(T item)
        {
            _items.Remove(item);
            _context.SaveChanges();
        }
    }
}
