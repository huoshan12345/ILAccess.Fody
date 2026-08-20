param ([bool]$norestore)

$ErrorActionPreference = "Stop"

$isGithub = [string]::IsNullOrEmpty($Env:GITHUB_ACTION) -eq $false
Write-Output "IsGithub = $isGithub, NoRestore = $norestore"

$buildDir = [io.path]::combine($MyInvocation.MyCommand.Definition, "..")
$rootDir = [io.path]::combine($buildDir, "..")
$sln = [io.path]::combine($rootDir, "ILAccess.Fody.slnx")

$pkgPath = [io.path]::combine($buildDir, "*.nupkg")
$snupkgPath = [io.path]::combine($buildDir, "*.snupkg")
Remove-Item $pkgPath
Remove-Item $snupkgPath

$verPath = ([io.path]::combine($buildDir, "pkg.version"))
$ver = Get-Content -Path $verPath
$key = $Env:NUGET_APIKEY
$source = "https://api.nuget.org/v3/index.json"

$useMyget = $false
if ($useMyget) {
  $key = $Env:MYGET_APIKEY
  $source = "https://www.myget.org/F/huoshan12345/api/v2/package"
}

if ([string]::IsNullOrEmpty($key)) {
  throw "the api key is empty"
}
if ([string]::IsNullOrEmpty($ver)) {
  throw "the version is empty"
}

$command = @'
dotnet pack $sln `
--nologo -v q -c Release `
--include-symbols -p:SymbolPackageFormat=snupkg `
--output $buildDir -p:PackageVersion=$ver
'@

if ($norestore -eq $true) {
  $command = $command + " --no-restore"
}

Invoke-Expression $command

if ($Lastexitcode -ne 0) {
  throw "failed with exit code $LastExitCode"
}

Write-Output "Packing finished."

if ($isGithub) {
  Write-Output "Uploading..."

  $files = Get-ChildItem $pkgPath
  foreach ($file in $files) {
    Write-Output "Uploading $($file.Basename)"

    # Push the .nupkg to NuGet.org (we will detect the .snupkg and push it for you)
    & dotnet nuget push $file -k $key --source $source -t 50 --skip-duplicate
    if ($Lastexitcode -ne 0) {
      throw "failed with exit code $LastExitCode"
    }
  }

  Write-Output "Uploading finished."
}
else {
  $files = Get-ChildItem $pkgPath
  foreach ($file in $files) {
    $name = $file.Basename.Substring(0, $file.Basename.Length - $ver.Length - 1)
    Write-Output "Removing $($name) from nuget cache"
    $packageLocalDir = [io.path]::combine( $env:USERPROFILE, ".nuget", "packages", $name.ToLower(), $ver);
    Remove-Item $packageLocalDir -Recurse -Force -ErrorAction SilentlyContinue
  }
}