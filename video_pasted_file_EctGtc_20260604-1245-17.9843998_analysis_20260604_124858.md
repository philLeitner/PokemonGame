Hier ist eine detaillierte Analyse der im Video gezeigten Verhaltensweisen bezüglich der Sperren:

**Welche Sperren werden gezeigt?**
Im Video sind zwei Arten von Sperren zu sehen:
1.  **Sperrbedingungen für Orte:** Unter "SPERRBEDINGUNGEN" wählt der Spieler für Route 2 die Option **"Unterirdisch (immer zugänglich)"** aus.
2.  **Sperren für Verbindungen:** Unter "VERBINDUNGEN" nutzt der Spieler das Dropdown-Menü, um die Art der Wegsperre zu definieren. Hier wählt er die Option **"abOrden"** (benötigt eine bestimmte Anzahl an Orden, um die Verbindung zu nutzen).

**Was macht der Spieler?**
1.  Der Spieler befindet sich im Map-Editor, wählt "Route 2" aus und aktiviert die Checkbox für die Sperrbedingung "Unterirdisch (immer zugänglich)".
2.  Anschließend wechselt er in die Ansicht "Weltkarte" und navigiert durch verschiedene Orte (klickt sich durch die Map).
3.  Er kehrt in den Map-Editor zurück und wählt den Ort "Orania" aus. Im rechten Menü unter "VERBINDUNGEN" ändert er die Sperre für den Weg zu "Route 11". Er öffnet das Dropdown-Menü, das auf "Normal" steht, und wählt "abOrden" aus.
4.  Danach wählt er den Ort "Viridian-Wald.2" aus und wiederholt den Vorgang: Er ändert die Verbindung zu "Route 2" über das Dropdown-Menü von "Normal" auf "abOrden".

**Was sollte eigentlich passieren vs. was passiert wirklich? (Fehlerbeschreibung)**

Hier zeigt sich ein deutlicher **UI-Bug (Benutzeroberflächen-Fehler)** im Bereich der Verbindungen:

*   **Was eigentlich passieren sollte (Soll-Zustand):**
    Wenn der Spieler im Dropdown-Menü eine Option wie "abOrden" auswählt, sollte das Dropdown-Element als solches bestehen bleiben und lediglich den neu gewählten Wert ("abOrden") anzeigen. Das zusätzliche Eingabefeld für die benötigte Anzahl der Orden sollte daneben oder darunter erscheinen. Der Spieler sollte jederzeit wieder auf das Dropdown-Menü klicken können, um seine Auswahl zu korrigieren (z. B. zurück auf "Normal" oder auf "Surfer" ändern).
*   **Was wirklich passiert (Ist-Zustand):**
    Sobald der Spieler im Dropdown-Menü "abOrden" anklickt, **verschwindet das gesamte Dropdown-Menü**. Es wird stattdessen durch einfachen Text ("abOrden") und das dazugehörige Zahleneingabefeld ersetzt.
*   **Das fehlerhafte Verhalten:**
    Es gibt zwar keine aufpoppende Fehlermeldung, aber das Verhalten der Benutzeroberfläche ist fehlerhaft. Da das Dropdown-Menü nach der Auswahl verschwindet, **nimmt das System dem Spieler die Möglichkeit, die Sperr-Art im Nachhinein wieder zu ändern**. Wenn der Spieler sich verklickt hat oder die Sperre später anpassen möchte, kann er nicht einfach eine andere Option auswählen. Er ist gezwungen, die komplette Verbindung über das Mülleimer-Symbol zu löschen und völlig neu anzulegen.