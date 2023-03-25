#Requires -Version 7.2

[CmdletBinding()]
param (
    [Parameter(Mandatory)][ValidateSet("Debug", "Kubernetes")][string]$Mode
)

$valuesFile = "$PSScriptRoot\Deployment\$Mode-values.yaml"

# helm dependency build
helm upgrade -f $valuesFile gpu-tracker "$PSScriptRoot\GpuTrackerHelmChart\" --atomic --namespace "gpu-tracker" --create-namespace --install --timeout 5m

# if ($Mode -eq "Debug") {
#     kubectl port-forward service/kafka-ui 8080:8080 --namespace=kafka
#     # kubectl port-forward service/
# }

