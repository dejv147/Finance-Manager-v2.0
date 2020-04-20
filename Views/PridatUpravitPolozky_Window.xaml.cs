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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře PridatUpravitPolozky_Window.xaml
   /// Okenní formulář slouží k vytvoření nebo úpravě kolekce položek s následným uložení do vybraného záznamu.
   /// </summary>
   public partial class PridatUpravitPolozky_Window : Window
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Název položky
      /// </summary>
      private string Nazev;

      /// <summary>
      /// Hodnota položky
      /// </summary>
      private double Cena;

      /// <summary>
      /// Stručný popis položky
      /// </summary>
      private string Popis;

      /// <summary>
      /// Kategorie do které spadá konkrétní položka
      /// </summary>
      private Kategorie KategoriePolozky;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;



      /// <summary>
      /// Konstruktor třídy spravující okenní formulář pro úpravu položek.
      /// </summary>
      public PridatUpravitPolozky_Window()
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Nastavení barvy pozadí
         HlavniOknoPolozekGrid.Background = Controller.BarvaPozadi;

         // Nastavení příznakového bitu
         ZavrenoBezUlozeni = 1;
      }


      /// <summary>
      /// Úvodní nastavení okna.
      /// </summary>
      /// <param name="PridavaciRezim">TRUE - Režim okna pro přidání nových položek, FALSE - Režim okna pro úpravu stávajících položek</param>
      public void UvodniNastaveni(bool PridavaciRezim)
      {
         // Nastavení názvu okna
         Title = PridavaciRezim ? "Přidat nové položky" : "Upravit položky";

         // Nastavení ikony okna
         Icon = new BitmapImage(new Uri(Path.Combine(Validace.VratCestuSlozkyAplikace(), "Icons\\NewFile.png")));

         // Úvodní inicializace pomocných proměnných
         Nazev = "";
         Popis = "";
         Cena = 0;
         KategoriePolozky = Kategorie.Nevybrano;

         // Vytvoření instance třídy pro nastavení stylů tlačítek
         GrafickePrvky Grafika = new GrafickePrvky();

         // Nastavení stylu tlačítek
         Grafika.NastavTlacitkoPRIDAT(PridatButton);
         Grafika.NastavTlacitkoODEBRAT(OdebratButton);

         // Nastavení zadávacích polí
         NazevPolozkyTextBox.Text = Nazev;
         PopisTextBox.Text = Popis;
         Controller.NastavKategorieDoComboBoxu(KategorieComboBox);
      }



      /// <summary>
      /// Uložení kolekce položek do seznamu položek vybraného záznamu, který je udržován v kontroléru aplikace.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UlozitButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Uložení zpracovávaných položek do kolekce vybraného záznamu
            Controller.PridejZobrazovanePolozkyDoVybranehoZaznamu();

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
      /// Přidání nové položky do kolekce zpracovávaných položek v kontroléru aplikace.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PridatButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Kontrola zda byl zadán název
            if (!(Nazev.Length > 0))
               throw new ArgumentException("Zadejte název!");

            // Kontrola zda byla zadána cena
            if (!(Cena.ToString().Length > 0))
               throw new ArgumentException("Zadejte cenu!");

            // Kontrola zda byla vybrána kategorie
            if (KategoriePolozky == Kategorie.Nevybrano)
               throw new ArgumentException("Vyberte kategorii!");

            // Přidání nové položky
            Controller.PridejPolozkuDoInterniKolekce(new Polozka(Nazev, Cena, KategoriePolozky, Popis));
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Obsluha události vyvolaná při stisku tlačítka pro odebrání vybrané položky z kolekce zpracovávaných položek.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void OdebratButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Zobrazení varovného okna s načtením zvolené volby
            MessageBoxResult VybranaVolba = MessageBox.Show("Opravdu chcete vybranou položku odstranit?", "Upozornění", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Smazání vybrané položky v případě stisku tlačíka YES
            switch (VybranaVolba)
            {
               case MessageBoxResult.Yes:
                  Controller.OdeberPolozkuZeZaznamu();   // Odebrání vybrané položky
                  break;

               case MessageBoxResult.No:
                  break;
            }  
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }


      /// <summary>
      /// Uložení zadaného textu do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void NazevPolozkyTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Nazev = NazevPolozkyTextBox.Text.ToString();
      }

      /// <summary>
      /// Uložení zadaného čísla do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void CenaTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (CenaTextBox.Text.Length > 0)
               Cena = Validace.NactiCislo(CenaTextBox.Text);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení do boxu načtené hodnotu (smazání neplatného obsahu)
            CenaTextBox.Text = Cena.ToString();
         }
      }

      /// <summary>
      /// Uložení výčtového typu na základě indexu vybraného prvku v rozbalovacím okně.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void KategorieComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         KategoriePolozky = (Kategorie)(KategorieComboBox.SelectedIndex + 1);
      }

      /// <summary>
      /// Uložení zadaného textu do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PopisTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Popis = PopisTextBox.Text.ToString();
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
