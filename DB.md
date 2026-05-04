# DB Reference

## Schema

### Users
| Column        | Type         | Notes                        |
|---|---|---|
| Id            | INT PK       |                              |
| Name          | STRING       |                              |
| Email         | STRING       | unique                       |
| Password      | STRING       | bcrypt                       |
| Role          | STRING       | mechanic, manager            |
| CreatedAt     | TIMESTAMP    |                              |
| UpdatedAt     | TIMESTAMP    |                              |

### Sheets 
| Column        | Type         | Notes                                              |
|---|---|---|
| Id            | INT PK       |                                                    |
| Code          | STRING  | codice veicolo                                     |
| Brand         | STRING | nullable                                           |
| Vehicle       | STRING | nullable                                           |
| InspectionDate| DATE         |                                                    |
| SheetStatus   | ENUM         | Draft, Submitted, Approved, Rejected               |
| RejectionNote | STRING         | nullable, popolato su Reject                       |
| SubmittedAt   | TIMESTAMP    | nullable                                           |
| ReviewedAt    | TIMESTAMP    | nullable                                           |
| ReviewedById  | INT FK       | nullable → Users                                   |
| CreatedById   | INT FK       | → Users                                            |
| CreatedAt     | TIMESTAMP    |                                                    |
| UpdatedAt     | TIMESTAMP    |        

### SheetTechnicians (pivot)
| Column   | Type    | Notes                    |
|---|---|---|
| SheetId  | INT FK  | → Sheets                 |
| UserId   | INT FK  | → Users                  |
| PK       | (SheetId, UserId) | chiave composta |

### DefectCatalog
| Column      | Type         | Notes                                      |
|---|---|---|
| Id          | INT PK       |                                            |
| Code        | VARCHAR(20)  | unique                    |
| Category    | ENUM         | Bodywork, Lighting, Fasteners, Frame |
| Description | STRING        |                                            |
| Gravity     | TINYINT      | 0-5         |

### SheetDefectItems
| Column          | Type        | Notes                              |
|---|---|---|
| Id              | INT PK      |                                    |
| SheetId         | INT FK      | → Sheets                           |
| DefectCatalogId | INT FK      | → DefectCatalog                    |
| IsSeen          | BOOL  |                                    |
| ExtentLow       | BOOL  | nullable         |
| ExtentMedium    | BOOL  | nullable                                    |
| ExtentHigh      | BOOL  | nullable                                   |
| IntensityLow    | BOOL  | nullable         |
| IntensityMedium | BOOL  |   nullable                                  |
| IntensityHigh   | BOOL  |  nullable                                   |
| IsPs            | BOOL  | nullable                           |
| IsNa            | BOOL  | nullable                           |
| IsNr            | BOOL  | nullable                           |
| IsNp            | BOOL  | nullable                           |
| HasPhoto        | BOOL  | aggiornato automaticamente         |
| Notes           | STRING        | nullable                           |
| CreatedAt       | TIMESTAMP   |                                    |
| UpdatedAt       | TIMESTAMP   |     

### Attachments
| Column        | Type         | Notes                              |
|---|---|---|
| Id            | INT PK       |                                    |
| SheetDefectId | INT FK       | → SheetDefectItems                 |
| FileName      | STRING       | nome originale                     |
| FilePath      | STRING       | nome univoco su disco (GUID)       |
| MimeType      | STRING       | image/jpeg, image/png, image/webp  |
| FileSize      | BIGINT       | bytes, max 5MB                     |
| UploadedById  | INT FK       | → Users                            |
| UploadedAt    | TIMESTAMP    |    

### ApiKeys
| Column    | Type         | Notes                        |
|---|---|---|
| Id        | INT PK       |                              |
| Name      | VARCHAR(100) | nome sistema terzo           |
| KeyHash   | VARCHAR(255) | SHA256 della key in chiaro   |
| IsActive  | BOOL   |                              |
| CreatedAt | TIMESTAMP    |                              |

## Relazioni
```text
Users
├──< Sheets (CreatedById)
├──< Sheets (ReviewedById)
├──< SheetTechnicians >── Sheets
└──< Attachments

Sheets
└──< SheetDefectItems

DefectCatalog
└──< SheetDefectItems

SheetDefectItems
└──< Attachments

ApiKeys (standalone)
```

## Vincoli impliciti dall'Excel

- `ExtentLow`, `ExtentMedium`, `ExtentHigh` e `IntensityLow`, `IntensityMedium`, `IntensityHigh`  
  sono campi booleani nullable (`true / false / null`) e sono indipendenti tra loro

- `IsNa`, `IsNr`, `IsNp`, `IsPs`  
  sono campi booleani nullable e indipendenti tra loro

- `HasPhoto` viene aggiornato automaticamente quando si aggiungono o eliminano allegati

- Quando una scheda è `Approved`, diventa immutabile  
  (non è possibile modificarla o aggiungere difetti)

- Il manager non può modificare le schede  
  può solo approvarle o rifiutarle

- Solo i tecnici assegnati possono caricare o eliminare foto

