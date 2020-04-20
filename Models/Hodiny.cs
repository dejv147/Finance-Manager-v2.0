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
   /// Pomocná třída pro řízení zobrazovaných hodin.
   /// Třída nastavuje zobrazovaný formát data i času a nastavuje polohu hodinových ručiček pro analogové hodiny.
   /// </summary>
   public static class Hodiny
   {
      /// <summary>
      /// Metoda pro výpočet úhlu natočení vteřinové ručičky analogových hodin pro aktuální čas.
      /// </summary>
      /// <returns>Uhel natočení vteřinové ručičky</returns>
      public static double NastavUhelVterinoveRucicky()
      {
         return DateTime.Now.Second * 6;
         /*
          * Celá minuta má 60 sekund což odpovídá 360 stupňům celého kruhu. Proto je třeba při výpočtu úhlu počet sekund vynásobit 6. 
          * 60 sekund * 6 = 360 stupňů = celý kruh
          */
      }

      /// <summary>
      /// Metoda pro výpočet úhlu natočení miutové ručičky analogových hodin pro aktuální čas.
      /// </summary>
      /// <returns>Uhel natočení minutové ručičky</returns>
      public static double NastavUhelMinutoveRucicky()
      {
         return (DateTime.Now.Minute * 6 + DateTime.Now.Second * 0.1);
         /*
          * Celá hodina má 60 minut což odpovídá 360 stupňům celého kruhu. Proto je třeba při výpočtu úhlu počet minut vynásobit 6. 
          * Dále je k úhlu připočten počet sekund vynásobený 1/60 minuty (6 stupňů) pro plynulý pohyb minutové ručičky (pohybuje se každou vteřinu a neskáče jen při změně hodnoty minuty).
          * 60 sekund * 0,1 = 6 stupňů = 1 minuta
          */
      }

      /// <summary>
      /// Metoda pro výpočet úhlu natočení hodinové ručičky analogových hodin pro aktuální čas.
      /// </summary>
      /// <returns>Uhel natočení hodinové ručičky</returns>
      public static double NastavUhelHodinoveRucicky()
      {
         return (DateTime.Now.Hour * 30 + DateTime.Now.Minute * 0.5);
         /*
          * Na ciferníku je 12 hodin, každé hodině tedy odpovídá posun o úhel 30 stupňů (12 hodin * 30 stupňů = 360 stupňů = celý kruh), proto je počet hodin třeba vynásobit 30.
          * Dále je k úhlu připočten počet minut vynásobený 1/60 hodiny (30 stupňů) pro plynulý pohyb ručičky (pohybuje se každou minutu a neskáče jen při změně hodnoty hodiny).
         */
      }

      /// <summary>
      /// Vrátí aktuální čas ve zvoleném formátu
      /// </summary>
      /// <param name="volba">0 - dlouhý formát času (24h), 1 - krátký formát času (12h)</param>
      /// <returns>Textový řetězec obsahující aktuální čas</returns>
      public static string VratAktualniCas(byte volba)
      {
         if (volba == 0)
            return DateTime.Now.ToLongTimeString();

         else if (volba == 1)
            return DateTime.Now.ToString("hh:mm:ss");

         else
            throw new ArgumentException("Špatně zvolený požadovaný formát času!");
      }

      /// <summary>
      /// Vrátí aktuální datum v nastaveném formátu
      /// </summary>
      /// <returns>Textový řetězec obsahující aktuální datum</returns>
      public static string VratAktualniDatum()
      {
         return DateTime.Now.ToString("dd.MM.yyyy");
      }

      /// <summary>
      /// Vrátí aktuální den v týdnu
      /// </summary>
      /// <returns>Textový řetězec obsahující den v týdnu</returns>
      public static string VratAktualniDen()
      {
         return DateTime.Now.DayOfWeek.ToString();
      }

      /// <summary>
      /// Metoda vrátí textovou reprezentaci dne v týdnu.
      /// </summary>
      /// <param name="DenVTydnu">Den v týdnu</param>
      /// <returns>Textová reprezentace zadaného dne</returns>
      public static string VratDenVTydnu(DayOfWeek DenVTydnu)
      {
         string Den = "";

         switch (DenVTydnu)
         {
            case DayOfWeek.Monday:     Den = "Pondělí";  break;
            case DayOfWeek.Tuesday:    Den = "Úterý";    break;
            case DayOfWeek.Wednesday:  Den = "Středa";   break;
            case DayOfWeek.Thursday:   Den = "Čtvrtek";  break;
            case DayOfWeek.Friday:     Den = "Pátek";    break;
            case DayOfWeek.Saturday:   Den = "Sobota";   break;
            case DayOfWeek.Sunday:     Den = "Neděle";   break;
         }
         return Den;
      }

      /// <summary>
      /// Metoda vrátí textovou reprezentaci zadaného měsíce.
      /// </summary>
      /// <param name="Mesic">Číslo měsíce</param>
      /// <returns>Textová reprezentace zadaného měsíce</returns>
      public static string VratMesicTextove(int Mesic)
      {
         string mesic = "";

         switch (Mesic)
         {
            case 1:  mesic = "Leden";     break;
            case 2:  mesic = "Únor";      break;
            case 3:  mesic = "Březen";    break;
            case 4:  mesic = "Duben";     break;
            case 5:  mesic = "Květen";    break;
            case 6:  mesic = "Červen";    break;
            case 7:  mesic = "Červenec";  break;
            case 8:  mesic = "Srpen";     break;
            case 9:  mesic = "Září";      break;
            case 10: mesic = "Říjen";     break;
            case 11: mesic = "Listopad";  break;
            case 12: mesic = "Prosinec";  break;
            default: break;
         }
         return mesic;
      }
   }
}
