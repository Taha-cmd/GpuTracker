dotnet build

Get-ChildItem "Models" | % Name | % { $_.Replace(".cs", "") } | ForEach-Object {
    dotnet avro create --type "GpuTracker.Producer.Models.$_" --assembly ".\bin\Debug\net7.0\GpuTracker.Producer.dll" | Out-File "Schemas/$_.avsc" -Force
}
