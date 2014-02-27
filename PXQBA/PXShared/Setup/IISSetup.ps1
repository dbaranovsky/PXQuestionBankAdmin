#
# This script will create and setup the IIS environment for a 
# front-end PX webserver.
#
# PARAMETERS
#
# envPrefix - lcl, dev, qa, pr. This is used to determin the correct
#             host headers.
#
# physRoot - root of the physical path for all sites.
#
# xbookRoot - root of the physical path for the xbookapp site.
#

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$False)]
    [string]$envPrefix,
    
    [Parameter(Mandatory=$True)]
    [string]$physRoot,
	
	[Parameter(Mandatory=$False)]
    [string]$xbookRoot
)

$appPoolFrameworkVersion = "v4.0"

if (Test-Path variable:\envPrefix) {
    $prefix = $envPrefix
}
else {
    $prefix = ""
}


# Create site infomration
#----------------------------

$pxSite = @{
    "AppPoolPath" = "IIS:\AppPools\PX";
    "AppPoolName" = "PX";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\PX";
    "PhysicalPath" = $physRoot + "\SiteRoot";
    "SiteBindings" = (@{protocol="http";bindingInformation="*:80:" + $prefix + "whfreeman.com"},
                      @{protocol="http";bindingInformation="*:80:" + $prefix + "worthpublishers.com"},
                      @{protocol="http";bindingInformation="*:80:" + $prefix + "bedfordstmartins.com"});
    "Type" = "Site";
}

$bfwglobalSite = @{
    "AppPoolPath" = "IIS:\AppPools\ASP.NET v4.0 Classic";
    "AppPoolName" = "ASP.NET v4.0 Classic";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\PX\BFWglobal";
    "PhysicalPath" = $physRoot + "\BFWglobal";
    "SiteBindings" = (@{protocol="http";bindingInformation="*:80:" + $prefix + "whfreeman.com"},
                      @{protocol="http";bindingInformation="*:80:" + $prefix + "worthpublishers.com"},
                      @{protocol="http";bindingInformation="*:80:" + $prefix + "bedfordstmartins.com"});
    "Type" = "Application";
}

$pxBetaSite = @{
    "AppPoolPath" = "IIS:\AppPools\PX";
    "AppPoolName" = "PX";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\PX";
    "PhysicalPath" = $physRoot + "\PX\PXPub";
    "SiteBindings" = "";
    "Type" = "Application";
}

$pxEportfolioSite = @{
    "AppPoolPath" = "IIS:\AppPools\PX";
    "AppPoolName" = "PX";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\PX\eportfolio";
    "PhysicalPath" = $physRoot + "\PXEportfolio\PXEportfolio";
    "SiteBindings" = "";
    "Type" = "Application";
}

$pxBetaSecureSite = @{
    "AppPoolPath" = "IIS:\AppPools\PX";
    "AppPoolName" = "PX";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\PX\secure";
    "PhysicalPath" = $physRoot + "\PX\PXPub";
    "SiteBindings" = "";
    "Type" = "Application";
}

$proxyPageSite = @{
    "AppPoolPath" = "IIS:\AppPools\ProxyPage";
    "AppPoolName" = "ProxyPage";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\ProxyPage";
    "PhysicalPath" = $physRoot + "\SiteRoot";
    "SiteBindings" = (@{protocol="http";bindingInformation="*:80:" + $prefix + "proxy.whfreeman.com"},
                      @{protocol="http";bindingInformation="*:80:" + $prefix + "proxy.worthpublishers.com"},
                      @{protocol="http";bindingInformation="*:80:" + $prefix + "proxy.bedfordstmartins.com"});
    "Type" = "Site";
}

$proxyPageBetaSite = @{
    "AppPoolPath" = "IIS:\AppPools\ProxyPage";
    "AppPoolName" = "ProxyPage";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\ProxyPage";
    "PhysicalPath" = $physRoot + "\PX\ProxyPage\ProxyPage";
    "SiteBindings" = "";
    "Type" = "Application";
}

$proxyPageBetaSecureSite = @{
    "AppPoolPath" = "IIS:\AppPools\ProxyPage";
    "AppPoolName" = "ProxyPage";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\ProxyPage\secure";
    "PhysicalPath" = $physRoot + "\PX\ProxyPage\ProxyPage";
    "SiteBindings" = "";
    "Type" = "Application";
}

$pxBfwPubSite = @{
    "AppPoolPath" = "IIS:\AppPools\px.bfwpub.com";
    "AppPoolName" = "px.bfwpub.com";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\px.bfwpub.com";
    "PhysicalPath" = $physRoot + "\SiteRoot";
    "SiteBindings" = (@{protocol="http";bindingInformation="*:80:" + $prefix + "px.bfwpub.com"});
    "Type" = "Site";
}

$pxWebApiSite = @{
    "AppPoolPath" = "IIS:\AppPools\api";
    "AppPoolName" = "api";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\px.bfwpub.com\api";
    "PhysicalPath" = $physRoot + "\PXWebAPI\PXWebAPI";
    "SiteBindings" = "";
    "Type" = "Application";
}

$pxWebApiHelpSite = @{
    "AppPoolPath" = "IIS:\AppPools\api";
    "AppPoolName" = "api";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\px.bfwpub.com\help";
    "PhysicalPath" = $physRoot + "\PXWebAPI\PXWebAPI_TestClient";
    "SiteBindings" = "";
    "Type" = "Application";
}

$pxHtsSite = @{
    "AppPoolPath" = "IIS:\AppPools\PxHTS";
    "AppPoolName" = "PxHTS";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\px.bfwpub.com\PxHTS";
    "PhysicalPath" = $physRoot + "\PxHTS\server\PxHTS";
    "SiteBindings" = "";
    "Type" = "Application";
}

$pxEgSite = @{
    "AppPoolPath" = "IIS:\AppPools\PxEG";
    "AppPoolName" = "PxEG";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\px.bfwpub.com\PxEG";
    "PhysicalPath" = $physRoot + "\PxEG\PxEG";
    "SiteBindings" = "";
    "Type" = "Application";
}

$pxXbookAppSite = @{
    "AppPoolPath" = "IIS:\AppPools\xbook";
    "AppPoolName" = "xbook";
    "FrameworkVersion" = $appPoolFrameworkVersion;
    "SitePath" = "IIS:\Sites\PX\xbookapp";
    "PhysicalPath" = $xbookRoot + "\xbook";
    "SiteBindings" = "";
    "Type" = "Application";
}

$siteList = $pxSite, $bfwglobalSite, $pxBetaSite, $pxEportfolioSite, $pxBetaSecureSite, $proxyPageSite, $proxyPageBetaSite, $proxyPageBetaSecureSite, $pxBfwPubSite, $pxWebApiSite, $pxWebApiHelpSite, $pxHtsSite, $pxEgSite

# add the xbook site to the list of sites to create if the root is specified
if ($xbookRoot -ne "") {
    $siteList += $pxXbookAppSite
}

# Installation
#---------------
Import-Module WebAdministration

cd IIS:\

foreach ($s in $siteList) {

    # create the AppPool if it does not already exist
    $appPoolPath = $s.item("AppPoolPath")
    $appPoolName = $s.item("AppPoolName")
    $appPoolFrameworkVersion = $s.item("FrameworkVersion")
    $siteBindings = $s.item("SiteBindings")
    $webRoot = $s.item("PhysicalPath")
    $sitePath = $s.item("SitePath")
    $type = $s.item("Type")

    Write-Host "Attemting to setup site " + $sitePath
    $pool = Get-Item $appPoolPath -ErrorAction SilentlyContinue
    if (!$pool) { 
        Write-Host "App pool" + $appPoolPath + " does not exist, creating..." 
        new-item $appPoolPath
        $pool = Get-Item $appPoolPath
    } else {
        Write-Host "App pool" + $appPoolPath + " exists." 
    }

    Write-Host "Set .NET framework version:" $appPoolFrameworkVersion
    Set-ItemProperty $appPoolPath managedRuntimeVersion $appPoolFrameworkVersion

    Write-Host "Set identity..."
    Set-ItemProperty $appPoolPath -name processModel -value @{identitytype="NetworkService"}

    Write-Host "Checking site..."
    $site = Get-Item $sitePath -ErrorAction SilentlyContinue
    if (!$site) { 
        Write-Host "Site does not exist, creating..." 
        $id = (dir iis:\sites | foreach {$_.id} | sort -Descending | select -first 1) + 1

        if ($type -eq "Site") {
            new-item $sitePath -bindings $siteBindings -id $id -physicalPath $webRoot
        }
        elseif ($type -eq "Application") {
            new-item $sitePath -physicalPath $webRoot -Type "Application" 
        }
    } else {
        Write-Host "Site exists. Complete"
    }

    Write-Host "Set app pool..."
    Set-ItemProperty $sitePath -name applicationPool -value $appPoolName
    Set-ItemProperty $sitePath -name physicalPath -value $webRoot

    if ($type -eq "Site") {
        Write-Host "Set bindings..."
        Set-ItemProperty $sitePath -name bindings -value $siteBindings   
    }    

    Write-Host "------------------------------------------------------------"
}

$betaSitePath = ("IIS:\Sites\PX\beta")
$betaProxySitePath = ("IIS:\Sites\ProxyPage\beta")
if(Test-Path $betaSitePath) {
	Write-Host "PX Beta site exists, removing..."
	Remove-Item IIS:\Sites\PX\beta -recurse
	Write-Host "PX Beta site removed."	
}
if(Test-Path $betaProxySitePath) {
	Write-Host "PXProxy Beta site exists, removing..."	
	Remove-Item IIS:\Sites\ProxyPage\beta -recurse
	Write-Host "PXProxy Beta site removed."
}

Write-Host "IIS configuration complete!"