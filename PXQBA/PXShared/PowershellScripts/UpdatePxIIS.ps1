[CmdletBinding()]
Param(
    [Parameter(Mandatory=$False)]
    [string]$pxsite,
    
    [Parameter(Mandatory=$False)]
    [string]$testsite
)

$curdir = Get-Item ".\"

write-host -ForegroundColor Yellow "Looking for PXPub.csproj"
$pxdir = get-childitem -recurse PXPub.csproj

write-host -ForegroundColor Yellow "Looking for TestController.csproj"
$testdir = get-childitem -recurse TestController.csproj

if($pxsite -eq "") {
    $pxsite = "PX"
}
if($testsite -eq ""){
    $testsite = "test"
}

Import-Module WebAdministration

cd IIS:\

Write-Host -ForegroundColor Green "Setting physical path for $pxsite to $($pxdir.Directory.FullName)"
Set-ItemProperty "IIS:\Sites\$pxsite" -name physicalPath -value $pxdir.Directory.FullName

Write-Host -ForegroundColor Green "Setting physical path for $testsite to $($testdir.Directory.FullName)"
Set-ItemProperty "IIS:\Sites\$testsite" -name physicalPath -value $testdir.Directory.FullName

cd $curdir