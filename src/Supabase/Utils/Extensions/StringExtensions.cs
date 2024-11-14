using System.Diagnostics.CodeAnalysis;

namespace Supabase.Utils.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);
}