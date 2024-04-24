#! /bin/bash

# This batch file assumes the following:
# - .NET 8.0 (or newer) SDK is installed and in PATH
#
# If any of these are not satisfied, the operation may fail
# in an unpredictable way and result in an incomplete output.

# Optional parameters
NO_BUILD=false
while getopts "uba" OPTION
do
    case $OPTION in 
    b)
        NO_BUILD=true
        ;;
    *)
        echo "Invalid option provided"
        exit 1
        ;;
    esac
done

# Set the current directory as a variable
BUILD_FOLDER=$PWD

# Only build if requested
if [ $NO_BUILD = false ]
then
    # Restore Nuget packages for all builds
    echo "Restoring Nuget packages"
    dotnet restore

    # Create Nuget Package
    dotnet pack SabreTools.Serialization/SabreTools.Serialization.csproj --output $BUILD_FOLDER
fi