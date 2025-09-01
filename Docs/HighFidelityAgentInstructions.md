### Logika Agenta Wysokiej Wierności (Wersja z Analizą Taktyczną)

Ta instrukcja opisuje proces, który zapewnia maksymalną dokładność i dodaje warstwę analizy taktycznej, aby agent mógł pełnić rolę doradcy.

---

#### **Żelazne Zasady (Konstytucja Agenta)**

1.  **Zasada Jednego Źródła Prawdy:** Jedynym dozwolonym źródłem informacji jest plik `.jsonl`.
2.  **Zasada Kompletności Wyszukiwania:** Zadaniem agenta jest znalezienie **wszystkich** relewantnych informacji.
3.  **Zasada Zero Halucynacji:** Jeśli informacji nie ma w `.jsonl`, dla agenta ona nie istnieje. Odpowiedzią na brak danych jest błąd.

---

### Krok 1: Inicjalizacja i Przygotowanie Środowiska

1.  **Wczytanie Bazy Wiedzy:** Wczytaj cały plik `.jsonl` do struktury `Dictionary<string, string>` w pamięci.
2.  **Parsowanie Zapytania:** Rozbij zapytanie na `team1`, `team2`, `format`, `lang`.
3.  **Wstępna Walidacja Drużyn:** Sprawdź, czy klucze `Kill Team Composition: {team1}` i `Kill Team Composition: {team2}` istnieją w słowniku. Jeśli nie, przerwij i zwróć błąd.

---

### Krok 2: Faza Wyszukiwania (Retrieval) i Budowy Wewnętrznego Obiektu

Utwórz w pamięci **Wewnętrzny Obiekt Roboczy** (instancję klasy C#), który odzwierciedla finalny schemat JSON. Wypełniaj go danymi, wykonując zapytania do słownika w pamięci dla obu drużyn (pobierz listy operatywów/ployów/sprzętu, a następnie ich szczegóły).

---

### Krok 3: Faza Weryfikacji i Uziemienia (Grounding & Validation)

Uruchom wewnętrzny audyt na **Wewnętrznym Obiekcie Roboczym**, aby upewnić się, że wszystkie pobrane listy zgadzają się z pobranymi szczegółami i żadne kluczowe dane nie są puste. W przypadku niespójności, przerwij i zwróć szczegółowy raport o błędach.

---

### Krok 4: Faza Analizy Taktycznej i Syntezy (Nowy Krok)

Po pomyślnej walidacji, uruchom tę procedurę, aby znaleźć kluczowe interakcje między drużynami.

1.  **Utwórz w Obiekcie Roboczym nową sekcję `TacticalAnalysis`**.
2.  **Uruchom serię testów porównawczych i dodaj wyniki do `TacticalAnalysis`:**
    *   **Test na Przebicie Pancerza:** IF Drużyna B ma wysoki `Save`, THEN znajdź w Drużynie A broń/ploye z `Piercing (Prc)` lub `AP` i odnotuj to jako skuteczną kontrę.
    *   **Test na Ukrycie:** IF Drużyna B polega na `Conceal` (np. zasada `Shadow-dweller`), THEN znajdź w Drużynie A sprzęt/umiejętności ignorujące ukrycie (np. `Auspex`) i odnotuj to jako bezpośrednią kontrę.
    *   **Test na Wytrzymałość:** IF Drużyna B ma dużo `Wounds`, THEN znajdź w Drużynie A broń z wysokim `Damage` lub `Mortal Wounds (MW)` i wskaż je jako priorytet.
    *   **Test na Walkę Wręcz:** IF Drużyna B jest silna w zwarciu, THEN znajdź w Drużynie A mechaniki ułatwiające odwrót (`Fall Back`), ogłuszenie (`Stun`) lub strzelanie w walce i wskaż je jako kluczowe do przetrwania.
    *   **Test na Specjalne Obrony:** IF Drużyna B ma zasady typu `Feel No Pain`, THEN znajdź w Drużynie A broń z dużą liczbą ataków (`A`) lub zasadą `Relentless` i wskaż je jako sposób na przełamanie obrony.

---

### Krok 5: Tłumaczenie Danych (dawny Krok 4)

Jeśli język docelowy jest inny niż `en`, przetłumacz odpowiednie pola w **zweryfikowanym** Wewnętrznym Obiekcie Roboczym.

---

### Krok 6: Generowanie Wyniku Końcowego (dawny Krok 5, z rozszerzeniem)

1.  **Osadzanie Opisów Zasad (Rule Embedding):** Podczas generowania wyniku, gdy napotkasz broń z listą zasad specjalnych (SR), dla **każdej** z nich dołącz jej pełny opis (pobrany wcześniej z `Weapon Rule: {SR}`).
2.  **Dodanie Sekcji Taktycznej:** W finalnym pliku JSON lub Markdown, dodaj nową sekcję `TacticalAnalysis`, w której umieścisz wszystkie wpisy wygenerowane w Kroku 4.
3.  **Serializacja / Formatowanie:**
    *   **JSON:** Serializuj Wewnętrzny Obiekt Roboczy (teraz zawierający sekcję `TacticalAnalysis`) do stringa.
    *   **Markdown:** Użyj obiektu do wypełnienia szablonu, dodając na końcu nową sekcję `## Tactical Analysis`.
