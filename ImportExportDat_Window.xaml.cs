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
using System.Xml.Serialization;

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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře ImportExportDat_Window.xaml
   /// Třída slouži ke správě okna pro možnost importovat data ze souboru, nebo exportovat záznamy do souboru.
   /// </summary>
   public partial class ImportExportDat_Window : Window
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;


      /// <summary>
      /// Konstruktor třídy pro správu okenního formuláře pro import a export dat.
      /// </summary>
      /// <param name="ImportExport">1 - Import dat, 0 - Export dat</param>
      public ImportExportDat_Window(byte ImportExport)
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Nastavení barvy pozadí
         Background = Controller.BarvaPozadi;

         // Nastavení stylu okna dle zvoleného režimu funkce
         switch(ImportExport)
         {
            case 1: NastavStylImport();   break;
            case 0: NastavStylExport();   break;
            default: break;
         }
      }



      /// <summary>
      /// Nastavení okna do režimu pro import dat včetně vytvoření potřebných tlačítek.
      /// </summary>
      private void NastavStylImport()
      {
         // Nastavení okna
         Title = "Import dat";
         Icon = new BitmapImage(new Uri(Path.Combine(Validace.VratCestuSlozkyAplikace(), "Icons\\Disketa_e_sipkou.ico")));
         Height = 300;
         Width = 500;

         // Nastavení tlačítka pro možnost přidání nových záznamů
         PridatNoveZaznamyButton.Content = "Přidat nové záznamy";
         PridatNoveZaznamyButton.Width = 400;
         PridatNoveZaznamyButton.Height = 80;
         PridatNoveZaznamyButton.FontSize = 36;
         PridatNoveZaznamyButton.Margin = new Thickness(0, 0, 0, 120);
         PridatNoveZaznamyButton.Foreground = Brushes.DarkBlue;
         PridatNoveZaznamyButton.Background = Brushes.Pink;
         
         // Nastavení viditelnosti tlačítka a přidání události pro možnost ragovat na kliknutí na tlačítko
         PridatNoveZaznamyButton.Visibility = Visibility.Visible;
         PridatNoveZaznamyButton.Click += PridatNoveZaznamyButton_Click;

         // Nastavení tlačítka pro možnost aktualizace stávajících záznamů
         AktualizovatZaznamyButton.Content = "Přepsat data novými záznamy";
         AktualizovatZaznamyButton.Width = 400;
         AktualizovatZaznamyButton.Height = 80;
         AktualizovatZaznamyButton.FontSize = 28;
         AktualizovatZaznamyButton.Margin = new Thickness(0, 120, 0, 0);
         AktualizovatZaznamyButton.Foreground = Brushes.DarkBlue;
         AktualizovatZaznamyButton.Background = Brushes.DeepPink;

         // Nastavení viditelnosti tlačítka a přidání události pro možnost ragovat na kliknutí na tlačítko
         AktualizovatZaznamyButton.Visibility = Visibility.Visible;
         AktualizovatZaznamyButton.Click += AktualizovatZaznamyButton_Click;

         // Zrušení nepotřebných komponent
         UlozitButton.Visibility = Visibility.Collapsed;
         VyhedatButton.Visibility = Visibility.Collapsed;
         SeznamZaznamuProExportCanvas.Visibility = Visibility.Collapsed;
      }

      /// <summary>
      /// Nastavení okna do režimu pro export dat včetně vytvoření potřebných tlačítek.
      /// </summary>
      private void NastavStylExport()
      {
         Title = "Export dat";
         Icon = new BitmapImage(new Uri(Path.Combine(Validace.VratCestuSlozkyAplikace(), "Icons\\Disketa.png")));
         Height = 600;
         Width = 450;

         // Nastavení tlačítka pro Uložení vybraných záznamů do souboru
         UlozitButton.Content = "Exportovat data";
         UlozitButton.Width = 200;
         UlozitButton.Height = 50;
         UlozitButton.FontSize = 24;
         UlozitButton.Margin = new Thickness(200, 0, 20, 20);
         UlozitButton.VerticalAlignment = VerticalAlignment.Bottom;
         UlozitButton.HorizontalAlignment = HorizontalAlignment.Left;
         UlozitButton.Foreground = Brushes.DarkGreen;
         UlozitButton.Background = Brushes.Orange;

         // Nastavení viditelnosti tlačítka pro export dat a přidání události pro možnost ragovat na kliknutí na tlačítko
         UlozitButton.Visibility = Visibility.Visible;
         UlozitButton.Click += UlozitButton_Click;

         // Nastavení tlačítka pro vyhledání záznamů k exportu
         VyhedatButton.Content = "Vyhledat";
         VyhedatButton.Width = 150;
         VyhedatButton.Height = 50;
         VyhedatButton.FontSize = 24;
         VyhedatButton.Margin = new Thickness(20, 0, 20, 20);
         VyhedatButton.VerticalAlignment = VerticalAlignment.Bottom;
         VyhedatButton.HorizontalAlignment = HorizontalAlignment.Left;
         VyhedatButton.Foreground = Brushes.DarkGreen;
         VyhedatButton.Background = Brushes.CadetBlue;

         // Nastavení viditelnosti tlačítka pro vyhledání dat a přidání události pro možnost ragovat na kliknutí na tlačítko
         VyhedatButton.Visibility = Visibility.Visible;
         VyhedatButton.Click += VyhedatButton_Click;

         // Nastavení plátna pro vykreslení seznamu vybraných záznamů určených k exportu
         SeznamZaznamuProExportCanvas.Width = 300;
         SeznamZaznamuProExportCanvas.Height = 450;
         SeznamZaznamuProExportCanvas.Margin = new Thickness(20, 20, 0, 0);
         SeznamZaznamuProExportCanvas.VerticalAlignment = VerticalAlignment.Top;
         SeznamZaznamuProExportCanvas.HorizontalAlignment = HorizontalAlignment.Left;
         SeznamZaznamuProExportCanvas.Visibility = Visibility.Visible;
         SeznamZaznamuProExportCanvas.Background = Controller.BarvaPozadi;


         // Zrušení nepotřebných komponent
         PridatNoveZaznamyButton.Visibility = Visibility.Collapsed;
         AktualizovatZaznamyButton.Visibility = Visibility.Collapsed;

         // Vykreslení seznamu záznamů určených pro export
      }


      /// <summary>
      /// Obsluha tlačítko pro přepsání stávajících záznamů nově importovanými záznamy ze souboru.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void AktualizovatZaznamyButton_Click(object sender, RoutedEventArgs e)
      {
         // Zobrazení upozornění s uložením zvolené volby
         MessageBoxResult VybranaMoznost = MessageBox.Show("Původní záznamy budou smazány a budou nahrazeny novými! \n\t\tPřejete si pokračovat?", 
                                                            "Pozor!", MessageBoxButton.YesNo, MessageBoxImage.Question);

         // Reakce na stisknuté tlačítko
         switch (VybranaMoznost)
         {
            case MessageBoxResult.Yes:
               Controller.ImportujZaznamy(true);
               Close();
               break;
            case MessageBoxResult.No:
               break;
            default: break;
         }
      }

      /// <summary>
      /// Obsluha tlačítko pro přidání nových záznamů importovaných ze souboru do kolekce stávajících záznamů přihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PridatNoveZaznamyButton_Click(object sender, RoutedEventArgs e)
      {
         // Načtení záznamů ze souboru do kolekce záznamů přihlášeného uživatele
         Controller.ImportujZaznamy(false);

         // Zavření okna
         Close();
      }


      /// <summary>
      /// Obsluha tlačítka vyhledání požadovaných záznamů určených k exportu do souboru.
      /// </summary>
      /// <param name="sender">Zvolené tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VyhedatButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Otevření okna pro možnost vyhledat konkrétní záznamy
            Controller.OtevriOknoVyhledat();

            // Aktualizace vykreslení vybraných záznamů
            Controller.VykresliSeznamZaznamu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Obsluha tlačítka pro provedení exportu dat do souboru.
      /// </summary>
      /// <param name="sender">Zvolené tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UlozitButton_Click(object sender, RoutedEventArgs e)
      {
         // Export vybraných záznamů do souboru
         Controller.ExportujZaznamy();

         // Zavření okna
         Close();
      }

   }
}
