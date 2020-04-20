using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Aplikace pro správu financí určena pouze pro osobní užití.
/// ----------------------------------------------------------
/// Aplikace pracuje vždy pouze s daty patřící přihlášenému uživateli, 
/// avšak do souboru ukládá a při spuštění načítá veškerá data pro zpracování, tedy data všech registrovaných uživatelů.
/// Veškerá data aplikace (záznamy všech uživatelů) jsou uložena v paměti a ke konkrétním datům (záznamům) se lze dostat pouze přes přihlášení konkrétního uživatele (jméno a heslo).
/// 
/// Aplikace implementuje zjednodušenou strukturu MVC architektury, kdy je aplikace rozdělena do 3 sekcí. 
/// Třídy View jsou rozděleny na pohledy psané v XAML kódu a slouží pro zobrazení dat v okenním formuláři a třídy obsluhující dané pohledy, které slouží k nastavení okenních formulářů a načtení dat určených k zobrazení.
/// Třídy Models jsou funkční třídy které uchovávají různé funkce a metody, které jsou využity pro zpracování dat, provedení různých úkonů pro zajištění správného chodu aplikace a předání dat určených k zobrazení uživateli.
/// Třídy Controllers slouží k propojení pohledů a funkčních tříd. Zprostředkovává komunikaci, předávání dat a požadavků mezi jednotlivými třídami a uchovává metody pro zobrazování oken aplikace.
/// 
/// V hlavním okně aplikace je zobrazen stručný přehled a je zde uživatelské rozhraní pro správu aplikace i pro možnost využití dalších funkcí aplikace pracujících v samostatných oknech.
/// V úvodu je otevřeno okno pro přihlášení uživatele a po úspěšném přihlášení je zobrazeno hlavní okno aplikace, které je stále otevřeno při chodu aplikace. Po zavření hlavního okna je aplikace ukončena.
/// 
/// 
/// Autor projektu: Bc. David Halas
/// Link publikovaného projektu: https://github.com/dejv147/Finance-Manager-v2.0
/// </summary>
namespace SpravceFinanci_v2
{
   /// <summary>
   /// Třída reprezentující jednoho uživatele. 
   /// Uchovává veškeré informace o uživateli včetně kolekce záznamů patřící danému uživateli.
   /// </summary>
   public class Uzivatel
   {
      /// <summary>
      /// Jméno uživatele
      /// </summary>
      public string Jmeno { get; set; }

      /// <summary>
      /// Heslo uživatele
      /// </summary>
      public string Heslo { get; set; }

      /// <summary>
      /// Seznam záznamů uživatele
      /// </summary>
      public ObservableCollection<Zaznam> SeznamZaznamuUzivatele { get; set; }

      /// <summary>
      /// Text v poznámkovém bloku daného uživatele
      /// </summary>
      public string Poznamka { get; set; }

      /// <summary>
      /// Příznakový bit uchovávající informaci zda má být poznámkový blok pro uživatele zobrazen. 
      /// 0 - poznámky jsou uschovány, 1 - poznámky jsou zobrazeny
      /// </summary>
      public byte PoznamkaZobrazena { get; set; }


      /// <summary>
      /// Konstruktor třídy pro vytvoření nového uživatele
      /// </summary>
      public Uzivatel(string Jmeno, string Heslo)
      {
         this.Jmeno = Jmeno;
         this.Heslo = Heslo;
         SeznamZaznamuUzivatele = new ObservableCollection<Zaznam>();
         Poznamka = "";
         PoznamkaZobrazena = 0;
      }

      /// <summary>
      /// Bezparametrický konstruktor třídy pro možnost ukládání dat do souboru
      /// </summary>
      public Uzivatel() { }

      /// <summary>
      /// Metoda pro vrácení jména a hesla v textové podobě při výpisu uživatele.
      /// </summary>
      /// <returns>Jméno a heslo</returns>
      public override string ToString()
      {
         return String.Format("Jméno: {0}; \tHeslo: {1}", Jmeno, Heslo);
      }

   }
}
