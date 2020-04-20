# **Aplikace pro správu osobních finančních transakcí** 
> Aplikace je vytvořena technologií WPF (Okenní formulářová aplikace), tedy pomocí grafických oken, kdy každé spustitelné okno má jinou funkci. 
Všechny okna aplikace jsou řízena a spoštěna kontrolní třídou aplikace představující vrstvu Controller v architektuře MVC.
Třídy spravující funkci okenních formulářů představují vrstvu View architektury MVC sloužící k načtení vstupních dat, předání dat kontroléru a zobrazení dat načtených z kontroléru.
Třídy uchovávající logickou vrstvu aplikace včetně všech potřebných nezávislých metod představuje vstrstvu Models.


Aplikace umožňuje správu financí více osob díky využití uživatelských účtů, kdy zpracovává pouze data patřící přihlášenému uživateli. 
Každý uživatel je identifikovám uživatelským jménem a heslem. Díky tomu není možné číst ani upravovat data jiného uživatele bez znalosti těchto údajů. 
***

Program zpracovává a uchovává jednotlivé finanční záznamy představující konkrétní finanční transakce (například nákup v obchodě - účtenka). 
Každý záznam obsahuje základní údaje pro jeho identifikaci, je přidělen do určité kategorie pro možnost třídění, může obsahovat textovou poznámku a seznam položek (například jednotlivé položky na účtence). 
Záznamy je možné přidávat, upravovat a smazat. 
Záznamy lze vhyledat dle zvolených parametrů, exportovat vybrané zánamy do souboru, nebo zobrazit statisticky vyhodnocené záznamy v podobě grafu. 
Graf může vyhodnocovat záznamy dle kategorií, nebo dle zvoleného časového období, kdy vyhodnocuje záznamy v jednotlivých časových intervalech.
***

Hlavní okno aplikace
---
Po přihlášení se zobrazí hlavní okno aplikace, které obsahuje 2 postranní MENU, zobrazuje záznamy aktuálního měsíce včetně stručného finančního vyhodnocení tohoto měsíce. 
V levém MENU jsou funkce pro práci se záznamy (přidání nových záznamů, jejich úprava, vyhledávání a statisticky vyhodnocený přehled v podobě grafů). 
Pravé MENU obsahuje uživatelské rozhraní pro správu aplikace. 
Je zde program pro spuštění jednoduché kalkulačky, možnost importovat uložené záznamy a export vybraných záznamů do souboru (formáty TXT, CSV, XML). 
Dále je zde tlačítko pro otevření okna Nastavení, kde je možné vybrat si barvu pozadí a zobrazit v hlavním okně poznámkovů blok. 
Dále je zde možnost zobrazit informační okno, kde je popis celé aplikace (stručný návok jak aplikaci používat a co všechno umožňuje).
***
    
> Autor projektu: ***bc. David Halas***
