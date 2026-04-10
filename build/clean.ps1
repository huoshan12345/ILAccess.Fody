Set-Location $PSScriptRoot

Get-ChildItem ..\ -include bin,obj,bld,Backup,_UpgradeReport_Files,Debug,Release,ipch,*.nupkg,*.snupkg,TestResults -Recurse `
| where-object { $_.fullname.Contains("node_modules") -eq $false } `
| foreach ($_) { remove-item $_.fullname -Force -Recurse }

Write-Output "Finished. Press any key to exit."
Read-Host