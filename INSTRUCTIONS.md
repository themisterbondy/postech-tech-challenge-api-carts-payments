# Instruções para Deploy do MyFood Carts & Payments WebAPI (Docker Local)

## Construção e Publicação da Imagem Docker
Execute os comandos abaixo para construir e publicar a imagem Docker da aplicação:

```sh
docker build -t themisterbondy/myfood-carts-payments-webapi -f src/Postech.Fiap.CartsPayments.WebApi/Dockerfile .

docker push themisterbondy/myfood-carts-payments-webapi
```

## Criação do Namespace
Crie o namespace para a aplicação do MyFood Carts & Payments WebAPI:

```sh
kubectl create namespace myfood-namespace
```

## ConfigMaps para o MyFood Carts & Payments WebAPI

### ConfigMap para Banco de Dados
Crie o ConfigMap para as credenciais de conexão com o banco de dados:

```sh
kubectl create configmap myfood-db-carts-payments-config --namespace=myfood-namespace --from-literal=ConnectionStrings__SQLConnection="Host=host.docker.internal;Database=myfooddb-cartspayments;Username=myfooduser;Password=blueScreen@666"
```

### ConfigMap para MyFood Products Client
Crie o ConfigMap para configurar a URL base do serviço de produtos:

```sh
kubectl create configmap myfood-products-config --namespace=myfood-namespace --from-literal=MyFoodProductsHttpClientSettings__BaseUrl="http://myfood-products-webapi:80/api"
```

### ConfigMap para Azure Storage Account
Crie o ConfigMap para armazenar as configurações da conta de armazenamento Azure:

```sh
kubectl create configmap myfood-storage-account-config --namespace=myfood-namespace --from-literal=AzureStorageSettings__ConnectionString="UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://host.docker.internal"
```

## Instalação do MyFood Carts & Payments WebAPI com Helm
Use o seguinte comando para instalar a aplicação no Kubernetes com Helm:

```sh
helm install myfood-carts-payments-webapi .\charts\webapi\ --namespace myfood-namespace
```

## Atualização do MyFood Carts & Payments WebAPI com Helm
Se precisar atualizar a aplicação, execute:

```sh
helm upgrade myfood-carts-payments-webapi .\charts\webapi\ --namespace myfood-namespace
```

## Redirecionamento de Porta no MyFood Carts & Payments WebAPI
Para acessar a aplicação localmente, utilize o comando:

```sh
kubectl port-forward svc/myfood-carts-payments-webapi 60074:80 -n myfood-namespace
```

## Monitoramento e Depuração

### Obter a Lista de Pods
Monitore os Pods no namespace com o seguinte comando:

```sh
kubectl get pods --namespace myfood-namespace --watch
```

### Acessar um Pod Interativamente
Entre em um Pod da aplicação de forma interativa:

```sh
kubectl exec -it myfood-carts-payments-webapi-75ccdb8997-dg624 -- /bin/sh
```

### Descrever um Pod
Obtenha mais informações sobre um Pod específico:

```sh
kubectl describe pod myfood-carts-payments-webapi-64d46cb67-nkbzl --namespace myfood-namespace
```

### Verificar Logs dos Pods
Verifique os logs de execução dos Pods com os comandos abaixo:

```sh
kubectl logs myfood-carts-payments-webapi-64d46cb67-nkbzl --namespace myfood-namespace
```

### Remover o Deployment
Para remover apenas o deployment do MyFood Carts & Payments WebAPI:

```sh
kubectl delete deployment myfood-carts-payments-webapi --namespace myfood-namespace
```

## Remover Recursos do MyFood Carts & Payments WebAPI

### Remover o Namespace
Para excluir o namespace e seus recursos, use:

```sh
kubectl delete namespace myfood-namespace
```