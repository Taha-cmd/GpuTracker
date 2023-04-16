[CmdletBinding()]
param (
    [Parameter(Mandatory)][string] $Component,
    [Parameter(Mandatory)][string] $Tag,
    [Parameter(Mandatory)][string] $Version
)

$private:buildContext = "$PSScriptRoot\.."
$private:repository = "se22m001"
$private:fullName = "$repository/$($Tag):$($Version)"
$private:latest = "$repository/$($Tag):latest"

docker build $buildContext --file "$PSScriptRoot/Dockerfiles/$Component.Dockerfile" --tag $fullName
docker tag $fullName $latest

docker push $fullName
docker push $latest