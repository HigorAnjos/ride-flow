using MassTransit;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Infrastructure.MessageBus.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class MemberInfoExtensions
    {
        private static readonly KebabCaseEndpointNameFormatter Instance = new(false);

        public static string ToKebabCaseString(this MemberInfo member)
            => Instance.SanitizeName(member.Name);
    }
}
