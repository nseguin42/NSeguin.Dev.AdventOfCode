using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace NSeguin.Dev.AdventOfCode;

public static class PropertyInfoHelper
{
    public static PropertyInfo GetPropertyInfo<TObject, TProperty>(
        Expression<Func<TObject, TProperty>> propertySelector)
    {
        return Cache<TObject, TProperty>.GetPropertyInfo(propertySelector);
    }

    private static class Cache<TObject, TProperty>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ConcurrentDictionary<string, PropertyInfo> PropertySelectors
            = new();

        public static PropertyInfo GetPropertyInfo(
            Expression<Func<TObject, TProperty>> propertySelector)
        {
            string key = propertySelector.ToString();
            return PropertySelectors.TryGetValue(key, out PropertyInfo? propertyInfo)
                ? propertyInfo
                : GetAndCachePropertyInfo(propertySelector, key);
        }

        private static PropertyInfo GetAndCachePropertyInfo(
            Expression<Func<TObject, TProperty>> propertySelector,
            string key)
        {
            if (propertySelector.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException(
                    "The expression is not a member expression",
                    nameof(propertySelector));
            }

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException(
                    "The member is not a property",
                    nameof(propertySelector));
            }

            PropertySelectors[key] = propertyInfo;
            return propertyInfo;
        }
    }
}
