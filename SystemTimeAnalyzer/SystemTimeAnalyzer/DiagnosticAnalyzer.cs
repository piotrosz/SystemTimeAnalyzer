using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SystemTimeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemTimeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SystemTimeAnalyzer";

        internal static readonly LocalizableString Title = "Use SystemTime.Now()";
        internal static readonly LocalizableString MessageFormat = "Use SystemTime.Now() instead of DateTime.Now";
        internal static readonly LocalizableString Description = "Do not use DateTime.Now use SystemTime.Now()";
        internal const string Category = "Usage";

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

            if (memberAccessExpression?.Name.ToString() != "Now" ||
                memberAccessExpression.Expression.ToString() != "DateTime")
            {
                return;
            }

            if (memberAccessExpression
                .Ancestors(ascendOutOfTrivia: false)
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
