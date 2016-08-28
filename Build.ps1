$solution = "$project.sln"
$test = "test\\SerilogMetrics.Tests\\project.json"
$projectFolder = "src\\SerilogMetrics"
$project = $projectFolder + "\\project.json"

function Invoke-Build()
{
    Write-Output "Building"

    if(Test-Path .\artifacts) {
        echo "build: Cleaning .\artifacts"
        Remove-Item .\artifacts -Force -Recurse
    }

    & dotnet restore $test --verbosity Warning
    & dotnet restore $project --verbosity Warning
    
    # calculate version, only when on a branch
    if ($(git log -n 1 --pretty=%d HEAD).Trim() -ne '(HEAD)')
    {
        Write-Output "Determining version number using gitversion"
        
        Push-Location $projectFolder 
        & dotnet gitversion $project --verbosity Warning
        Pop-Location

    }
    else
    {
        Write-Output "In a detached HEAD mode, unable to determine the version number using gitversion"     
    }
  

    & dotnet test $test -c Release
    if($LASTEXITCODE -ne 0) 
    {
        Write-Output "The tests failed"
        exit 1 
    }
  
    & dotnet pack $project -c Release -o .\artifacts 
  
    if($LASTEXITCODE -ne 0) 
    {
        Write-Output "Packing the sink failed"
        exit 1 
    }
    Write-Output "Building done"
}

$ErrorActionPreference = "Stop"
Invoke-Build 
