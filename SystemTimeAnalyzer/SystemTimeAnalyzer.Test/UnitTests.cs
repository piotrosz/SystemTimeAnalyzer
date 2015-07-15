using TestHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace SystemTimeAnalyzer.Test
{
    public class SystemAnalyzerTests : CodeFixVerifier
    {
        [Fact]
        public void WhenCodeEmptyThenNoDiagnosticsIsShown()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        
        [Fact(Skip = "Need to work on this one")]
        public void WhenDateTimeNowUsedThenBothDiagnosticsAndCodeFixTriggered()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            var date = DateTime.Now;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = SystemTimeDiagnosticAnalyzer.DiagnosticId,
                Message = "Use SystemTime instead of DateTime",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] 
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 24)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            var date = SystemTime.Now();
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SystemTimeCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SystemTimeDiagnosticAnalyzer();
        }
    }
}
