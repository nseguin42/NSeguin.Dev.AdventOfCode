using System.Linq.Expressions;
using System.Reflection;

namespace NSeguin.Dev.AdventOfCode;

public static class PatchHelper
{
    /// <summary>
    ///     If <paramref name="value" /> is not null, set the property selected by <paramref name="propertySelector" />
    ///     to <paramref name="value" />.
    /// </summary>
    /// <param name="obj">The object to patch.</param>
    /// <param name="value">The value to patch with.</param>
    /// <param name="propertySelector">The property selector.</param>
    /// <typeparam name="TObject">The type of the object to patch.</typeparam>
    /// <typeparam name="TProperty">The type of the property to patch.</typeparam>
    /// <returns>
    ///     The patched object. If <typeparamref name="TObject" /> is a reference type, the instance is the same as
    ///     <paramref name="obj" />.
    /// </returns>
    public static TObject Patch<TObject, TProperty>(
        this TObject obj,
        TProperty? value,
        Expression<Func<TObject, TProperty>> propertySelector) where TObject : class
    {
        if (value is null)
        {
            return obj;
        }

        PropertyInfo propertyInfo = PropertyInfoHelper.GetPropertyInfo(propertySelector);
        PatchImpl(obj, propertyInfo, value);
        return obj;
    }

    /// <summary>
    ///     Patch all properties of <paramref name="obj" /> with the values of <paramref name="value" />.
    ///     If <paramref name="value" /> is null, nothing is patched. For each property of <typeparamref name="TObject" />
    ///     with a public getter and setter, if the corresponding property on <paramref name="value" /> is not null,
    ///     the property on <paramref name="obj" /> is set to the value of the property on <paramref name="value" />.
    /// </summary>
    /// <param name="obj">The object to patch.</param>
    /// <param name="value">The object to patch with.</param>
    /// <typeparam name="TObject">The type of the object to patch.</typeparam>
    /// <returns>
    ///     The patched object. If <typeparamref name="TObject" /> is a reference type, the instance is the same as
    ///     <paramref name="obj" />.
    /// </returns>
    public static TObject Patch<TObject>(this TObject obj, TObject? value)
    {
        if (value is null)
        {
            return obj;
        }

        IReadOnlyList<PropertyInfo> properties = Cache<TObject>.Properties;
        foreach (PropertyInfo propertyInfo in properties)
        {
            PatchImpl(obj, propertyInfo, propertyInfo.GetValue(obj));
        }

        return obj;
    }

    private static void PatchImpl<TObject, TProperty>(
        TObject obj,
        PropertyInfo propertyInfo,
        TProperty? value)
    {
        if (value is null)
        {
            return;
        }

        if (propertyInfo is not {CanRead: true, CanWrite: true})
        {
            throw new InvalidOperationException("The property is not readable and writable");
        }

        propertyInfo.SetValue(obj, value);
    }

    private static class Cache<TObject>
    {
        public static readonly IReadOnlyList<PropertyInfo> Properties = typeof(TObject)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(
                propertyInfo => propertyInfo is
                {
                    CanRead: true, CanWrite: true, SetMethod: {IsStatic: false},
                    GetMethod: {IsStatic: false}
                })
            .ToArray();
    }
}
