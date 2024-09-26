#!/bin/bash
#To make the .sh file executable
#sudo chmod +x ./thisFile.sh


# lists .NET versions installed
#Find install locations
dotnet --info
dotnet --list-sdks
dotnet --list-runtimes