# PROJECT.md — Scelte architetturali

## Stack

- .NET 8 Web API
- Entity Framework Core 8 + Pomelo (MySQL 8)
- JWT Authentication 
- BCrypt.Net per hashing password
- Serilog per logging strutturato 
- Docker + docker-compose
- Frontend: HTML + Vanilla JS 

## Struttura repository

```text
MechanicalSheets/
MechanicalSheets.Api/
Controllers/
AuthController.cs         — login, register
SheetsController.cs       — CRUD schede + workflow
DefectItemsController.cs  — aggiunta/rimozione difetti
DefectCatalogController.cs — lista catalogo difetti
AttachmentsController.cs  — upload/download/delete foto
FilesController.cs        — serve file fisici con auth JWT
IntegrationController.cs  — endpoint API Key per sistemi terzi
Services/
AuthService.cs            — login, register, generazione JWT
SheetService.cs           — logica business + workflow stati
Models/                     — entità EF Core
DTOs/                       — oggetti input/output API
Data/
AppDbContext.cs           — DbContext + mapping relazioni
DbSeeder.cs               — seed automatico dati iniziali
Middleware/
ExceptionHandlingMiddleware.cs — gestione errori globale
ApiKeyMiddleware.cs            — autenticazione API Key
Enums/
SheetStatusEnum.cs        — Draft, Submitted, Approved, Rejected
DefectCategoryEnum.cs     — Bodywork, Lighting, Fasteners, Frame
Migrations/                 — generate da EF Core
frontend/
index.html                  — vanilla JS
scripts/
test_workflow.sh            — script test processo completo
docker-compose.yml
DB.md
PROJECT.md
README.md
```

## Decisioni architetturali

### 1. DefectCatalog separato dalle righe compilate
Il catalogo difetti è una lista predefinita — non è inventata
dal meccanico a runtime. Separare `DefectCatalog` da `SheetDefectItems` evita
duplicazione dei dati e permette di aggiornare descrizioni e categorie senza
toccare le schede esistenti. La FK è intera (`DefectCatalogId`) per semplicità
e coerenza con tutte le altre relazioni del progetto.

### 2. SheetStatus come enum tipizzato
Usare `SheetStatusEnum` invece di una stringa libera garantisce che solo stati
validi possano essere scritti nel DB a livello compilatore, senza bisogno di
validazioni aggiuntive. Il trade-off è che aggiungere un nuovo stato richiede
una migration — accettabile per questo dominio stabile.

### 3. DefectCategory come enum tipizzato
Stessa motivazione di SheetStatus. Le categorie sono fisse e
non devono essere modificabili dall'utente.

### 4. JWT stateless
JWT non richiede lookup sul DB ad ogni richiesta. Il token contiene id, ruolo
e scadenza. Trade-off: impossibile invalidare un token prima della scadenza
senza una blacklist. Accettabile per questo contesto — la scadenza è 8 ore.

### 5. Workflow tramite eccezioni tipizzate
Il service layer lancia eccezioni .NET standard (`KeyNotFoundException`,
`UnauthorizedAccessException`, `InvalidOperationException`).
`ExceptionHandlingMiddleware` le intercetta e le mappa ai codici HTTP corretti
(404, 403, 400). I controller rimangono puliti e il service layer è ignaro di HTTP.

### 6. API Key per integrazione terzi
Gli endpoint `/api/integration/*` usano header `X-Api-Key` separato dal JWT.
La key è salvata come hash SHA256 — mai in chiaro nel DB.
L'endpoint `PUT /api/integration/sheets/{id}/status` permette ai sistemi terzi
di cambiare lo stato della scheda con validazione delle transizioni di stato.


### 7. Migration + seed automatici all'avvio

Il database viene migrato automaticamente all’avvio se necessario.

Se le tabelle sono vuote, viene eseguito anche il seed con dati di test.

Tutto avviene automaticamente quando si esegue `docker-compose up` o `dotnet run`, senza bisogno di comandi manuali.

### 9. Frontend vanilla JS

Il frontend è stato scelto in vanilla JS perché il frontend è pensato principalmente per testare il workflow.

Si è data priorità alla semplicità di avvio e alla connessione rapida con il backend, evitando build step o tool aggiuntivi.


## Trade-off e cosa è stato lasciato fuori

## Lasciato fuori

Per ragioni di semplicità e in base alle esigenze attuali del progetto:

- **Upload su S3/object storage** — i file sono salvati in locale su volume Docker

- **Paginazione** — le liste non sono paginate

- **UserRoleEnum** — è stato evitato l’enum e si è preferito usare semplici stringhe, dato che i ruoli sono solo due. Questo è stato fatto anche per valutare meglio, in un progetto piccolo, quando conviene usare enum o stringhe man mano che il sistema cresce

### Trade-off 
- **Migration automatica all'avvio**: comoda per Docker, in produzione si
  preferirebbe un job di migrazione separato con rollback controllato
- **Seed in ogni ambiente**: il seeder è idempotente (`Any()`) ma in produzione
  reale si userebbe un flag esterno o un ambiente separato per i dati di test
- **Secrets in appsettings.json**: la JWT key e la connection string sono
  committed nel repository in chiaro. In produzione si userebbe un secrets
  manager (Azure Key Vault, AWS Secrets Manager, o semplicemente variabili
  d'ambiente iniettate dal CI/CD) per non esporre credenziali nel codice.
  Per questo progetto di valutazione è una scelta consapevole per semplicità
  di setup — `docker-compose up` funziona senza configurazione aggiuntiva.






