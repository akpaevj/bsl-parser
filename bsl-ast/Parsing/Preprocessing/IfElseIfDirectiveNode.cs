using BSL.AST.Lexing;
using BSL.AST.Parsing.Nodes;
using BSL.AST.Parsing.Nodes.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Parsing.Preprocessing
{
    internal class IfElseIfDirectiveNode : BslNode
    {
        public BslToken IfElseIfToken { get; internal set; } = null!;
        public ExpressionNode Expression { get; internal set; } = null!;
        public BslToken ThenToken { get; internal set; } = null!;
    }
}
