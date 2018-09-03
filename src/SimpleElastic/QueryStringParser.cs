using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleElastic
{
    internal static class QueryStringParser
    {
        private static MemoryCache _methodCache = new MemoryCache(new MemoryCacheOptions());

        public static string GetQueryString(object source)
        {
            string result = null;

            switch (source)
            {
                case null:
                    break;
                case IDictionary<string, object> dictionary:
                    result = String.Join("&", dictionary.Select(v => $"{v.Key}={v.Value}"));
                    break;
                case Map map:
                    result = String.Join("&", map.Values.Select(v => $"{v.Key}={v.Value}"));
                    break;
                default:
                    result = _methodCache.GetOrCreate(source.GetType(), CreateMap)(source);
                    break;
            }

            if (String.IsNullOrEmpty(result))
            {
                return String.Empty;
            }
            else
            {
                return String.Concat("?", result);
            }
        }

        private static Func<object, string> CreateMap(ICacheEntry cacheEntry)
        {
            var sourceType = cacheEntry.Key as Type;

            var properties = sourceType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .OrderBy(p => p.Name); // Ensure we get a consistent result for unit tests

            var formatString = String.Join("&", properties.Select((p, i) => $"{p.Name}={{{i}}}"));
            var formatMethod = typeof(String).GetMethod(nameof(String.Format), new[] { typeof(string), typeof(object[]) });

            var parameter = Expression.Parameter(typeof(object));
            var castParameter = sourceType == typeof(object) ? (Expression)parameter : Expression.Convert(parameter, sourceType);

            var propertyAccessors = properties
                .Select(p => Expression.Property(castParameter, p))
                .Select(p =>
                {
                    if (p.Type == typeof(bool))
                    {
                        var toString = typeof(Boolean).GetMethod(nameof(Boolean.ToString), Type.EmptyTypes);
                        var toLower = typeof(String).GetMethod(nameof(String.ToLower), Type.EmptyTypes);

                        return (Expression)Expression.Call(Expression.Call(p, toString), toLower);
                    }
                    if (p.Type != typeof(object))
                    {
                        return Expression.Convert(p, typeof(object));
                    }

                    return p;
                });

            var arguments = Expression.NewArrayInit(typeof(object), propertyAccessors);
            var call = Expression.Call(formatMethod, Expression.Constant(formatString), arguments);
            return Expression.Lambda<Func<object, string>>(call, parameter).Compile();
        }
    }
}