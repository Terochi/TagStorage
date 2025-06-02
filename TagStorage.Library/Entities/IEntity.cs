using System.Reflection;

namespace TagStorage.Library.Entities;

public interface IEntity
{
    int Id { get; }

    public string[] GetFieldNames(bool ignoreId = true)
    {
        var propertyInfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).AsEnumerable();

        if (ignoreId)
        {
            propertyInfos = propertyInfos.Where(p => p.Name != nameof(Id));
        }

        return propertyInfos
               .Select(p => p.Name.ToLowerInvariant())
               .ToArray();
    }

    public string[] GetFieldValues(bool ignoreId = true)
    {
        var propertyInfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).AsEnumerable();

        if (ignoreId)
        {
            propertyInfos = propertyInfos.Where(p => p.Name != nameof(Id));
        }

        return propertyInfos
               .Select(p => $"'{p.GetValue(this)!}'")
               .ToArray()!;
    }
}
