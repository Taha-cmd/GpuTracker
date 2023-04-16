#Requires -Version 7.2

# basic and quick installation guide: https://argo-cd.readthedocs.io/en/stable/getting_started/
# the installation can be further customized using the community maintained helm chart: https://github.com/argoproj/argo-helm/tree/main/charts/argo-cd
# there is a core installation that does not include the ui https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/core-install.yaml

$private:installationManifestUrl = "https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml"

kubectl create namespace argocd | Out-Null
kubectl apply -n argocd -f $installationManifestUrl | Out-Null

$private:passwordBase64 = kubectl get secrets/argocd-initial-admin-secret -n argocd -o template='{{.data.password}}'

# port forward the "argocd-server" service and login with "admin" and this password
[Text.Encoding]::Utf8.GetString([Convert]::FromBase64String($passwordBase64)) | Out-Host


