param($installPath, $toolsPath, $package, $project)

$MvcVersion = 3;
foreach($item in $project.Object.References)
{
	if ($item.Name -eq 'System.Web.Mvc')
	{
		if ($item.MajorVersion  -eq 4)
		{
			$MvcVersion = 4;
		}
	}
}
if($MvcVersion -eq 4)
{
	$project.Object.References | Where-Object { $_.Name -eq 'MVC3ControlsToolkit' } | ForEach-Object { $_.Remove() }
}
if($MvcVersion -ne 4)
{
	$project.Object.References | Where-Object { $_.Name -eq 'MVC4ControlsToolkit' } | ForEach-Object { $_.Remove() }
}