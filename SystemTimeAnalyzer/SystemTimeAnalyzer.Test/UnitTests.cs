using System;
using TestHelper;
using SystemTimeAnalyzer;
using xUnit;

namespace SystemTimeAnalyzer.Test
{
    public class UnitTest : CodeFixVerifier
    {
        [Fact]
        public void WhenCodeEmptyThenNoDiagnosticsIsShown()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [Fact]
        public void WhenDateTimeNowUsedThenBothDiagnosticsAndCodeFixTriggered()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;

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
                        new DiagnosticResultLocation("Test0.cs", 11, 23)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;

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
