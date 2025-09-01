### Instrukcja: Kolekcjonowanie Danych Drużyny (Team Collection)

**Cel:** Ta instrukcja definiuje proces "kolekcjonowania" danych. Opisuje, jak agent ma przyjąć nazwę drużyny, zbudować jej pełny profil i zapisać go w pamięci stanu sesji, gotowego do późniejszego wykorzystania.

**Architektura:** Agent (C#) z Pamięcią Stanu <-> **Lokalna Baza Wiedzy (`.jsonl` w pamięci)**

**Logika Agenta:**

1.  **Wejście:** Nazwa drużyny, np. `Mandrakes`.

2.  **Sprawdzenie Stanu Sesji:** Agent sprawdza, które miejsce w pamięci jest wolne.
    *   **IF** `team1Profile` jest `null`, agent będzie zapisywał dane jako "Drużyna 1".
    *   **ELSE IF** `team2Profile` jest `null`, agent będzie zapisywał dane jako "Drużyna 2".
    *   **ELSE** oba miejsca są zajęte. Agent odpowiada: `Pamięć jest pełna. Wpisz "podsumowanie", aby porównać zebrane drużyny, lub "reset", aby zacząć od nowa.` i przerywa proces.

3.  **Wykonanie Procesu Zbierania Danych:**
    *   Agent wykonuje **wszystkie kroki** z instrukcji `single_team_profile_guide.md` dla podanej na wejściu nazwy drużyny.
    *   Wynikiem tej operacji jest kompletny, zweryfikowany **Obiekt Profilu Drużyny**.

4.  **Zapis do Pamięci Stanu Sesji:**
    *   Agent zapisuje stworzony obiekt w odpowiednim, wolnym miejscu w pamięci (`team1Profile` lub `team2Profile`).

5.  **Potwierdzenie dla Użytkownika:**
    *   Agent zwraca potwierdzenie, informując, która drużyna została zapisana i na której pozycji.
        ```markdown
        **Analiza i Zapis Drużyny 1: Mandrakes Zakończone.**

        Dane gotowe do dalszej analizy. Podaj nazwę drugiej drużyny lub wpisz "podsumowanie", jeśli druga drużyna została już wybrana.
        ```
