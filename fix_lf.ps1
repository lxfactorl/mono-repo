param([string]$Path)
$content = Get-Content -Path $Path -Raw
$content = $content -replace "`r`n", "`n"
[System.IO.File]::WriteAllText((Resolve-Path $Path).ProviderPath, $content)
