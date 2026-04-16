$reg = [regex]"(\d+)\.(\d+)\.(\d+)";
$callback = {
  $m = $args[0]
  $major = [int]$m.Groups[1].Value
  $minor = [int]$m.Groups[2].Value
  $build = [int]$m.Groups[3].Value
  $ver = $major * 100 + $minor * 10 + $build
  $ver++

  $digits = New-Object System.Collections.Generic.List[int]
  $number = $ver
  For ($i = 3; $i -gt 0; $i--) {
    $digit = $number % 10
    $number = ($number - $digit) / 10;
    $digits.Add($digit);
  }
  $digits.Reverse()
  $str = [system.String]::Join(".", $digits)
  $str
}
$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
$path = Join-Path $root "pkg.version"

$prev = Get-Content -Path $path
$new = $reg.Replace($prev, $callback)
Set-Content -Path $path -Value $new
Write-Output "Updated version $prev -> $new"
Read-Host