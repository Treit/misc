$env:SUBSCRIPTION_ID = (az account list | ConvertFrom-Json | ?{$_.user.name -eq "mtreit@hotmail.com"}).id
az account set --subscription $env:SUBSCRIPTION_ID

# resource group
$env:RESOURCE_GROUP = "redistest"
$env:RESOURCE_LOCATION="westus2"
az group create -g $env:RESOURCE_GROUP --location $env:RESOURCE_LOCATION

# redis cache
$env:REDIS_CACHE_NAME = "mtreitcache"
#az redis create --location $env:RESOURCE_LOCATION --name $env:REDIS_CACHE_NAME --resource-group $env:RESOURCE_GROUP --sku "Basic" --vm-size "c3"



# cleanup
# az group delete --resource-group $env:RESOURCE_GROUP --yes