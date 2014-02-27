[CmdletBinding()]
Param([Parameter(Mandatory=$False)]
      [string]$platformXPathRoot
)

if(!($platformXPathRoot -ne "")) {$platformXPathRoot = "D:\User Data\My Documents\GitHub\PlatformX\"  };

Set-ExecutionPolicy Unrestricted
$startTime = (get-date);

# set user's GitSourceControl Repository Path
# This will be the base path that all Test DLL willbe searched from.
# ---------------------------------------------------------------------
#$platformXPath = "D:\User Data\My Documents\GitHub\PlatformX" ;
#-----------------------------------------------------------------------

# to display results summary for each Test DLL that was executed
Function get-ResultsSummary($resultsFile)
{
    # get summary block from results file
    Write-Host -ForegroundColor Yellow 'Results from:' $resultsFile 
    
    $xmlData = [xml](Get-Content $resultsFile)       

    Write-Host 'Results for Test File: ' $resultsFile 
    write-host 'Passed' `t $xmlData.TestRun.ResultSummary.Counters.passed ;
    write-host 'Failed' `t $xmlData.TestRun.ResultSummary.Counters.failed ;
    write-host 'Total' `t $xmlData.TestRun.ResultSummary.Counters.total ;
    
    # if some tests failed then move to Failed Folder

     # Run all PX Tests for all PX solutions and save results for each Test.dll in separate file
   if([int]$xmlData.TestRun.ResultSummary.Counters.failed -eq 0)
   {
     #move Passed Tests to Pass folder
     move-item -path $resultsFile -Force -Destination  ($testRun.TestResultsPath +'\Passed')
   }

    # add to aggregate counters 
   
    $testRun.SummaryTotals.TotalPassed += [int]$xmlData.TestRun.ResultSummary.Counters.passed
    $testRun.SummaryTotals.TotalFailed += [int]$xmlData.TestRun.ResultSummary.Counters.failed
    $testRun.SummaryTotals.TotalTests += [int]$xmlData.TestRun.ResultSummary.Counters.total
  
}



# to display aggregated results summary for All Test DLL that wwere executed
Function get-TestRunResultsSummary
{
    Write-Host "------ Totals for Test Run ----------"
    write-host 'Passed' `t $testRun.SummaryTotals.TotalPassed ;

    if([int]$testRun.SummaryTotals.TotalFailed -gt 0)
    {
        write-host -ForegroundColor Red 'Failed' `t $testRun.SummaryTotals.TotalFailed ;
        start-process explorer 'C:\Users\kpatel\PxTestResults' # show files for folder with error results
    }
    else
    {
        write-host 'Failed' `t $testRun.SummaryTotals.TotalFailed ;
    }
    
    write-host 'Total' `t $testRun.SummaryTotals.TotalTests;

}



$ResultsTotals = @{
    "TotalPassed" = 0;
    "TotalFailed" = 0;
    "TotalTests" = 0 ;
    "TotalIgnored" = 0;

}


$testRun = @{
    "MstestExePath" = "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\mstest.exe";    # path to MSTest exe
    "TestResultsPath" = $env:USERPROFILE +"\PxTestResults"; # px test results folder that will store the output of each test DLL under test
    "SourceCodeRoot" = $platformXPathRoot ;    
    "SummaryTotals" = $ResultsTotals ;    
    
}

Function Show-FailedTests
{
    $temp = $testRun.TestResultsPath + '\*.trx' ;
    $failedFiles = Get-ChildItem -Path $temp;


    foreach($resultsFile in $failedFiles)
    {
        $failedXml = [xml](Get-Content $resultsFile)       

        Write-Host 'Results for Faield Test File:-'
        write-host -ForegroundColor Cyan $resultsFile 
   
        write-host -ForegroundColor Red 'Tests Failed' `t $failedXml.TestRun.ResultSummary.Counters.failed ;
        $failedXml.TestRun.Results.UnitTestResult | where {$_.Outcome -eq "Failed" } | select -Property InnerText |fl
        
   

    }

}


if(!(Test-Path $testRun.item("SourceCodeRoot")))
{   
    write-warning -ForegroundColor Red "Test file Paths Not Found for source code at:" $testRun.item("SourceCodeRoot");
    
}
else
{  
   clear ;
  
   # Run all PX Tests for all PX solutions and save results for each Test.dll in separate file
   if(!(Test-Path $testRun.TestResultsPath))
   {
     #create test results folder
     new-item -path $testRun.TestResultsPath -ItemType Directory      
     
   }

  
   

   sl $testRun.item("SourceCodeRoot")
   
   $mstestPath = $testRun.item("MstestExePath") ;    
   # discover all test DLLs with debug configuration
   $tcList = Get-ChildItem -include '*Test*' -recurse $testRun.item("SourceCodeRoot") -Exclude '*.config'| where {$_.FullName -like '*bin\debug*.dll'} | Select {$_.FullName} ;
   
   Write-Host -ForegroundColor Yellow "Discovered" $tcList.Count "Test DLLs..."
   write-host -ForegroundColor Yellow "Now running tests for each Test DLL discovered..."

   $testDllSummary = @{ "TestDLLName" = ""; "IgnoredCount" = "0"; "PassedCount" = "0"; "FailedCount" = "0" ; "TotalCount" = "0";}  

   
   # delete previous test run results file
   Remove-Item ($testRun.item("TestResultsPath")+'\*') -Recurse  ; 
   new-item -path ($testRun.TestResultsPath + '\Passed') -ItemType Directory 

   foreach($tc in $tcList)
      {
        Write-Host -ForegroundColor Yellow (get-date) "-------- Starting Test run for tests in " + $tc.'$_.FullName' ;       
        # remove last test results file               
        $ShortFileName = split-path $tc.'$_.FullName' -leaf ;
        $TestResultsFile = $testRun.item("TestResultsPath") + "\" + $ShortFileName + ".Results.trx";
        $resultsArg = "/resultsfile:" + $TestResultsFile ;
        # run tests
        $Testdll = "/testcontainer:" + $tc.'$_.FullName' ;

        

        & $mstestPath $Testdll $resultsArg ;

        # some tests may be ignored so results.trx file may not be created so ignore summary call
        if(Test-Path $TestResultsFile)
        {
            # get summary of results for this Test DLL run
            get-ResultsSummary -resultsFile $TestResultsFile ;
        }      

        Write-Host -ForegroundColor Yellow (get-date) "--------  Completed Test run ---------" ;

        }

        #--- DONE All Tests display summary of all tests
        get-TestRunResultsSummary
        
        Show-FailedTests

        Write-host -ForegroundColor Cyan 'Total time Taken: ' ((get-date) - $startTime).ToString()
       
   
    
}

