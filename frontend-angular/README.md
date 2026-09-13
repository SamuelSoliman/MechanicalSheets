# MechanicalSheets — Angular frontend (side-by-side col vanilla `frontend/`)

Angular 20 standalone + signals + Angular Material, testi in italiano.

## Concetti (Vue → Angular)

| Vue / vanilla | Angular qui |
|---|---|
| SFC `*.vue` | `login.ts` + `login.html` + `login.scss` |
| `ref()` / `v-model` | `signal()` / ReactiveForms |
| `vue-router beforeEach` | `authGuard` in `src/app/core/auth.guard.ts` |
| `axios interceptor` | `auth.interceptor.ts` + `error.interceptor.ts` |
| `fetch(API + path)` | `HttpClient` + `environment.apiUrl` |

## Comandi

```bash
cd frontend-angular
npm install
npm start          # http://localhost:4200, proxy /api → http://localhost:5000
npm run build      # output in dist/
```

## Docker (step 7)

Un solo comando dalla root, 4 servizi:

```bash
docker-compose up --build -d
```

| Servizio | URL |
|---|---|
| API | http://localhost:5000 |
| Frontend vanilla | http://localhost:8080 |
| Frontend Angular | http://localhost:8081 (`/api/` → `api:8080`, SPA fallback) |

Nessuna modifica CORS necessaria: tutte le chiamate usano URL relativi `/api/...`
(same-origin via proxy/dev-server/nginx), quindi `FrontendPolicy (:8080)` non viene mai valutata.

Login di test (tutti `password123`): `mario.rossi@test.com` (mechanic), `manager@test.com` (manager).

## Stato

- [x] Step 1–3: scaffold CLI + Material, modelli TS, auth core (`AuthService`, interceptor, guard, `Login`)
- [x] Step 4: servizi API (`SheetService`, `DefectCatalog/DefectItem/AttachmentService`)
- [x] Step 5: shell `Nav` + `StatusBadge` + `SheetsList` (tab ruolo, pending badge, card, gating Invia/Approva/Rifiuta)
- [x] Step 6: `SheetDetail` + `SheetCreate` + dialog (`DefectDialog`, `PhotoDialog`, `RejectDialog`)
- [x] Step 7: Docker `:8081` + verifica finale (compose 4 servizi, workflow API OK)
