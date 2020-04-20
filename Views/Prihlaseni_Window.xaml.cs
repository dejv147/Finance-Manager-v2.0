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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře Prihlaseni_Window.xaml
   /// Třída obsluhuje okenní formulář pro přihlášení uživatele.
   /// </summary>
   public partial class Prihlaseni_Window : Window
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Pomocná proměnná pro uchování zadaného jména uživatele
      /// </summary>
      private string Jmeno;

      /// <summary>
      /// Pomocná proměnná pro uchování zadaného hesla uživatele
      /// </summary>
      private string Heslo;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;


      /// <summary>
      /// Barva pro nastavení tlačítka. 
      /// Barva vytvořena dle bitových hodnot jednotlivých barevných složek.
      /// </summary>
      private SolidColorBrush MyColorTyrkys = new SolidColorBrush(Color.FromArgb(255, 90, 216, 188));

      /// <summary>
      /// Barva pro nastavení tlačítka. 
      /// Barva vytvořena dle bitových hodnot jednotlivých barevných složek.
      /// </summary>
      private SolidColorBrush MyClolorFialovoModra = new SolidColorBrush(Color.FromArgb(255, 50, 10, 165));




      /// <summary>
      /// Konstruktor třídy pro správu okenního formuláře Prihlaseni
      /// </summary>
      public Prihlaseni_Window()
      {
         // Nastavení instance kontroléru aplikace
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Inicializace okenního formuláře
         InitializeComponent();
         NastavTlacitka();
         Jmeno = "";        
         Heslo = "";

         // Nastavení hodnoty příznakového bytu pro defaultní nastavení
         ZavrenoBezUlozeni = 1;                                      

         // Nastavení barvy tlačítka při úvodním zobrazení
         ZobrazHesloButton.Background = MyColorTyrkys;               

         // Skrytí hesla při úvodním zobrazení
         SkrytHeslo();   
      }



      /// <summary>
      /// Metoda pro nastavení designu a vlastností tlačítek zobrazovaných v okenním formuláři.
      /// </summary>
      private void NastavTlacitka()
      {
         // Vytvoření instance třídy pro práci s grafickými prvky
         GrafickePrvky Grafika = new GrafickePrvky();

         // Nastavení tlačítka Přihlásit se
         Grafika.NastavTlacitkoPRIHLASIT(PrihlaseniButton);
         PrihlaseniButton.IsDefault = true;
         PrihlaseniButton.Click += PrihlaseniButton_Click;

         // Nastavení tlačítka Zobrazit heslo
         Grafika.NastavTlacitkoZOBRAZITHESLO(ZobrazHesloButton);
         ZobrazHesloButton.Background = MyColorTyrkys;
         ZobrazHesloButton.Click += ZobrazHesloButton_Click;

         // Nastavení tlačítka Registrovat se
         Grafika.NastavTlacitkoREGISTROVAT(RegistraceButton);
         RegistraceButton.Click += RegistraceButton_Click;
      }

     
      /// <summary>
      /// Pomocná metoda pro zobrazení zadaného hesla.
      /// Metoda skryje pole pro zadání hesla a překryje jej textovým polem zobrazující zadané heslo.
      /// </summary>
      private void ZobrazHeslo()
      {
         HesloUzivateleTextBlock.Width = HesloUzivatelePasswordBox.Width;  // Nastavení stejné šířky pro optické překrytí bloku
         HesloUzivateleTextBlock.Text = Heslo;                             // Zobrazení hesla do textového bloku
         HesloUzivateleTextBlock.Visibility = Visibility.Visible;          // Nastavení viditelnosti bloku pro zobrazení hesla
         HesloUzivatelePasswordBox.Visibility = Visibility.Hidden;         // Skrytí pole pro zadávání hesla
      }

      /// <summary>
      /// Pomocná metoda pro skrytí textového pole zobrazující zadané heslo.
      /// Metoda skryje zobrazené heslo a obnový pole pro zadání hesla.
      /// </summary>
      private void SkrytHeslo()
      {
         HesloUzivateleTextBlock.Text = "";                                // Smazání hesla ze zobrazovacího pole
         HesloUzivatelePasswordBox.Visibility = Visibility.Visible;        // Nastavení viditelnosti bloku pro zadávání hesla
         HesloUzivateleTextBlock.Visibility = Visibility.Hidden;           // Skrytí textového bloku pro zobrazení hesla
      }



      /// <summary>
      /// Uložení zadaného hesla do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZadaniHesla(object sender, RoutedEventArgs e)
      {
         // Uložení textu napsaného v PasswordBox do proměnné
         Heslo = HesloUzivatelePasswordBox.Password.ToString();
      }

      /// <summary>
      /// Uložení zadaného jména do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZadaniJmena(object sender, TextChangedEventArgs e)
      {
         // Uložení textu napsaného v TextBox do proměnné
         Jmeno = JmenoUzivateleTextBox.Text.ToString();
      }

      /// <summary>
      /// Metoda obsluhující kliknutí na tlačítko pro Zobrazení/Skrytí hesla.
      /// Metoda nejprve zkontroluje zda bylo heslo zadáno a pokud ano, vyvolá pomocnou metodu pro zobrazení zadaného hesla, nebo pro zobrazení pole pro zadání hesla.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZobrazHesloButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // V případě že není zadání heslo, vyvolá se varovné okno s upozorněním
            if (!(Heslo.Length > 0))
               throw new ArgumentException("Zadejte heslo!");

            // Volání pomocné metody podle toho, jaké tlačítko je zobrazeno v okně aplikace
            if (ZobrazHesloButton.Content.ToString() == "Zobrazit heslo")
            {
               ZobrazHeslo();
               ZobrazHesloButton.Content = "Skrýt heslo";            // Nastavení kontextu na tlačítku
               ZobrazHesloButton.Background = MyClolorFialovoModra;  // Nastavení barvy tlačítka
            }
            else
            {
               SkrytHeslo();
               ZobrazHesloButton.Content = "Zobrazit heslo";         // Nastavení kontextu na tlačítku
               ZobrazHesloButton.Background = MyColorTyrkys;         // Nastavení barvy tlačítka
            }

         }
         catch (Exception ex)    // V případě vyjímky se objeví varovné okno
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Otevření okna pro registraci nového uživatele.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void RegistraceButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Otevření okna pro registraci nového uživatele včetně kontroly zda byl uživatel přihlášen do aplikace
            if (Controller.obsluhyUdalosti.PrihlasitWindow_RegistraceButtonClick())
               ZavrenoBezUlozeni = 0;

            // Zavření přihlašovacího okna
            Close();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda obsluhující stisk tlačítka pro přihlášení.
      /// Provede kontrolu zda daný uživatel existuje a při splnění podmínek provede přihlášení uživatele do systému.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrihlaseniButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Přihlášení uživatele do aplikace
            if (Controller.obsluhyUdalosti.PrihlasitWindow_PrihlaseniButtonClick(Jmeno, Heslo))
            {
               // Zavření přihlašovacího okna
               ZavrenoBezUlozeni = 0;
               Close();
            }

            // Pokud se přihlášení uživatele nezdaří, okno se nezavře
            else
               return;
               
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Zobrazení varovné zprávy při zavírání okna.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         // Pokud je okno zavřeno křížkem (bez přihlášení) zobrazí se varovné okno
         if (ZavrenoBezUlozeni == 1)
         {
            MessageBoxResult VybranaMoznost = MessageBox.Show("Nejste přihlášen! \nChcete vstoupit anonymě?", "Pozor", MessageBoxButton.YesNo, MessageBoxImage.Question);

            switch (VybranaMoznost)
            {
               case MessageBoxResult.Yes:
                  Controller.OdhlasUzivatele();
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
