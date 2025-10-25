$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition

$pkgPath = ([io.path]::combine($root, "*.nupkg"))
Remove-Item $pkgPath

$ver_path = Join-Path $root "pkg.version"
$ver = Get-Content -Path $ver_path
$key = $Env:NUGET_APIKEY
$nuget = "https://api.nuget.org/v3/index.json"
$isGithubAction = [string]::IsNullOrEmpty($Env:GITHUB_ACTION) -eq $false

if ([string]::IsNullOrEmpty($key)) {
  throw "the api key is empty"
}
if ([string]::IsNullOrEmpty($ver)) {
  throw "the version is empty"
}
$srcPath = [io.path]::combine($root, "..", "src")
$path = [io.path]::combine($srcPath, "ILAccess")

Write-Output "Packing $($path.Basename)"
& dotnet clean $path --nologo -v q
& dotnet pack $path --nologo -v q -c Release --include-symbols --output $root -p:PackageVersion=$ver
if ($Lastexitcode -ne 0)	{
  throw "failed with exit code $LastExitCode"
}
Write-Output "Packing finished."


$files = Get-ChildItem $pkgPath

if ($isGithubAction) {
  Write-Output "Uploading..."
  foreach ($file in $files) {
    Write-Output "Uploading $($file.Basename)"
    & dotnet nuget push $file -k $key --source $nuget -t 50 --skip-duplicate
    if ($Lastexitcode -ne 0) {
      throw "failed with exit code $LastExitCode"
    }
  }

  Write-Output "Uploading finished."
}
else {
  $packageName = Split-Path $path -Leaf
  $packageLocalDir = [io.path]::combine( $env:USERPROFILE, ".nuget", "packages", $packageName.ToLower(), $ver);
  Remove-Item $packageLocalDir -Recurse -Force -ErrorAction SilentlyContinue
}


