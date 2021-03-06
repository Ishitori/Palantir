Param(
[Parameter(Mandatory=$True)]
[string]$project_path
 )
   
$doc = New-Object System.Xml.XmlDocument
$doc.Load($project_path)
$manager = New-Object System.Xml.XmlNamespaceManager($doc.NameTable)
$manager.AddNamespace("tns", "http://schemas.microsoft.com/developer/msbuild/2003");

$node = $doc.SelectSingleNode("/tns:Project/tns:PropertyGroup[contains(@Condition, 'Debug')]", $manager)
$import_node = $doc.SelectSingleNode("/tns:Project/tns:Import[contains(@Project, 'Microsoft.StyleCop.targets')]", $manager)

if (!$import_node -or !$node.CodeAnalysisRuleSet -or !$node.RunCodeAnalysis -or !$node.StyleCopTreatErrorsAsWarnings -or $node.StyleCopTreatErrorsAsWarnings.InnerText -eq "false")
{
    write "FAILED"
    write "StyleCop DISABLED"
    return
}


write "SUCCESS"
write "StyleCop ENABLED"