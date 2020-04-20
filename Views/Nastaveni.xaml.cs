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
   ///  Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře Nastaveni.xaml
   ///  Třída slouží k nastavení barvy pozadí všech oken aplikace a příznaku zobrazení poznámkového bloku v hlavním okně. 
   ///  Dle zvoleného zaškrtávacího tlačítka se nastaví barva pozadí do kontroléru aplikace pro možnost nastavení této barvy do všech oken aplikace. 
   ///  Dle zaškrtnutí pole pro zobrazení poznámky se nastaví příznakový bit přihlášeného uživatele a nastaví se zobrazení poznámového bloku včetně textu přihlášeného uživatele.
   /// </summary>
   public partial class Nastaveni : Window
   {
      /// <summary>
      /// Pomocná proměnná pro nastavení barvy pozadí v aplikaci.
      /// </summary>
      private Brush BarvaPozadi;

      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Konstruktor třídy pro správu okenního formuláře Nastaveni.
      /// </summary>
      public Nastaveni()
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Přijetí instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Úvodní nastavení okna
         UvodniNastaveni();
      }

      /// <summary>
      /// Úvodní nastavení okna pro nastavení.
      /// </summary>
      private void UvodniNastaveni()
      {
         // Nastavení zaškrtávacího pole dle uloženého nastavení uživatele
         ZobrazitPoznamkuCheckBox.IsChecked = Controller.VratZobrazeniPoznamky() == 1 ? true : false;

         // Nastavení barvy pozadí dle zvolené volby uživatele
         BarvaPozadi = Controller.BarvaPozadi;
         Background = BarvaPozadi;

         // Nastavení zaškrtávacích políček při otevření okna
         UrciNastavenouBarvuPozadi();
      }

      /// <summary>
      /// Identifikace nastavené barvy pozadí.
      /// </summary>
      private void UrciNastavenouBarvuPozadi()
      {
         PrvniBarvaRadioButton.IsChecked     =     BarvaPozadi == PrvniBarva.Fill      ? true : false;
         DruhaBarvaRadioButton.IsChecked     =     BarvaPozadi == DruhaBarva.Fill      ? true : false;
         TretiBarvaRadioButton.IsChecked     =     BarvaPozadi == TretiBarva.Fill      ? true : false;
         CtvrtaBarvaRadioButton.IsChecked    =     BarvaPozadi == CtvrtaBarva.Fill     ? true : false;
         PataBarvaRadioButton.IsChecked      =     BarvaPozadi == PataBarva.Fill       ? true : false;
         SestaBarvaRadioButton.IsChecked     =     BarvaPozadi == SestaBarva.Fill      ? true : false;
         SedmaBarvaRadioButton.IsChecked     =     BarvaPozadi == SedmaBarva.Fill      ? true : false;
         OsmaBarvaRadioButton.IsChecked      =     BarvaPozadi == OsmaBarva.Fill       ? true : false;
         DevataBarvaRadioButton.IsChecked    =     BarvaPozadi == DevataBarva.Fill     ? true : false;
         DesataBarvaRadioButton.IsChecked    =     BarvaPozadi == DesataBarva.Fill     ? true : false;
         JedenactaBarvaRadioButton.IsChecked =     BarvaPozadi == JedenactaBarva.Fill  ? true : false;
         DvanactaBarvaRadioButton.IsChecked  =     BarvaPozadi == DvanactaBarva.Fill   ? true : false;
      }

      /// <summary>
      /// Nastavení vbrané barvy pozadí do interní proměnné.
      /// </summary>
      /// <param name="NazevRadioButtonu">Název zvoleného RadioButtonu</param>
      private void NastavBarvu(string NazevRadioButtonu)
      {
         // Určení zvolené barvy na základě názvu zvoleného zaškrtávacího políčka
         BarvaPozadi = VratBarvu(NazevRadioButtonu);

         // Nastavení vybrané barvy na pozadí
         Background = BarvaPozadi;
      }

      /// <summary>
      /// Metoda pro zjištění barvy na základě jména zvoleného zaškrtávacího políčka.
      /// </summary>
      /// <param name="OznaceniButtonu">Jméno RadioButtonu</param>
      /// <returns>Barva ukázkového pole</returns>
      private Brush VratBarvu(string OznaceniButtonu)
      {
         // Vytvoření proměnné s nastavením defaultní barvy
         Brush Barva = Brushes.LightBlue;

         // Určení barvy na základě názvu zvoleného RadioButtonu
         if (OznaceniButtonu.Contains("Prvni")) { Barva = PrvniBarva.Fill; }
         else if (OznaceniButtonu.Contains("Druha")) { Barva = DruhaBarva.Fill; }
         else if (OznaceniButtonu.Contains("Treti")) { Barva = TretiBarva.Fill; }
         else if (OznaceniButtonu.Contains("Ctvrta")) { Barva = CtvrtaBarva.Fill; }
         else if (OznaceniButtonu.Contains("Pata")) { Barva = PataBarva.Fill; }
         else if (OznaceniButtonu.Contains("Sesta")) { Barva = SestaBarva.Fill; }
         else if (OznaceniButtonu.Contains("Sedma")) { Barva = SedmaBarva.Fill; }
         else if (OznaceniButtonu.Contains("Osma")) { Barva = OsmaBarva.Fill; }
         else if (OznaceniButtonu.Contains("Devata")) { Barva = DevataBarva.Fill; }
         else if (OznaceniButtonu.Contains("Desata")) { Barva = DesataBarva.Fill; }
         else if (OznaceniButtonu.Contains("Jedenacta")) { Barva = JedenactaBarva.Fill; }
         else if (OznaceniButtonu.Contains("Dvanacta")) { Barva = DvanactaBarva.Fill; }

         // Návratová hodnota - barva dle názvu zvoleného obdélníku
         return Barva;
      }


      /// <summary>
      /// Změna nastavení poznámkového bloku při změně zaškrtávacího pole.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událot</param>
      private void ZobrazitPoznamkuCheckBox_Checked(object sender, RoutedEventArgs e)
      {
         // Nastavení zobrazení poznámkového bloku v hlavním okně
         Controller.NastavZobrazeniPoznamky(1);
      }

      /// <summary>
      /// Změna nastavení poznámkového bloku při změně zaškrtávacího pole.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událot</param>
      private void ZobrazitPoznamkuCheckBox_Unchecked(object sender, RoutedEventArgs e)
      {
         // Zrušení zobrazení poznámkového bloku v hlavním okně
         Controller.NastavZobrazeniPoznamky(0);
      }

      /// <summary>
      /// Obsluha události při změně zaškrtávacího políčka pro výběr barvy pozadí.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void BarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         // Přetypování objektu zpět na původní datový typ (RadioButton)
         RadioButton button = sender as RadioButton;

         // Volání metody zjištění vybrané barvy na základě jména zvoleného zaškrtávácího políčka a nastavení této barvy do interní proměnné
         NastavBarvu(button.Name);
      }

      /// <summary>
      /// Událost vyvolána při zavírání okna pro uložení provedených změn.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         Controller.BarvaPozadi = BarvaPozadi;
      }

   }
}
