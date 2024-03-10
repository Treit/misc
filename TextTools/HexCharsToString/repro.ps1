if (!(Test-Path .\bin\Debug\net8.0\HexCharsToString.exe)) { dotnet build }

.\bin\Debug\net8.0\HexCharsToString.exe "69 6E 74 6C 76 33 5F 70 6F 6B E9 6D 6F 6E" utf8
Write-Host ""
.\bin\Debug\net8.0\HexCharsToString.exe "69 6E 74 6C 76 33 5F 70 6F 6B E9 6D 6F 6E" latin1
Write-Host ""
.\bin\Debug\net8.0\HexCharsToString.exe "69 6E 74 6C 76 33 5F 70 6F 6B E3 A9 6D 6F 6E" utf8
Write-Host ""
.\bin\Debug\net8.0\HexCharsToString.exe "69 6E 74 6C 76 33 5F 70 6F 6B E3 A9 6D 6F 6E" latin1