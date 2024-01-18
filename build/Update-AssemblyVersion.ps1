<#
.SYNOPSIS
  Builds the version number of Constellation application and updates an AssemblyInfo.cs file.
.DESCRIPTION
  This scripts allows to determine the version number of a Constellation application
  by following the version management convention specified in the
  https://dev.azure.com/kpmgfr/Constellation/_wiki/wikis/Constellation.wiki/1756/Version-management
  wiki page.
  This script do the following operation:
  - Determines the version number by following the Constellation versionning convention.
    Two kind of version is determined:
    - Standard version: <major>.<minor>.<build>.<revision>
    - Informational version: <major>.<minor>.<build>.<revision>-<branch>+<git commit>
    The "<major>.<minor>.<build>" components version are determined with the 'Version' parameter.
    The "<revision>" component version is determined with the 'Revision' parameter.
    The "<branch>" part of the informational version is determined with the 'Tag' parameter.
    The "<git commit>" part of the informational version is determined with the 'GitCommit' parameter.
    Only the first 8 characters of the Git commit are retrieved.
    If the "PullRequestID" parameter is specified, the "Tag" parameter is ignored and
    the "<branch>" part of the informational version will be define to "pr<id>". "<id>"
    represents the "PullRequestID" parameter value.
  - A "Version" property is exported to the Azure Pipelines, which represents the standard version.
  - A "InformationalVersion" property is exported to the Azure Pipelines, which represents the informational version.
  - Updates the build number of the Azure DevOps build using the informational version.
  - Defines the AssemblyVersion and AssemblyInformationalVersion C# attributes into the AssemblyInfo.cs
    which the part is specified in the "AssemblyInfoFile" parameter.
.PARAMETER AssemblyInfoFile
    The path of the AssemblyInfo.cs to update and to apply the AssemblyVersion and AssemblyInformationalVersion C# attributes
    with the standard version and informational version.
.PARAMETER Version
    Version to define for the "<major>.<minor>.<build>" components.
.PARAMETER Revision
    Revision of the version to define.
.PARAMETER Tag
    Branch name to define in the <branch> part of the informational version.
.PARAMETER PullRequestID
    ID of the pull request to define in the <branch> part of the informational version.
    If this parameter is specified, the Tag parameter is ignored and the <branch> part
    will be define to the following format "pr<PullRequestID>".
.PARAMETER GitCommit
    Full Git commit to specify in the <git-commit> part of the informational version.
    Only the first 8 characters are retrieved.
.LINK
  https://dev.azure.com/kpmgfr/Constellation/_wiki/wikis/Constellation.wiki/1756/Version-management
.NOTES
  Version:        1.0.0
  Author:         Gilles TOURREAU (gtourreau@kpmg.fr)
  Creation Date:  14/09/2020
  Purpose/Change:
  - 14/09/2020: Initial version.
.EXAMPLE
  .\Update-AssemblyVersion.ps1 -AssemblyInfoFile "ConsoleApp\AssemblyInfo.cs" -Revision 8 -Tag "master" -GitCommit "ab12cd8914ade934" -Version "4.7.2"
  The script will produce the following versions:
  - Standard version: 4.7.2.8
  - Informational version: 4.7.2.8-master+ab12cd89
  Also the "ConsoleApp\AssemblyInfo.cs" will be updated to define the AssemblyVersion and AssemblyInformationalVersion C# attributes.
.EXAMPLE
  .\Update-AssemblyVersion.ps1 -Revision 8 -PullRequestID "1040" -GitCommit "ab12cd8914ade934" -Version "4.7.2"
  The script will produce the following versions:
  - Standard version: 4.7.2.8
  - Informational version: 4.7.2.8-pr1040+ab12cd89
#>
param
(
    [Parameter()]
    [string]
    $AssemblyInfoFile,

    [Parameter(Mandatory = $true)]
    [string]
    $Version,

    [Parameter(Mandatory = $true)]
    [int]
    $Revision,

    [Parameter()]
    [string]
    $Tag,

    [Parameter()]
    [string]
    $PullRequestID,

    [Parameter(Mandatory = $true)]
    [string]
    $GitCommit
)

function Update-AssemblyAttribute([string]$content, [string]$name, [string]$value)
{
    $regex = "^\[assembly: $name(?:Attribute)?\(""(.*?)""\)\]"
    $matchesFound = [regex]::Matches($content, $regex, "Multiline")

    if ($matchesFound.Count -gt 0)
    {
        $newAttribute = "[assembly: $name(""$value"")]"
        $content = [regex]::Replace($content, $regex, $newAttribute, "Multiline")
    }
    else
    {
        $content = "$content`r`n[assembly: $name(""$value"")]"
    }

    return $content
}

try
{
    ### Builds the version of the assembly. ###
    $assemblyVersion = [System.Version]::Parse($Version)
    $assemblyVersion = "$($assemblyVersion.Major).$($assemblyVersion.Minor).$($assemblyVersion.Build).$Revision"

    ### Builds the informational version (<a.b.c.d>-<tag>+<gitcommit>) ###
    $informationalVersion = $assemblyVersion

    # Append the pull request ID if specified in the arguments to the informational version
    if ($PullRequestID)
    {
        $informationalVersion = "$informationalVersion+pr$PullRequestID"
    }
    else
    {
        # Append the tag if specified in the arguments to the informational version
        if ($Tag)
        {
            $informationalVersion = "$informationalVersion+$Tag"
        }
    }

    # Append the Git commit
    $informationalVersion = "$informationalVersion-$($GitCommit.Substring(0, 8))"
  
    Write-Host "Version: $assemblyVersion"
    Write-Host "Informational version: $informationalVersion"

    ### Updates the AssemblyInfo file if need ###
    if ($AssemblyInfoFile)
    {
        if (-Not(Test-Path $AssemblyInfoFile))
        {
            Write-Error "The AssemblyInfo file '$AssemblyInfoFile' does not exists."
            Exit 1
        }

        # Read the AssemblyInfo file.
        $assemblyInfoFileContent = Get-Content -Path $AssemblyInfoFile -Raw

        # Updates the AssemblyInfo file with the new version.
        $assemblyInfoFileContent = Update-AssemblyAttribute $assemblyInfoFileContent "AssemblyVersion" "$assemblyVersion"
        
        # Updates the AssemblyInfo file with the new informational version.
        $assemblyInfoFileContent = Update-AssemblyAttribute $assemblyInfoFileContent "AssemblyInformationalVersion" "$informationalVersion"
        
        # Writes the new updated AssemblyInfo file.
        Set-Content -Path $AssemblyInfoFile -Value $assemblyInfoFileContent

        Write-Host "'$AssemblyInfoFile' has been updated with the new assembly version attributes"
    }

    # Exports variable to Azure Pipelines agent
    Write-Host "##vso[task.setvariable variable=Version]$assemblyVersion"
    Write-Host "##vso[task.setvariable variable=InformationalVersion]$informationalVersion"
    Write-Host "##vso[build.updatebuildnumber]$informationalVersion"
}
catch
{
    Write-Error $_
    Exit 2
}