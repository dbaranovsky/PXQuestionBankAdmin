# Powershell script executed after a deployment through Octopus.
# This step is primarily used to set the correct virtual directory paths
# which is only necessary due to the weird PX deployment of beta/secure

Import-Module WebAdministration

$fullPath = (resolve-path .)
#$siteName1 = ("IIS:\Sites\" + $OctopusWebSiteName)
$siteName2 = ("IIS:\Sites\" + $OctopusWebSiteName2)

#Set-ItemProperty $siteName1 -Name physicalPath -Value $fullPath
Set-ItemProperty $siteName2 -Name physicalPath -Value $fullPath

# This step is used to create Unique Release Version:
# $OctopusTaskId - is an integer used to identity the current task in Octopus. You can use this if you need to record something unique about each deployment. 
# To uniquely identify each deployment we will use ("R" + $OctopusPackageVersion + "T" + $OctopusTaskId)

#$OctopusOriginalPackageDirectoryPath = "C:\Development\PX Development\PX Main\PXBranchCheckouts\Trunk\PX\PXPub"
#$OctopusTaskId = "tasks-14081"
#$OctopusPackageVersion = "2013.01.31.22"

$OctopusTaskId = $OctopusTaskId.Replace("tasks-", "") 
$OctopusPackageVersion = $OctopusPackageVersion.Replace(".", "") 

$file = ($OctopusOriginalPackageDirectoryPath + "\version.txt")
$version = ("R" + $OctopusPackageVersion + "T" + $OctopusTaskId)

Write-Host "OctopusOriginalPackageDirectoryPath = $OctopusOriginalPackageDirectoryPath"
Write-Host "OctopusTaskId = $OctopusTaskId"
Write-Host "OctopusPackageVersion = $OctopusPackageVersion"
Write-Host "file = $file"
Write-Host "version = $version"

echo $version | Out-File $file

if(![System.Diagnostics.EventLog]::SourceExists('PlatformX'))
{
    [System.Diagnostics.EventLog]::CreateEventSource('PlatformX','Application')
}

