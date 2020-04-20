using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

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
   /// Třída slouží pro validaci vstupních dat. 
   /// Metody kontrolují správnost zadaných údajů od uživatele a převádí vstupní data do potřebného formátu za účelem snadného zpracování programem.
   /// </summary>
   public static class Validace
   {
      /// <summary>
      /// Převedení čísla zadaného uživatelem do potřebného formátu pro možnost následného zpracování.
      /// </summary>
      /// <param name="ZadanyText">Textová reprezentace čísla</param>
      /// <returns>Číslo v potřebném formátu</returns>
      public static double NactiCislo(string ZadanyText)
      {
         // Interní proměnná pro uložení načteného čísla
         double Cislo;

         // Odstranění mezer v zadávaném čísle
         ZadanyText = ZadanyText.Replace(" ", "");

         // Nahrazení desetinné tečky desetinnou čárkou
         ZadanyText = ZadanyText.Replace(".", ",");

         // Kontrola zda bylo zadáno číslo
         while (!double.TryParse(ZadanyText, out Cislo))
            throw new ArgumentException("Zadali jste číslo ve špatném formátu!");

         // Návratová hodnota
         return Cislo;
      }

      /// <summary>
      /// Metoda pro načtení data zadaného jako vstupní data od uživatele do potřebného formátu pro následné zpracování.
      /// </summary>
      /// <param name="Datum">Datum zadané od uživatele</param>
      /// <returns>Datum v potřebném formátu</returns>
      public static DateTime NactiDatum(DateTime? Datum)
      {
         // Kontrola zda bylo zadání datum
         if (Datum == null)
            throw new ArgumentException("Nebylo zadáno datum!");

         // Kontrola zda se nejedná o budoucí datum
         if (Datum.Value.Date > DateTime.Now)
            MessageBox.Show("Zadáváte budoucí datum", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Information);

         // Navrácení data bez složky uchovávající informace o čase
         return Datum.Value.Date;
      }

      /// <summary>
      /// Metoda pro rozdělení předaného textového řetězce dle předaných separátorů. 
      /// Oddělené části textu jsou uloženy do pole textových řetězců.
      /// </summary>
      /// <param name="Text">Textový řetězec pro rozdělení</param>
      /// <param name="OddelovaciZnaky">Pole znaků sloužící pro oddělení částí textu</param>
      /// <returns>Pole textových řetězců oddělených zadanými separátory</returns>
      public static string[] RozdelText(string Text, char[] OddelovaciZnaky)
      {
         string[] PoleOddelenychSlov = Text.Split(OddelovaciZnaky);
         return PoleOddelenychSlov;
      }

      /// <summary>
      /// Metoda pro složení slov do věty. 
      /// V parametru je předáno pole textových řetězců, které se postupně vkládají do jediného textového řetězce s mezerou jako oddělením jednotlivých slov.
      /// </summary>
      /// <param name="Slova">Pole textových řetězců</param>
      /// <returns>Textový řetězec obsahující všechny řetězce předané v parametru</returns>
      public static string SlozSlovaDoTextu(string[] Slova)
      {
         return string.Join(" ", Slova);
      }

      /// <summary>
      /// Smazání posledního znaku v řetězci
      /// </summary>
      /// <param name="Zprava">Text ke zpracování</param>
      /// <returns>Upravený text</returns>
      public static string SmazPosledniZnak(string Zprava)
      {
         if (Zprava.Length == 0)
            return "";

         string UpravenaZprava = Zprava;
         UpravenaZprava = UpravenaZprava.Remove(UpravenaZprava.Length - 1);
         return UpravenaZprava;
      }

      /// <summary>
      /// Metoda pro navrácení cesty ke složce aplikace v textové podobě.
      /// </summary>
      /// <returns>Textový řetězec obsahující cestu ke složce aplikace</returns>
      public static string VratCestuSlozkyAplikace()
      {
         // Cesta ke složce Debug (složka ze které se spouští aplikace)
         string cesta = Environment.CurrentDirectory;

         // Rozdělení cesty na jednotlivé složky
         string[] Podslozky = cesta.Split('\\');

         // Smazání obsahu proměnné pro možnost vypsání nově upravené cesty ke složce
         cesta = "";

         // Spojení jednotlivých složek zpět do cesty k souboru s vynecháním složky "bin" a složky "Debug"
         for (int i = 0; i < Podslozky.Length; i++)
         {
            // Kontrola zda se nejedná o složky určené k vynechání, správné složky se přiřadí do cesty včetně oddělovače (znak '\')
            if (!(Podslozky[i].Contains("bin") || Podslozky[i].Contains("Debug")))
               cesta += Podslozky[i] + "\\";
         }

         // Odstranění posledního znaku (přebytečný znak '\' na konci cesty)
         SmazPosledniZnak(cesta);

         // Návratová hodnota reprezentující cestu ke složce aplikace v textové podobě
         return cesta;
      }

      /// <summary>
      /// Pomocná metoda pro kontrolu zda zadané helso splňuje minimální požadavky na bezpečnost.
      /// V metodě je kontrolováno celkem 6 bezpečnostních prvků a vrací TRUE pokud heslo splňuje alespoň nastavený minimální počet prvků (zde min 2 prvky).
      /// </summary>
      /// <param name="heslo">Heslo ke kontrole</param>
      /// <returns>TRUE/FALSE vyhodnocení bezpečnosti hesla; Počet nedostatků; Textový řetězec obsahující bezpečnostní nedostatky hesla</returns>
      public static (bool, int, string) KontrolaMinimalniBezpecnostiHesla(string heslo)
      {
         // Vytvoření proměnných pro návratovou hodnotu
         bool BezpecnostSplnena = false;
         string BezpecnostniZprava = "\n";
         int PocetSplnenychPodminekHesla = 0;

         // Pomocné proměnné pro kontrolu hesla:
         ///////////////////////////////////////
         string KontrolniHeslo = heslo;                                 // Uložení zadaného hesla do pomocného řetězce pro analýzu bezpečnosti
         const int MaximalniPocetNedostatku = 4;                        // Maximální počet nesplňených bezpečnostních prvků
         int PocetNedostatku = 0;                                       // Pomocná proměnná pro zjištění počtu nedostatků v zadaném hesle
         string Pismena = "abcdefghijklmnopqrstuvwxyz";                 // Pomocný řetězec pro kontrolu písmen
         string Cislice = "0123456789";                                 // Pomocný řetězec pro kontrolu číslic
         string SpecialniZnaky = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ "; // Pomocný řetězec pro kontorlu speciálních znaků
         string PismenaSDiakritikou = "ěščřžýáíéúůťďňó";                // Pomocný řetězec pro kontorlu písmen s diakritikou
         int PocetVelkychPismen = 0;                                    // Pomocný čítač velkých písmen 
         int PocetMalychPismen = 0;                                     // Pomocný čítač malých písmen
         int PocetCislic = 0;                                           // Pomocný čítač číslic
         int PocetSpecialnichZnaku = 0;                                 // Pomocný čítač speciálních znaků
         int PocetPismenSDiakritikou = 0;                               // Pomocný čítač písmen s diakritikou



         // Zjišťování splnění jednotlivých bezpečnostních požadavků:
         ////////////////////////////////////////////////////////////

         // Kontrola zda heslo obsahuje alespoň 1 malé písmeno
         foreach (char pismeno in Pismena)
         {
            PocetMalychPismen += KontrolniHeslo.Contains(pismeno.ToString()) ? 1 : 0;
         }

         // Kontrola zda heslo obsahuje alespoň 1 velké písmeno
         foreach (char pismeno in Pismena)
         {
            PocetVelkychPismen += KontrolniHeslo.Contains(pismeno.ToString().ToUpper()) ? 1 : 0;
         }

         // Kontrola zda heslo obsahuje alespoň 1 číslici
         foreach (char cislice in Cislice)
         {
            PocetCislic += KontrolniHeslo.Contains(cislice.ToString()) ? 1 : 0;
         }

         // Kontrola zda heslo obsahuje alespoň 1 speciální znak
         foreach (char znak in SpecialniZnaky)
         {
            PocetSpecialnichZnaku += KontrolniHeslo.Contains(znak.ToString()) ? 1 : 0;
         }

         // Kontrola zda heslo obsahuje alespoň 1 písmeno s diakritikou (malé nebo velké)
         foreach (char pismeno in PismenaSDiakritikou)
         {
            PocetPismenSDiakritikou += (KontrolniHeslo.Contains(pismeno.ToString()) || KontrolniHeslo.Contains(pismeno.ToString().ToUpper())) ? 1 : 0;
         }



         // Kontrolní podmínky pro zjištění nedostatků v zadaném hesle:
         //////////////////////////////////////////////////////////////

         // Minimální délka hesla
         if (KontrolniHeslo.Length < 5)
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo nesplňuje minimání délku. \n";
         }

         // Pokud není počet malých písmen v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetMalychPismen > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje malé písmeno. \n";
         }

         // Pokud není počet velkých písmen v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetVelkychPismen > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje velké písmeno. \n";
         }

         // Pokud není počet číslic v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetCislic > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje číslici. \n";
         }

         // Pokud není počet speciálních znaků v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetSpecialnichZnaku > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje speciální znak. \n";
         }

         // Pokud není počet písmen s diakritikou v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetPismenSDiakritikou > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje písmeno s diakritikou. \n";
         }

         // Nastavení pomocné proměné počtu splněných podmínek pro určení síly hesla
         PocetSplnenychPodminekHesla = 6 - PocetNedostatku;

         // Návratová hodnota nastavena podle toho, zda zadané heslo splňuje minimální bezpečnostní nastavení
         BezpecnostSplnena = PocetNedostatku > MaximalniPocetNedostatku ? false : true;


         // Návratová hodnota informující zda je splněna minimální bezpečnost hesla 
         // a vracející počet splněných bezpečnostních podmínek 
         // a textovou zprávu informující o nedostatcích kontrolovaného hesla
         return (BezpecnostSplnena, PocetSplnenychPodminekHesla, BezpecnostniZprava);
      }

   }
}
