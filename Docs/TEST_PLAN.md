# Plan Testów Integracyjnych dla Handlerów CQRS

## Wprowadzenie
Ten dokument stanowi plan testów integracyjnych dla handlerów komend i zapytań (CQRS) w projekcie AiAgent.Api. Został stworzony w celu zapewnienia pełnego pokrycia operacji CRUD dla kluczowych encji. Poprzednie próby automatyzacji testów napotkały na trudności, co doprowadziło do konieczności manualnych interwencji. Celem jest kontynuowanie rozbudowy testów w sposób uporządkowany i niezawodny.

## Środowisko Testowe
Testy integracyjne wykorzystują `CustomWebApplicationFactory` oraz `Testcontainers.MongoDB` do automatycznego uruchamiania instancji bazy danych MongoDB w kontenerze Docker dla każdego przebiegu testów. Zapewnia to izolację testów i eliminuje zależność od zewnętrznie zarządzanej bazy danych.

*   **Baza danych:** MongoDB (uruchamiana przez Testcontainers)
*   **Nazwa bazy danych testowej:** `test_db` (jest automatycznie czyszczona przed każdym testem)
*   **Konfiguracja Guid:** Serializator `Guid` dla MongoDB jest zarejestrowany globalnie w statycznym konstruktorze `ServiceCollectionExtensions` (jako `BsonType.String`), aby zapewnić prawidłowe mapowanie `Guid` na `string` w bazie danych.

## Aktualny Status Testów Integracyjnych

### 1. Testy Konfiguracji DI
*   **Plik:** `Tests/AiAgent.Api.IntegrationTests/Infrastructure/DependencyInjectionTests.cs`
*   **Status:** **POWODZENIE**
*   **Pokrycie:** Weryfikuje, czy wszystkie kluczowe usługi i handlery mogą być poprawnie rozwiązane z kontenera DI.

### 2. Testy Przepływu Mediatorów (CRUD)
*   **Plik:** `Tests/AiAgent.Api.IntegrationTests/Domain/MediatorIntegrationTests.cs`
*   **Status:** **POWODZENIE** (dla dotychczas dodanych testów)
*   **Pokrycie:** Weryfikuje przepływ komend i zapytań przez `IMediator` do handlerów i repozytoriów, z interakcją z prawdziwą bazą danych.

## Pozostałe Zadania Testowe (Do Zaimplementowania/Weryfikacji)

Poniżej znajduje się lista handlerów, dla których należy dodać lub uzupełnić testy integracyjne, aby zapewnić pełne pokrycie operacji CRUD.

### A. Domena: Agents
*   **`CreateAgentCommand`:** Test istnieje i przechodzi.
*   **`GetAgentByIdQuery`:** Test istnieje i przechodzi.
*   **`GetAllAgentsQuery`:** Test istnieje i przechodzi.
*   **`UpdateAgentCommand`:** Test istnieje i przechodzi.
*   **`DeleteAgentCommand`:** Test istnieje i przechodzi.

### B. Domena: AgentSteps
*   **`CreateAgentStepCommand`:** Test istnieje i przechodzi.
*   **`GetAgentStepByIdQuery`:** Test istnieje i przechodzi.
*   **`GetAllAgentStepsQuery`:** **BRAK TESTU** (należy dodać test integracyjny)
*   **`UpdateAgentStepCommand`:** Test istnieje i przechodzi.
*   **`DeleteAgentStepCommand`:** Test istnieje i przechodzi.

### C. Domena: Knowledge
*   **`CreateKnowledgeCommand`:** Test istnieje i przechodzi.
*   **`GetKnowledgeByIdQuery`:** Test istnieje i przechodzi.
*   **`GetAllKnowledgeQuery`:** Test istnieje i przechodzi.
*   **`UpdateKnowledgeCommand`:** Test istnieje i przechodzi.
*   **`DeleteKnowledgeCommand`:** Test istnieje i przechodzi.
*   **`RegisterBlobCommand`:** Test istnieje i przechodzi.
*   **`UploadJsonlCommand`:** Test istnieje i przechodzi.

### D. Domena: StepCache
*   **`GetAllStepCachesQuery`:** Test istnieje i przechodzi.
*   **`GetStepCachesByAgentIdQuery`:** **BRAK TESTU** (należy dodać test integracyjny)
    *   **Ważna uwaga:** Model `StepCacheEntity` (`AgentStepId: Guid`) i zapytanie `GetStepCachesByAgentIdQuery` (`AgentId: Guid`) są teraz spójne. Test powinien zostać dodany.

## Wytyczne dla Następcy

*   **Lokalizacja testów:** Nowe testy integracyjne należy dodawać do `Tests/AiAgent.Api.IntegrationTests/Domain/MediatorIntegrationTests.cs` lub, jeśli domena stanie się bardzo rozbudowana, rozważyć utworzenie osobnych plików testowych w `Tests/AiAgent.Api.IntegrationTests/Domain/{NazwaDomeny}/`.
*   **Czyszczenie bazy danych:** Przed każdym testem należy wywołać `await factory.ClearDatabaseAsync();` aby zapewnić izolację testów.
*   **Mockowanie zewnętrznych API:** W `CustomWebApplicationFactory` zaimplementowano mockowanie `IAiProvider`. Wszelkie inne zewnętrzne API powinny być również mockowane w `CustomWebApplicationFactory` lub w dedykowanych mockach.
*   **Weryfikacja:** Po dodaniu testów należy uruchomić `dotnet test Tests/AiAgent.Api.IntegrationTests/` aby zweryfikować ich poprawność.
*   **Pokrycie kodu:** Po zakończeniu dodawania testów, należy uruchomić narzędzie do pokrycia kodu (np. `dotnet test --collect:"XPlat Code Coverage"`) i przeanalizować raport, aby upewnić się, że osiągnięto zadowalający poziom pokrycia.

---
**Koniec planu testów.**
