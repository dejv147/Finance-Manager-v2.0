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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře PridatUpravitZaznam_Window.xaml
   /// Třídá slouží pro zpracování zadaných dat a předání dat kontroléru v závislosti na režimu okna. 
   /// Okno může pracovat v upravovacím režimu, kdy načte a zobrazí data o konkrétním záznamu a poté načte a předá uživatelem upravená data kontroléru pro přepis původních dat záznamu.
   /// Okno může také pracovat v přidávacím režimu, kdy zobrazí prázdné přidávací pole a po vyplnění polí uživatelem načte a předá kontroléru data pro vytvoření nového záznamu a přidání nového záznamu do kolekce záznamů přihlášeného uživatele.
   /// </summary>
   public partial class PridatUpravitZaznam_Window : Window
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
      /// Datum záznamu
      /// </summary>
      private DateTime Datum;

      /// <summary>
      /// Hodnota celkového příjmu nebo výdaje daného záznamu
      /// </summary>
      private double PrijemVydaj_Hodnota;

      /// <summary>
      /// Poznámka k záznamu
      /// </summary>
      private string Poznamka;

      /// <summary>
      /// Vybraná kategorie záznamu
      /// </summary>
      private Kategorie KategorieZaznamu;

      /// <summary>
      /// Značka zda se jedná o výdaj nebo příjem
      /// </summary>
      private KategoriePrijemVydaj PrijemNeboVydaj;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;



      /// <summary>
      /// Konstruktor třídy spravující okenní formulář pro práci se záznamem.
      /// </summary>
      public PridatUpravitZaznam_Window()
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Nastavení příznakového bitu
         ZavrenoBezUlozeni = 1;

         // Nastavení barvy pozadí
         HlavniOknoZaznamuGrid.Background = Controller.BarvaPozadi;
      }


      /// <summary>
      /// Úvodní nastavení okna v režimu úpravy existujícího záznamu.
      /// </summary>
      /// <param name="zaznam">Záznam určený k úpravě</param>
      public void UvodniNastaveniRezimuUpravovani(Zaznam zaznam)
      {
         // Úvodní nastavení okna
         Title = "Úprava existujícího záznamu";
         Icon = new BitmapImage(new Uri(Path.Combine(Validace.VratCestuSlozkyAplikace(), "Icons\\Disketa.png")));

         // Úvodní nastavení interních proměnných
         Nazev = zaznam.Nazev;
         Datum = zaznam.Datum;
         PrijemNeboVydaj = zaznam.PrijemNeboVydaj;
         PrijemVydaj_Hodnota = zaznam.Hodnota_PrijemVydaj;
         Poznamka = zaznam.Poznamka;
         KategorieZaznamu = zaznam.kategorie;

         // Vytvoření instance třídy pro nastavení stylů tlačítek
         GrafickePrvky Grafika = new GrafickePrvky();

         // Nastavení stylu tlačítek
         Grafika.NastavTlacitkoULOZIT(UlozitButton);
         Grafika.NastavTlacitkoUPRAVITPOZNAMKU(NastavPoznamkuButton);
         Grafika.NastavTlacitkoUPRAVITPOLOZKY(NastavPolozkuButton);

         // Nastavení zadávacích polí
         NastavZadavaciPole();

         // Nastavení zobrazení kategorie a hodnoty záznamu
         KategorieComboBox.SelectedIndex = (int)KategorieZaznamu - 1;
         PrijemVydajComboBox.SelectedIndex = (int)PrijemNeboVydaj;
         PrijemVydajTextBox.Text = PrijemVydaj_Hodnota.ToString();
      }

      /// <summary>
      /// Úvodní nastavení okna v režimu přidání nového záznamu.
      /// </summary>
      public void UvodniNastaveniRezimuPridavani()
      {
         // Úvodní nastavení okna
         Title = "Přidat nový záznam";
         Icon = new BitmapImage(new Uri(Path.Combine(Validace.VratCestuSlozkyAplikace(), "Icons\\NewFile.png")));

         // Úvodní nastavení interních proměnných
         Nazev = "";
         Datum = DateTime.Now.Date;
         PrijemVydaj_Hodnota = 0;
         Poznamka = "";
         KategorieZaznamu = Kategorie.Nevybrano;

         // Vytvoření instance třídy pro nastavení stylů tlačítek
         GrafickePrvky Grafika = new GrafickePrvky();

         // Nastavení stylu tlačítek
         Grafika.NastavTlacitkoULOZIT(UlozitButton);
         Grafika.NastavTlacitkoPRIDATPOZNAMKU(NastavPoznamkuButton);
         Grafika.NastavTlacitkoPRIDATPOLOZKY(NastavPolozkuButton);

         // Nastavení zadávacích polí
         NastavZadavaciPole();
      }

      /// <summary>
      /// Nastavení zadávacích polí do výchozího nasttavení.
      /// </summary>
      private void NastavZadavaciPole()
      {
         NazevZaznamuTextBox.Text = Nazev;
         DatumZaznamuDatePicker.SelectedDate = Datum;
         Controller.NastavKategorieDoComboBoxu(KategorieComboBox);
      }



      /// <summary>
      /// Uložení zadaného názvu do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void NazevZaznamuTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Nazev = NazevZaznamuTextBox.Text.ToString();
      }

      /// <summary>
      /// Obsluha udáosti vyvolané při zadávání hodnoty do textového bloku. 
      /// Při zadávání se zadané číslo načítá do interní proměnné pro následné zpracování.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrijemVydajTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (PrijemVydajTextBox.Text.Length > 0)
               PrijemVydaj_Hodnota = Validace.NactiCislo(PrijemVydajTextBox.Text);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení do boxu načtenou hodnotu (smazání neplatného obsahu)
            PrijemVydajTextBox.Text = PrijemVydaj_Hodnota.ToString();
         }
      }

      /// <summary>
      /// Načtení vybraného data do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumZaznamuDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            Datum = Validace.NactiDatum(DatumZaznamuDatePicker.SelectedDate);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            // Zobrazení data z paměti
            DatumZaznamuDatePicker.SelectedDate = Datum;
         }
      }

      /// <summary>
      /// Uložení konkrétního výčtového typu do kategorie dle zvoleného typu z rozbalovací nabídky.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void KategorieComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         KategorieZaznamu = (Kategorie)(KategorieComboBox.SelectedIndex + 1);
      }

      /// <summary>
      /// Uložení značky zda se vytváří příjem nebo výdaj.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrijemVydajComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         PrijemNeboVydaj = PrijemVydajComboBox.SelectedIndex == 0 ? KategoriePrijemVydaj.Prijem : KategoriePrijemVydaj.Vydaj;
      }


      /// <summary>
      /// Obslužná metoda pro předání zadaných parametrů záznamu kontroléru.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UlozitButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Kontrola zda byl zadán název
            if (!(Nazev.Length > 0))
               throw new ArgumentException("Zadejte název!");

            // Kontrola zda bylo vybrán druh záznamu (příjem/výdaj)
            if (!(PrijemVydajComboBox.SelectedIndex == 0 || PrijemVydajComboBox.SelectedIndex == 1))
               throw new ArgumentException("Zvolte zda se jedná o příjem nebo výdaj");

            // Kontrola zda byla zadána hodnota příjmu/výdaje
            if (!(PrijemVydaj_Hodnota.ToString().Length > 0))
               throw new ArgumentException("Zadejte hodnotu");

            // Kontrola zda byla vybrána kategorie
            if (KategorieZaznamu == Kategorie.Nevybrano)
               throw new ArgumentException("Vyberte kategorii!");

            // Nastavení parametrů zadaných uživatelem do záznamu v kontroléru aplikace
            Controller.UpravZaznam(Nazev, Datum, PrijemVydaj_Hodnota, PrijemNeboVydaj, Poznamka, KategorieZaznamu);

            // Zavření okna
            ZavrenoBezUlozeni = 0;
            Close();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Otevření okenního formuláře pro přidání položek k záznamu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void NastavPolozkuButton_Click(object sender, RoutedEventArgs e)
      {
         // Pokud jsou nějaké položky v seznamu položek určených k zobrazení, otevře se okno pro úpravu položek
         if (Controller.VratPocetZobrazovanychPolozek() > 0)
            Controller.OtevriOknoPridatUpravitPolozky(0);

         // Pokud v sezanmu položek žádné nejsou, otevře se okno pro přidání nových položek
         else
            Controller.OtevriOknoPridatUpravitPolozky(1);
      }

      /// <summary>
      /// Otevření okenního formuláře pro přidání poznámky k záznamu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void NastavPoznamkuButton_Click(object sender, RoutedEventArgs e)
      {
         // Vytvoření poznámkového bloku pro možnost upravit text poznámky
         TextBox PoznamkovyBlok = new TextBox();
         PoznamkovyBlok.Text = Poznamka;

         // Otevření okna pro úpravu textu poznámky
         Controller.OtevriOknoPoznamky(PoznamkovyBlok);

         // Načtení upraveného textu poznámky
         Poznamka = PoznamkovyBlok.Text;
      }


      /// <summary>
      /// Zobrazení upozornění při zavírání okna.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         // Pokud je okno zavřeno křížkem (bez uložení dat) zobrazí se varovné okno
         if (ZavrenoBezUlozeni == 1)
         {
            MessageBoxResult VybranaMoznost = MessageBox.Show("Provedené změny nebudou uloženy! \nChcete pokračovat?", "Pozor", MessageBoxButton.YesNo, MessageBoxImage.Question);

            switch (VybranaMoznost)
            {
               case MessageBoxResult.Yes:
                  break;

               case MessageBoxResult.No:
                  e.Cancel = true;
                  break;
            }
         }

         // Pokud je okno zavřeno voláním funkce (s uložením dat) tak se okno zavře bez varování
         else
         {
            e.Cancel = false;
         }
      }
      
   } 
}
