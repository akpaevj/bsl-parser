using BSL.AST.Parsing;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace BSL.AST.Lexing
{
    public partial class BslLexer
    {
        private BslKind _languageKind;

		private int _currentPosition = 0;
        private int _currentColumn = 0;
        private int _currentLine = 1;
        private readonly List<BslToken> _tokens = [];
        private List<BslTrivia> _leadingTrivias = [];
        private bool _readMultilineString = false;
        private int _countedQuotes = 0;

		public IReadOnlyList<BslToken> Tokenize(ReadOnlySpan<char> source, BslKind bslKind = BslKind.OneC)
        {
            _languageKind = bslKind;

			while (_currentPosition < source.Length)
            {
				_leadingTrivias = Trivias(source);

				if (_currentPosition == source.Length)
                    break;

                var peek = source[_currentPosition];

                if (_readMultilineString || peek == '"')
                    _tokens.Add(StringLiteral(source));
				else if (peek == '.')
					AddSimpleToken(source, TokenType.DOT);
				else if (peek == ',')
					AddSimpleToken(source, TokenType.COMMA);
				else if (peek == ';')
					AddSimpleToken(source, TokenType.SEMICOLON);
				else if (peek == '=')
					AddSimpleToken(source, TokenType.EQUAL);
				else if (peek == '(')
					AddSimpleToken(source, TokenType.OPEN_PARENT);
				else if (peek == ')')
					AddSimpleToken(source, TokenType.CLOSE_PARENT);
				else if (peek == '[')
					AddSimpleToken(source, TokenType.OPEN_BRACKET);
				else if (peek == ']')
					AddSimpleToken(source, TokenType.CLOSE_BRACKET);
				else if (peek == '\'')
                    _tokens.Add(DateTimeLiteral(source));
                else if (char.IsDigit(peek))
                    _tokens.Add(NumericLiteral(source));
                else if (peek == '/')
                    if (NextChar(source) == '/')
                        throw new NotImplementedException();
                    else
                        AddSimpleToken(source, TokenType.DIVIDE);
                else if (peek == '+')
                    AddSimpleToken(source, TokenType.PLUS);
                else if (peek == '-')
                    AddSimpleToken(source, TokenType.MINUS);
				else if (peek == '?')
					AddSimpleToken(source, TokenType.QUESTION_MARK);
				else if (peek == '*')
                    AddSimpleToken(source, TokenType.MULTIPLY);
                else if (peek == '%')
                    AddSimpleToken(source, TokenType.MODULO);
                else if (peek == '>')
                    if (NextChar(source) == '=')
                        AddSimpleToken(source, TokenType.GREATER_OR_EQUAL, 2);
                    else
                        AddSimpleToken(source, TokenType.GREATER);
                else if (peek == '<')
                {
                    var nextChar = NextChar(source);

                    if (nextChar == '=')
                        AddSimpleToken(source, TokenType.LESS_OR_EQUAL, 2);
                    else if (nextChar == '>')
                        AddSimpleToken(source, TokenType.NOT_EQUAL, 2);
                    else
                        AddSimpleToken(source, TokenType.LESS);
                }
                else if (peek == '&')
                    AddSimpleToken(source, TokenType.AMPERSAND);
				else if (peek == '~')
					AddSimpleToken(source, TokenType.MARK);
				else if (peek == ':')
					AddSimpleToken(source, TokenType.END_OF_MARK);
				else if (TryGetCharSequence(source, out var position))
                {
                    if (TryGetKeyword(source, position, out var token))
                        _tokens.Add(token);
                    else if (TryGetBooleanLiteral(source, position, out token))
                        _tokens.Add(token);
                    else if (TryGetUndefinedLiteral(source, position, out token))
                        _tokens.Add(token);
                    else if (TryGetNullLiteral(source, position, out token))
                        _tokens.Add(token);
                    else
                        _tokens.Add(GetIdentifier(source, position));
                }
                else
                    AddUnknownToken(source, position);

                var lastToken = _tokens[^1];

				lastToken.LeadingTrivias = _leadingTrivias;
				lastToken.TrailingTrivias = Trivias(source, true);
			}

			var eofToken = new BslToken("", TokenType.EOF, new SourcePosition(_currentPosition, _currentPosition, _currentLine, _currentColumn))
			{
				LeadingTrivias = _leadingTrivias
			};
			_tokens.Add(eofToken);

			return _tokens;
        }

        private BslToken StringLiteral(ReadOnlySpan<char> source)
        {
			var startPosition = _currentPosition;
			var startColumn = _currentColumn;
			var line = _currentLine;

            var endOffset = 0;

			while (_currentPosition < source.Length)
			{
				if (source[_currentPosition] == '\r')
                {
                    _readMultilineString = true;
					break;
				}

				var nextCh = NextChar(source);

				if (source[_currentPosition] == '"')
					_countedQuotes++;

				_currentPosition++;
				_currentColumn++;

				if (_countedQuotes > 1 && _countedQuotes % 2 == 0 && nextCh != '"')
                {
                    endOffset = 1;
					_readMultilineString = false;
					break;
				}
			}

            if (!_readMultilineString)
                _countedQuotes = 0;

			var position = new SourcePosition(startPosition, _currentPosition, line, startColumn);
			var value = source[(startPosition + 1)..(_currentPosition - endOffset)];

			return new(value.ToString().Replace("\"\"", "\""), TokenType.STRING, position);
        }

		private BslToken DateTimeLiteral(ReadOnlySpan<char> source)
        {
			var startPosition = _currentPosition;
			var startColumn = _currentColumn;
            var started = false;

			while (_currentPosition < source.Length)
			{
                var ch = source[_currentPosition];

				_currentPosition++;
				_currentColumn++;

				if (ch == '\'' && !started)
                {
					started = true;
                    continue;
				}

				if (ch == '\'' && started)
                    break;
			}

			var position = new SourcePosition(startPosition, _currentPosition, _currentLine, startColumn);
			var value = source[startPosition.._currentPosition];

			return new(value.ToString(), TokenType.DATE, position);
        }

        private bool TryGetCharSequence(ReadOnlySpan<char> source, out SourcePosition position)
        {
            var startPosition = _currentPosition;
			var startColumn = _currentColumn;

			while (_currentPosition < source.Length)
            {
                var ch = source[_currentPosition];

                if (char.IsLetterOrDigit(ch) || ch == '_')
                {
					_currentPosition++;
					_currentColumn++;
				}
				else
                    break;
            }

            if (startPosition != _currentPosition)
            {
				position = new SourcePosition(startPosition, _currentPosition, _currentLine, startColumn);
                return true;
			}

            position = default;
            return false;
        }

        private static bool TryGetBooleanLiteral(ReadOnlySpan<char> source, SourcePosition position, out BslToken token)
        {
            var charSequence = source[position.StartPosition..position.EndPosition];

            if (ParserHelper.BilingualTokenValueIs(charSequence, "ИСТИНА", "TRUE"))
            {
				token = new BslToken(source[position.StartPosition..position.EndPosition].ToString(), TokenType.TRUE, position);
                return true;
            }
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЛОЖЬ", "FALSE"))
            {
                token = new BslToken(source[position.StartPosition..position.EndPosition].ToString(), TokenType.FALSE, position);
				return true;
            }

            token = default;

            return false;
        }

        private BslToken NumericLiteral(ReadOnlySpan<char> source)
        {
            var startPosition = _currentPosition;
			var startColumn = _currentColumn;

			while (_currentPosition < source.Length)
            {
                var c = source[_currentPosition];

                if (char.IsDigit(c) || c == '.')
				{
					_currentPosition++;
					_currentColumn++;
				}
				else
                    break;
            }

            var position = new SourcePosition(startPosition, _currentPosition, _currentLine, startColumn);
            var value = source[position.StartPosition..position.EndPosition];

            return new(value.ToString(), TokenType.NUMBER, position);
        }

        private static bool TryGetUndefinedLiteral(ReadOnlySpan<char> source, SourcePosition position, out BslToken token)
        {
            var charSequence = source[position.StartPosition..position.EndPosition];

            if (ParserHelper.BilingualTokenValueIs(charSequence, "НЕОПРЕДЕЛЕНО", "UNDEFINED"))
            {
                token = new BslToken(charSequence.ToString(), TokenType.UNDEFINED, position);
                return true;
            }

            token = default;

            return false;
        }

        private static bool TryGetNullLiteral(ReadOnlySpan<char> source, SourcePosition position, out BslToken token)
        {
            var charSequence = source[position.StartPosition..position.EndPosition];

            if (charSequence.Equals("NULL", StringComparison.Ordinal))
            {
                token = new BslToken(source[position.StartPosition..position.EndPosition].ToString(), TokenType.NULL, position);
                return true;
            }

            token = default;

            return false;
        }

        private static bool TryGetKeyword(ReadOnlySpan<char> source, SourcePosition position, out BslToken token)
        {
            var charSequence = source[position.StartPosition..position.EndPosition];

			TokenType? type = null;

			if (ParserHelper.BilingualTokenValueIs(charSequence, "ПРОЦЕДУРА", "PROCEDURE"))
				type = TokenType.PROCEDURE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ФУНКЦИЯ", "FUNCTION"))
				type = TokenType.FUNCTION;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦПРОЦЕДУРЫ", "ENDPROCEDURE"))
				type = TokenType.END_PROCEDURE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦФУНКЦИИ", "ENDFUNCTION"))
				type = TokenType.END_FUNCTION;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЭКСПОРТ", "EXPORT"))
				type = TokenType.EXPORT;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЕСЛИ", "IF"))
                type = TokenType.IF;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ТОГДА", "THEN"))
                type = TokenType.THEN;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИНАЧЕЕСЛИ", "ELSIF"))
                type = TokenType.ELSE_IF;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИНАЧЕ", "ELSE"))
                type = TokenType.ELSE;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦЕСЛИ", "ENDIF"))
                type = TokenType.END_IF;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ВОЗВРАТ", "RETURN"))
				type = TokenType.RETURN;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ДЛЯ", "FOR"))
                type = TokenType.FOR;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "КАЖДОГО", "EACH"))
                type = TokenType.EACH;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИЗ", "IN"))
                type = TokenType.IN;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПО", "TO"))
                type = TokenType.TO;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПОКА", "WHILE"))
                type = TokenType.WHILE;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЦИКЛ", "DO"))
                type = TokenType.DO;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦЦИКЛА", "ENDDO"))
                type = TokenType.END_DO;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "И", "AND"))
				type = TokenType.AND;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИЛИ", "OR"))
				type = TokenType.OR;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "НЕ", "NOT"))
				type = TokenType.NOT;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПОПЫТКА", "TRY"))
				type = TokenType.TRY;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ИСКЛЮЧЕНИЕ", "EXCEPT"))
				type = TokenType.EXCEPT;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ВЫЗВАТЬИСКЛЮЧЕНИЕ", "RAISE"))
				type = TokenType.RAISE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "КОНЕЦПОПЫТКИ", "ENDTRY"))
				type = TokenType.END_TRY;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПРОДОЛЖИТЬ", "CONTINUE"))
				type = TokenType.CONTINUE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПРЕРВАТЬ", "BREAK"))
				type = TokenType.BREAK;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "НОВЫЙ", "NEW"))
				type = TokenType.NEW;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПЕРЕМ", "VAR"))
                type = TokenType.VAR;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ПЕРЕЙТИ", "GOTO"))
                type = TokenType.GO_TO;
            else if (ParserHelper.BilingualTokenValueIs(charSequence, "ВЫПОЛНИТЬ", "EXECUTE"))
                type = TokenType.EXECUTE;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ЗНАЧ", "VAL"))
				type = TokenType.VAL;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "ДОБАВИТЬОБРАБОТЧИК", "ADDHANDLER"))
				type = TokenType.ADD_HANDLER;
			else if (ParserHelper.BilingualTokenValueIs(charSequence, "УДАЛИТЬОБРАБОТЧИК", "REMOVEHANDLER"))
				type = TokenType.REMOVE_HANDLER;

			if (type == null)
            {
				token = default;
                return false;
			}
            else
            {
				var value = source[position.StartPosition..position.EndPosition];
				token = new BslToken(value.ToString(), (TokenType)type, position);
                return true;
			}
        }

        private static BslToken GetIdentifier(ReadOnlySpan<char> source, SourcePosition position)
        {
			var value = source[position.StartPosition..position.EndPosition];
			return new BslToken(value.ToString(), TokenType.IDENTIFIER, position);
        }

		private void AddUnknownToken(ReadOnlySpan<char> source, SourcePosition position = default)
		{
            if (position.Equals(default))
            {
                var startPosition = _currentPosition;
                var startColumn = _currentColumn;

				while (_currentPosition < source.Length)
				{
					var c = source[_currentPosition];

					if (c == ';' || c == '\r')
						break;
					else
                    {
						_currentPosition++;
						_currentColumn++;
					}
				}

                position = new SourcePosition(startPosition, _currentPosition, _currentLine, startColumn);
			}

			var value = source[position.StartPosition..position.EndPosition];
			var token = new BslToken(value.ToString(), TokenType.UNKNOWN, position);

            _tokens.Add(token);
		}

        private char NextChar(ReadOnlySpan<char> source)
            => _currentPosition + 1 >= source.Length ? '\0' : source[_currentPosition + 1];

        private void AddSimpleToken(ReadOnlySpan<char> source, TokenType tokenType, int length = 1)
        {
            var startPosition = _currentPosition;
            var startColumn = _currentColumn;
            _currentPosition += length;
			_currentColumn += length;

			var position = new SourcePosition(startPosition, _currentPosition, _currentLine, startColumn);

			var value = source[position.StartPosition..position.EndPosition];
			_tokens.Add(new BslToken(value.ToString(), tokenType, position));
        }

        private List<BslTrivia> Trivias(ReadOnlySpan<char> source, bool trailing = false)
        {
            var items = new List<BslTrivia>();

            while (_currentPosition < source.Length)
            {
                var peek = source[_currentPosition];

                if (peek == '\r')
                {
                    EndOfLineTrivia(source, items);

                    if (trailing)
                        break;
                }
                else if (char.IsWhiteSpace(peek))
                    WhiteSpaceTrivia(source, items);
                else if (peek == '/' && NextChar(source) == '/')
                    CommentTrivia(source, items);
				else if (peek == '#')
					PreprocessingTrivia(source, items);
				else
                    break;
            }

            return items;
        }

		private void WhiteSpaceTrivia(ReadOnlySpan<char> source, List<BslTrivia> trivias)
		{
			var startPosition = _currentPosition;
			var startColumn = _currentColumn;

			while (_currentPosition < source.Length)
			{
				var c = source[_currentPosition];

				if (char.IsWhiteSpace(c) && c != '\r')
				{
					_currentPosition++;
					_currentColumn++;
				}
				else
					break;
			}

			var position = new SourcePosition(startPosition, _currentPosition, _currentLine, startColumn);
			var value = source[position.StartPosition..position.EndPosition];

			trivias.Add(new(BslTriviaKind.WhiteSpace, value.ToString(), position));
		}

		private void EndOfLineTrivia(ReadOnlySpan<char> source, List<BslTrivia> trivias)
		{
			var startPosition = _currentPosition;
			var startColumn = _currentColumn;

			if (NextChar(source) == '\n')
				_currentPosition++;

			_currentPosition++;

			var position = new SourcePosition(startPosition, _currentPosition, _currentLine, startColumn);
			_currentLine++;
			_currentColumn = 0;

			var value = source[position.StartPosition..position.EndPosition];

			trivias.Add(new(BslTriviaKind.EndOfLine, value.ToString(), position));
		}

		private void CommentTrivia(ReadOnlySpan<char> source, List<BslTrivia> trivias)
		{
			var startPosition = _currentPosition;
			var startColumn = _currentColumn;
			var line = _currentLine;

			while (_currentPosition < source.Length)
			{
				if (source[_currentPosition] == '\r')
					break;

				_currentPosition++;
				_currentColumn++;
			}

			var position = new SourcePosition(startPosition, _currentPosition, line, startColumn);
			var value = source[position.StartPosition..position.EndPosition];

            trivias.Add(new(BslTriviaKind.Comment, value.ToString(), position));
		}

        private void PreprocessingTrivia(ReadOnlySpan<char> source, List<BslTrivia> trivias)
        {
			var startPosition = _currentPosition;
			var startColumn = _currentColumn;
			var line = _currentLine;

			while (_currentPosition < source.Length)
			{
				if (source[_currentPosition] == '\r')
					break;

				_currentPosition++;
				_currentColumn++;
			}

			var position = new SourcePosition(startPosition, _currentPosition, line, startColumn);
			var value = source[position.StartPosition..position.EndPosition];

            var preprocessorText = value[1..];

            if (preprocessorText.Length > 0)
            {
				var startTextRange = preprocessorText.FirstWordRange();
				var startText = preprocessorText[startTextRange];

				if (ParserHelper.BilingualTokenValueIs(startText, "ОБЛАСТЬ", "REGION"))
					trivias.Add(new(BslTriviaKind.RegionDirective, preprocessorText[startTextRange.End.Value..].Trim().ToString(), position));
				else if (ParserHelper.BilingualTokenValueIs(startText, "КОНЕЦОБЛАСТИ", "ENDREGION"))
					trivias.Add(new(BslTriviaKind.EndRegionDirective, preprocessorText.ToString(), position));
                else
                {
                    if (_languageKind == BslKind.OneC)
                    {
						if (ParserHelper.BilingualTokenValueIs(startText, "ЕСЛИ", "IF"))
							trivias.Add(new(BslTriviaKind.IfDirective, preprocessorText.ToString(), position));
						else if (ParserHelper.BilingualTokenValueIs(startText, "ИНАЧЕЕСЛИ", "ELSIF"))
							trivias.Add(new(BslTriviaKind.ElseIfDirective, preprocessorText.ToString(), position));
						else if (ParserHelper.BilingualTokenValueIs(startText, "ИНАЧЕ", "ELSE"))
							trivias.Add(new(BslTriviaKind.ElseDirective, preprocessorText.ToString(), position));
						else if (ParserHelper.BilingualTokenValueIs(startText, "КОНЕЦЕСЛИ", "ENDIF"))
							trivias.Add(new(BslTriviaKind.EndIfDirective, preprocessorText.ToString(), position));
						else if (ParserHelper.BilingualTokenValueIs(startText, "ВСТАВКА", "INSERT"))
							trivias.Add(new(BslTriviaKind.InsertDirective, preprocessorText.ToString(), position));
						else if (ParserHelper.BilingualTokenValueIs(startText, "КОНЕЦВСТАВКИ", "ENDINSERT"))
							trivias.Add(new(BslTriviaKind.EndInsertDirective, preprocessorText.ToString(), position));
						else if (ParserHelper.BilingualTokenValueIs(startText, "УДАЛЕНИЕ", "DELETE"))
							trivias.Add(new(BslTriviaKind.DeleteDirective, preprocessorText.ToString(), position));
						else if (ParserHelper.BilingualTokenValueIs(startText, "КОНЕЦУДАЛЕНИЯ", "ENDDELETE"))
							trivias.Add(new(BslTriviaKind.EndDeleteDirective, preprocessorText.ToString(), position));
                        else
							trivias.Add(new(BslTriviaKind.UnknownDirective, value.ToString(), position));
					}
                    else
                    {
						if (ParserHelper.BilingualTokenValueIs(startText, "ИСПОЛЬЗОВАТЬ", "USE"))
							trivias.Add(new(BslTriviaKind.UseDirective, preprocessorText[startTextRange.End.Value..].Trim().ToString(), position));
						else if (ParserHelper.BilingualTokenValueIs(startText, "NATIVE", "NATIVE"))
							trivias.Add(new(BslTriviaKind.Native, preprocessorText.ToString(), position));
						else
							trivias.Add(new(BslTriviaKind.UnknownDirective, value.ToString(), position));
					}
                }
			}
            else
				trivias.Add(new(BslTriviaKind.UnknownDirective, value.ToString(), position));
		}
	}
}