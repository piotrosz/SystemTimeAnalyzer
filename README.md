# SystemTimeAnalyzer

Replace: 
- DateTime.Now with SystemTime.Now(),
- DateTime.Today with SystemTime.Now().Date.

![alt tag](http://if.pw.edu.pl/~ludwik/images/systemtime.now.png)

The idea of SystemTime is described here:
http://ayende.com/blog/3408/dealing-with-time-in-tests

NuGet package:
https://www.nuget.org/packages/SystemTimeAnalyzerAndFix/

TODO:
- Improve unit tests
- Add missing SystemTime using statement if SystemTime is defined and "using" is missing
