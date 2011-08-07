param($installPath, $toolsPath, $package, $project)

$project.Object.References.Add("Punchy"); 
$project.Object.References.Add("AjaxMin"); 
$project.Object.References.Add("Punchy.Plugin.DotLessCss"); 
$project.Object.References.Add("Punchy.Plugin.GoogleClosure"); 
$project.Object.References.Add("Punchy.Plugin.MicrosoftAjaxMinifier"); 
$project.Object.References.Add("Punchy.Plugin.YahooYuiCompressor"); 