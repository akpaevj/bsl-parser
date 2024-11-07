using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Tests.Parser
{
	public class ErrorRecoveringTests
	{
		[Fact]
		public void Test()
		{
			var data = TestHelper.ParseModule("Перем А; 1 > 0;");
		}
	}
}
