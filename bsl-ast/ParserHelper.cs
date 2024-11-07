using System.Runtime.CompilerServices;

namespace BSL.AST
{
	internal static class ParserHelper
    {
        internal static bool BilingualTokenValueIs(ReadOnlySpan<char> sequence, string ru, string en)
            => sequence.Equals(ru, StringComparison.OrdinalIgnoreCase)
                || sequence.Equals(en, StringComparison.OrdinalIgnoreCase);
    }
}