using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře ZobrazitPrijmyVydaje_Window.xaml
   /// Okenní formulář slouží k zobrazení záznamů v kategorii Příjem, nebo Výdaj s možností filtrace zobrazených záznamů dle určitých parametrů.
   /// </summary>
   public partial class ZobrazitPrijmyVydaje_Window : Window
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Volba zda je okno určeno pro výpis příjmů nebo výdajů.
      /// </summary>
      private byte Volba;

      /// <summary>
      /// Spodní hranice datumu pro vyhledání konkrétních záznamů
      /// </summary>
      private DateTime Datum_MIN;

      /// <summary>
      /// Horní hranice datumu pro vyhledání konkrétních záznamů
      /// </summary>
      private DateTime Datum_MAX;

      /// <summary>
      /// Spodní hranice hodnoty pro vyhledání konkrétních záznamů
      /// </summary>
      private double Hodnota_MIN;

      /// <summary>
      /// Horní hranice hodnoty pro vyhledání konkrétních záznamů
      /// </summary>
      private double Hodnota_MAX;

      /// <summary>
      /// Pomocná proměnná pro výpis příjmů nebo výdajů
      /// </summary>
      private string slovo;



      /// <summary>
      /// Konstruktor třídy pro správu okna zobrazující příjmy nebo výdaje.
      /// </summary>
      /// <param name="ZobrazitPrijmyNeboVydaje">1 - zobrazit příjmy, 0 - zobrazit výdaje</param>
      public ZobrazitPrijmyVydaje_Window(byte ZobrazitPrijmyNeboVydaje)
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Nastavení stylu okna dle požadovaného zobrazení
         if (ZobrazitPrijmyNeboVydaje == 1)
            NastavStylPrijmu();
         else
            NastavStylVydaju();
      }



      /// <summary>
      /// Úvodní nastavení okna pro zobrazení příjmů nebo výdajů.
      /// </summary>
      public void UvodniNastaveni()
      {
         // Nastavení barvy pozadí
         Background = Controller.BarvaPozadi;
         SeznamZaznamuCANVAS.Background = Controller.BarvaPozadi;

         // Nastavení data
         Datum_MIN= new DateTime(DateTime.Now.Year, 1, 1);
         Datum_MAX = DateTime.Now.Date;
         DatumMIN_DatePicker.SelectedDate = Datum_MIN;
         DatumMAX_DatePicker.SelectedDate = Datum_MAX;

         // Nastavení hodnoty
         Hodnota_MIN = 0;
         Hodnota_MAX = 100000;
         HodnotaMIN_TextBox.Text = Hodnota_MIN.ToString();
         HodnotaMAX_TextBox.Text = Hodnota_MAX.ToString();

         // Úvodní vykreslení seznamu záznamů
         AktualizujZobrazeniZaznamu();
      }

      /// <summary>
      /// Nastavení okna pro zobrazení příjmů.
      /// </summary>
      private void NastavStylPrijmu()
      {
         Title = "Příjmy";
         slovo = "příjmů";
         Volba = 1;
         Controller.AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj.Prijem);
      }

      /// <summary>
      /// Nastavení okna pro zobrazení výdajů.
      /// </summary>
      private void NastavStylVydaju()
      {
         Title = "Výdaje";
         slovo = "výdajů";
         Volba = 0;
         Controller.AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj.Vydaj);
      }

      /// <summary>
      /// Aktualizace vykreslení vyhledávaných záznamů.
      /// </summary>
      private void AktualizujZobrazeniZaznamu()
      {
         // Vykreslení seznamu záznamů
         Controller.VykresliSeznamZaznamu();

         // Vypsání přehledu shrnující základní údaje o zobrazovaných záznamech
         VypisSouhrnyPrehled();
      }

      /// <summary>
      /// Vykreslení bloku pro souhrný přehled o vyhledaných záznamech.
      /// </summary>
      private void VypisSouhrnyPrehled()
      {
         // Pomocne proměnné
         int PocetZaznamu = Controller.VratPocetZobrazovanychZaznamu();
         (double Prijmy, double Vydaje) CelkovaHodnota = Controller.VratPrijmyVydajeZobrazovanychZaznamu();

         // Načtení celkové hodnoty příjmů nebo výdajů, dle zvoleného stylu okna
         double Hodnota = Volba == 1 ? CelkovaHodnota.Prijmy : CelkovaHodnota.Vydaje;

         // Výpis získaných údajů do okna
         PocetZaznamuTextBlock.Text = " Bylo nalezeno " + PocetZaznamu.ToString() + " záznamů";
         CelkovaHodnotaTextBlock.Text = "Celková výše " + slovo + " je " + Hodnota.ToString() + " Kč";
      }



      /// <summary>
      /// Obsluha tlačítka pro vyhledání záznamů dle zadaných parametrů.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VyhledatButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Pokud není vybráno kritérium pro hledání zobrazí se chybová zpráva
            if (HledatDleHodnoty_CheckBox.IsChecked == false && HledatDleDatumu_CheckBox.IsChecked == false)
               throw new ArgumentException("Zvolte kritérium pro hledání");

            // Vyhledávání dle data
            if (HledatDleDatumu_CheckBox.IsChecked == true)
            {
               Controller.AktualizujZobrazovaneZaznamyDleData(Datum_MIN, Datum_MAX);
            }

            // Je zatrženo vyhledávání dle hodnoty
            if (HledatDleHodnoty_CheckBox.IsChecked == true)
            {
               if (!(Hodnota_MIN.ToString().Length > 0))
                  throw new ArgumentException("Zadejte minimální hodnotu pro vyhledávání");

               if (!(Hodnota_MAX.ToString().Length > 0))
                  throw new ArgumentException("Zadejte maximální hodnotu pro vyhledávání");

               // Načtení vyhledaných dat do interní kolekce pro zobrazení
               Controller.AktualizujZobrazovaneZaznamyDleHodnoty(Hodnota_MIN, Hodnota_MAX);
            }

            // Upozornění pro případ že zadaným kritértiím nevyhovují žádné záznamy
            if (!(Controller.VratPocetZobrazovanychZaznamu() > 0))
            {
               Controller.AktualizujZobrazovaneZaznamy();

               // Načtení příjmů nebo výdajů do kolekce zobrazovaných dat dle zvoleného stylu zobrazení okna
               if (Volba == 1)
                  Controller.AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj.Prijem);
               else
                  Controller.AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj.Vydaj);

               // Zobrazení varovné zprávy
               throw new ArgumentException("Nebyly nalezeny žádné záznamy");
            }

            // Vykreslení aktualizované kolekce dat pro zobrazení
            AktualizujZobrazeniZaznamu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      /// <summary>
      /// Obsluha tlačítka pro zrušení všech filtrů vyhledávání. 
      /// Metoda nahraje do kolekce dat pro zobrazení kolekci všech dat a obnoví vykreslený seznam záznamů.
      /// </summary>
      /// <param name="sender">Tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZrusitButton_Click(object sender, RoutedEventArgs e)
      {
         Controller.AktualizujZobrazovaneZaznamy();

         // Načtení příjmů nebo výdajů do kolekce zobrazovaných dat dle zvoleného stylu zobrazení okna
         if (Volba == 1)
            Controller.AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj.Prijem);
         else
            Controller.AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj.Vydaj);

         // Aktualizace vykreslení seznamu záznamů
         AktualizujZobrazeniZaznamu();
      }


      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné.
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HodnotaMAX_TextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (HodnotaMAX_TextBox.Text.Length > 0)
               Hodnota_MAX = Validace.NactiCislo(HodnotaMAX_TextBox.Text);
            else
               Hodnota_MAX = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            HodnotaMAX_TextBox.Text = Hodnota_MAX.ToString();
         }
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné.
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HodnotaMIN_TextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (HodnotaMIN_TextBox.Text.Length > 0)
               Hodnota_MIN = Validace.NactiCislo(HodnotaMIN_TextBox.Text);
            else
               Hodnota_MIN = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            HodnotaMIN_TextBox.Text = Hodnota_MIN.ToString();
         }
      }

      /// <summary>
      /// Načtení data do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumMIN_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            Datum_MIN = Validace.NactiDatum(DatumMIN_DatePicker.SelectedDate);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Načtení data do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumMAX_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            Datum_MAX = Validace.NactiDatum(DatumMAX_DatePicker.SelectedDate);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

   }
}
