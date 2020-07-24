﻿using System.Linq;
using Iwentys.Models.Exceptions;

namespace Iwentys.Database.Repositories
{
    public interface IGenericRepository<TEntity, TKey>
    {
        TEntity Create(TEntity entity);
        IQueryable<TEntity> Read();
        TEntity ReadById(TKey key);
        TEntity Update(TEntity entity);
        void Delete(TKey key);
    }

    public static class GenericRepositoryExtensions
    {
        public static TEntity Get<TEntity, TKey>(this IGenericRepository<TEntity, TKey> repository, TKey key)
        {
            return repository.ReadById(key) ?? throw EntityNotFoundException.Create(repository.GetType().Name, key);
        }
    }
}