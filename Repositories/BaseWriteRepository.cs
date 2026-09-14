using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dorbit.Framework.Database.Abstractions;
using Dorbit.Framework.Entities.Abstractions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Repositories.Abstractions;

namespace Dorbit.Framework.Repositories;

public class BaseWriteRepository<TEntity, TKey>(IDbContext dbContext) : BaseReadRepository<TEntity, TKey>(dbContext), IWriterRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    private readonly IDbContext _dbContext = dbContext;

    public virtual Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return _dbContext.InsertEntityAsync<TEntity, TKey>(entity, cancellationToken);
    }

    public virtual async Task<TEntity> InsertIfNotExistsAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var existsEntity = await GetByIdAsync(entity.Id, cancellationToken);
        if (existsEntity is not null) return existsEntity;
        return await InsertAsync(entity, cancellationToken);
    }

    public virtual Task BulkInsertAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        var entities = Set().AsEnumerable().Where(predicate).ToList();
        return BulkInsertAsync(entities, cancellationToken);
    }

    public virtual Task BulkInsertAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return _dbContext.BulkInsertEntityAsync<TEntity, TKey>(entities, cancellationToken);
    }

    public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return _dbContext.UpdateEntityAsync<TEntity, TKey>(entity, cancellationToken);
    }

    public virtual Task BulkUpdateAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        return BulkUpdateAsync(Set().AsEnumerable().Where(predicate).ToList(), cancellationToken);
    }

    public virtual Task BulkUpdateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return _dbContext.BulkUpdateEntityAsync<TEntity, TKey>(entities, cancellationToken);
    }

    public virtual Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return _dbContext.DeleteEntityAsync<TEntity, TKey>(entity, cancellationToken);
    }

    public virtual Task BulkDeleteAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        var entities = Set().AsEnumerable().Where(predicate).ToList();
        return BulkDeleteAsync(entities, cancellationToken);
    }

    public virtual Task BulkDeleteAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return _dbContext.BulkDeleteEntityAsync<TEntity, TKey>(entities, cancellationToken);
    }

    public virtual Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id is Guid guid)
        {
            return guid != Guid.Empty ? UpdateAsync(entity, cancellationToken) : InsertAsync(entity, cancellationToken);
        }

        if (entity.Id.GetType().IsNumeric())
        {
            var longValue = Convert.ToInt64(entity.Id);
            return longValue > 0 ? UpdateAsync(entity, cancellationToken) : InsertAsync(entity, cancellationToken);
        }

        return InsertAsync(entity, cancellationToken);
    }

    //================== Extended Methods ==================\\
    public Task<TEntity> InsertWithPatchObjectAsync<TPatch>(TPatch patch, CancellationToken cancellationToken = default)
    {
        return InsertAsync(Activator.CreateInstance<TEntity>().PatchObject(patch), cancellationToken);
    }

    public async Task<TEntity> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        return await DeleteAsync(entity, cancellationToken);
    }
    
    public async Task<TEntity> UpdateWithPatchObjectAsync<TPatch>(TKey key, TPatch patch, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(key, cancellationToken);
        return await UpdateWithPatchObjectAsync(entity, patch, cancellationToken);
    }

    public async Task<TEntity> UpdateWithPatchObjectAsync<TPatch>(TEntity entity, TPatch patch, CancellationToken cancellationToken = default)
    {
        entity = entity.PatchObject(patch);
        return await UpdateAsync(entity, cancellationToken);
    }

    public async Task<TEntity> UpdateWithJsonAsync<TPatch>(TKey key, JsonElement patch, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(key, cancellationToken);
        entity = entity.PatchObjectWithJson<TEntity, TPatch>(patch);
        return await UpdateAsync(entity, cancellationToken);
    }

    public Task<TEntity> UpdateWithJsonAsync<TPatch>(TEntity entity, JsonElement patch, CancellationToken cancellationToken = default)
    {
        entity = entity.PatchObjectWithJson<TEntity, TPatch>(patch);
        return UpdateAsync(entity, cancellationToken);
    }

    public async Task<TEntity> SaveAsync<TPatch>(TKey id, TPatch patch, CancellationToken cancellationToken = default) where TPatch : class
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            entity.Id = id;
            return await UpdateWithPatchObjectAsync(entity, patch, cancellationToken);
        }
        else
        {
            entity = patch.MapTo(Activator.CreateInstance<TEntity>());
            entity.Id = id;
            return await InsertAsync(entity, cancellationToken);
        }
    }

    public async Task<TEntity> SaveAsync<TDto>(TEntity entity, TDto dto, CancellationToken cancellationToken = default)
    {
        if (entity is not null)
        {
            var id = entity.Id;
            entity = dto.MapTo(Activator.CreateInstance<TEntity>());
            entity.Id = id;
        }
        else
        {
            entity = dto.MapTo(Activator.CreateInstance<TEntity>());
        }

        return await SaveAsync(entity, cancellationToken);
    }
}

public class BaseWriteRepository<TEntity>(IDbContext dbContext) : BaseWriteRepository<TEntity, Guid>(dbContext)
    where TEntity : class, IEntity<Guid>;