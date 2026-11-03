#!/usr/bin/env bash
# =============================================================================
#  Pet Health API - Provisionamento da infraestrutura no Azure (Sprint 4)
#  Executar no Git Bash (Windows) ou Linux, já logado com: az login
#
#  Cria:
#   - Resource Group            rg-pethealth-sprint4
#   - App Service Plan (Linux)  plan-pethealth-sprint4   (SKU B1)
#   - Web App .NET 8            pethealth-api-rm563454   (com Managed Identity)
#   - Azure Key Vault           kv-pethealth-563454      (RBAC)
#   - Segredos no Key Vault     oracle-connection, mongo-connection,
#                               jwt-chave, admin-senha
#   - App Settings do Web App   como REFERÊNCIAS ao Key Vault (sem senha exposta)
#
#  As senhas são digitadas na hora (read -s): não ficam no script, no Git
#  nem no histórico do terminal.
# =============================================================================
set -euo pipefail

# Git Bash converte argumentos que começam com "/" em caminhos do Windows.
# Isso quebra os --scope /subscriptions/... das atribuições de papel.
export MSYS_NO_PATHCONV=1

# No Windows o az devolve "\r" no fim das saídas -o tsv; isso quebra os IDs.
tsv() { az "$@" -o tsv | tr -d '\r'; }

# ------------------------- Parâmetros (ajuste se precisar) -------------------
SUBSCRIPTION_ID="${SUBSCRIPTION_ID:-}"          # vazio = assinatura padrão do az
LOCATION="${LOCATION:-centralus}"
RG="${RG:-rg-pethealth-sprint4}"
PLAN="${PLAN:-plan-pethealth-sprint4}"
PLAN_SKU="${PLAN_SKU:-B1}"
WEBAPP="${WEBAPP:-pethealth-api-rm563454}"
KV="${KV:-kv-pethealth-563454}"
ORACLE_USER="${ORACLE_USER:-rm563454}"
ORACLE_DATASOURCE="${ORACLE_DATASOURCE:-oracle.fiap.com.br:1521/orcl}"
# -----------------------------------------------------------------------------

echo "==> Assinatura"
if [ -n "$SUBSCRIPTION_ID" ]; then az account set --subscription "$SUBSCRIPTION_ID"; fi
az account show --query "{assinatura:name, id:id, usuario:user.name}" -o table
SUB_ID=$(tsv account show --query id)

echo
echo "==> Digite os segredos (nada aparece na tela enquanto digita)"
read -r -s -p "Senha do Oracle FIAP do usuário $ORACLE_USER: " ORACLE_PASSWORD; echo
read -r -s -p "Connection string do MongoDB Atlas (mongodb+srv://...): " MONGO_CONNECTION; echo
read -r -s -p "Nova senha do admin da API (ex.: PetHealth@Sprint4, sem aspas): " ADMIN_SENHA; echo
[ -n "$MONGO_CONNECTION" ] || MONGO_CONNECTION="mongodb://mongo-nao-configurado:27017"
JWT_CHAVE=$(openssl rand -base64 48 | tr -d '\r\n')
ORACLE_CONNECTION="User Id=${ORACLE_USER};Password=${ORACLE_PASSWORD};Data Source=${ORACLE_DATASOURCE};"

echo
echo "==> Registrando providers (só demora na primeira vez)"
az provider register --namespace Microsoft.Web --wait
az provider register --namespace Microsoft.KeyVault --wait

echo "==> Resource Group $RG ($LOCATION)"
az group create --name "$RG" --location "$LOCATION" -o none

echo "==> App Service Plan $PLAN (Linux $PLAN_SKU)"
az appservice plan create --name "$PLAN" --resource-group "$RG" \
  --location "$LOCATION" --is-linux --sku "$PLAN_SKU" -o none

echo "==> Web App $WEBAPP (.NET 8)"
az webapp create --name "$WEBAPP" --resource-group "$RG" --plan "$PLAN" \
  --runtime "DOTNETCORE:8.0" -o none

az webapp config set --name "$WEBAPP" --resource-group "$RG" \
  --startup-file "dotnet PetHealthAPI.dll" --always-on true -o none
az webapp update --name "$WEBAPP" --resource-group "$RG" --https-only true -o none

echo "==> Managed Identity do Web App"
WEBAPP_PRINCIPAL_ID=$(tsv webapp identity assign --name "$WEBAPP" --resource-group "$RG" \
  --query principalId)

echo "==> Key Vault $KV (modelo de permissão RBAC)"
az keyvault create --name "$KV" --resource-group "$RG" --location "$LOCATION" \
  --enable-rbac-authorization true -o none
KV_ID=$(tsv keyvault show --name "$KV" --query id)

echo "==> Permissões no Key Vault"
ME_ID=$(tsv ad signed-in-user show --query id)
# Você: pode criar/atualizar segredos
az role assignment create --role "Key Vault Secrets Officer" \
  --assignee-object-id "$ME_ID" --assignee-principal-type User --scope "$KV_ID" -o none
# Web App: só pode LER segredos (princípio do menor privilégio)
az role assignment create --role "Key Vault Secrets User" \
  --assignee-object-id "$WEBAPP_PRINCIPAL_ID" --assignee-principal-type ServicePrincipal --scope "$KV_ID" -o none

echo "    aguardando 60s a propagação das permissões..."
sleep 60

echo "==> Gravando segredos no Key Vault"
az keyvault secret set --vault-name "$KV" --name oracle-connection --value "$ORACLE_CONNECTION" -o none
az keyvault secret set --vault-name "$KV" --name mongo-connection  --value "$MONGO_CONNECTION"  -o none
az keyvault secret set --vault-name "$KV" --name jwt-chave         --value "$JWT_CHAVE"         -o none
az keyvault secret set --vault-name "$KV" --name admin-senha       --value "$ADMIN_SENHA"       -o none
unset ORACLE_PASSWORD ORACLE_CONNECTION MONGO_CONNECTION ADMIN_SENHA JWT_CHAVE

echo "==> App Settings do Web App (referências ao Key Vault)"
az webapp config appsettings set --name "$WEBAPP" --resource-group "$RG" -o none --settings \
  "ConnectionStrings__OracleConnection=@Microsoft.KeyVault(VaultName=${KV};SecretName=oracle-connection)" \
  "MongoDb__ConnectionString=@Microsoft.KeyVault(VaultName=${KV};SecretName=mongo-connection)" \
  "Jwt__Chave=@Microsoft.KeyVault(VaultName=${KV};SecretName=jwt-chave)" \
  "AdminPadrao__Senha=@Microsoft.KeyVault(VaultName=${KV};SecretName=admin-senha)" \
  "ASPNETCORE_ENVIRONMENT=Production" \
  "Observabilidade__ExportarTracesNoConsole=false"

az webapp restart --name "$WEBAPP" --resource-group "$RG" -o none

echo
echo "================================================================"
echo " Infra criada com sucesso!"
echo " Assinatura (Subscription ID): $SUB_ID"
echo " Resource Group:               $RG"
echo " Web App:                      https://${WEBAPP}.azurewebsites.net"
echo " Key Vault:                    $KV"
echo " Key Vault ID (para o passo do Variable Group):"
echo "   $KV_ID"
echo "================================================================"
