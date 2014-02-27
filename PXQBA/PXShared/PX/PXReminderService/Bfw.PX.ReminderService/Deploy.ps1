# These variables should be set via the Octopus web portal:
#
#   ServiceName         - Name of the Windows service
#   ServiceExecutable   - Path to the .exe containing the service
# 
# sc.exe is the Service Control utility in Windows
  
$service = Get-Service $ServiceName -ErrorAction SilentlyContinue

$fullPath = Resolve-Path $ServiceExecutable

if (! $service)
{
    Write-Host "The service will be installed"
    
    New-Service -Name $ServiceName -BinaryPathName $fullPath -StartupType Automatic
}
else
{
    Write-Host "The service will be stopped and reconfigured"

    Stop-Service $ServiceName -Force
     
    & "sc.exe" config $service.Name binPath= $fullPath start= auto | Write-Host
}

Write-Host "Starting the service"
Start-Service $ServiceName