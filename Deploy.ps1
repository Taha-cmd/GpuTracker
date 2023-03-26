#Requires -Version 7.2

[CmdletBinding()]
param (
    [Parameter(Mandatory)][ValidateSet("Debug", "Kubernetes")][string]$Mode
)

$valuesFile = "$PSScriptRoot\Deployment\$Mode-values.yaml"

# helm dependency build
helm upgrade -f $valuesFile gpu-tracker "$PSScriptRoot\GpuTrackerHelmChart\" --atomic --namespace "gpu-tracker" --create-namespace --install --timeout 5m