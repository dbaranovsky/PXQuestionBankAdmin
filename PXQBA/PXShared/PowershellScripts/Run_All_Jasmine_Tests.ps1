[CmdletBinding()]
Param([Parameter(Mandatory=$False)]
      [string]$platformXPathRoot
)

if(!($platformXPathRoot -ne "")) {$platformXPathRoot = "D:\User Data\My Documents\GitHub\"  };
clear;
cd c:\


# make sure that you have the IIS virtual for TestController web site running
# you should get a positive result by going to this URL: http://lcl.whfreeman.com/tests/runners/contenttreewidget/ContentTreeWidgetTest.html
# before running this script

# $testControllerSolutionPath = $platformXPathRoot + "PlatformX\TestController" ;
# $testControllerViewScript = $platformXPathRoot + "PlatformX\TestController\UpdateViews.ps1" 
# cd $testControllerSolutionPath
# & $testControllerViewScript


$chutzpahTestPath = $platformXPathRoot +"PlatformX\PX\PXPub\Tests\Chutzpah.2.4.3"; 
cd $chutzpahTestPath

$chutzpahTestExe = $chutzpahTestPath + "\chutzpah.console.exe" ;

#run jasmine tests
write-host -ForegroundColor Green "Running Jasmine Client Tests...."
& $chutzpahTestExe ..\runners 