using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST
{
	public static class ReadOnlySpanExtensions
	{
		public static Range FirstWordRange(this ReadOnlySpan<char> span)
		{
			var startIndex = 0;
			int currentIndex;
			var started = false;
			for (currentIndex = 0; currentIndex < span.Length; currentIndex++)
			{
				var ch = span[currentIndex];

				if (!started && char.IsLetter(ch))
				{
					startIndex = currentIndex;
					started = true;
					continue;
				}

				if (started && !char.IsLetter(ch))
					break;
			}

			return new Range(startIndex, currentIndex);
		}
	}
}
