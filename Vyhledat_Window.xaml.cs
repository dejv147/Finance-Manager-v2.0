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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře Vyhledat_Window.xaml
   /// Okenní formulář slouží pro vyhledávání záznamů dle zadaných parametrů filtrů.
   /// </summary>
   public partial class Vyhledat_Window : Window
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Název záznamu
      /// </summary>
      private string Nazev;

      /// <summary>
      /// Pomocná proměnná pro uchování data začátku aktuálního měsíce
      /// </summary>
      private DateTime PrvniDenAktualnihoMesice;

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
      /// Kategorie záznamu
      /// </summary>
      private Kategorie kategorie;

      /// <summary>
      /// Spodní hranice počtu položek pro vyhledání konkrétních záznamů
      /// </summary>
      private int PocetPolozek_MIN;

      /// <summary>
      /// Horní hranice počtu položek pro vyhledání konkrétních záznamů
      /// </summary>
      private int PocetPolozek_MAX;



      /// <summary>
      /// Konstruktor třídy pro správu okenního formuláře Vyhledat.
      /// </summary>
      public Vyhledat_Window()
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();
      }



      /// <summary>
      /// Úvodní nastavení okna pro vyhledávání.
      /// </summary>
      public void UvodniNastaveni()
      {
         // Nastavení barvy pozadí
         Background = Controller.BarvaPozadi;

         // Nastavení plátna pro výpis záznamů
         SeznamZaznamuCANVAS.Visibility = Visibility.Visible;
         SeznamZaznamuCANVAS.Background = Controller.BarvaPozadi;

         // Nastavení prvního dne aktuálního měsíce
         PrvniDenAktualnihoMesice = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

         // Nastavení zadávacích polí do výchozího nastavení
         ResetujZadavaciPole();

         // Vykreslení seznamu záznamů 
         AktualizujZobrazeniZaznamu();
      }

      /// <summary>
      /// Nastavení zadávacích polí do výchozího nastavení.
      /// </summary>
      private void ResetujZadavaciPole()
      {
         // Vymazání názvu
         Nazev = "";
         NazevTextBox.Text = Nazev;

         // Vymazání pole pro hodnotu
         Hodnota_MIN = 0;
         Hodnota_MAX = 9999999999;
         HodnotaMINTextBox.Text = "";
         HodnotaMAXTextBox.Text = "";

         // Vymazání pole pro počet položek
         PocetPolozek_MIN = 0;
         PocetPolozek_MAX = 10000;
         PolozkyMINTextBox.Text = "";
         PolozkyMAXTextBox.Text = "";

         // Nastavení data 
         Datum_MIN = PrvniDenAktualnihoMesice;
         Datum_MAX = DateTime.Now.Date;
         DatumMIN_DatePicker.SelectedDate = Datum_MIN;
         DatumMAX_DatePicker.SelectedDate = Datum_MAX;

         // Vyplnění rozbalovací nabídky jednotlivými prvky
         Controller.NastavKategorieDoComboBoxu(KategorieComboBox);

         // Nastavení defaultní hodnoty kategorie
         kategorie = Kategorie.Nevybrano;
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

         // Výpis získaných údajů do okna
         PocetZaznamuTextBlock.Text = " Bylo nalezeno " + PocetZaznamu.ToString() + " záznamů";
         CelkovePrijmyTextBlock.Text = " Celková hodnota příjmů je " + CelkovaHodnota.Prijmy.ToString() + " Kč  ";
         CelkoveVydajeTextBlock.Text = " Celková hodnota výdajů je " + CelkovaHodnota.Vydaje.ToString() + " Kč  ";
      }



      /// <summary>
      /// Obluha tlačítka pro vyhledání záznamů dle zadaných parametrů.
      /// Kontrola zvolených filtrů a následné vyhledání záznamů dle zadaných kritérií.
      /// </summary>
      /// <param name="sender">Tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VyhledatButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Pokud je zvoleno zobrazení příjmů a zároveň není zvoleno zobrazení výdajů, vyhledají se všechny příjmy
            if (Prijmy_CheckBox.IsChecked == true && Vydaje_CheckBox.IsChecked == false)
               Controller.AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj.Prijem);

            // Pokud je zvoleno zobrazení výdajů a zároveň není zvoleno zobrazení příjmů, vyhledají se všechny výdaje
            if (Prijmy_CheckBox.IsChecked == false && Vydaje_CheckBox.IsChecked == true)
               Controller.AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj.Vydaj);

            // Kontrola zda je zvoleno zobrazení příjmů nebo výdajů
            if (Prijmy_CheckBox.IsChecked == false && Vydaje_CheckBox.IsChecked == false)
               throw new ArgumentException("Zvolte jaké záznamy chcete vyhledat! \n\tPříjmy - Výdaje");

            // Vyhledání záznamů se zadaným názvem
            if (Nazev_CheckBox.IsChecked == true)
            {
               // Kontrola zda byl zadán název
               if (!(Nazev.Length > 0))
                  throw new ArgumentException("Zadejte název!");

               // Aktualizace záznamů se zadaným názvem
               Controller.AktualizujZobrazovaneZaznamyDleNazvu(Nazev);
            }

            // Vyhledání záznamů dle data
            if (Datum_CheckBox.IsChecked == true)
               Controller.AktualizujZobrazovaneZaznamyDleData(Datum_MIN, Datum_MAX);

            // Vyhledání záznamů dle hodnoty
            if (Hodnota_CheckBox.IsChecked == true)
            {
               if (!(Hodnota_MIN.ToString().Length > 0))
                  throw new ArgumentException("Zadejte minimální hodnotu pro vyhledávání");

               if (!(Hodnota_MAX.ToString().Length > 0))
                  throw new ArgumentException("Zadejte maximální hodnotu pro vyhledávání");

               // Aktualizace záznamů v zadaném rozmezí hodnoty
               Controller.AktualizujZobrazovaneZaznamyDleHodnoty(Hodnota_MIN, Hodnota_MAX);
            }

            // Vyhledání záznamů dle kategorie
            if (Kategorie_CheckBox.IsChecked == true)
            {
               // Kontrola zda byla vybrána kategorie
               if (kategorie == Kategorie.Nevybrano)
                  throw new ArgumentException("Vyberte kategorii!");

               // Aktualizace záznamu dle kategorie
               Controller.AktualizujZobrazovaneZaznamyDleKategorie(kategorie);
            }

            // Vyhledání záznamů dle počtu položek
            if (Polozky_CheckBox.IsChecked == true)
            {
               if (!(PocetPolozek_MIN.ToString().Length > 0))
                  throw new ArgumentException("Zadejte minimální počet položek pro vyhledávání");

               if (!(PocetPolozek_MAX.ToString().Length > 0))
                  throw new ArgumentException("Zadejte maximální počet položek pro vyhledávání");

               // Aktualizace záznamu s počtem položek v zadaném rozmezí
               Controller.AktualizujZobrazovaneZaznamyDlePoctuPolozek(PocetPolozek_MIN, PocetPolozek_MAX);
            }

            // Upozornění pro případ že zadaným kritértiím nevyhovují žádné záznamy
            if (!(Controller.VratPocetZobrazovanychZaznamu() > 0))
            {
               Controller.AktualizujZobrazovaneZaznamy();
               throw new ArgumentException("Nebyly nalezeny žádné záznamy");
            }

            // Vykreslení aktualizovaného seznamu záznamů s vyhledanými záznamy
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
         // Zrušení označení filtrů pro vyhledávání
         Nazev_CheckBox.IsChecked = false;
         Datum_CheckBox.IsChecked = false;
         Hodnota_CheckBox.IsChecked = false;
         Kategorie_CheckBox.IsChecked = false;
         Polozky_CheckBox.IsChecked = false;
         Prijmy_CheckBox.IsChecked = true;
         Vydaje_CheckBox.IsChecked = true;

         // Aktualizace vykreslení všech záznamů
         Controller.AktualizujZobrazovaneZaznamy();
         AktualizujZobrazeniZaznamu();
      }

      /// <summary>
      /// Obsluha vyvolána dvojklikem na tlačítko zrušit. 
      /// Metoda resetuje zadávací pole do výchozího nastavení.
      /// </summary>
      /// <param name="sender">Tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZrusitButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         // Pokud uživatel kliknul na tlačítko vícekrát, resetují se zadávací pole
         ResetujZadavaciPole();
      }

      /// <summary>
      /// Obsluha tlačítka pro odebrání záznamu.
      /// Metoda vyvolá obsluhu události v kontroléru (stejná událost jako v hlavním okně) pro smazání vybraného záznamu.
      /// </summary>
      /// <param name="sender">Tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void OdebratButton_Click(object sender, RoutedEventArgs e)
      {
         Controller.obsluhyUdalosti.OdebratZaznam_Click(sender, e);
      }


      /// <summary>
      /// Načtení zadaného textu do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void NazevTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Nazev = NazevTextBox.Text.ToString();
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PolozkyMAXTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (PolozkyMAXTextBox.Text.Length > 0)
               PocetPolozek_MAX = (int)Validace.NactiCislo(PolozkyMAXTextBox.Text);
            else
               PocetPolozek_MAX = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            PolozkyMAXTextBox.Text = PocetPolozek_MAX.ToString();
         }
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PolozkyMINTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (PolozkyMINTextBox.Text.Length > 0)
               PocetPolozek_MIN = (int)Validace.NactiCislo(PolozkyMINTextBox.Text);
            else
               PocetPolozek_MIN = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            PolozkyMINTextBox.Text = PocetPolozek_MIN.ToString();
         }
      }

      /// <summary>
      /// Uložení konkrétního výčtového typu do kategorie dle zvoleného typu z rozbalovací nabídky.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void KategorieComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         kategorie = (Kategorie)KategorieComboBox.SelectedIndex;
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HodnotaMAXTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (HodnotaMAXTextBox.Text.Length > 0)
               Hodnota_MAX = Validace.NactiCislo(HodnotaMAXTextBox.Text);
            else
               Hodnota_MAX = 100000;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            HodnotaMAXTextBox.Text = Hodnota_MAX.ToString();
         }
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HodnotaMINTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (HodnotaMINTextBox.Text.Length > 0)
               Hodnota_MIN = Validace.NactiCislo(HodnotaMINTextBox.Text);
            else
               Hodnota_MIN = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            HodnotaMINTextBox.Text = Hodnota_MIN.ToString();
         }
      }

      /// <summary>
      /// Načtení vybraného data do interní proměnné.
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

      /// <summary>
      /// Načtení vybraného data do interní proměnné.
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

   }
}
