using System;
using System.Collections.Generic;
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
   /// Třída uchovávající informace o konkrétní položce.
   /// </summary>
   public class Polozka
   {
      /// <summary>
      /// Název vytvořené položky
      /// </summary>
      public string Nazev { get; set; }

      /// <summary>
      /// Hodnota vytvořené položky
      /// </summary>
      public double Cena { get; set; }

      /// <summary>
      /// Textový popis vytvořené položky
      /// </summary>
      public string Popis { get; set; }

      /// <summary>
      /// Kategorie do které položka spadá.
      /// </summary>
      public Kategorie KategoriePolozky { get; set; }



      /// <summary>
      /// Konstruktor třídy pro vytvoření nové položky s nastavením všech parametrů předaných v konstruktoru.
      /// </summary>
      /// <param name="Nazev">Název položky</param>
      /// <param name="Cena">Hodnota položky</param>
      /// <param name="kategorie">Kategorie položky</param>
      /// <param name="popis">Textový popis položky</param>
      public Polozka(string Nazev, double Cena, Kategorie kategorie, string popis)
      {
         this.Nazev = Nazev;
         this.Cena = Cena;
         this.KategoriePolozky = kategorie;
         this.Popis = popis;
      }

      /// <summary>
      /// Bezparametrický konstruktor třídy pro možnost ukládání dat do souboru.
      /// </summary>
      public Polozka()
      {

      }


      /// <summary>
      /// Vrátí textový řetězec s názvem položky a její cenou.
      /// </summary>
      /// <returns>Textový řetězec</returns>
      public override string ToString()
      {
         if (Popis.Length > 0)
            return String.Format("{0} ({1}): {2} Kč", Nazev, Popis, Cena);
         else
            return String.Format("{0}: {1} Kč", Nazev, Cena);
      }

   }
}
