[CmdletBinding()]
Param([Parameter(Mandatory=$False)]
      [string]$platformXPathRoot
)

if(!($platformXPathRoot -ne "")) {$platformXPathRoot = "D:\User Data\My Documents\GitHub\"  };
# Build PX solutions

Set-ExecutionPolicy Unrestricted
clear ;

$startTime = (get-date);
# Set this to the path of the local repository folder to search for the solutions files

$msbuildCmd = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe";


Function Clean-Solution($Solution)
{
    if(Test-Path $Solution)
    {
        write-host ""
        write-host -ForegroundColor Green  "------ Running clean up of solution.... ------"       
        & $msbuildCmd $Solution /m /t:clean /p:Configuration=Debug /noconlog
        # & $msbuildCmd $Solution /m /t:clean /p:Configuration=Release /noconlog
    }
    else
    {
        write-Host -ForegroundColor Red "Solution Path Error: File Not Found...!"
    }
    

}

Function Build-Solution($solution)
{

if(Test-Path $Solution)
    {
        write-host -ForegroundColor Yellow "Now building solution...."
        # now build the solution for both targets
        & $msbuildCmd $Solution /m /t:Rebuild /p:Configuration=Debug
        #& $msbuildCmd $Solution /m /t:Rebuild /p:Configuration=Release
        write-host -ForegroundColor Green "------ Built Solution ---------" 
    }  
    else
    {
        write-Host "Solution Path Error: File Not Found...!"
    }

}


# $platformXPath = "D:\User Data\My Documents\GitHub\PxReview\PlatformX"; 

# List of solutions to build
$solutionList = @(($platformXPathRoot  + 'PlatformX\PX\PlatformX.sln'),
                   ($platformXPathRoot + 'PlatformX\PXEportfolio\PxEportfolio.sln'),
                   ($platformXPathRoot + 'PlatformX\PXWebAPI\PXWebAPI.sln'),
                   ($platformXPathRoot + 'PlatformX\TestController\TestController.sln'));

cd $platformXPathRoot
write-host -ForegroundColor Yellow "Now building solutions from list...."

foreach($sol in $solutionList)
{
    Clean-Solution($sol) ; # clean all first
    Build-Solution($sol) ; # rebuilds the solution

} 

#Done
write-host -ForegroundColor Cyan "Solutions Built:-"
foreach ($sl in $solutionList) { write-host -ForegroundColor Green "Completed build for $sl" }



# run script to copy views for TestController
write-host -ForegroundColor Cyan "Copying Views for TestController in order to support Jasmine Tests..."

$testControllerSolutionPath = $platformXPathRoot + "PlatformX\TestController" ;
$testControllerViewScript = $platformXPathRoot + "PlatformX\TestController\UpdateViews.ps1" 
cd $testControllerSolutionPath
& $testControllerViewScript


Write-host -ForegroundColor Cyan 'Total time Taken: ' ((get-date) - $startTime).ToString()