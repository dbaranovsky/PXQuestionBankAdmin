#Looks for PXPub.csproj and TestController.csproj in subfolders of –platformDir and updates your IIS sites (name configurable through parameters) physicalPath.
function Set-PXEnvironment($pxsite = "PX", $testsite = "test", $platformxDir = ".\", [bool]$copyViews = $false)
{
    $curdir = Get-Item ".\"
    cd $platformxDir

    write-host -ForegroundColor Yellow "Looking for PXPub.csproj"
    $pxdir = (get-childitem -recurse PXPub.csproj).Directory
    write-host -ForegroundColor Green "Found $($pxdir.FullName)"

    write-host -ForegroundColor Yellow "Looking for TestController.csproj"
    $testdir = (get-childitem -recurse TestController.csproj).Directory
    write-host -ForegroundColor Green "Found $($testdir.FullName)"

    Import-Module WebAdministration

    cd IIS:\

    Write-Host -ForegroundColor Green "Setting physical path for $pxsite to $($pxdir.FullName)"
    Set-ItemProperty "IIS:\Sites\$pxsite" -name physicalPath -value $pxdir.FullName

    Write-Host -ForegroundColor Green "Setting physical path for $testsite to $($testdir.FullName)"
    Set-ItemProperty "IIS:\Sites\$testsite" -name physicalPath -value $testdir.FullName

    cd $curdir
    if($copyViews) 
    {
        Set-TestControllerViews($platformxDir)
    }
}

#Looks for all views in subfolders of –platformDir and copys them into the test controllers Views folder for js unit testing
function Set-TestControllerViews($platformxDir = ".\")
{
    $curdir = Get-Item ".\"
	cd $platformxDir

    write-host -foregroundcolor Yellow "Looking for TestController.sln"
    $tcItem = (get-childitem -Recurse TestController.sln).Directory
    write-host -foregroundcolor Green "Found $tcItem"

    # Clear out all of the views that currently exist in the Views directory

    # Check to make sure the directory exists first.  If not create it
    if(!(test-path "$tcItem\TestController\Views")) {
        write-host -ForegroundColor Cyan "Creating new views directory at $tcItem\TestController"
	    new-item -path "$tcItem\TestController\Views" -ItemType Directory 
    } else {
        write-host -ForegroundColor Cyan "Removing old view from $tcItem\TestController\Views"
	    remove-item "$tcItem\TestController\Views\*" -recurse
    }

    # Get a list of all of the views folders excluding testcontroller and pxwebapi
    $pxDir = $tcItem.Parent.FullName
    $viewDirectories = gci "$pxDir" -recurse | where {$_.Name -like "Views" -and $_.FullName -like "*PXPub*"}

    # Copy the views into the test controller's view directory
    foreach( $v in $viewDirectories) {
	    $parent = $v.Parent.Name
	    $path = $v.FullName
	    write-host -foregroundcolor Cyan "Copying $parent"
	    cpi "$path" "$tcItem\TestController\Views\$parent" -recurse
    }

    cd $curdir
}

#Builds PX, PXWebAPI and TestController.  Then runs Set-TestControllerViews
function Build-AllPlatformX($platformXPathRoot = ".\")
{
    # Build PX solutions

    Set-ExecutionPolicy Unrestricted

    $startTime = (get-date);
    # Set this to the path of the local repository folder to search for the solutions files

    $msbuildCmd = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe";

    $currentDir = Get-Item ".\";

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
            & $msbuildCmd $Solution /m /t:Rebuild /p:Configuration=Debug "/clp:ErrorsOnly"
            #& $msbuildCmd $Solution /m /t:Rebuild /p:Configuration=Release
            write-host -ForegroundColor Green "------ Built Solution ---------" 
        }  
        else
        {
            write-Host "Solution Path Error: File Not Found...!"
        }

    }

    # List of solutions to build
    $solutionList = @(($platformXPathRoot  + 'PX\PlatformX.sln'),
                       ($platformXPathRoot + 'PXWebAPI\PXWebAPI.sln'),
                       ($platformXPathRoot + 'TestController\TestController.sln'));

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

    Set-TestControllerViews($platformXPathRoot)


    Write-host -ForegroundColor Cyan 'Total time Taken: ' ((get-date) - $startTime).ToString()

    #return to directory it was executed from
    cd $currentDir
}

#Runs .NET PX unit tests
function Run-PXUnitTests($platformXPathRoot = ".\")
{
    $startTime = (get-date);

    $currentDir = Get-Item ".\";

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

        $CS = Gwmi Win32_ComputerSystem -Comp "."
        $username = $CS.Username.Split("\")[1].ToLower()

        if([int]$testRun.SummaryTotals.TotalFailed -gt 0)
        {
            write-host -ForegroundColor Red 'Failed' `t $testRun.SummaryTotals.TotalFailed ;
            start-process explorer 'C:\Users\$username\PxTestResults' # show files for folder with error results
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
        "TestResultsPath" = $env:USERPROFILE +"\PxTestResults\" + (Split-Path -Leaf $platformXPathRoot); # px test results folder that will store the output of each test DLL under test
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

        #return to directory it was executed from
        cd $currentDir
    }
    else
    {  
  
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
       
        #return to directory it was executed from
        cd $currentDir
    }
}

#Runs all PX jasmine tests.
function Run-PXJasmineTests($platformXPathRoot = ".\")
{
    $currentDir = Get-Item ".\";


    # make sure that you have the IIS virtual for TestController web site running
    # you should get a positive result by going to this URL: http://lcl.whfreeman.com/tests/runners/contenttreewidget/ContentTreeWidgetTest.html
    # before running this script

    # $testControllerSolutionPath = $platformXPathRoot + "PlatformX\TestController" ;
    # $testControllerViewScript = $platformXPathRoot + "PlatformX\TestController\UpdateViews.ps1" 
    # cd $testControllerSolutionPath
    # & $testControllerViewScript


    $chutzpahTestPath = $platformXPathRoot +"PX\PXPub\Tests\Chutzpah.2.4.3"; 
    cd $chutzpahTestPath

    $chutzpahTestExe = (get-item ".\").FullName + "\chutzpah.console.exe" ;

    #run jasmine tests
    write-host -ForegroundColor Green "Running Jasmine Client Tests...."
    & $chutzpahTestExe ..\runners /silent /timeoutMilliseconds 10000

    #return to directory it was executed from
    cd $currentDir
}