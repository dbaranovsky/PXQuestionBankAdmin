I added all of our powershell scripts along with a new function that will update your IIS physical path for PX and TestController sites to a powershell module that we can execute right from a powershell commandline.

If you drop this file into the following folder, it should be included automatically whenever you open powershell (Make sure you are on version 3.0 at least):

C:\Users\[username]\Documents\WindowsPowerShell\Modules\px-tools

After creating the folder and dropping the file in, restart powershell for the commands to become available. 

Functions are as follows (all parameters are optional. Defaults are in parens):
•	Set-PXEnvironment [-pxsite (PX)] [-testsite (test)] [-platformDir (.\)] [-copyViews (false)] :  Looks for PXPub.csproj and TestController.csproj in subfolders of –platformDir and updates your IIS sites (name configurable through parameters) physicalPath.
•	Set-TestControllerViews [-platformDir (.\)] : Looks for all views in subfolders of –platformDir and copys them into the test controllers Views folder for js unit testing
•	Build-AllPlatformX [-platformXPathRoot (.\)] : Builds PX, PXWebAPI and TestController.  Then runs Set-TestControllerViews
•	Run-PXUnitTests [-platformXPathRoot (.\)] : Runs .NET PX unit tests
•	Run-PXJasmineTests [-platformXPathRoot (.\)] : Runs all PX jasmine tests.



NOTE: Windows 8 tends to block files not originating from the local machine.  If you are prompted to allow a blocked file when opening powershell, right-click on the px-tools.ps1 file -> properties.  Click the "Unblock" button on the properties tab.
