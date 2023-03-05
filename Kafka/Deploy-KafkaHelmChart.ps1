#Requires -Version 7.2

& helm repo add "bitnami" "https://charts.bitnami.com/bitnami" | Out-Null

helm upgrade -f "kafka-helm-chart-values.yaml" kafka bitnami/kafka --atomic --namespace "kafka" --create-namespace --install --timeout 1m

# deploy a kafka standalone client pod for debugging
kubectl run kafka-client --restart='Never' --image docker.io/bitnami/kafka:3.4.0-debian-11-r6 --namespace kafka --command -- sleep infinity | Out-Null
# kubectl exec --tty -i kafka-client --namespace kafka -- bash