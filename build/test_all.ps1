$ErrorActionPreference = "Stop"

$mode = if ($args[0] -eq 'Release') { "Release" } else { "Debug" }

Write-Output "mode = $mode"

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
$testDir = [io.path]::combine($root, "..\test")

$projects = (
  "ILAccess.Tests"
)

foreach ($project in $projects) {
  $path = [io.path]::combine($testDir, $project)
  & dotnet test $path --nologo -v q -c $mode
  if ($Lastexitcode -ne 0) {
    throw "failed with exit code $LastExitCode"
  }
}
