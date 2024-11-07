namespace BSL.AST.Lexing
{
    public enum TokenType
    {
        UNKNOWN,
        IDENTIFIER,
        EXPORT,
        EOF,

		AMPERSAND,
        EOL,
        COMMENT,
        WHITESPACE,
        QUESTION_MARK,

		STRING,
        DATE,
        TRUE,
        FALSE,
        NUMBER,
        UNDEFINED,
        NULL,

        DIVIDE,
        PLUS,
        MINUS,
        MULTIPLY,
        MODULO,

        EQUAL,
        GREATER,
        GREATER_OR_EQUAL,
        LESS,
        LESS_OR_EQUAL,
        NOT_EQUAL,

        MARK,
        END_OF_MARK,

        DOT,
        COMMA,
        SEMICOLON,

        OPEN_PARENT,
        CLOSE_PARENT,

        OPEN_BRACKET,
        CLOSE_BRACKET,

        IF,
        THEN,
        ELSE_IF,
        ELSE,
        END_IF,
        FOR,
        EACH,
        IN,
        TO,
        WHILE,
        DO,
        END_DO,
        PROCEDURE,
        FUNCTION,
        END_PROCEDURE,
        END_FUNCTION,
        VAR,
        GO_TO,
        RETURN,
        CONTINUE,
        BREAK,
        AND,
        OR,
        NOT,
        TRY,
        EXCEPT,
        RAISE,
        END_TRY,
        NEW,
        EXECUTE,
        VAL,

        ADD_HANDLER,
        REMOVE_HANDLER
    }
}
