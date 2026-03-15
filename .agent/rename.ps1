$target = "c:\Users\S\Antigravity\CoreConnect"
cd $target

$exclude = @('.git', '.vs', 'bin', 'obj', 'node_modules', 'TestResults', 'AppData', '.gemini')

function Is-Excluded($path) {
    foreach ($ex in $exclude) {
        if ($path -match "\\$ex\\") { return $true }
    }
    return $false
}

Write-Host "1. Text replace"
$files = Get-ChildItem -File -Recurse | Where-Object { 
    -not (Is-Excluded $_.FullName) -and (
        $_.Name -like "*.cs" -or $_.Name -like "*.cshtml" -or $_.Name -like "*.razor" -or 
        $_.Name -like "*.json" -or $_.Name -like "*.xml" -or $_.Name -like "*.csproj" -or 
        $_.Name -like "*.sln" -or $_.Name -like "*.md" -or $_.Name -like "*.sh" -or 
        $_.Name -like "*.ps1" -or $_.Name -like "*Dockerfile*" -or $_.Name -like "*.yml" -or 
        $_.Name -like "*.css" -or $_.Name -like "*.js" -or $_.Name -like "*.html" -or $_.Name -like "*.txt"
    )
}

foreach ($file in $files) {
    $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    if ($content -match "CoreConnect" -or $content -match "coreconnect" -or $content -match "CORECONNECT") {
        $newContent = $content -cReplace "CoreConnect", "CoreConnect" -cReplace "coreconnect", "coreconnect" -cReplace "CORECONNECT", "CORECONNECT"
        [System.IO.File]::WriteAllText($file.FullName, $newContent, [System.Text.Encoding]::UTF8)
        # Write-Host "Modified $($file.FullName)"
    }
}

Write-Host "2. File rename"
$filesToRename = Get-ChildItem -File -Recurse | Where-Object { 
    -not (Is-Excluded $_.FullName) -and $_.Name -match "CoreConnect|coreconnect|CORECONNECT"
}

foreach ($f in $filesToRename) {
    $newName = $f.Name -cReplace "CoreConnect", "CoreConnect" -cReplace "coreconnect", "coreconnect" -cReplace "CORECONNECT", "CORECONNECT"
    Rename-Item -Path $f.FullName -NewName $newName -PassThru
    Write-Host "Renamed file $($f.Name) -> $newName"
}

Write-Host "3. Directory rename"
$dirsToRename = Get-ChildItem -Directory -Recurse | Where-Object { 
    -not (Is-Excluded $_.FullName) -and $_.Name -match "CoreConnect|coreconnect|CORECONNECT"
} | Sort-Object -Property @{Expression={$_.FullName.Length}; Descending=$true}

foreach ($d in $dirsToRename) {
    $newName = $d.Name -cReplace "CoreConnect", "CoreConnect" -cReplace "coreconnect", "coreconnect" -cReplace "CORECONNECT", "CORECONNECT"
    Rename-Item -Path $d.FullName -NewName $newName -PassThru
    Write-Host "Renamed dir $($d.Name) -> $newName"
}

Write-Host "Done"
