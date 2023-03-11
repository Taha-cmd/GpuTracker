#Requires -Version 7.2

# helm dependency build
helm upgrade gpu-tracker "$PSScriptRoot\..\GpuTrackerHelmChart\" --atomic --namespace "gpu-tracker" --create-namespace --install --timeout 1m
# kubectl port-forward service/kafka-ui 8080:8080 --namespace=kafka
