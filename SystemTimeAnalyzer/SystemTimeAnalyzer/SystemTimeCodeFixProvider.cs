using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SystemTimeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SystemTimeCodeFixProvider)), Shared]
    public class SystemTimeCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SystemTimeDiagnosticAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
        
        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create("Replace with SystemTime.Now()", 
                    cancellationToken => ConvertToSystemTimeNowAsync(context.Document, diagnostic, cancellationToken)),
                diagnostic);

            return Task.FromResult(0);
        }

        private async static Task<Document> ConvertToSystemTimeNowAsync(
            Document document,
            Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            
            var memberAccessExpressionSyntax = root.FindNode(diagnostic.Location.SourceSpan)
                .DescendantNodesAndSelf()
                .OfType<MemberAccessExpressionSyntax>()
                .First();

            var newRoot = root.ReplaceNode(memberAccessExpressionSyntax,
                SyntaxFactory.ParseExpression("SystemTime.Now()")
                .WithLeadingTrivia(memberAccessExpressionSyntax.GetLeadingTrivia())
                .WithTrailingTrivia(memberAccessExpressionSyntax.GetTrailingTrivia()));

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}