# This batch file assumes the following:
# - .NET 8.0 (or newer) SDK is installed and in PATH
#
# If any of these are not satisfied, the operation may fail
# in an unpredictable way and result in an incomplete output.

# Optional parameters
param(
    [Parameter(Mandatory = $false)]
    [Alias("NoBuild")]
    [switch]$NO_BUILD
)

# Set the current directory as a variable
$BUILD_FOLDER = $PSScriptRoot

# Only build if requested
if (!$NO_BUILD.IsPresent)
{
    # Restore Nuget packages for all builds
    Write-Host "Restoring Nuget packages"
    dotnet restore

    # Create Nuget Package
    dotnet pack SabreTools.Serialization\SabreTools.Serialization.csproj --output $BUILD_FOLDER
}
