using System.Linq.Expressions;

namespace Persistence.Utilities
{
    public static class DbContextExtentions
    {

        public static IOrderedQueryable<TSource> DynamicOrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool asc = true)
        {
            if (asc)
                return source.OrderBy(keySelector);
            else
                return source.OrderByDescending(keySelector);
        }

    }
}
