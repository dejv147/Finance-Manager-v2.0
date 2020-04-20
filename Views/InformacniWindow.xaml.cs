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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře InformacniWindow.xaml
   /// Třída slouží pro otevření okna s výpisem základních informací o aplikaci. Výpis obsahuje stručný popis všech funkcí a návod pro obsluhu aplikace.
   /// </summary>
   public partial class InformacniWindow : Window
   {
      /// <summary>
      /// Textový návod k aplikaci
      /// </summary>
      private string Popis;

      /// <summary>
      /// Konstruktor třídy pro správu okna Info
      /// </summary>
      public InformacniWindow()
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Inicializace textového řetězce
         Popis = "";

         // Nastavení barvy pozadí okna
         Background = SpravceFinanciController.VratInstanciControlleru().BarvaPozadi;

         // Vypsání textového řetězce do okna aplikace
         VypisNavod();
      }

      /// <summary>
      /// Metoda pro vytvoření textového návodu k aplikaci a zobrazení textového řetězce do informačního okna.
      /// </summary>
      private void VypisNavod()
      {
         Nazev.Text = "Aplikace pro správu financí konkrétního uživatele.";

         Popis = "Každý registrovaný uživatel má vlastní kolekci dat, nelze tedy zobrazovat data jiného uživatele.\n";
         Popis += "V hlavním okně aplikace jsou zobrazeny poslední přidané záznamy včetně měsíčního přehledu.\n\n";

         Popis += "V levé části je k dispozici rozbalovací MENU pro práci se záznamy v aplikaci pro přihlášeného uživatele.\n";
         Popis += "V pravé části je k dispozici rozbalovací MENU pro správu funkcí uživatele. \n";
         Popis += "Je zde k dispozici jednoduchá kalulačka, možnost načtení a uložení dat do souboru, \nmožnost změnit barvu pozadí, zobrazení poznámkového bloku pro uživatele \na tlačítko pro odhlášení aktuálního uživatele.\n\n";

         Popis += "Aplikace zpracovává a uchovává kolekci jednotlivých záznamů, kde každý záznam představuje 1 finanční transakci.\n";
         Popis += "Uživatel má možnost záznamy přidávat (vytvářet), upravovat a odebírat (smazat). \n\n";

         Popis += "Každý záznam obsahuje základní údaje s možností přidat poznámku ke každému záznamu, \nvčetně možnosti přidat seznam položek k danému záznamu (účtence).\n";
         Popis += "Je možné zobrazit vybrané záznamy v samostatném seznamu, \ndále je možnost vyhledat určité záznamy dle zadaných filtrů vyhledávání.\n\n";

         Popis += "Je možné exportovat vybrané záznamy a uložit je do textového souboru (*.txt), \nseparovaného textového souboru (*.csv), nebo do datového souboru (*.xml).\n";
         Popis += "Záznamy uložené ve formátu XML je možné importovat do aplikace, \nkde je možnost zvolit přidání novách záznamů mezi existující, nebo nahradit stávající záznamy novými.\n\n";

         Popis += "Dále možnost zobrazit statisticky vyhodnocené údaje v podobě grafů. \nJe k dispozici sloupcový graf shrnující příjmy i výdaje v určitých časových intervalech.\n";
         Popis += "Hodnoty časové osy jsou nastavitelné dle požadovaného zobrazení.\n\n";
         Popis += "Druhým grafem je zobrazení příjmů a výdajů v zadaném časovém intervalu rozděleném na jednotlivé kategorie.\n\n";
         Popis += "V obou grafech je možné se pohybovat v rámci časové osy,\n nebo mezi jednotlivými kategoriemi pomocí červených šipek v právém spodním rohu grafu.\n";
         Popis += "Mezi jednotlivými grafy se lze přepínat pomocí záložek umístěných v horní části grafů.\n\n";


         // Vypsání textového řetězce do informační okna 
         NavodLabel.Content = Popis;
      }

   }
}
