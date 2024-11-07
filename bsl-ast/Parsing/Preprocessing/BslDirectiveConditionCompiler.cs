using BSL.AST.Diagnostics;
using BSL.AST.Parsing;
using BSL.AST.Parsing.Nodes.Expressions;
using BSL.AST.Parsing.Nodes.Expressions.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Parsing.Preprocessing
{
    internal class BslDirectiveConditionCompiler
    {
        public static (BslCompileContexts Context, IReadOnlyList<Diagnostic> Errors) Compile(ReadOnlySpan<char> source)
        {
            var parser = new BslParser();
            var directiveNode = parser.ParseIfElseIfDirective(source);

            if (parser.Diagnostics.Count > 0)
                return (BslCompileContexts.None, parser.Diagnostics);
            else
            {
                try
                {
                    return (HandleNode(directiveNode.Expression), []);
                }
                catch (BslSyntaxErrorException ex)
                {
                    return (BslCompileContexts.None, [ex.Error]);
                }
            }
        }

        private static BslCompileContexts HandleNode(ExpressionNode node)
        {
            if (node is AndExpressionNode and)
                return AndNode(and);
            else if (node is OrExpressionNode or)
                return OrNode(or);
            else if (node is NotExpressionNode not)
                return NotNode(not);
            else if (node is ParenthesizedExpressionNode parent)
                return ParenthesizedNode(parent);
            else if (node is IdentifierExpressionNode id)
            {
                if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "Сервер", "Server") ||
                    ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "НаСервере", "AtServer"))
                    return BslCompileContexts.Server;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "Клиент", "Client") ||
                    ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "НаКлиенте", "AtClient"))
                    return BslCompileContexts.Client;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "ТонкийКлиент", "ThinClient"))
                    return BslCompileContexts.ThinClient;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "МобильныйКлиент", "MobileClient"))
                    return BslCompileContexts.MobileClient;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "ВебКлиент", "WebClient"))
                    return BslCompileContexts.WebClient;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "ВнешнееСоединение", "ExternalConnection"))
                    return BslCompileContexts.ExternalConnection;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "ТолстыйКлиентУправляемоеПриложение", "ThickClientManagedApplication"))
                    return BslCompileContexts.ThickClientManagedApplication;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "ТолстыйКлиентОбычноеПриложение", "ThickClientOrdinaryApplication"))
                    return BslCompileContexts.ThickClientOrdinaryApplication;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "МобильноеПриложениеКлиент", "MobileAppClient"))
                    return BslCompileContexts.MobileAppClient;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "МобильноеПриложениеСервер", "MobileAppServer"))
                    return BslCompileContexts.MobileAppServer;
                else if (ParserHelper.BilingualTokenValueIs(id.IdentifierToken.Text, "МобильныйАвтономныйСервер", "MobileStandaloneServer"))
                    return BslCompileContexts.MobileStandaloneServer;
                else
                    throw new BslSyntaxErrorException(SyntaxDiagnosticsFactory.UnexpectedPreprocessorDirective(node.Position));
            }
            else
                throw new BslSyntaxErrorException(SyntaxDiagnosticsFactory.UnexpectedPreprocessorExpression(node.Position));
        }

        private static BslCompileContexts AndNode(AndExpressionNode node)
            => HandleNode(node.Left) & HandleNode(node.Right);

        private static BslCompileContexts OrNode(OrExpressionNode node)
            => HandleNode(node.Left) | HandleNode(node.Right);

        private static BslCompileContexts NotNode(NotExpressionNode node)
            => BslCompileContexts.All ^ HandleNode(node.Operand);

		private static BslCompileContexts ParenthesizedNode(ParenthesizedExpressionNode node)
			=> HandleNode(node.Expression);
	}
}
