param(
    [string]$AssemblyPath="$PSScriptRoot\..\..\Xpand.DLL\Plugins\Xpand.VSIX.dll"
)
$AssemblyPath=[System.IO.Path]::GetFullPath($AssemblyPath)
$version=New-Object System.Version ([System.Diagnostics.FileVersionInfo]::GetVersionInfo($AssemblyPath).FileVersion)

Get-ChildItem "$PSScriptRoot\ProjectTemplates\*.zip" -Recurse |foreach{
    $tempPath="$(Split-Path $_ -Parent)\temp"
    Expand-Archive -Force $_ -DestinationPath $tempPath
    
    $vsTemplate=(Get-ChildItem $tempPath -Filter *.vstemplate | Select -First 1).FullName
    $content=Get-Content $vsTemplate
    $content = $content -ireplace 'eXpandFramework v([^ ]*)', "eXpandFramework v$($version.Major).$($version.Minor)"
    $content = $content -ireplace 'Xpand.VSIX, Version=([^,]*)', "Xpand.VSIX, Version=$($version.ToString())"
    Set-Content $vsTemplate $content
    Get-ChildItem $tempPath | Compress-Archive -DestinationPath $_ -Force 
    Remove-Item $tempPath -Recurse -Force
}