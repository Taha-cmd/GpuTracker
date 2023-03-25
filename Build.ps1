#Requires -Version 7.2

& Build/Build-DockerImage -Component "App" -Tag "gpu-tracker-app"
& Build/Build-DockerImage -Component "Consumer" -Tag "gpu-tracker-consumer"
& Build/Build-DockerImage -Component "Producer" -Tag "gpu-tracker-producer"