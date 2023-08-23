$allprojects = dir -rec -filt *.csproj
$projects = $allprojects | ?{$_.FullName -notmatch "Test"}

$esc = [char]27
$ret = "$esc[0m"
$colorOld = "$esc[1;31m"
$colorNew = "$esc[1;36m"
$colorCore = "$esc[1;33m"
$colorNetstandard = "$esc[1;32m"
$colorErr = "$esc1;31m"

$totalcount = $allprojects.Length
$nontestcount = $projects.Length
$newcount = 0
$oldcount = 0
$netstandardcount = 0
$netcoreappcount = 0

$projects | %{
    $s = Get-Content $_.FullName -Raw
    
    if ($s.Contains("Sdk=`"Microsoft.NET.Sdk") -or $s.Contains("Sdk=`"MSBuild.Sdk.Extras"))
    {
        $p = (Split-Path $_.FullName) + "\packages.config"
        $newcount++

        if (!($s -match "netstandard"))
        {
            if ($s -match "netcoreapp")
            {
                Write-Host "$($colorCore)[NetCoreApp] $($_.FullName)"
                $netcoreappcount++;
            }
            else
            {
                if ($s -match "OutputType.+Ex")
                {
                    return;
                }
    
                Write-Host "$($colorNew)[New] $($_.FullName)"
            }
        }
        else 
        {
            Write-Host "$($colorNetstandard)[Netstandard] $($_.FullName)"
            $netstandardcount++;
        }
        
        if (Test-Path ($p))
        {
            Write-Host "$($colorErr)    -> packages.config"
            Remove-Item $p
        }
    }
    else
    {
        Write-Host "$($colorOld)[Old] $($_.FullName)"
        $oldcount++
    }
}

Write-Host ""
Write-Host "$totalcount total projects."
Write-Host "$nontestcount non-test projects."
Write-Host ""
Write-Host "$($colorNew)New format: $newcount projects."
Write-Host "$($colorOld)Old format: $oldcount projects."
Write-Host ""
Write-Host "$($colorNetstandard).NET Standard: $netstandardcount projects."
Write-Host "$($colorCore).NET Core: $netcoreappcount projects."
Write-Host "$ret"