$buildid = $args[0]
$file = $args[1]
$now = Get-Date -Format "yyyy.MM.dd."
$version = $now + "$buildid"
Write-Host "buildid = $buildid"
Write-Host "version = $version"
Write-Host "file = $file"

echo $version | Out-File $file