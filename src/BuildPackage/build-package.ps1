param (

    [Parameter(Mandatory)]
    [string]
    [Alias("v")]
    $version, #version to build

    [Parameter()]
    [string]
    $suffix, # optional suffix to append to version (for pre-releases)

    [Parameter()]
    [string]
    $env = 'release', #build environment to use when packing

    [Parameter()]
    [switch]
    $pushToLocalNugetFeed = $true, #push to a local nuget feed

    [Parameter()]
    [string]
    $localNugetFeed = 'C:\Temp\packages' #local nuget feed location
)

if ($version.IndexOf('-') -ne -1) {
    Write-Host "Version shouldn't contain a - (remember version and suffix are seperate)"
    exit
}

$fullVersion = $version;

if (![string]::IsNullOrWhiteSpace($suffix)) {
    $fullVersion = -join($version, '-', $suffix)
}

$majorFolder = $version.Substring(0, $version.LastIndexOf('.'))

$outFolder = ".\$majorFolder\$version\$fullVersion"

if (![string]::IsNullOrWhiteSpace($suffix)) {
    $suffixFolder = $suffix;
    if ($suffix.IndexOf('.') -ne -1) {
        $suffixFolder = $suffix.substring(0, $suffix.indexOf('.'))
    }
    $outFolder = ".\$majorFolder\$version\$version-$suffixFolder\$fullVersion"
}

"----------------------------------"
Write-Host "Version  :" $fullVersion
Write-Host "Config   :" $env
Write-Host "Folder   :" $outFolder
"----------------------------------"; ""

dotnet restore ..

""; "##### Packaging"; "----------------------------------" ; ""

dotnet pack ..\Bento.Core\Bento.Core.csproj --no-restore -c $env -o $outFolder /p:ContinuousIntegrationBuild=true,version=$fullVersion

dotnet pack ..\Bento.Editor\Bento.Editor.csproj --no-restore -c $env -o $outFolder /p:ContinuousIntegrationBuild=true,version=$fullVersion

#.\nuget pack "Bento.Editor\Bento.Editor.nuspec" -version $fullVersion -OutputDirectory $outFolder

if ($pushToLocalNugetFeed) {
    #""; "##### Publishing to local nuget feed"; "----------------------------------" ; ""
    .\nuget init $outFolder $localNugetFeed
}

Write-Host "Bento Packaged : $fullVersion"

#Remove-Item ".\last-build-*" 
#Out-File -FilePath ".\last-build-$fullVersion.txt" -InputObject $fullVersion