# 🐾 Pet Health API — DevOps Tools & Cloud Computing

> **FIAP · 2TDSPV · Entrega da 4ª Sprint · Prof. João Menk**
> CI/CD da Pet Health API (.NET 8) com **Azure DevOps**, deploy em **Azure Web App**, segredos no **Azure Key Vault** e banco **Oracle (FIAP)**.

## 📑 Sumário

1. [Descrição da solução](#1-descrição-da-solução)
2. [Diagrama da arquitetura + fluxo CI/CD](#2-diagrama-da-arquitetura--fluxo-cicd)
3. [Banco de dados em nuvem](#3-banco-de-dados-em-nuvem)
4. [Projeto no Azure DevOps](#4-projeto-no-azure-devops)
5. [Pipelines CI/CD](#5-pipelines-cicd)
6. [Variáveis de ambiente protegidas](#6-variáveis-de-ambiente-protegidas-usuários-senhas-e-tokens)
7. [Como reproduzir](#7-como-reproduzir)
8. [Integrantes](#8-integrantes)

---

## 🔗 Links da entrega

| Item                    | Link                                                                                                                                 |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Projeto no Azure DevOps | [dev.azure.com/pethealth-fiap/Sprint 4 – Azure DevOps](https://dev.azure.com/pethealth-fiap/Sprint%204%20%E2%80%93%20Azure%20DevOps) |
| Repositório GitHub      | [github.com/DVKevin/PetHealthAPI-DevOps-sp4](https://github.com/DVKevin/PetHealthAPI-DevOps-sp4)                                     |
| Vídeo (YouTube)         | [youtu.be/Ki3vGJctFpg](https://youtu.be/Ki3vGJctFpg)                                                                                 |
| API publicada (Swagger) | [pethealth-api-rm563454.azurewebsites.net](https://pethealth-api-rm563454.azurewebsites.net)                                         |
| Health check            | [/health](https://pethealth-api-rm563454.azurewebsites.net/health)                                                                   |

## 1. Descrição da solução

A **Pet Health API** é a API REST da plataforma Pet Health (Challenge FIAP 2026 × CLYVO VET). Ela centraliza a jornada de saúde do animal: cadastro de **tutores e pets**, **carteira de vacinação** com alerta de próxima dose, **histórico de consultas**, **medicamentos** e um **diário de cuidados gamificado** (XP e níveis). O acesso é protegido por **JWT** com perfis `Admin` e `Usuario`.

Nesta Sprint a aplicação é entregue com **CI/CD no Azure DevOps**: cada push na branch `master` compila, executa os testes automáticos e gera um artefato; o novo artefato dispara automaticamente o deploy no **Azure Web App**, que lê seus segredos do **Azure Key Vault** e persiste os dados no **Oracle (FIAP)**.

**Stack de tecnologias**

| Camada                                  | Tecnologia                                                                                               |
| --------------------------------------- | -------------------------------------------------------------------------------------------------------- |
| Aplicação                               | .NET 8 · ASP.NET Core Web API · Clean Architecture · EF Core 8 · JWT · Swagger · Serilog · OpenTelemetry |
| Banco de dados relacional (obrigatório) | **Oracle Database — Oracle FIAP** (`oracle.fiap.com.br:1521/ORCL`)                                       |
| Banco NoSQL (diário de cuidados)        | MongoDB Atlas M0 (cluster hospedado na Azure)                                                            |
| Testes automáticos                      | xUnit · Moq · FluentAssertions · WebApplicationFactory + EF Core InMemory · Coverlet (cobertura)         |
| Repositório                             | Azure Repos (Git, branch `master`) + espelho no GitHub                                                   |
| CI/CD                                   | Azure Pipelines (YAML): `azure-pipelines-ci.yml` e `azure-pipelines-cd.yml`                              |
| Artefato                                | Azure Pipelines Artifacts (`pethealth-api` → `PetHealthAPI.zip`)                                         |
| Deploy (PaaS)                           | **Azure Web App** Linux · .NET 8 · App Service Plan B1 · região Central US                               |
| Segredos                                | **Azure Key Vault** (RBAC) + Managed Identity do Web App + Variable Group vinculado ao Key Vault         |
| Infraestrutura                          | Azure CLI (`scripts/infra-azure.sh`)                                                                     |

## 2. Diagrama da arquitetura + fluxo CI/CD

![Arquitetura e fluxo CI/CD](docs/arquitetura-devops.png)

> Versão vetorial: [`docs/arquitetura-devops.svg`](docs/arquitetura-devops.svg). Linhas azuis = fluxo da aplicação; linhas roxas tracejadas = implantação e operação (DevOps); números = ordem da esteira CI/CD.

| #   | O que acontece                                                                        | Ferramenta                                  |
| --- | ------------------------------------------------------------------------------------- | ------------------------------------------- |
| 1   | Desenvolvedor faz commit e `git push` na `master` (espelho no GitHub)                 | IDE + Git Bash → Azure Repos                |
| 2   | O push na `master` dispara automaticamente a pipeline **PetHealth-CI**                | Azure Pipelines (trigger CI)                |
| 3   | Restore, build e **testes automáticos** (unitários + integração) com cobertura        | Azure Pipelines · xUnit · Coverlet          |
| 4   | Publish da API e **publicação do artefato** `pethealth-api`                           | Azure Pipelines Artifacts                   |
| 5   | O **novo artefato** dispara automaticamente a pipeline **PetHealth-CD**               | Azure Pipelines (pipeline resource trigger) |
| 6   | O CD lê os segredos protegidos do Variable Group `pethealth-keyvault`                 | Library + Azure Key Vault                   |
| 7   | Deploy do `.zip` no Azure Web App via Service Connection (ARM)                        | Azure Pipelines → Azure Web App             |
| 8   | O Web App lê os segredos no Key Vault via **Managed Identity** (Key Vault references) | Azure Key Vault                             |
| 9   | A API persiste o CRUD no Oracle FIAP                                                  | EF Core → Oracle                            |
| 10  | A API grava o diário de cuidados no MongoDB Atlas                                     | MongoDB.Driver → Atlas                      |
| 11  | Usuário final consome a API por HTTPS (Swagger / REST)                                | Navegador / Postman                         |
| 12  | Equipe comprova a persistência com `SELECT` no banco                                  | SQL Developer → Oracle                      |

## 3. Banco de dados em nuvem

**Oracle da FIAP** (opção aceita na Sprint). As tabelas `TB_PH_*` são criadas pela migration `Sprint4Inicial` (EF Core). Consultas de evidência do CRUD: [`docs/crud-evidencias.sql`](docs/crud-evidencias.sql).

No vídeo, o CRUD completo (inserir, consultar, atualizar e deletar) é feito em **duas tabelas**, `TB_PH_TUTOR` e `TB_PH_PET`, via Swagger no Azure Web App, com `SELECT` no Oracle depois de cada operação.

## 4. Projeto no Azure DevOps

| Campo             | Valor                                                                                     |
| ----------------- | ----------------------------------------------------------------------------------------- |
| Project Name      | `Sprint 4 – Azure DevOps`                                                                 |
| Description       | Projeto para entrega da Sprint 4 do professor João Menk + integrantes (RM - nome - turma) |
| Visibility        | Private                                                                                   |
| Version control   | Git                                                                                       |
| Work item process | Scrum                                                                                     |
| Professor         | convidado com acesso **Basic**                                                            |

## 5. Pipelines CI/CD

**CI — `azure-pipelines-ci.yml` (pipeline `PetHealth-CI`)**

| Regra                                | Como foi atendida                                                                                    |
| ------------------------------------ | ---------------------------------------------------------------------------------------------------- |
| Conectada ao repositório             | Pipeline YAML criada a partir do Azure Repos do projeto                                              |
| Dispara a cada alteração na `master` | `trigger: branches: include: [ master ]`                                                             |
| Execução de testes                   | `dotnet test` nos projetos `tests/**` (unitários + integração) com resultados e cobertura publicados |
| Geração e publicação do artefato     | `dotnet publish` (zip) + `PublishPipelineArtifact` → artefato `pethealth-api`                        |

**CD — `azure-pipelines-cd.yml` (pipeline `PetHealth-CD`)**

| Regra                      | Como foi atendida                                                                                              |
| -------------------------- | -------------------------------------------------------------------------------------------------------------- |
| Dispara após novo artefato | `resources.pipelines` com `trigger` na pipeline `PetHealth-CI` (branch `master`); `trigger: none` para commits |
| Deploy em Azure Web App    | Task `AzureWebApp@1` (Linux, `DOTNETCORE\|8.0`, zip deploy) no environment `producao`                          |
| Validação pós-deploy       | Smoke test em `/health/live`, `/health` e login do administrador                                               |

## 6. Variáveis de ambiente protegidas (usuários, senhas e tokens)

| Segredo                                         | Onde fica       | Como chega na aplicação                                                                                                |
| ----------------------------------------------- | --------------- | ---------------------------------------------------------------------------------------------------------------------- |
| `oracle-connection` (usuário + senha do Oracle) | Azure Key Vault | App Setting `ConnectionStrings__OracleConnection` = Key Vault reference                                                |
| `mongo-connection` (usuário + senha do Atlas)   | Azure Key Vault | App Setting `MongoDb__ConnectionString` = Key Vault reference                                                          |
| `jwt-chave` (chave de assinatura dos tokens)    | Azure Key Vault | App Setting `Jwt__Chave` = Key Vault reference                                                                         |
| `admin-senha` (senha do administrador)          | Azure Key Vault | App Setting `AdminPadrao__Senha` = Key Vault reference; usada no smoke test do CD via Variable Group (mascarada `***`) |

- Nenhum segredo está no Git: `appsettings.json` traz as chaves **vazias**; `appsettings.Testing.json` contém apenas valores fictícios usados pelos testes automatizados e **não é publicado** no Web App.
- O Web App acessa o Key Vault com **Managed Identity** e papel _Key Vault Secrets User_ (somente leitura).
- O Azure DevOps acessa o Key Vault pelo **Variable Group `pethealth-keyvault`** (Library), vinculado ao cofre.

## 7. Como reproduzir

```bash
# 1. Infraestrutura (Git Bash, logado com az login)
bash scripts/infra-azure.sh

# 2. Tabelas no Oracle (uma vez)
dotnet ef database update --project src/PetHealthAPI.Infraestrutura --startup-project src/PetHealthAPI

# 3. Azure DevOps: Service Connection "sc-azure-pethealth", Variable Group "pethealth-keyvault",
#    pipelines "PetHealth-CI" (azure-pipelines-ci.yml) e "PetHealth-CD" (azure-pipelines-cd.yml)

# 4. Qualquer push na master executa CI -> artefato -> CD -> Web App
git push
```

## 8. Integrantes

| Nome                       | RM     | Turma  |
| -------------------------- | ------ | ------ |
| Matheus Arazin de Oliveira | 556649 | 2TDSPV |
| Artur Pioli Silva          | 565597 | 2TDSPV |
| Kevin Martins Campos       | 563454 | 2TDSPV |
| Pedro Gabriel Claes        | 566058 | 2TDSPV |

---

## 📁 Estrutura do repositório

```
PetHealthAPI-DevOps-sp4/
├── azure-pipelines-ci.yml      # Pipeline de CI (build, testes, artefato)
├── azure-pipelines-cd.yml      # Pipeline de CD (deploy no Azure Web App + smoke test)
├── scripts/infra-azure.sh      # Provisionamento: RG, App Service, Web App, Key Vault
├── docs/
│   ├── arquitetura-devops.png  # Diagrama da arquitetura + fluxo CI/CD
│   └── crud-evidencias.sql     # SELECTs usados como evidência do CRUD
├── src/                        # Código da API (.NET 8)
└── tests/                      # Testes unitários e de integração (xUnit)
```

---

_Challenge 2026 — FIAP × CLYVO VET · Grupo Pet Health_
