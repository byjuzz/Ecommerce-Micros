using System.Text.Json;

namespace ServiceCommon.Mapping
{
    public static class DtoMapperExtension
    {
        public static T? MapTo<T>(this object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            return JsonSerializer.Deserialize<T>(
                JsonSerializer.Serialize(value)
            );
        }

    }
}

//public static T MapTo<T>(this object value) // que el methodo de descerializacion no deba retornar null
//{
//    if (value == null) throw new ArgumentNullException(nameof(value));

//    var json = JsonSerializer.Serialize(value);
//    var result = JsonSerializer.Deserialize<T>(json);

//    if (result == null)
//    {
//        throw new InvalidOperationException($"No se pudo mapear el objeto al tipo {typeof(T)}.");
//    }

//    return result;
//}
