# https://bitbucket.org/rbwhitaker/xna-beyond-vs-2010/downloads/

function RemoveIfExists($path) {
    if((Test-Path $path) -eq $True) {
		Remove-Item $path -recurse;
	}
}

function RunInstaller($path) {
    Start-Process -FilePath msiexec.exe -ArgumentList /i, $path, /quiet -Wait;
}

function RunInstaller2([String]$path, [String]$extraOption) {
    Start-Process -FilePath msiexec.exe -ArgumentList /a, $path, /quiet, $extraOption -Wait;
}

function InstallXna($appName, $pathToExe, $installLocation, $extensionCacheLocation, $version) {
	$vsInstalled = test-path "$pathToExe";
	if ($vsInstalled -eq $True) {
		write-host "  $appName is installed on this machine. XNA will be added there.";
		write-host "  Copying files.";
		copy-item $xnaLocation $installLocation -recurse -force;
		
		write-host "  Updating configuration for this version.";
		$content = Get-Content ($installLocation + "\XNA Game Studio 4.0\extension.vsixmanifest");
		$content = $content -replace "Version=`"10.0`">", "Version=`"$version`">`r`n        <Edition>WDExpress</Edition>";
		$content | Out-File ($installLocation + "\XNA Game Studio 4.0\extension.vsixmanifest") -encoding ASCII;
		
		write-host "  Clearing the extensions cache.";
		RemoveIfExists($extensionCacheLocation);
		
		write-host "  Rebuilding the extension cache. This may take a few minutes.";
		Start-Process -FilePath $pathToExe -ArgumentList /setup -Wait
		write-host "  Finished rebuilding cache.";
		write-host "  XNA Game Studio 4.0 is now installed for $appName!";
	}
}

# Don't do anything if Visual Studio is already running.
if ((Get-Process "WDExpress" -ErrorAction SilentlyContinue) -or (Get-Process "devenv" -ErrorAction SilentlyContinue)) {
    write-host "Cannot install XNA while a version of Visual Studio is running. Exiting script...";
    return;
}

if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(`
    [Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    write-warning "You do not have Administrator rights to run this script!`nPlease re-run this script as an Administrator!"
    break;
}

$currentLocation = (Get-Location).ToString();

write-host "`r`n";
write-host "Step 1/4: Downloading XNA Installer" -foregroundcolor "Blue";

$downloadLocation = ($currentLocation + "\XNAGS40_setup.exe");
if ((Test-Path ".\XNAGS40_setup.exe") -eq $False) {
    write-host "  Downloading XNA 4.0 Refresh Installer to $downloadLocation. This may take several minutes.";
    $wc = New-Object System.Net.WebClient
    $wc.DownloadFile("http://download.microsoft.com/download/E/C/6/EC68782D-872A-4D58-A8D3-87881995CDD4/XNAGS40_setup.exe", $downloadLocation)
    write-host "  Download Complete.";
} else {
    write-host "  XNA 4.0 Refresh Installer already downloaded. Skipping download step.";
}
write-host "`r`n";
write-host "Step 2/4: Running Installers" -foregroundcolor "Blue";
write-host "  Extracting components from XNA 4.0 Refresh Installer.";
start-process -FilePath .\XNAGS40_setup.exe -ArgumentList /extract:XNA, /quiet -Wait;

write-host "  Running Redists.msi";
RunInstaller("`"$currentLocation\XNA\redists.msi`"");

$XnaInProgramFiles = "C:\Program Files (x86)\Microsoft XNA";

write-host "  Running XLiveRedist.msi";
RunInstaller("`"$XnaInProgramFiles\XNA Game Studio\v4.0\Setup\XLiveRedist.msi`"")

write-host "  Running xnafx40_redist.msi";
RunInstaller("`"$XnaInProgramFiles\XNA Game Studio\v4.0\Redist\XNA FX Redist\xnafx40_redist.msi`"")

write-host "  Running xnaliveproxy.msi";
RunInstaller("`"$XnaInProgramFiles\XNA Game Studio\v4.0\Setup\xnaliveproxy.msi`"")

write-host "  Running xnags_platform_tools.msi";
RunInstaller("`"$XnaInProgramFiles\XNA Game Studio\v4.0\Setup\xnags_platform_tools.msi`"")

write-host "  Running xnags_shared.msi";
RunInstaller("`"$XnaInProgramFiles\XNA Game Studio\v4.0\Setup\xnags_shared.msi`"")

write-host "  Extracting extension files from xnags_visualstudio.msi";
RunInstaller2 "`"$XnaInProgramFiles\XNA Game Studio\v4.0\Setup\xnags_visualstudio.msi`"" "TARGETDIR=C:\XNA-temp\ExtractedExtensions\"

write-host "  Running arpentry.msi";
RunInstaller("`"$currentLocation\XNA\arpentry.msi`"")

$xnaLocation = ("C:\XNA-temp\ExtractedExtensions\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\Microsoft\XNA Game Studio 4.0");

write-host "`r`n";
write-host "Step 3/4: Adding Extensions to Installed Versions of Visual Studio" -foregroundcolor "Blue";

# The following process is done for:
#   * VS 2012 Professional (or another paid-for version)
#   * VS Express 2012 for Windows Desktop
#   * VS 2013 Professional (or another paid-for version)
#   * VS Express 2013 for Windows Desktop
#   * VS 2015 (All versions. 2015 will have Community Edition, which is better than Express.)
# 1. Check to see if the program is installed. If it isn't, we won't try to install there.
# 2. If we're going to install there, tell the user.
# 3. Copy the XNA extension to the new location.
# 4. Update the extension's version to the version that we're installing to.
# 5. For express, we need to also add the version name (WDExpress) to the list of supported editions.
# 6. Delete the extensions cache.
# 7. Rebuild the extensions cache.

# $appName = "Visual Studio 2012 Pro";
# $pathToExe = "${Env:VS110COMNTOOLS}..\IDE\devenv.exe";
# $installLocation = "${Env:VS110COMNTOOLS}..\IDE\Extensions\Microsoft";
# $extensionCacheLocation = "$home\AppData\Local\Microsoft\VisualStudio\11.0\Extensions";
# $version = "11.0";
# InstallXna $appName $pathToExe $installLocation $extensionCacheLocation $version;

# $appName = "Visual Studio Express 2012 for Windows Desktop";
# $pathToExe = "${Env:VS110COMNTOOLS}..\IDE\WDExpress.exe";
# $installLocation = "${Env:VS110COMNTOOLS}..\IDE\WDExpressExtensions\Extensions"
# $extensionCacheLocation = "$home\AppData\Local\Microsoft\WDExpress\11.0\Extensions";
# $version = "11.0";
# InstallXna $appName $pathToExe $installLocation $extensionCacheLocation $version;

# $appName = "Visual Studio 2013 Pro";
# $pathToExe = "${Env:VS120COMNTOOLS}..\IDE\devenv.exe";
# $installLocation = "${Env:VS120COMNTOOLS}..\IDE\Extensions\Microsoft";
# $extensionCacheLocation = "$home\AppData\Local\Microsoft\VisualStudio\12.0\Extensions";
# $version = "12.0";
# InstallXna $appName $pathToExe $installLocation $extensionCacheLocation $version;

# $appName = "Visual Studio Express 2013 for Windows Desktop";
# $pathToExe = "${Env:VS120COMNTOOLS}..\IDE\WDExpress.exe";
# $installLocation = "${Env:VS120COMNTOOLS}..\IDE\WDExpressExtensions\Extensions"
# $extensionCacheLocation = "$home\AppData\Local\Microsoft\WDExpress\12.0\Extensions";
# $version = "12.0";
# InstallXna $appName $pathToExe $installLocation $extensionCacheLocation $version;

$appName = "Visual Studio 2015";
$pathToExe = "${Env:VS140COMNTOOLS}..\IDE\devenv.exe";
$installLocation = "${Env:VS140COMNTOOLS}..\IDE\Extensions\Microsoft";
$extensionCacheLocation = "$home\AppData\Local\Microsoft\VisualStudio\14.0\Extensions";
$version = "14.0";
InstallXna $appName $pathToExe $installLocation $extensionCacheLocation $version;

write-host "`r`n";
write-host "Step 4/4: Cleanup" -foregroundcolor "Blue";
write-host "  Deleting extracted temporary files.";
RemoveIfExists("$currentLocation\XNA");
RemoveIfExists("C:\XNA-temp\");
RemoveIfExists("C:\xnags_visualstudio.msi");

write-host "`r`nInstallation Complete." -foregroundcolor "Yellow";
