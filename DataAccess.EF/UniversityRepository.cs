using Models;

namespace DataAccess.EF
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    public class UniversityRepository<T> : IRepository<T> where T : class
    {
    private readonly UniversityContext _context;
    protected DbSet<T> DbSet;

    public UniversityRepository(UniversityContext context)
    {
        _context = context;
        context.Database.EnsureCreated();
        DbSet = _context.Set<T>();
    }

    public List<T> GetAll()
    {
        return DbSet.ToList();
    }

    public T GetById(int id)
    {
        return DbSet.Find(id);
    }

    public T Create(T entity)
    {
        var result = DbSet.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
        _context.SaveChanges();
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
        _context.SaveChanges();
    }

    public void Remove(int id)
    {
        var entity = GetById(id);
        Remove(entity);
    }
    }
}
