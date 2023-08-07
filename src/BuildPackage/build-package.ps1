param (
    [Parameter(Mandatory)]
    [string]
    $version = (Read-Host -Prompt "Version to build using semantic versioning e.g. 1.0.1"),

    [Parameter()]
    [string]
    $suffix = (Read-Host -Prompt "Optional: suffix to append to version (for pre-releases)"),

    [Parameter()]
    [string]
    $env = (Read-Host -Prompt "Optional: build environment to use when packing, defaults to 'release'"),

    [Parameter()]
    [switch]
    $pushToLocalNugetFeed,

    $pushToLocalNugetFeedUserInput = (Read-Host -Prompt "Optional: push to a local nuget feed? (Y/N defaults to Y)"),

    [Parameter()]
    [string]
    $localNugetFeedPath
)

if ([string]::IsNullOrWhiteSpace($env)) {
    $env = 'release'
}

if ([string]::IsNullOrWhiteSpace($pushToLocalNugetFeedUserInput))
{
    $pushToLocalNugetFeed = $true
}
elseif ($pushToLocalNugetFeedUserInput -eq 'Y') {
    $pushToLocalNugetFeed = $true
}
else
{
    $pushToLocalNugetFeed = $false
}

if ($pushToLocalNugetFeed) {
    $localNugetFeedPathUserInput = Read-Host -Prompt "Optional: local nuget feed location (defaults to C:\Temp\packages)"
}

if ([string]::IsNullOrWhiteSpace($localNugetFeedPathUserInput))
{
    $localNugetFeedPath = 'C:\Temp\packages'
}

if ($version -notmatch "\d+(?:\.\d+)+") {
    Write-Host "Please enter your version number using semantic versioning e.g. 1.0.1"
    exit
}

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

if ($pushToLocalNugetFeed) {
    #""; "##### Publishing to local nuget feed"; "----------------------------------" ; ""
    .\nuget init $outFolder $localNugetFeed
}

Write-Host "Bento Packaged : $fullVersion"