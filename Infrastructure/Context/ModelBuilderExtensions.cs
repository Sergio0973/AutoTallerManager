using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Context;

internal static class ModelBuilderExtensions
{
    public static void ApplySingleValueObjectConversions(this ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes().ToList();

        foreach (var entityType in entityTypes)
        {
            var clrType = entityType.ClrType;
            var properties = clrType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead);

            foreach (var property in properties)
            {
                if (!TryCreateValueConverter(property.PropertyType, out var converter))
                {
                    continue;
                }

                modelBuilder.Entity(clrType)
                    .Property(property.Name)
                    .HasConversion(converter);
            }
        }
    }

    private static bool TryCreateValueConverter(Type propertyType, out ValueConverter converter)
    {
        converter = null!;

        var valueProperty = propertyType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
        if (valueProperty is null)
        {
            return false;
        }

        var createMethod = propertyType.GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static,
            null,
            new[] { valueProperty.PropertyType },
            null);

        if (createMethod is null)
        {
            return false;
        }

        var factoryMethod = typeof(ModelBuilderExtensions)
            .GetMethod(nameof(CreateConverter), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(propertyType, valueProperty.PropertyType);

        converter = (ValueConverter)factoryMethod.Invoke(null, null)!;
        return true;
    }

    private static ValueConverter CreateConverter<TValueObject, TValue>()
        where TValueObject : notnull
    {
        var valueObjectParameter = Expression.Parameter(typeof(TValueObject), "valueObject");
        var toProviderExpression = Expression.Lambda<Func<TValueObject, TValue>>(
            Expression.Property(valueObjectParameter, "Value"),
            valueObjectParameter);

        var providerParameter = Expression.Parameter(typeof(TValue), "value");
        var createMethod = typeof(TValueObject).GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static,
            null,
            new[] { typeof(TValue) },
            null)!;

        var fromProviderExpression = Expression.Lambda<Func<TValue, TValueObject>>(
            Expression.Call(createMethod, providerParameter),
            providerParameter);

        return new ValueConverter<TValueObject, TValue>(toProviderExpression, fromProviderExpression);
    }
}
