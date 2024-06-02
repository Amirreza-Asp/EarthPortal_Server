using System.Linq.Expressions;
using System.Reflection;

namespace Persistence.Utilities
{
    public static class QueryUtility
    {
        public static IOrderedQueryable<TEntityType> SortMeDynamically<TEntityType>(this IQueryable<TEntityType> query, string propertyname, bool desc = false)
        {
            propertyname = propertyname ?? "CreatedAt";

            propertyname = typeof(TEntityType).GetPropertyExactName(propertyname);

            var param = Expression.Parameter(typeof(TEntityType), "s");
            var prop = Expression.PropertyOrField(param, propertyname);

            // برای properyهایی با type DateTime فقط بر اساس Date مرتب کند
            if (prop.Type == typeof(DateTime) || prop.Type == typeof(DateTime?))
            {
                var dateProp = Expression.Property(prop, "Date");
                var sortLambda = Expression.Lambda(dateProp, param);

                if (desc)
                    return Queryable.OrderByDescending(query, (dynamic)sortLambda);
                else
                    return Queryable.OrderBy(query, (dynamic)sortLambda);
            }
            else
            {
                var sortLambda = Expression.Lambda(prop, param);

                if (desc)
                    return Queryable.OrderByDescending(query, (dynamic)sortLambda);
                else
                    return Queryable.OrderBy(query, (dynamic)sortLambda);
            }
        }

        public static IOrderedQueryable<TEntityType> ThenSortMeDynamically<TEntityType>(this IOrderedQueryable<TEntityType> query, string propertyname, bool desc = false)
        {
            propertyname = propertyname ?? "CreatedAt";

            propertyname = typeof(TEntityType).GetPropertyExactName(propertyname);

            var param = Expression.Parameter(typeof(TEntityType), "s");
            var prop = Expression.PropertyOrField(param, propertyname);

            // برای properyهایی با type DateTime فقط بر اساس Date مرتب کند
            if (prop.Type == typeof(DateTime) || prop.Type == typeof(DateTime?))
            {
                var dateProp = Expression.Property(prop, "Date");
                var sortLambda = Expression.Lambda(dateProp, param);

                if (desc)
                    return Queryable.ThenByDescending(query, (dynamic)sortLambda);
                else
                    return Queryable.ThenBy(query, (dynamic)sortLambda);
            }
            else
            {
                var sortLambda = Expression.Lambda(prop, param);

                if (desc)
                    return Queryable.ThenByDescending(query, (dynamic)sortLambda);
                else
                    return Queryable.ThenBy(query, (dynamic)sortLambda);
            }
        }

        public static IOrderedQueryable<T> SortBy<T>(this IQueryable<T> query, string by, bool desc = true) where T : class
        {
            by = by ?? "ShortLink";

            by = typeof(T).GetPropertyExactName(by);

            //return desc
            //    ? query.OrderByDescending(o => o.GetType().GetProperty(by).GetValue(o))
            //    : query.OrderBy(o => o.GetType().GetProperty(by).GetValue(o));

            if (desc)
                return query.OrderByDescending(o => o.GetType().GetProperty(by).GetValue(o));
            return query.OrderBy(o => o.GetType().GetProperty(by).GetValue(o));
        }


        public static Expression<Func<T, bool>> FilterExpression<T>(string propertyName, string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;
            try
            {
                propertyName = typeof(T).GetPropertyExactName(propertyName);
                var propertyType = typeof(T).GetPropertyType(propertyName);

                object castedPropertyValue = new object();
                string @operator = string.Empty;
                if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                {
                    @operator = "Equals";
                    castedPropertyValue = bool.Parse(propertyValue);
                }
                if (propertyType == typeof(int) || propertyType == typeof(int?))
                {
                    @operator = "Equals";
                    castedPropertyValue = int.Parse(propertyValue);
                }
                if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                {
                    @operator = "Equals";
                    castedPropertyValue = Guid.Parse(propertyValue);
                }
                if (propertyType == typeof(string))
                {
                    @operator = "Contains";
                    castedPropertyValue = propertyValue;
                }



                var parameterExp = Expression.Parameter(typeof(T), "type");
                var propertyExp = Expression.Property(parameterExp, propertyName);


                var someValue = Expression.Constant(castedPropertyValue, propertyType);

                if (propertyType == typeof(int?))
                {

                    MethodInfo methodbool = propertyType.GetMethod(@operator, new[] { propertyType });

                    var convertedSomeValue = Expression.Convert(someValue, typeof(object));

                    var containsMethodExp = Expression.Call(propertyExp, methodbool, convertedSomeValue);
                    return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);

                }
                if (propertyType == typeof(Guid?))
                {

                    MethodInfo methodbool = propertyType.GetMethod(@operator, new[] { propertyType });

                    var convertedSomeValue = Expression.Convert(someValue, typeof(object));

                    var containsMethodExp = Expression.Call(propertyExp, methodbool, convertedSomeValue);
                    return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);

                }
                if (propertyType == typeof(bool?))
                {

                    MethodInfo methodbool = propertyType.GetMethod(@operator, new[] { propertyType });

                    var convertedSomeValue = Expression.Convert(someValue, typeof(object));

                    var containsMethodExp = Expression.Call(propertyExp, methodbool, convertedSomeValue);
                    return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);

                }
                else
                {
                    MethodInfo method = propertyType.GetMethod(@operator, new[] { propertyType });

                    var containsMethodExp = Expression.Call(propertyExp, method, someValue);
                    return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);

                }


            }
            catch (Exception ex)
            {

                return null;
            }

        }


        public static Expression<Func<T, T>> SelectExpression<T>(string propertyName, string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;
            try
            {
                propertyName = typeof(T).GetPropertyExactName(propertyName);
                var propertyType = typeof(T).GetPropertyType(propertyName);

                var parameterExp = Expression.Parameter(typeof(T), "type");
                var propertyExp = Expression.Property(parameterExp, propertyName);


                // var someValue = Expression.Constant(castedPropertyValue, propertyType);
                //var containsMethodExp = Expression.Call(propertyExp, method, someValue);

                return Expression.Lambda<Func<T, T>>(null, parameterExp);
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        static string GetPropertyExactName(this Type type, string name)
        {
            return type.GetProperties()
                 .Where(x => x.Name.ToLower() == name.ToLower())
                 .Select(x => x.Name)
                 .FirstOrDefault();
        }

        static Type GetPropertyType(this Type type, string name)
        {
            return type.GetProperties()
                 .Where(x => x.Name.ToLower() == name.ToLower())
                 .Select(x => x.PropertyType)
                 .FirstOrDefault();
        }

        public static IQueryable<T> SelectDynamic<T>(this IQueryable<T> source, IEnumerable<string> fields)
        {
            var fieldNames = new List<string>();
            foreach (var field in fields)
            {
                fieldNames.Add(typeof(T).GetPropertyExactName(field));
            }

            ParameterExpression expression = Expression.Parameter(typeof(T), "s");
            MemberBinding[] bindings = new MemberBinding[fields.Count()];

            for (int i = 0; i < fields.Count(); i++)
            {
                var name = fieldNames.ElementAt(i);
                var binding = Expression.Bind(typeof(T).GetProperty(name), Expression.PropertyOrField(expression, name));
                bindings[i] = binding;
            }

            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            var lambda = Expression.Lambda<Func<T, T>>(Expression.MemberInit(Expression.New(typeof(T)), bindings), parameters);

            return source.Select(lambda);
        }

    }
}
