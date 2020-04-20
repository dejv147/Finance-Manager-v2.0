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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře Registrace_Window.xaml
   /// Třída obsluhuje okenní formulář pro registraci nového uživatele.
   /// </summary>
   public partial class Registrace_Window : Window
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;

      /// <summary>
      /// Pomocná proměnná pro uchování zadaného jména uživatele
      /// </summary>
      private string Jmeno;

      /// <summary>
      /// Pomocná proměnná pro uchování zadaného hesla uživatele
      /// </summary>
      private string Heslo;



      /// <summary>
      /// Konstruktor třídy pro správu okenního formuláře Registrace
      /// </summary>
      public Registrace_Window()
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Nastavení instance kontroléru aplikace
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Skrytí ukazatele síly hesla
         NastavUkazatelHesla(false);

         // Nastavení tlačítka
         NastavTlacitka();
      }



      /// <summary>
      /// Metoda pro nastavení designu a vlastností tlačítek zobrazovaných v okenním formuláři.
      /// </summary>
      private void NastavTlacitka()
      {
         // Vytvoření instance třídy pro práci s grafickými prvky
         GrafickePrvky Grafika = new GrafickePrvky();

         // Nastavení tlačítka Registrovat se (styl stejný jako tlačítko Přihlásit se)
         Grafika.NastavTlacitkoPRIHLASIT(RegistraceButton);
         RegistraceButton.Content = "Registrovat se";
         RegistraceButton.Click += RegistraceButton_Click;
      }

      /// <summary>
      /// Metoda pro nastavení ukazatele síly hesla.
      /// Provede kontrolu bezpečnosti hesla a dle výsledků vykreslí ukazatel síly hesla.
      /// </summary>
      /// <param name="Zobrazit">TRUE - ukazatel hesla bude vykreslen, FALSE - ukazatel bude skryt</param>
      private void NastavUkazatelHesla(bool Zobrazit)
      {
         // Ukazatel hesla je zobrazen
         if (Zobrazit == true)
         {
            // Zobrazení ukazatele hesla
            UkazatelSilyHeslaStackPanel.Visibility = Visibility.Visible;

            try
            {
               // Vykreslení ukazatele na základě kontroly bezpečnosti hesla
               Controller.ZkontrolujSiluHesla(Heslo, UkazatelSilyHeslaCanvas);
            }
            catch (Exception ex)
            {
               // Zobrazení varovného okna pokud nastane vyjímka (pouze vyjímka infrmující o nedostatečné bezepčnosti hesla nebude zobrazena)
               if (!(ex.Message.Contains("Zadané heslo nesplňuje bezpečnostní požadavky! ")))
                  MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
         }

         // Ukazatel hesla není zobrazen
         else
         {
            // Skrytí ukazatele hesla
            UkazatelSilyHeslaStackPanel.Visibility = Visibility.Hidden;
         }
      }


      /// <summary>
      /// Uložení zadaného hesla do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZadaniHesla(object sender, RoutedEventArgs e)
      {
         // Uložení textu napsaného v PasswordBox do proměnné
         Heslo = HesloUzivatelePasswordBox.Password.ToString();

         // Zobrazení ukazatele síly zadaného hesla
         NastavUkazatelHesla(true);
      }

      /// <summary>
      /// Uložení zadaného jména do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZadaniJmena(object sender, TextChangedEventArgs e)
      {
         // Uložení textu napsaného v TextBox do proměnné
         Jmeno = JmenoUzivateleTextBox.Text.ToString();
      }

      /// <summary>
      /// Obsluhaudálosti při kliknutí na tlačítko pro registraci nového uživatele.
      /// Metoda registruje nového uživatele do aplikace včetně automatického přihlášení a zavře okno.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void RegistraceButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Registrace nového uživatele do aplikace včetně automatického přihlášení
            if (Controller.obsluhyUdalosti.RegistraceWindow_RegistraceButtonClick(Jmeno, Heslo)) 
            {
               // Zavření registračního okna
               ZavrenoBezUlozeni = 0;
               Close();
            }

            // Pokud se registrace uživatele nezdaří, okno se nezavře
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
         // Pokud je okno zavřeno křížkem (bez uložení dat) zobrazí se varovné okno
         if (ZavrenoBezUlozeni == 1)
         {
            MessageBoxResult VybranaMoznost = MessageBox.Show("Provedené změny nebudou uloženy! \nChcete pokračovat?", 
                                                               "Pozor", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
