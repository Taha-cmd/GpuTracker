#Requires -Version 7.2

$private:tagPrefix = "gpu-tracker" 

# for simplicity, we use the same version for all three components
$private:version = Get-Content "$PSScriptRoot/_version.txt"

# map component name to tag
$private:dockerImages = @{
    App      = "$tagPrefix-app:$version"
    Consumer = "$tagPrefix-consumer:$version"
    Producer = "$tagPrefix-producer:$version"
}

# build the images in parallel to save time
$dockerImages.keys | ForEach-Object -Parallel {
    $tags = $Using:dockerImages # Using is required to capture variables from the outer scope in -Parallel foreach
    & Build/Build-DockerImage -Component $_ -Tag $tags[$_]
}
