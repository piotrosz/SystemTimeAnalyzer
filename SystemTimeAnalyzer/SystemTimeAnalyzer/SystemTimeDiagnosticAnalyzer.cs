using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SystemTimeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemTimeDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SystemTimeAnalyzer";

        internal static readonly LocalizableString Title = "Use SystemTime.Now()";
        internal static readonly LocalizableString MessageFormat = "Use SystemTime instead of DateTime";
        internal static readonly LocalizableString Description = "Do not use DateTime, use SystemTime";
        internal const string Category = "Usage";

        private static readonly string[] Members = {"Now", "Today"};

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, 
            Title, 
            MessageFormat, 
            Category, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: Description,
            helpLinkUri: "http://ayende.com/blog/3408/dealing-with-time-in-tests");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // TODO: check if is not auto-generated
            // see: https://github.com/code-cracker/code-cracker/blob/master/src/Common/CodeCracker.Common/Extensions/GeneratedCodeAnalysisExtensions.cs

            var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node;

            if (memberAccessExpression?.Expression.ToString() != "DateTime" ||
                !Members.Contains(memberAccessExpression.Name.ToString()))
            {
                return;
            }

            if (memberAccessExpression
                .Ancestors()
                .OfType<ClassDeclarationSyntax>()
                .First()?.Identifier.Text == "SystemTime")
            {
                return;
            }

            var diagnostic = Diagnostic.Create(
                Rule, 
                memberAccessExpression.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }
    }
}
