param([string]$SolutionPath,[string]$ModuleProjectPath,[string]$HostProjectPath)
dotnet sln $SolutionPath add $ModuleProjectPath
dotnet add $HostProjectPath reference $ModuleProjectPath
