#!/bin/bash
# Test completo workflow: compilazione → submit → reject → fix → resubmit → approve → close

BASE="http://localhost:5000/api"
PASS=0
FAIL=0

# Colori per output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

check() {
    local description=$1
    local expected=$2
    local actual=$3

    if echo "$actual" | grep -qE "$expected"; then
        echo -e "${GREEN}✓ $description${NC}"
        PASS=$((PASS + 1))
    else
        echo -e "${RED}✗ $description${NC}"
        echo -e "  Expected: $expected"
        echo -e "  Got: $actual"
        FAIL=$((FAIL + 1))
    fi
}

echo -e "${YELLOW}=== TEST WORKFLOW COMPLETO ===${NC}"
echo ""

# ============================================================
echo -e "${YELLOW}--- 1. Login mechanic (Mario Rossi) ---${NC}"
# ============================================================
MECHANIC_RESPONSE=$(curl -s -X POST "$BASE/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"mario.rossi@test.com","password":"password123"}')

MECHANIC_TOKEN=$(echo $MECHANIC_RESPONSE | jq -r '.token')
check "Login mechanic riuscito" "eyJ" "$MECHANIC_TOKEN"

# ============================================================
echo -e "${YELLOW}--- 2. Login manager ---${NC}"
# ============================================================
MANAGER_RESPONSE=$(curl -s -X POST "$BASE/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"manager@test.com","password":"password123"}')

MANAGER_TOKEN=$(echo $MANAGER_RESPONSE | jq -r '.token')
check "Login manager riuscito" "eyJ" "$MANAGER_TOKEN"

# ============================================================
echo -e "${YELLOW}--- 3. Compilazione scheda (draft) ---${NC}"
# ============================================================
SHEET_RESPONSE=$(curl -s -X POST "$BASE/sheets" \
  -H "Authorization: Bearer $MECHANIC_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "code": "TEST-WORKFLOW-11n91",
    "brand": "Test Brand",
    "vehicle": "Test Vehicle",
    "inspectionDate": "2026-05-01",
    "technicianIds": []
  }')

SHEET_ID=$(echo $SHEET_RESPONSE | jq -r '.id')
check "Scheda creata in stato draft" "Draft" "$SHEET_RESPONSE"
echo "  Sheet ID: $SHEET_ID"

# ============================================================
echo -e "${YELLOW}--- 4. Submit senza difetti (deve fallire) ---${NC}"
# ============================================================
SUBMIT_FAIL=$(curl -s -X POST "$BASE/sheets/$SHEET_ID/submit" \
  -H "Authorization: Bearer $MECHANIC_TOKEN")

check "Submit senza difetti rifiutato" "almeno un difetto" "$SUBMIT_FAIL"

# ============================================================
echo -e "${YELLOW}--- 5. Aggiungi difetto alla scheda ---${NC}"
# ============================================================
# Recupera il primo difetto dal catalogo
FIRST_DEFECT_ID=$(curl -s "$BASE/defect-catalog" \
  -H "Authorization: Bearer $MECHANIC_TOKEN" | jq -r '.[0].id')

DEFECT_RESPONSE=$(curl -s -X POST "$BASE/sheets/$SHEET_ID/defects" \
  -H "Authorization: Bearer $MECHANIC_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"defectCatalogId\": $FIRST_DEFECT_ID,
    \"isSeen\": true,
    \"extentLow\": true,
    \"extentMedium\": false,
    \"extentHigh\": false,
    \"intensityLow\": true,
    \"intensityMedium\": false,
    \"intensityHigh\": false,
    \"isPs\": false,
    \"isNa\": false,
    \"isNr\": false,
    \"isNp\": false,
    \"notes\": \"Difetto aggiunto dal test workflow\"
  }")

check "Difetto aggiunto" "defectCatalogId" "$DEFECT_RESPONSE"

# ============================================================
echo -e "${YELLOW}--- 6. Submit scheda ---${NC}"
# ============================================================
SUBMIT_RESPONSE=$(curl -s -X POST "$BASE/sheets/$SHEET_ID/submit" \
  -H "Authorization: Bearer $MECHANIC_TOKEN")

check "Scheda submitted" "submitted" "$SUBMIT_RESPONSE"


# ============================================================
echo -e "${YELLOW}--- 7. Mechanic non può approvare (deve fallire) ---${NC}"
# ============================================================
APPROVE_FAIL=$(curl -s -o /dev/null -w "%{http_code}" \
  -X POST "$BASE/sheets/$SHEET_ID/approve" \
  -H "Authorization: Bearer $MECHANIC_TOKEN")

check "Mechanic non può approvare (403)" "403" "$APPROVE_FAIL"

# ============================================================
echo -e "${YELLOW}--- 8. Manager reject con nota ---${NC}"
# ============================================================
REJECT_RESPONSE=$(curl -s -X POST "$BASE/sheets/$SHEET_ID/reject" \
  -H "Authorization: Bearer $MANAGER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"rejectionNote": "Mancano foto per i difetti rilevati"}')

check "Scheda rejected" "Rejected" "$REJECT_RESPONSE"
check "Nota di rifiuto presente" "Mancano foto" "$REJECT_RESPONSE"

# ============================================================
echo -e "${YELLOW}--- 9. Fix scheda (mechanic modifica dopo reject) ---${NC}"
# ============================================================
FIX_RESPONSE=$(curl -s -X PUT "$BASE/sheets/$SHEET_ID" \
  -H "Authorization: Bearer $MECHANIC_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "brand": "Test Brand Fix"
  }')

check "Fix scheda applicato" "Test Brand Fix" "$FIX_RESPONSE"

SECOND_DEFECT_ID=$(curl -s "$BASE/defect-catalog" \
  -H "Authorization: Bearer $MECHANIC_TOKEN" | jq -r '.[1].id')

SECOND_DEFECT_RESPONSE=$(curl -s -X POST "$BASE/sheets/$SHEET_ID/defects" \
  -H "Authorization: Bearer $MECHANIC_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"defectCatalogId\": $SECOND_DEFECT_ID,
    \"isSeen\": true,
    \"extentMedium\": true,
    \"intensityMedium\": true,
    \"isPs\": false,
    \"isNa\": false,
    \"isNr\": false,
    \"isNp\": false,
    \"notes\": \"Difetto aggiunto dopo fix\"
  }")

check "Secondo difetto aggiunto" "defectCatalogId" "$SECOND_DEFECT_RESPONSE"

# ============================================================
echo -e "${YELLOW}--- 10. Resubmit dopo fix ---${NC}"
# ============================================================
RESUBMIT_RESPONSE=$(curl -s -X POST "$BASE/sheets/$SHEET_ID/submit" \
  -H "Authorization: Bearer $MECHANIC_TOKEN")

check "Scheda resubmitted" "Submitted" "$RESUBMIT_RESPONSE"

# ============================================================
echo -e "${YELLOW}--- 11. Manager approva ---${NC}"
# ============================================================
APPROVE_RESPONSE=$(curl -s -X POST "$BASE/sheets/$SHEET_ID/approve" \
  -H "Authorization: Bearer $MANAGER_TOKEN")

check "Scheda approved" "Approved" "$APPROVE_RESPONSE"

# ============================================================
echo -e "${YELLOW}--- 12. Mechanic non può modificare scheda approved ---${NC}"
# ============================================================
EDIT_APPROVED=$(curl -s -X PUT "$BASE/sheets/$SHEET_ID" \
  -H "Authorization: Bearer $MECHANIC_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"brand": "Tentativo modifica"}')

check "Modifica scheda approved bloccata" "Impossibile" "$EDIT_APPROVED"

# ============================================================
echo -e "${YELLOW}--- 13. reject via Integration API ---${NC}"
# ============================================================
REJECTAPI_RESPONSE=$(curl -s -X PUT "$BASE/integration/sheets/$SHEET_ID/status" \
  -H "X-Api-Key: test-api-key-12345" \
  -H "Content-Type: application/json" \
  -d '{"NewStatus": "Rejected", "RejectionNote": "Rifiutata da sistema ERP"}')

check "Scheda rejected via Integration API" "successo" "$REJECTAPI_RESPONSE"

# ============================================================
echo -e "${YELLOW}--- 14. Integration API senza key (deve fallire) ---${NC}"
# ============================================================
NO_KEY=$(curl -s "$BASE/integration/sheets")
check "Integration API senza key rifiutata" "mancante" "$NO_KEY"

# ============================================================
echo ""
echo -e "${YELLOW}=== RISULTATI ===${NC}"
echo -e "${GREEN}Passati: $PASS${NC}"
echo -e "${RED}Falliti: $FAIL${NC}"

if [ $FAIL -eq 0 ]; then
    echo -e "${GREEN}✓ Tutti i test superati!${NC}"
else
    echo -e "${RED}✗ $FAIL test falliti${NC}"
    exit 1
fi