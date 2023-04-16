#Requires -Version 7.2

# map component name to tag
$private:dockerImages = @{
    App      = "gpu-tracker-app"
    Consumer = "gpu-tracker-consumer"
    Producer = "gpu-tracker-producer"
}

# build the images in parallel to save time
$dockerImages.keys | ForEach-Object -Parallel {
    $tags = $Using:dockerImages # Using is required to capture variables from the outer scope in -Parallel foreach
    & Build/Build-DockerImage -Component $_ -Tag $tags[$_]
}
