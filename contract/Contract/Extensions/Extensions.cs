using System.ComponentModel;

namespace Contract.Extensions
{
    public static class Extensions
    {
        public static Guid? Coalesce(this Guid? guid, params Guid?[] coalesceGuids)
        {
            if (guid is not null && guid != Guid.Empty)
                return guid;

            if (coalesceGuids.Length > 0)
                return coalesceGuids.First().Coalesce(coalesceGuids.Skip(1).ToArray());

            return default;
        }

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
                throw new ArgumentException($"{nameof(enumerationValue)} necessita ser do tipo enum para obter a descrição", nameof(enumerationValue));

            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumerationValue.ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) action(item);
        }
    }
}
