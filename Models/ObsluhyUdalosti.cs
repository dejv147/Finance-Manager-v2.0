using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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
   /// Třída uchovává metody pro obsluhu událostí, které nejsou přímo vázány na konkrétní třídu (nevyžadují konkrétní interní data třídy).
   /// </summary>
   class ObsluhyUdalosti
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Pomocná proměnná pro kontrolu aktuálního času.
      /// Tato proměnná je využita pro zajištění, že se čas bude aktualizovat jen když dojde ke změně časové hodnoty (šetření výpočetního času).
      /// </summary>
      private int PomocneSekundy;

      /// <summary>
      /// Pomocná proměnná informující zda je rozbaleno levé postranní MENU v hlavním okně.
      /// </summary>
      private bool RozbalenoLeveMenuHlavnihoOkna;

      /// <summary>
      /// Pomocná proměnná informující zda je rozbaleno pravé postranní MENU v hlavním okně.
      /// </summary>
      private bool RozbalenoPraveMenuHlavnihoOkna;


      /// <summary>
      /// Konstruktor třídy pro obslužné metody událostí.
      /// </summary>
      public ObsluhyUdalosti()
      {
         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Nastavení úvodní hodnoty do pomocné proměnné časovače
         PomocneSekundy = 0;

         // Úvodní nastavení pomocných proměných pro obsluhu událostí ovládajících postranní MENU v hlavním okně
         RozbalenoLeveMenuHlavnihoOkna = false;
         RozbalenoPraveMenuHlavnihoOkna = false;
      }



      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Události kontroléru */

      /// <summary>
      /// Metoda pro řízení zobrazování aktuálního času. 
      /// Metoda se spouští v krátkých časových intervalech a při každém spuštění kontroluje zda došlo ke změně (aktualizaci) času. 
      /// Pokud se časová hodnota změnila je volána pomocná metoda pro aktualizaci hodnot zobrazujících aktuální čas.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void CasovacSpousteniCasu_Tick(object sender, EventArgs e)
      {
         // Pokud došlo ke změně času zavolá se metoda pro aktualizaci času zobrazovaného v hlavním okně aplikace
         if (DateTime.Now.Second != PomocneSekundy)
            Controller.AktualizujDatumCas();

         // Uložení aktuálního času do pomocné proměnné
         PomocneSekundy = DateTime.Now.Second;
      }



      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Události hlavního okna */

      /// <summary>
      /// Obsluha události vyvolané při změně textu poznámkového bloku zobrazeného v hlavním okně aplikace. 
      /// Obsluha provede uložení nového textu do interní proměnné přihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PoznamkovyBlokTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         // Převední objektu na původní datový typ
         TextBox PoznamkovyBlok = sender as TextBox;

         // Uložení textu do vlastnosti přihlášeného uživatele
         Controller.NastavTextPoznamkyUzivateli(PoznamkovyBlok.Text);
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na daný blok.
      /// Vykreslení postranního MENU v plné velikosti včetně ovládacích prvků.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void LeveMENU_MouseMove(object sender, MouseEventArgs e)
      {
         // Zabezpečení proti opakovanému vykreslování (kurzor jednou najede na daný blok, příznaková proměnná se přepne)
         if (!RozbalenoLeveMenuHlavnihoOkna)
         {
            // Volání metody pro překreslení plátna 
            Controller.VykresliLevePostraniMENU(false);

            // Nastavení příznakové proměnné
            RozbalenoLeveMenuHlavnihoOkna = true;
         }
      }

      /// <summary>
      /// Obsluha události při opuštění daného bloku kurzorem myši. 
      /// Vykreslené postranní MENU se schová do postranní lišty.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void LeveMENU_MouseLeave(object sender, MouseEventArgs e)
      {
         // Zabezpečení proti opakovanému vykreslování (kurzor jednou odjede, příznaková proměnná se přepne)
         if (RozbalenoLeveMenuHlavnihoOkna)
         {
            // Volání metody pro překreslení plátna 
            Controller.VykresliLevePostraniMENU(true);

            // Nastavení příznakové proměnné
            RozbalenoLeveMenuHlavnihoOkna = false;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na daný blok.
      /// Vykreslení postranního MENU v plné velikosti včetně ovládacích prvků.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PraveMENU_MouseMove(object sender, MouseEventArgs e)
      {
         // Zabezpečení proti opakovanému vykreslování (kurzor jednou najede na daný blok, příznaková proměnná se přepne)
         if (!RozbalenoPraveMenuHlavnihoOkna)
         {
            // Volání metody pro překreslení plátna 
            Controller.VykresliPravePostraniMENU(false);

            // Nastavení příznakové proměnné
            RozbalenoPraveMenuHlavnihoOkna = true;
         }
      }

      /// <summary>
      /// Obsluha události při opuštění daného bloku kurzorem myši. 
      /// Vykreslené postranní MENU se schová do postranní lišty.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PraveMENU_MouseLeave(object sender, MouseEventArgs e)
      {
         // Zabezpečení proti opakovanému vykreslování (kurzor jednou odjede, příznaková proměnná se přepne)
         if (RozbalenoPraveMenuHlavnihoOkna)
         {
            // Volání metody pro překreslení plátna 
            Controller.VykresliPravePostraniMENU(true);

            // Nastavení příznakové proměnné
            RozbalenoPraveMenuHlavnihoOkna = false;
         }
      }



      /// <summary>
      /// Otevření okna pro přidání nového záznamu.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PridatZaznam_Click(object sender, RoutedEventArgs e)
      {
         // Otevření okna pro přidání nového záznamu
         Controller.OtevriOknoPridatUpravitZaznam(1);
      }

      /// <summary>
      /// Metoda pro smazání vybraného záznamu.
      /// Metoda je vyvolána stiskem tlačítka pro odebrání záznamu.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void OdebratZaznam_Click(object sender, RoutedEventArgs e)
      {
         Controller.SmazZaznam();
      }

      /// <summary>
      /// Otevření okna pro možnost vyhledávání záznamů dle zvolených kritérií.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Vyhledat_Click(object sender, RoutedEventArgs e)
      {
         // Otevření okna Vyhledat
         Controller.OtevriOknoVyhledat();
      }

      /// <summary>
      /// Otevření okna pro možnost prohlížení veškerých záznamů z kategorie Příjmy.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void ZobrazPrijmy_Click(object sender, RoutedEventArgs e)
      {
         // Otevření okna pro zobrazení příjmů
         Controller.OtevriOknoZobrazitPrijmyVydaje(KategoriePrijemVydaj.Prijem);
      }

      /// <summary>
      /// Otevření okna pro možnost prohlížení veškerých záznamů z kategorie Výdaje.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void ZobrazVydaje_Click(object sender, RoutedEventArgs e)
      {
         // Otevření okna pro zobrazení výdajů
         Controller.OtevriOknoZobrazitPrijmyVydaje(KategoriePrijemVydaj.Vydaj);
      }

      /// <summary>
      /// Otevření okna pro zobrazení statisticky zpracovaných dat (záznamů).
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Statistika_Click(object sender, RoutedEventArgs e)
      {
         Controller.ZobrazStatistiku();
      }


      /// <summary>
      /// Metoda obsluhující tlačítko Kalkulacka.
      /// Otevření projektu z jiného jmeného prostoru. 
      /// Po kliknutí na tlačítko se otevře projekt realizující jednoduchou kalkulačku ve WPF.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void KalkulackaButton_Click(object sender, RoutedEventArgs e)
      {
         Controller.OtevriKalkulacku();
      }

      /// <summary>
      /// Otevření okenního formuláře pro možnost importovat data ze souboru.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void ImportDat_Click(object sender, RoutedEventArgs e)
      {
         ImportExportDat_Window ImportWindow = new ImportExportDat_Window(1);
         ImportWindow.ShowDialog();
      }

      /// <summary>
      /// Otevření okenního formuláře pro možnost exportovat data do souboru (uložení záznamů do textového souboru).
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void ExportDat_Click(object sender, RoutedEventArgs e)
      {
         Controller.OtevriOknoExportDat();
      }

      /// <summary>
      /// Uložení dat v aplikaci do souboru.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Ulozit_Click(object sender, RoutedEventArgs e)
      {
         Controller.ProvedUlozeniDat();
      }

      /// <summary>
      /// Otevření okenního formuláře pro možnost změnit nastavení určitých vlastností aplikace.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Nastaveni_Click(object sender, RoutedEventArgs e)
      {
         // Otevření okna nastavení
         Nastaveni nastaveniWindow = new Nastaveni();
         nastaveniWindow.ShowDialog();

         // Aktualizace vykreslení obrazovky hlavního okna aplikace
         Controller.HlavniOkno.AktualizujUvodniObrazovku();
      }

      /// <summary>
      /// Zobrazení informačního okna při kliknutí na příslušné tlačítko
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void InformaceButton_Click(object sender, RoutedEventArgs e)
      {
         InformacniWindow informacniWindow = new InformacniWindow();
         informacniWindow.ShowDialog();
      }

      /// <summary>
      /// Obsluha události pro odhlášení aktuálního uživatele a vykreslení obrazovky pro nepřihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Odhlasit_Click(object sender, RoutedEventArgs e)
      {
         Controller.OdhlasUzivatele();
      }

      /// <summary>
      /// Metoda obsluhující tlačítko pro přihlášení.
      /// Tlačítko je umístěno v obrazovce pro nepřihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PrihlasitButton_Click(object sender, RoutedEventArgs e)
      {
         // Nastavení úvodního zobrazení (vyžádání přihlášení uživatele do aplikace)
         Controller.NastavUvodniZobrazeni();

         // Aktualizace vykreslení hlavního okna
         Controller.HlavniOkno.AktualizujUvodniObrazovku();
      }

      /// <summary>
      /// Metoda pro otevření okna pro registraci nového uživatele.
      /// Tlačítko je umístěno v obrazovce pro nepřihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void RegistrovatButton_Click(object sender, RoutedEventArgs e)
      {
         // Otevření okna pro možnost registrace nového uživatele
         Registrace_Window registrace_Window = new Registrace_Window();
         registrace_Window.ShowDialog();

         // Aktualizace vykreslení hlavního okna
         Controller.HlavniOkno.AktualizujUvodniObrazovku();
      }




      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Obslužné metody pro události okna Prihlaseni */

      /// <summary>
      /// Obslužná metoda, která se spustí při vyvolání událost kliknutí na tlačítko Přihlásit v okně Prihlaseni.
      /// Metoda provede přihlášení uživatele do aplikace na základě uživatelského jména a hesla.
      /// </summary>
      /// <param name="Jmeno">Jméno uživatele</param>
      /// <param name="Heslo">Heslo uživatele</param>
      /// <returns>TRUE - Přihlášení proběhlo úspěšně, FALSE - Přihlášení se nezdařilo</returns>
      public bool PrihlasitWindow_PrihlaseniButtonClick(string Jmeno, string Heslo)
      {
         // Kontrola zda je zadáno jméno
         if (!(Jmeno.Length > 0))
            throw new ArgumentException("Zadejte jméno!");

         // Kontrola zda je zadáno heslo
         if (!(Heslo.Length > 0))
            throw new ArgumentException("Zadejte heslo!");

         // Přihlášení uživatele do aplikace
         bool Volba= Controller.PrihlasUzivatele(Jmeno, Heslo);

         // Kontrola přihlášení uživatele
         if (Controller.VratJmenoPrihlasenehoUzivatele().Length > 0)
            return true;
         else
            return false;
      }

      /// <summary>
      /// Obslužná metoda, která se spustí při vyvolání událost kliknutí na tlačítko Registrovat v okně Prihlaseni.
      /// Otevření okna pro registraci nového uživatele
      /// </summary>
      /// <returns>TRUE - Registrovaný uživatel je přihlášen, FALSE - Registrovaný uživatel není přihlášen</returns>
      public bool PrihlasitWindow_RegistraceButtonClick()
      {
         // Otevření okna pro registraci nového uživatele
         Registrace_Window registrace = new Registrace_Window();
         registrace.ShowDialog();

         // Kontrola přihlášení uživatele
         if (Controller.VratJmenoPrihlasenehoUzivatele().Length > 0)
            return true;
         else
            return false;
      }



      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Obslužné metody pro události okna Registrace */

      /// <summary>
      /// Obslužná metoda, která se spustí při vyvolání událost kliknutí na tlačítko Registrovat v okně Registrace.
      /// Metoda provede registraci nového uživatele do systému a v případě úspěšné registrace provede automatické přihlášení nového uživatele
      /// </summary>
      /// <param name="Jmeno">Jméno uživatele</param>
      /// <param name="Heslo">Heslo uživatele</param>
      /// <returns>TRUE - Registrace proběhla úspěšně, FALSE - Registrace se nezdařila</returns>
      public bool RegistraceWindow_RegistraceButtonClick(string Jmeno, string Heslo)
      {
         // Kontrola zda bylo zadáno jméno
         if (!(Jmeno.Length > 2))
            throw new ArgumentException("Zadejte jméno! (alespoň 3 znaky)");

         // Kontrola zda bylo zadáno heslo
         if (!(Heslo.Length > 0))
            throw new ArgumentException("Zadejte heslo!");

         // Kontrola zda heslo splňuje minimální bezpečnostní prvky
         Controller.ZkontrolujSiluHesla(Heslo, null);

         // Registrace nového uživatele do aplikace
         bool UspesnaRegistrace = Controller.RegistrujUzivatele(Jmeno, Heslo);

         // Přihlášení uživatele do aplikace v případě že registrace proběhla úspěšně
         if (UspesnaRegistrace)
            Controller.PrihlasUzivatele(Jmeno, Heslo);

         // Návratová hodnota informující zda registrace proběhla úspěšně
         return UspesnaRegistrace;
      }

   }
}
