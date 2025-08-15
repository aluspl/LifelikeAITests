# Plan Rozwoju Abstrakcyjnego Systemu Agentów AI

Ten dokument opisuje kroki niezbędne do przebudowy istniejącej aplikacji w modularny system agentów, gdzie ich definicje są przechowywane w bazie danych i mogą być dynamicznie wykonywane.

---

### Krok 1: Definicja Modeli i Struktury Danych w Bazie Danych

Zrealizowany.

---

### Krok 2: Implementacja Rdzenia Logiki - Agent Orchestrator

Zrealizowany.

---

### Krok 3: Refaktoryzacja Istniejącego Agenta `KillTeam`

Zrealizowany.

---

### Krok 4: Udostępnienie Funkcjonalności przez API

Zrealizowany.

---

### Krok 5: Testowanie

Zrealizowany.

---

### Krok 6: Udoskonalenie Konfiguracji Testów

Zrealizowany.

---

### Krok 7: Refaktoryzacja Domeny Knowledge

**Cel:** Zmiana sposobu przechowywania i zarządzania wiedzą, aby wspierać zarówno wiedzę osadzoną (inline, z JSONL), jak i referencje do zewnętrznych obiektów blob (URL).

**Faza 1: Refaktoryzacja Modelu Danych**

1.  **Wprowadzenie `KnowledgeSourceType` Enum**:
    *   Stwórz nowy enum `KnowledgeSourceType` w `AiAgent.Api/Domain/Knowledge/Enums/` z wartościami `Inline` i `BlobUrl`.

2.  **Refaktoryzacja `KnowledgeEntity`**:
    *   Zmień nazwę `KnowledgeEntity.cs` na `KnowledgeBaseEntity.cs` (lub podobną, aby odzwierciedlała, że jest to baza wiedzy, a nie pojedynczy element).
    *   Dodaj pole `KnowledgeSourceType SourceType`.
    *   Jeśli `SourceType` to `Inline`:
        *   Dodaj pole `List<KnowledgeItem> Items`. `KnowledgeItem` będzie nową zagnieżdżoną klasą/rekordem.
    *   Jeśli `SourceType` to `BlobUrl`:
        *   Dodaj pole `string BlobUrl`.
    *   Usuń istniejące pole `Content`.
    *   Zachowaj pola `Key` i `Module`.

3.  **Definicja `KnowledgeItem`**:
    *   Stwórz nową klasę/rekord `KnowledgeItem.cs` w `AiAgent.Api/Domain/Knowledge/Models/` z właściwościami `string Key` i `string Value`.

**Faza 2: Dostosowanie Warstwy Repozytorium i Usług**

4.  **Aktualizacja `IKnowledgeRepository` i `KnowledgeRepository`**:
    *   Zmień `KnowledgeRepository` tak, aby pracował z `KnowledgeBaseEntity`.
    *   Zaktualizuj metody `InsertAsync`, `GetByKeyAndModuleAsync`, `GetAllByModuleAsync`, aby obsługiwały nową strukturę `KnowledgeBaseEntity`.

5.  **Refaktoryzacja `DataKnowledgeService`**:
    *   Zmodyfikuj `UploadKnowledgeDataAsync` (obecnie `UploadKnowledgeDataAsync(Stream fileStream, string moduleString)`) do:
        *   Parsowania JSONL do `List<KnowledgeItem>`.
        *   Tworzenia `KnowledgeBaseEntity` z `SourceType = Inline` i wypełniania `Items`.
        *   Wstawiania tej `KnowledgeBaseEntity` do repozytorium.
    *   Dodaj nową metodę `UploadBlobKnowledgeAsync(string key, string module, string blobUrl)` do tworzenia `KnowledgeBaseEntity` z `SourceType = BlobUrl`.

**Faza 3: CRUD API dla KnowledgeBaseEntity**

6.  **Stworzenie `KnowledgeController.cs`**:
    *   Zaimplementuj pełny CRUD dla `KnowledgeBaseEntity`.
    *   `GET /api/knowledgebases`
    *   `GET /api/knowledgebases/{id}`
    *   `POST /api/knowledgebases` (dla obu typów: Inline i BlobUrl)
    *   `PUT /api/knowledgebases/{id}`
    *   `DELETE /api/knowledgebases/{id}`
    *   Dodaj specyficzne endpointy dla ładowania JSONL i rejestrowania URL blobów:
        *   `POST /api/knowledgebases/upload-jsonl` (zastępuje stary `/knowledge/upload`)
        *   `POST /api/knowledgebases/register-blob`

**Faza 4: Orchestrator i Istniejący Kod**

7.  **Aktualizacja `AgentOrchestratorService`**:
    *   Zmodyfikuj sposób pobierania `knowledgeContent`.
    *   Jeśli `SourceType` to `Inline`, iteruj `knowledgeBaseEntity.Items`, aby uzyskać zawartość.
    *   Jeśli `SourceType` to `BlobUrl`, pobierz zawartość z URL (na razie dodaj komentarz o konieczności implementacji pobierania z URL).

**Faza 5: Testowanie**

8.  **Aktualizacja Testów Jednostkowych**:
    *   Stwórz `KnowledgeControllerTests.cs` dla nowego CRUD.
    *   Zaktualizuj `DataKnowledgeServiceTests.cs` (jeśli istnieje, w przeciwnym razie stwórz) dla nowych metod uploadu.
9.  **Aktualizacja Testów Integracyjnych**:
    *   Zmodyfikuj `CustomWebApplicationFactory` tak, aby poprawnie mockował nowe `IKnowledgeRepository`.
    *   Zaktualizuj wszelkie istniejące testy integracyjne, które opierają się na wiedzy.

---

### Raport Postępu i Kolejne Kroki

**Podsumowanie Zmian:**

1.  **`AgentStepEntity` CRUD i Refaktoryzacja:**
    *   `AgentStep` (stara klasa zagnieżdżona) usunięta.
    *   `AgentStepEntity` stworzona jako encja najwyższego poziomu (`AiAgent.Api/Domain/Database/Entites/AgentStepEntity.cs`).
    *   `IAgentStepRepository` i `AgentStepRepository` stworzone.
    *   `AgentEntity` zaktualizowana do używania `List<string> StepIds`.
    *   `AgentStepsController` stworzony dla operacji CRUD na `AgentStepEntity`.
    *   `AgentOrchestratorService` zaktualizowany do pobierania `AgentStepEntity` po ID.

2.  **Refaktoryzacja Domeny `Knowledge` (w toku):**
    *   `KnowledgeSourceType` enum stworzony.
    *   `KnowledgeItem` klasa stworzona.
    *   `KnowledgeEntity` (dawniej `KnowledgeBaseEntity`) zrefaktoryzowana, aby zawierała `SourceType`, `Items`, `BlobUrl` i używała `Key` jako identyfikatora logicznego.
    *   `IKnowledgeRepository` i `KnowledgeRepository` zaktualizowane do pracy z `KnowledgeEntity`.
    *   `DataKnowledgeService` zaktualizowany do obsługi `KnowledgeEntity` i zawiera `UploadBlobKnowledgeAsync`.
    *   `KnowledgeController` (dawniej `KnowledgeBasesController`) stworzony dla CRUD na `KnowledgeEntity`.
    *   `AgentOrchestratorService` zaktualizowany do przetwarzania `KnowledgeEntity` na podstawie `SourceType` (tymczasowo z pominięciem parsowania JSON).

3.  **Refaktoryzacja Endpointów:**
    *   `InstructionsEndpoints.cs` stworzony, przenosząc endpointy `/instructions` z `Program.cs`.
    *   Stare, bezpośrednie mapowania endpointów `/knowledge` usunięte z `Program.cs`.

4.  **Czyszczenie Projektu i Konfiguracja Testów:**
    *   `IChatService` i powiązane pliki usunięte.
    *   `IKillTeamAnalysisService`, `KillTeamAnalysisService`, `IAiAnalysisService`, `AiAnalysisService` usunięte.
    *   `TestScopedDataSeeder` stworzony dla testów integracyjnych.
    *   `CustomWebApplicationFactory` zaktualizowany do używania `TestScopedDataSeeder` i mockowania wszystkich niezbędnych repozytoriów i usług dla izolowanych testów integracyjnych.
    *   `BaseEntity` nie posiada już właściwości `Key`.
    *   `ApiKey`, `InstructionEntity`, `KnowledgeEntity` posiadają właściwości `Key` do identyfikacji logicznej.
    *   `DataSeederService` zaktualizowany do używania `Key` do identyfikacji logicznej.
    *   Wszystkie testy jednostkowe (`AgentOrchestratorServiceTests`, `AgentStepsControllerTests`, `DataKnowledgeServiceTests`, `KnowledgeControllerTests`) zaktualizowane i przechodzące.
    *   Testy integracyjne (`AgentsControllerIntegrationTests`) zaktualizowane i przechodzące (po tymczasowym pominięciu parsowania JSON w `AgentOrchestratorService`).

**Pozostałe Zadania (do wykonania):**

1.  **Przywrócenie parsowania JSON w `AgentOrchestratorService`:**
    *   Ponowne włączenie logiki parsowania JSON w `AgentOrchestratorService.cs` (usunięcie tymczasowego obejścia).

2.  **`IDataKnowledgeService` i `DataKnowledgeService`:**
    *   Dodanie nowej metody `PopulateKnowledgeFromDictionaryAsync(string key, string module, Dictionary<string, string> data)` do tworzenia `KnowledgeEntity` z `SourceType.Inline` ze słownika.

3.  **Testy dla `Knowledge` (kontynuacja):**
    *   Upewnienie się, że wszystkie testy jednostkowe i integracyjne dla `Knowledge` są kompleksowe i przechodzą po powyższych zmianach.

4.  **Finalna weryfikacja i commit:**
    *   Uruchomienie `dotnet build` i `dotnet test` na całej solucji.
    *   Stworzenie kompleksowego commita do repozytorium.

---