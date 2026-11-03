# 🧪 Roteiro de testes — Pet Health API (VS Code)

Passo a passo para validar **todos os requisitos da Sprint 4** no VS Code. Execute os comandos no terminal integrado (**Ctrl + '**), na raiz do repositório.

---

## 0. Preparação (uma vez)

**Extensões do VS Code:**

| Extensão | Para quê |
|---|---|
| **C# Dev Kit** (Microsoft) | Compilar, depurar e ver os testes no painel *Testing* |
| **REST Client** (Huachao Mao) | Executar o arquivo `docs/pethealth.http` |

**Ferramentas:** .NET SDK 8, Docker Desktop aberto (*Engine running*), `dotnet-ef` 8.0.11.

**Connection string do Oracle** (fica fora do Git):

```powershell
dotnet user-secrets set "ConnectionStrings:OracleConnection" "User Id=SEU_RM;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/orcl;" --project src\PetHealthAPI
```

---

## 1. Compilação — evita a penalidade de -20 pts

```powershell
dotnet build
```

✅ Esperado: `Compilação com êxito` · `0 Erro(s)`

---

## 2. Testes automatizados — evita a penalidade de -30 pts

```powershell
dotnet test
```

✅ Esperado: `Aprovado!` nos dois projetos (`PetHealthAPI.Tests.Unit` e `PetHealthAPI.Tests.Integration`), `Com falha: 0`.

> Alternativa visual: painel **Testing** (ícone de frasco na barra lateral) → **Run Tests**.

---

## 3. Cobertura de Domínio e Aplicação

```powershell
.\scripts\gerar-cobertura.ps1
```

✅ Esperado: resumo com a cobertura de `PetHealthAPI.Dominio` e `PetHealthAPI.Aplicacao` no terminal. Abra `coveragereport\index.html` no navegador para ver linha a linha.

> Se o PowerShell bloquear o script: `powershell -ExecutionPolicy Bypass -File .\scripts\gerar-cobertura.ps1`

---

## 4. Infraestrutura (MongoDB e Oracle)

```powershell
docker compose up -d
```
```powershell
docker ps
```

✅ Esperado: container `pethealth-mongo` com status `Up`.

```powershell
dotnet ef database update --project src\PetHealthAPI.Infraestrutura --startup-project src\PetHealthAPI
```

✅ Esperado: `Done.` (se já estiver aplicada, não faz nada).

---

## 5. Executar a API

```powershell
dotnet run --project src\PetHealthAPI
```

✅ Esperado no terminal:
- `Now listening on: http://localhost:5000`
- na primeira execução: `Administrador padrão criado: admin@pethealth.com`

Deixe esse terminal aberto e abra um **segundo terminal** (botão **+**) para os próximos passos.

---

## 6. Health checks

Abra `http://localhost:5000/health` no navegador.

✅ Esperado: `"status": "Healthy"` com `oracle`, `mongodb` e `memoria` em `Healthy`.

> Teste de resiliência (opcional): `docker compose stop` → recarregue `/health` → `mongodb` e o status geral ficam `Degraded` (a API continua no ar). Depois: `docker compose start`.

---

## 7. Roteiro de requisições (REST Client)

Abra `docs/pethealth.http` e clique em **Send Request** acima de cada bloco, **na ordem**. Os IDs e o token são capturados automaticamente.

| # | Requisição | Resultado esperado | Requisito validado |
|---|---|---|---|
| 1 | `GET /health` | 200, Healthy | Health checks |
| 2 | `GET /health/live` | 200 | Health checks |
| 3 | `GET /api/pets` sem token | 401 + ProblemDetails | JWT |
| 4 | Login admin | 200 + `token`, perfil `Admin` | JWT |
| 5 | `GET /api/autenticacao/eu` | 200, perfil `Admin` | JWT |
| 6 | Login com senha errada | 401 "Email ou senha inválidos." | JWT + erros |
| 7 | `POST /api/tutores` | 201 com `dados` + `links` | CRUD + HATEOAS (Oracle) |
| 8 | Tutor com email inválido | 400 "O email do tutor é inválido." | Domínio rico + erros |
| 9 | `POST /api/pets` | 201 com links (tutor, vacinas, pontos...) | HATEOAS |
| 10 | Mais 2 pets | 201, 201 | — |
| 11 | Pet com peso 0 | 400 "O peso do pet deve ser maior que zero." | Validação de domínio |
| 12 | Pets paginados (`tamanhoPagina=2`) | 200, 2 itens, links `proxima` e `ultima` | Paginação + ordenação + filtro |
| 13 | Pets por espécie, idade desc | 200 ordenado | Filtros + ordenação |
| 14 | `GET /api/tutores/{id}/pets` | 200 | Relacionamento |
| 15 | `POST /api/vacinas` | 201 | CRUD |
| 16 | Vacinas `vencendoEmDias=60` | 200 com a V10 | Filtro de negócio |
| 17 | `POST /api/consultas` | 201 | CRUD |
| 18 | `POST /api/medicamentos` | 201 com link `encerrar` | HATEOAS por estado |
| 19 | `PATCH .../encerrar` | 200, `ativo: false`, **sem** link `encerrar` | HATEOAS por estado |
| 20 | `POST /api/registros-diarios` | 201, `pontos: 30` | **MongoDB** |
| 21 | Segundo registro | 201, `pontos: 40` | MongoDB |
| 22 | `GET .../pets/{id}/pontos` | 200, `totalPontos: 70`, `nivel: 1` | Agregação MongoDB |
| 23 | Registros ordenados por pontos | 200 | Paginação no MongoDB |
| 24 | `GET /api/pets/999999` com `X-Correlation-ID` | 404 + `correlationId: "teste-roteiro-404"` | Tratamento global + rastreabilidade |
| 25 | Registrar usuário comum | 201, perfil `Usuario` | JWT |
| 26 | Login do usuário comum | 200 + token | JWT |
| 27 | `DELETE` como usuário comum | 403 "Acesso negado" | Autorização por perfil |
| 28 | `DELETE` como admin | 204 | Autorização por perfil |

---

## 8. Swagger

Abra `http://localhost:5000`:

1. `POST /api/autenticacao/login` → **Try it out** → corpo `{ "email": "admin@pethealth.com", "senha": "Admin@123" }` → **Execute** → copie o `token`.
2. Clique em **Authorize** (cadeado) → cole só o token → **Authorize**.
3. Teste qualquer endpoint: todos mostram descrição, parâmetros de filtro e códigos de resposta.

---

## 9. Logging estruturado

Logs em `src\PetHealthAPI\logs\`:

- `app-AAAAMMDD.log` — texto, com `[CorrelationId]` em cada linha;
- `app-AAAAMMDD.json` — JSON estruturado (um evento por linha).

```powershell
Select-String -Path src\PetHealthAPI\logs\*.json -Pattern "teste-roteiro-404"
```

✅ Esperado: as linhas da requisição 24 (o mesmo ID enviado no header aparece no log e na resposta).

---

## 10. Conferir os dados nos bancos

**MongoDB:**

```powershell
docker exec -it pethealth-mongo mongosh pethealth --eval "db.registros_diarios.find().limit(5)"
```

**Oracle** (SQL Developer):

```sql
SELECT * FROM TB_PH_TUTOR;
SELECT * FROM TB_PH_PET;
SELECT * FROM "__EFMigrationsHistory_PetHealth";
```

✅ Esperado: os registros criados no passo 7 e a migration `Sprint4Inicial`.

---

## ✔️ Checklist final

- [ ] `dotnet build` com 0 erros
- [ ] `dotnet test` com 0 falhas
- [ ] Relatório de cobertura gerado
- [ ] `/health` Healthy (Oracle + MongoDB + memória)
- [ ] 28 requisições do `pethealth.http` com os resultados esperados
- [ ] Swagger com Authorize funcionando
- [ ] Logs com correlation ID em `src\PetHealthAPI\logs\`
- [ ] Dados visíveis no Oracle e no MongoDB
