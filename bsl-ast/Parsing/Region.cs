﻿using BSL.AST.Lexing;

namespace BSL.AST.Parsing
{
	public class Region
	{
		public BslTrivia StartTrivia { get; internal set; } = default;
		public BslTrivia FinishTrivia { get; internal set; } = default;

		public string Name { get; internal set; } = string.Empty;
	}
}