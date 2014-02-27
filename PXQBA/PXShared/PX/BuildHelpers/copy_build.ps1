
$source = $args[0]
$destination = $args[1]
$exclude = $args[2]

Write-Host "Source = $source"
Write-Host "Destination = $destination"
Write-Host "Exclude = $exclude"

if(Test-Path "$destination") {
	Get-ChildItem -Recurse $destination | Remove-Item -Recurse -Force
}

xcopy $source $destination /Y /E /EXCLUDE:$exclude