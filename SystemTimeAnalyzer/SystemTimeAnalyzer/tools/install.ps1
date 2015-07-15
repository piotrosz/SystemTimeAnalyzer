param($installPath, $toolsPath, $package, $project)

$analyzersPath = join-path $toolsPath "analyzers"

# Install language specific analyzers.
# $project.Type gives the language name like (C# or VB.NET)
$languageAnalyzersPath = join-path $analyzersPath $project.Type

foreach ($analyzerFilePath in Get-ChildItem $languageAnalyzersPath -Filter *.dll)
{
    if($project.Object.AnalyzerReferences)
    {
        $project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
    }
}
