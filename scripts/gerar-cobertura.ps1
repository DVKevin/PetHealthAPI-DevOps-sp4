# ============================================================================
# Pet Health API - Relatório de cobertura das camadas Domínio e Aplicação
#
# Uso (na raiz do repositório):   .\scripts\gerar-cobertura.ps1
# Resultado: coveragereport\index.html (abra no navegador) e resumo no terminal
# ============================================================================
$ErrorActionPreference = "Stop"

Remove-Item -Recurse -Force TestResults, coveragereport -ErrorAction SilentlyContinue

Write-Host "1/3 Executando testes com coleta de cobertura..." -ForegroundColor Cyan
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults
if ($LASTEXITCODE -ne 0) { throw "Há testes falhando. Corrija antes de gerar a cobertura." }

Write-Host "2/3 Verificando o ReportGenerator..." -ForegroundColor Cyan
$instalado = dotnet tool list --global | Select-String "dotnet-reportgenerator-globaltool"
if (-not $instalado) {
    dotnet tool install --global dotnet-reportgenerator-globaltool
}

Write-Host "3/3 Gerando relatório (somente Domínio e Aplicação)..." -ForegroundColor Cyan
reportgenerator `
    -reports:"TestResults/**/coverage.cobertura.xml" `
    -targetdir:"coveragereport" `
    -reporttypes:"Html;TextSummary" `
    -assemblyfilters:"+PetHealthAPI.Dominio;+PetHealthAPI.Aplicacao"

Get-Content coveragereport\Summary.txt | Select-Object -First 25
Write-Host "Relatório completo: coveragereport\index.html" -ForegroundColor Green
