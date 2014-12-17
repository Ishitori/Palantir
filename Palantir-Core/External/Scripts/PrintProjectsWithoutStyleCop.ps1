$solution_path = 'E:\VSProjects\Palantir\'
write "StyleCop integration is missing in the following projects"
get-childitem $solution_path -recurse -include "*.csproj" -exclude "bin", "obj", "_ReSharper.Palantir", ".hg" | foreach {
   
    $doc = New-Object System.Xml.XmlDocument
    $doc.Load($_.FullName)
    $manager = New-Object System.Xml.XmlNamespaceManager($doc.NameTable)
    $manager.AddNamespace("tns", "http://schemas.microsoft.com/developer/msbuild/2003");
    $node = $doc.SelectSingleNode("/tns:Project/tns:PropertyGroup[contains(@Condition, 'Debug')]", $manager)
    $assemblyNode = $doc.SelectSingleNode("//tns:AssemblyName", $manager)
    
    if (!$node)
    {
        write $_.FullName    
        write "Does not contain 'Debug' node"
        return
    }
    
    if (!$assemblyNode.InnerText.StartsWith("Ix"))
    {
        # If the project doesn't starts with Bonasource then it is a 3rd party project
        # Use AddPrefixToAssemblies.ps1 if it is Bonasource project
        return
    }

    if (!$node.CodeAnalysisRuleSet -or
	!$node.RunCodeAnalysis -or
	!$node.StyleCopTreatErrorsAsWarnings -or
	$node.CodeAnalysisRuleSet -ne "`$(SolutionDir)\External\CodingStandards.ruleset")
    {
        write $_.FullName
    }
}