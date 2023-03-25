[CmdletBinding()]
param (
    [Parameter(Mandatory)][string] $Component,
    [Parameter(Mandatory)][string] $Tag
)

$private:buildContext = "$PSScriptRoot\.."
$private:repository = "se22m001"
$private:fullName = "$repository/$Tag"

docker build $buildContext --file "$PSScriptRoot/Dockerfiles/$Component.Dockerfile" --tag $fullName
docker push $fullName