namespace FlagExplorerApp.Infrastructure.Persistance.Documents.Extensions;

internal static class TypeExtensionMethods
{
    public static string GetNameForDocument(this Type type)
    {
        if (type.IsArray)
        {
            return GetNameForDocument(type.GetElementType()!) + "[]";
        }

        if (type.IsGenericType)
        {
            return $"{type.Name[..type.Name.LastIndexOf("`", StringComparison.InvariantCulture)]}<{string.Join(", ", type.GetGenericArguments().Select(GetNameForDocument))}>";
        }

        return type.Name;
    }
}
