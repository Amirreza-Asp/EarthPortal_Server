using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Application.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Utilities;
using System.Linq.Expressions;

namespace Persistence.Repositories
{

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        protected readonly IMapper _mapper;

        public Repository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _mapper = mapper;
        }

        public async Task<TDto?> FirstOrDefaultAsync<TDto>(Expression<Func<TEntity, bool>>? filters = null, CancellationToken cancellationToken = default) where TDto : class
        {
            return
                await _dbSet
                    .Where(filters)
                    .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default, bool isTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            if (!isTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<ListActionResult<TDto>> GetAllAsync<TDto>(GridQuery query, Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default) where TDto : class
        {
            var actionResult = new ListActionResult<TDto>();

            var queryContext = _dbSet.AsQueryable();

            //filter
            if (query.Filters != null && query.Filters.Any())
            {
                for (int i = 0; i < query.Filters.Count; i++)
                {
                    var filterExpression = QueryUtility.FilterExpression<TEntity>(query.Filters[i].column, query.Filters[i].value);
                    if (filterExpression != null)
                        queryContext = queryContext.Where(filterExpression);
                }
            }

            if (filter != null)
                queryContext = queryContext.Where(filter);

            //total count
            var total = await queryContext.CountAsync(cancellationToken);

            //sort
            if (query.Sorted != null && query.Sorted.Length > 0)
            {
                for (int i = 0; i < query.Sorted.Length; i++)
                {
                    queryContext = queryContext.SortMeDynamically(query.Sorted[i].column, query.Sorted[i].desc);
                }
            }

            var result = await queryContext
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            actionResult.Data = result.ToList();
            actionResult.Total = total;
            actionResult.Page = query.Page;
            actionResult.Size = query.Size;

            return actionResult;
        }

        public virtual async Task<List<TDto>> GetAllAsync<TDto>(Expression<Func<TEntity, bool>>? filters = null, CancellationToken cancellationToken = default) where TDto : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (filters != null)
                query = query.Where(filters);

            return
                await query
                    .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
        }

        public void Create(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Remove(object id)
        {
            var entity = _dbSet.Find(id);

            if (entity != null)
                _dbSet.Remove(entity);
        }

        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filters = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filters != null)
                query = query.Where(filters);

            return await query.CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filters = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filters != null)
                query = query.Where(filters);

            return await query.AnyAsync();
        }


    }
}
