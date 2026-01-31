$source = "C:\Users\user\マイドライブ\dev\mcp\ComputerUse\source"
$dest = "C:\Users\user\MCPComputerUse"

Write-Host "Copying MCPComputerUse source to $dest..."
if (-not (Test-Path $dest)) {
    New-Item -ItemType Directory -Path $dest -Force
}

Copy-Item -Path "$source\*" -Destination $dest -Recurse -Force
Write-Host "Done."
