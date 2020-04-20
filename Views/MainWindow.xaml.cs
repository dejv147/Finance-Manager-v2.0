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
using System.Windows.Navigation;
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
   ///  Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře MainWindow.xaml
   ///  Třída slouží pro správu hlavního okna aplikace, které slouží jako výchozí při spuštění aplikace a jsou z něj otevírána ostatní okna aplikace.
   /// </summary>
   public partial class MainWindow : Window
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Instance třídy pro grafickou reprezentaci záznamů a správu graficky zobrazeného seznamu záznamů.
      /// </summary>
      private GrafickeZaznamy GrafickyZpracovaneZaznamy;
    


      /// <summary>
      /// Konstruktor třídy pro správu hlavního okna aplikace.
      /// Při úvodní inicializaci hlavního okna je vytvořena instance kontroléru aplikace, který dále spravuje funkci celé aplikace včetně tohoto okna.
      /// </summary>
      public MainWindow()
      {
         // Inicializace hlavního okna
         InitializeComponent();

         // Inicializace instance třídy pro správu grafického zobrazení seznamu záznamů
         GrafickyZpracovaneZaznamy = new GrafickeZaznamy(UvodniSeznamZaznamuCanvas, InformacniBublinaCanvas);

         // Vytvoření kontroléru pro řízení aplikace
         Controller = SpravceFinanciController.VratInstanciControlleru();
         Controller.UvodniNastaveniKontroleru(this);

         // Úvodní nastavení zobrazení při spuštění aplikace
         Controller.NastavUvodniZobrazeni();

         // Aktualizace vykreslení hlavního okna při první spuštění po přihlášení uživatele
         if (Controller.VratJmenoPrihlasenehoUzivatele().Length > 0)
            AktualizujUvodniObrazovku();
      }



      /// <summary>
      /// Aktualizace vykreslení hlavního okna aplikace (obnova úvodního zobrazení).
      /// </summary>
      public void AktualizujUvodniObrazovku()
      {
         // Pokud není uživatel přihlášen, aktualizace vykreslení se neprovede
         if (!(Controller.VratJmenoPrihlasenehoUzivatele().Length > 0))
            return;

         // Nastavení poznámkového bloku
         Controller.NastavPoznamkovyBlok();

         // Zobrazení seznamu záznamů 
         ZobrazSeznamZaznamu();

         // Zobrazení informačního bloku 
         ZobrazBlokInformaciMesice();

         // Vykreslení postranních MENU se skrytými ovládacími prvky
         Controller.VykresliPostranniMenuHlavnihoOkna();
      }


      /// <summary>
      /// Vykreslení seznamu záznamů aktuálního měsíce do úvodního okna
      /// </summary>
      private void ZobrazSeznamZaznamu()
      {
         // Zrušení označení vybraného záznamu
         Controller.ZrusOznaceniZaznamu();

         // Vykreslení seznamu záznamů z aktuálního měsíce
         Controller.VykresliSeznamZaznamuAktualnihoMesiceDoHlavnihoOkna(GrafickyZpracovaneZaznamy);
      }

      /// <summary>
      /// Zobrazení informačního bloku o zobrazených záznamech
      /// </summary>
      private void ZobrazBlokInformaciMesice()
      {
         // Vykrewslení informačního bloku shrnující základní informace o aktuálním měsíci
         Controller.VykresliInformacniBlokMesicnihoPrehledu(PrehledCanvas);
      }


      /// <summary>
      /// Metoda pro zavření hlavního okna aplikace
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HlavniOknoWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         // Pokud je uživatel přihlášen, provede se jeho odhlášení před ukončením aplikace
         if (Controller.VratJmenoPrihlasenehoUzivatele().Length > 0)
            Controller.UkonceniAplikace();
      }

   }
}
