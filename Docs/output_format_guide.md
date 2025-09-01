### Instrukcja: Standardowy Format Odpowiedzi (Markdown)

**Cel:** Ta instrukcja definiuje standardowy, ustrukturyzowany format wyjściowy w Markdown dla odpowiedzi generowanych przez agenta. Agent musi ściśle przestrzegać tej struktury podczas składania finalnego raportu.

---

### **Schemat Odpowiedzi w Formacie Markdown**

```markdown
# Analiza Taktyczna Kill Team

## Drużyna 1: {Team1.Name}

### Zasady Frakcyjne
*   **{FactionRule1.Name}:** {FactionRule1.Description}
*   **{FactionRule2.Name}:** {FactionRule2.Description}
*   ...

### Operatywy
---
#### **{Operative1.Name}**
*   **Statystyki:** M: {Op1.Stats.M}, APL: {Op1.Stats.APL}, GA: {Op1.Stats.GA}, DF: {Op1.Stats.DF}, SV: {Op1.Stats.SV}, W: {Op1.Stats.W}
*   **Zdolności:**
    *   **{Op1.Ability1.Name}:** {Op1.Ability1.Description}
    *   **{Op1.Ability2.Name} ({Op1.Ability2.Cost} AP):** {Op1.Ability2.Description}
    *   ...
*   **Uzbrojenie:**
    *   **{Op1.Weapon1.Name}:** A: {W1.A}, BS/WS: {W1.BS}, D: {W1.D}
        *   **Efekty:**
            *   **{W1.Rule1.Name}:** {W1.Rule1.Description}
            *   ...
    *   ...
---
#### **{Operative2.Name}**
*   ... (ta sama struktura co powyżej)
---

### Ploye
*   **Strategiczne:**
    *   **{StratPloy1.Name}:** {StratPloy1.Description}
    *   ...
*   **Taktyczne:**
    *   **{TacPloy1.Name}:** {TacPloy1.Description}
    *   ...

---
---

## Drużyna 2: {Team2.Name}

*(... ta sama struktura co dla Drużyny 1 ...)*

---
---

## Analiza Taktyczna i Kluczowe Interakcje

*   **[Przewaga/Kontra 1]:** {TacticalAnalysis.Interaction1.Description}
*   **[Przewaga/Kontra 2]:** {TacticalAnalysis.Interaction2.Description}
*   ...
```

### **Wyjaśnienie Komponentów Schematu:**

*   **Nagłówki (`#`, `##`, `###`):** Służą do logicznego podziału na sekcje: cała analiza, poszczególne drużyny, ich zasady, operatywy i ploye.
*   **Separatory (`---`):** Używane do wizualnego oddzielenia poszczególnych operatywów oraz głównych sekcji, co znacznie poprawia czytelność.
*   **Listy Wypunktowane (`*`, `    *`):** Używane do listowania zasad, zdolności, broni i ich efektów. Zapewniają spójność i przejrzystość.
*   **Pogrubienie (`**`):** Używane do wyróżnienia kluczowych terminów, takich jak nazwy zasad, zdolności czy broni.
*   **Zmienne w nawiasach klamrowych (`{...}`):** Reprezentują miejsca, w które agent ma wstawić dane ze swojego **Wewnętrznego Obiektu Roboczego**. Na przykład `{Team1.Name}` oznacza, że w tym miejscu ma się znaleźć wartość pola `Name` z obiektu `team1Profile`.

**Instrukcja dla Agenta:** Podczas generowania odpowiedzi w formacie Markdown, agent musi wziąć ten schemat jako szablon i wypełnić go danymi zebranymi i zweryfikowanymi w poprzednich krokach.
