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
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře Statistika_Window.xaml
   /// Třída slouží k vytvoření instance třídy spravující zpracování dat a vykreslení dat do okna. 
   /// V této třídě probíhá úvodní nastavení okna a předání plátna pro vykreslení instanci třídy pro statistické zpracování dat a vykreslení grafů.
   /// </summary>
   public partial class Statistika_Window : Window
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;


      /// <summary>
      /// Konstruktor třídy pro správu okenního formuláře spravující zobrazení statistických grafů.
      /// </summary>
      public Statistika_Window()
      {
         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Inicializace okenního formuláře
         InitializeComponent();

         // Úvodní nastavení okna
         Title = "Statistika";
         Height = 750;
         Width = 1250;
         Icon = new BitmapImage(new Uri(System.IO.Path.Combine(Validace.VratCestuSlozkyAplikace(), "Icons\\Statistika.ico")));

         // Provedení úvodního nastavení zobrazovacích prvků
         UvodniNastaveniGrafu();
      }


      /// <summary>
      /// Nastavení barvy pozadí všech zobrazovacích prvků.
      /// </summary>
      public void UvodniNastaveniGrafu()
      {
         // Nastavení barvy zobrazovaných kontejnerů pro grafy
         StatistickeGrafyTabControl.Background = Controller.BarvaPozadi;

         // Nastavení barvy pozadí všech částí grafů
         CasovyPrehledCanvasGraf.Background = Controller.BarvaPozadi;
         CasovyPrehledCanvasOvladaciPrvky.Background = Controller.BarvaPozadi;
         CasovyPrehledCanvasInfo.Background = Controller.BarvaPozadi;
         PrehledKategoriiCanvasGraf.Background = Controller.BarvaPozadi;
         PrehledKategoriiCanvasOvladaciPrvky.Background = Controller.BarvaPozadi;
         PrehledKategoriiCanvasInfo.Background = Controller.BarvaPozadi;
      }

   }
}
