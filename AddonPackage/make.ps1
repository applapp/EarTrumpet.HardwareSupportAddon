param(
[string]$OutDir,
[string]$TargetName,
[string]$ConfigurationName
)

trap
{
    $ErrorActionPreference = "Continue";
    Write-Error $_
    Exit 1
}

$ErrorActionPreference = "Stop";

$OutDir = resolve-path $OutDir
$Dll = join-path "$OutDir" "$TargetName.dll"
if (!(Test-Path $DLL)) {
	Write-Host "Input DLL not found! '$Dll'"
}

$AddonsDir = $OutDir
$OutDir = Resolve-Path "$OutDir\..\" | select -exp Path

$TempPackageDir = Join-Path $OutDir "PackageTemp.$TargetName"
$TempPackageManifest = Join-Path $TempPackageDir appxmanifest.xml
$OutAppxFile = Join-Path $OutDir "$TargetName.appx"

$packageXml = [xml](Get-Content ..\..\EarTrumpet\EarTrumpet.Package\Package.appxmanifest)
$packageVersion = "0.0.0.0"; #($packageXml.Package.Identity.Version)

if (Test-Path $TempPackageDir) {
	# HACK: Remove-Item returns NullRefException.
	cmd /c RD /S /Q "$TempPackageDir"
}

if (Test-Path $OutAppxFile) {
	cmd /c del /Q "$OutAppxFile"
}

mkdir $TempPackageDir | out-null
mkdir "$TempPackageDir\Versions\$packageVersion\" | out-null

echo "" > $TempPackageDir\none.exe
copy $Dll "$TempPackageDir\Versions\$packageVersion\$TargetName.dll"
if ($TargetName -eq "EarTrumpet.Test.Web") {
	copy "$AddonsDir\Microsoft*.dll" "$TempPackageDir\Versions\$packageVersion\"
	copy "$AddonsDir\System*.dll" "$TempPackageDir\Versions\$packageVersion\"
	copy "$AddonsDir\net*.dll" "$TempPackageDir\Versions\$packageVersion\"
	copy "$AddonsDir\newt*.dll" "$TempPackageDir\Versions\$packageVersion\"
}

if ($ConfigurationName -eq "Debug") {
	exit;
}

copy -Recurse .\PackageLayout\* $TempPackageDir

$assembly = [Reflection.Assembly]::LoadFile($Dll);

$metadata = @{}
[Reflection.CustomAttributeData]::GetCustomAttributes($assembly) |% {
	$key = $_.AttributeType.Name
	$metadata.Add($key, $_.ConstructorArguments[0].Value) 
}

# Fix $targetentrypoint$ by making $targetentrypoint = $targetentrypoint
$targetentrypoint = '$targetentrypoint'

$expandedAppxManifest = $ExecutionContext.InvokeCommand.ExpandString((Get-Content $TempPackageManifest))
Set-Content -Path $TempPackageManifest -Value $expandedAppxManifest

$ret = makeappx.exe pack /d "$TempPackageDir" /p $OutAppxFile
if ($ret -notcontains "Package creation succeeded.") {
	write-host $ret
} else {
	write-host "MakeAppx -> $OutAppxFile"
}

$ret = signtool.exe sign /a /v /fd SHA256 /f "..\..\EarTrumpet\EarTrumpet.Package\EarTrumpet.Package_StoreKey.pfx" $OutAppxFile
if ($ret -notcontains "Number of files successfully Signed: 1") {
	write-host $ret
} else {
	write-host "SignTool -> $OutAppxFile"
}