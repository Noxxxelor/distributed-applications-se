using Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Repositories
{
    public class BaseRepository<T>
        where T : class
    {
        private DbContext Context { get; set; }
        private DbSet<T> Items { get; set; }

        // Конструктор, который создает контекст и инициализирует DbSet
        public BaseRepository()
        {
            Context = new AppDbContext();
            Items = Context.Set<T>();
        }

        // Получаем все элементы из базы с возможностью фильтрации
        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = Items;

            // Если фильтр передан, применяем его
            if (filter != null)
                query = query.Where(filter);

            return query.ToList();
        }

        // Получаем первый элемент, соответствующий фильтру (если такой есть)
        public T FirstOrDefault(Expression<Func<T, bool>> filter)
        {
            return Items.FirstOrDefault(filter);
        }

        // Добавляем новый элемент в базу
        public void Add(T item)
        {
            Items.Add(item);
            Context.SaveChanges();
        }

        // Обновляем существующий элемент
        public void Update(T item)
        {
            Items.Update(item);
            Context.SaveChanges();
        }

        // Удаляем элемент из базы
        public void Delete(T item)
        {
            Items.Remove(item);
            Context.SaveChanges();
        }

        // Получаем элемент по Id
        public T GetById(int id)
        {
            return Items.Find(id);
        }
    }
}
