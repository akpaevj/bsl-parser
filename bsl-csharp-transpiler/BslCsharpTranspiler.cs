// See https://aka.ms/new-console-template for more information

using Microsoft.CodeAnalysis;

namespace BSL.Transpiler.Csharp
{
    [Generator]
    class BslCsharpTranspiler : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.SyntaxProvider.CreateSyntaxProvider()
            throw new NotImplementedException();
        }
    }   
}