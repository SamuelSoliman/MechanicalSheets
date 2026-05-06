# Mechanical Sheets API

Web API .NET 8 per la gestione di schede di intervento meccanico.

## Setup rapido



### Avvio
```bash
git clone <repo-url>
cd MechanicalSheets
docker-compose up --build -d
```

| Servizio | URL |
|---|---|
| API | http://localhost:5000 |
| Swagger UI | http://localhost:<port>/swagger |
| Frontend | http://localhost:8080 |

Per Swagger funziona solo con `dotnet run` (Development Environment)


Al primo avvio il DB viene migrato e popolato automaticamente con dati di test.

## Utenti attivi

| Nome | Email | Password | Ruolo |
|---|---|---|---|
| Mario Rossi | mario.rossi@test.com | password123 | mechanic |
| John Doe | john.doe@test.com | password123 | mechanic |
| Luca Bianchi | luca.bianchi@test.com | password123 | mechanic |
| Anna Verdi | anna.verdi@test.com | password123 | mechanic |
| Manager One | manager@test.com | password123 | manager |
| Manager Two | manager2@test.com | password123 | manager |

## API Key integrazione terzi

X-Api-Key: test-api-key-12345

## Script test workflow completo

```bash
chmod +x Scripts/test_workflow.sh
./Scripts/test_workflow.sh
```

Il script testa l'intero processo:
`compilazione → submit → reject → fix → resubmit → approve` e `closed` via integrazione api

## Endpoints principali

### Auth
| Method | Endpoint | Descrizione |
|---|---|---|
| POST | /api/auth/login | Login, ritorna JWT |
| POST | /api/auth/register | Registra nuovo utente |


### Schede
| Method | Endpoint | Ruolo | Descrizione |
|---|---|---|---|
| GET | /api/sheets | mechanic/manager | Lista schede |
| GET | /api/sheets/{id} | mechanic/manager | Dettaglio scheda |
| POST | /api/sheets | mechanic | Crea scheda |
| PUT | /api/sheets/{id} | mechanic | Modifica scheda (solo Draft/Rejected) |
| POST | /api/sheets/{id}/submit | mechanic | Invia per revisione |
| POST | /api/sheets/{id}/approve | manager | Approva |
| POST | /api/sheets/{id}/reject | manager | Rifiuta  |

### Difetti e Catalogo Diffetti
| Method | Endpoint | Ruolo | Descrizione |
|---|---|---|---|
| GET | /api/defect-catalog | mechanic/manager | Lista catalogo difetti |
| POST | /api/sheets/{id}/defects | mechanic | Aggiungi difetto |
| DELETE | /api/sheets/{id}/defects/{itemId} | mechanic | Rimuovi difetto |

### Allegati e Files
| Method | Endpoint | Ruolo | Descrizione |
|---|---|---|---|
| GET | /api/sheets/{id}/defects/{itemId}/attachments | mechanic/manager | Lista foto |
| POST | /api/sheets/{id}/defects/{itemId}/attachments | mechanic | Upload foto |
| DELETE | /api/sheets/{id}/defects/{itemId}/attachments/{attachmentId} | mechanic | Elimina foto |
| GET | /api/files/{fileName} | mechanic/manager | Scarica file con JWT |
### Integrazione terzi (X-Api-Key)
| Method | Endpoint | Descrizione |
|---|---|---|
| GET | /api/integration/sheets | Lista schede |
| GET | /api/integration/sheets/{id} | Dettaglio scheda |
| PUT | /api/integration/sheets/{id}/status | Cambia stato scheda  |



