using Newtonsoft.Json;

namespace Caprini.Extensions
{
    public static class ObjetcExtension
    {
        public static T DeepCopy<T>(this T other) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(other));
            }
            catch
            {
                return default;
            }
        }
    }
}