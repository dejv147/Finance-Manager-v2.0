using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

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
   /// Třída pro statistické zpracování a vyhodnocení zadaných dat. 
   /// Třída slouží pro zpracování dat a grafické zobrazení zpracovaných dat. 
   /// Dále slouží k obsluze okenního forluláře (plátna pro vykreslení statistických grafů a ovládacích prvků).
   /// </summary>
   public class Statistika
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;


      /// <summary>
      /// Blok uchovávající informace o skupině záznamů určených pro sjednocení do 1 sloupce grafu.
      /// </summary>
      public struct BlokZaznamu
      {
         public DateTime DatumPocatecni;
         public DateTime DatumKoncove;
         public string Nazev;
         public string Popisek;
         public int PocetZaznamu;
         public double CelkovaHodnota;
         public KategoriePrijemVydaj kategorie;
         public Brush Barva;
      }

      /// <summary>
      /// Uchování dat pro možnost zpracování a zobrazení údajů v grafu.
      /// </summary>
      public ObservableCollection<Zaznam> KolekceZaznamu { get; private set; }

      /// <summary>
      /// Plátno pro vykreslení samotného grafu.
      /// </summary>
      public Canvas PlatnoGrafuCanvas { get; private set; }

      /// <summary>
      /// Plátno pro vykreslení ovládacích prvků pro ovládání grafu.
      /// </summary>
      public Canvas PlatnoOvladacichPrvkuCanvas { get; private set; }

      /// <summary>
      /// Plátno pro vykreslení informačního okna, nebo oblasti pod grafem.
      /// </summary>
      public Canvas PlatnoInfoOblastiCanvas { get; private set; }

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaModra = Brushes.Blue;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaCervena = Brushes.Red;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaCerna = Brushes.Black;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaZluta = Brushes.Yellow;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaZelena = Brushes.Green;

      
      /// <summary>
      /// Textová reprezentace názvu grafu.
      /// </summary>
      public string Nazev { get; private set; }

      /// <summary>
      /// Grafická reprezentace názvu grafu.
      /// </summary>
      public TextBlock GrafickyNazev { get; private set; }

      /// <summary>
      /// Graficky reprezentovaná legenda grafu.
      /// </summary>
      public StackPanel Legenda { get; private set; }

      /// <summary>
      /// Datum, od kterého se zpracovávají data pro vykreslení grafu
      /// </summary>
      private DateTime PocatecniDatum;

      /// <summary>
      /// Datum, do kterého se zpracovávají data pro vykreslení grafu
      /// </summary>
      private DateTime KoncoveDatum;

      /// <summary>
      /// Uložení vybraného měsíce pro výběr dat k zobrazení do grafu Časový přehled.
      /// </summary>
      private int VybranyMesic;

      /// <summary>
      /// Uložení vybraného roku pro výběr dat k zobrazení do grafu Časový přehled.
      /// </summary>
      private int VybranyRok;

      /// <summary>
      /// Uložení vybraného koncového roku pro výběr dat k zobrazení do grafu Časový přehled v případě zobrazení roků.
      /// </summary>
      private int VybranyKoncovyRok;

      /// <summary>
      /// Výčtový typ pro výběr různých druhů zobrazení dat v časovém přehledu.
      /// </summary>
      public enum ZvoleneZobrazeniPrehledu { Dny, Tydny, Mesice, Roky }

      /// <summary>
      /// Proměnná pro práci s výčtovým typem pro určení požadovaného zobrazení grafu Časový přehled.
      /// </summary>
      private ZvoleneZobrazeniPrehledu ZobrazeniPrehledu;

      /// <summary>
      /// Pomocná proměnná pro výchozí souřadnici vykreslování popisu osy X grafu Kategorie
      /// </summary>
      private int SouradniceX_PrvniSloupec;

      /// <summary>
      /// Pomocná proměnná pro uložení mezery mezi bloky v grafu Kategorie
      /// </summary>
      private int OdstupHodnotX;

      /// <summary>
      /// Počet řádků (hodnot na ose Y) pro 1 graf.
      /// </summary>
      private const int PocetHodnotOsyY = 5;

      /// <summary>
      /// Počet prvků na ose X při vykreslení 1 stránky grafu.
      /// </summary>
      private int PocetHodnotOsyX;

      /// <summary>
      /// Pomocná proměnná pro uložení maximální hodnoty prvků v grafu pro určení hodnot na ose Y.
      /// </summary>
      private double MaximalniHodnota;

      /// <summary>
      /// Kolekce uchovávající hodnoty na ose Y.
      /// </summary>
      private List<int> HodnotyOsyY;

      /// <summary>
      /// Interní proměnná pro identifikaci vykreslované stránky grafu.
      /// </summary>
      private int CisloStrankyGrafu;

      /// <summary>
      /// Maximální počet stránek grafu v závislosti na počtu bloků určených k vykreslení.
      /// </summary>
      private int MaximalniPocetStranGrafu;

      
      /// <summary>
      /// Kolekce bloků reprezentujících všechny záznamy pro vykreslení kategorie Příjmy.
      /// </summary>
      public ObservableCollection<BlokZaznamu> KolekceBlokuZaznamu_Prijmy { get; private set; }

      /// <summary>
      /// Kolekce bloků reprezentujících všechny záznamy pro vykreslení kategorie Výdaje.
      /// </summary>
      public ObservableCollection<BlokZaznamu> KolekceBlokuZaznamu_Vydaje { get; private set; }

      /// <summary>
      /// Kolekce bloků zobrazovaných na 1 stránce grafu pro vykreslení výdajů.
      /// </summary>
      public ObservableCollection<BlokZaznamu> ZobrazeneBlokyVydaju { get; private set; }

      /// <summary>
      /// Kolekce bloků zobrazovaných na 1 stránce grafu pro vykreslení příjmů.
      /// </summary>
      public ObservableCollection<BlokZaznamu> ZobrazeneBlokyPrijmu { get; private set; }

      /// <summary>
      /// Popisky osy X určené k vykreslení pod grafem.
      /// </summary>
      public ObservableCollection<string> PopiskyOsyX { get; private set; }


      /// <summary>
      /// Proměnná informující zda je vykresleno informační okno.
      /// </summary>
      private bool InfoVykresleno;

      /// <summary>
      /// Proměnná informující zda jsou vykresleny informativní věty o celkových příjmech nebo výdajích.
      /// </summary>
      private bool InformativniVetyVykresleny;

      /// <summary>
      /// Proměnná pro určení zda budou do grafu vykresleny sloupce představující záznamy v kategorii Příjmy.
      /// </summary>
      private bool VykreslitPrijmy;

      /// <summary>
      /// Proměnná pro určení zda budou do grafu vykresleny sloupce představující záznamy v kategorii Výdaje.
      /// </summary>
      private bool VykreslitVydaje;

      /// <summary>
      /// Výčtový typ pro identifikaci grafu který je vykreslován.
      /// </summary>
      private enum TypGrafu { CasovyPrehled, Kategorie }

      /// <summary>
      /// Typ grafu, který je vykreslován.
      /// </summary>
      private TypGrafu TypVykreslovanehoGrafu;





      /// <summary>
      /// Konstruktor třídy pro zpracování a vykreslení dat. 
      /// Třída statisticky zpracuje záznamy a vykreslí potřebné grafy shrnující údaje o zpracovaných záznamech.
      /// </summary>
      /// <param name="OblastGrafu">Plátno pro vykreslení samotného grafu včetně osy Y</param>
      /// <param name="OvladaciPrvky">Plátno pro vykreslení ovládacích prvků pro možnost měnit nastavení a zobrazení grafu</param>
      /// <param name="OblastPodGrafem">Plátno pro vykreslení oblasti pod grafem, tedy popis osy X. Dále pro vykresení informační bubliny a šipek pro přepínání stránek grafu.</param>
      public Statistika(Canvas OblastGrafu, Canvas OvladaciPrvky, Canvas OblastPodGrafem)
      {
         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Uložení pláten pro vykreslení částí grafu
         PlatnoGrafuCanvas = OblastGrafu;
         PlatnoOvladacichPrvkuCanvas = OvladaciPrvky;
         PlatnoInfoOblastiCanvas = OblastPodGrafem;

         // Inicializace pomocných proměnných pro zpracování dat a vykreslení grafu
         Nazev = "";
         MaximalniHodnota = 0;
         MaximalniPocetStranGrafu = 1;
         CisloStrankyGrafu = 0;
         SouradniceX_PrvniSloupec = 0;
         OdstupHodnotX = 0;
         PocetHodnotOsyX = 6;
         HodnotyOsyY = new List<int>();
         PopiskyOsyX = new ObservableCollection<string>();
         KolekceBlokuZaznamu_Prijmy = new ObservableCollection<BlokZaznamu>();
         KolekceBlokuZaznamu_Vydaje = new ObservableCollection<BlokZaznamu>();
         ZobrazeneBlokyPrijmu = new ObservableCollection<BlokZaznamu>();
         ZobrazeneBlokyVydaju = new ObservableCollection<BlokZaznamu>();
         VykreslitPrijmy = true;
         VykreslitVydaje = true;
         InfoVykresleno = false;
      }




      /// <summary>
      /// Úvodní nastavení třídy pro vykreslování grafu Časový přehled včetně úvodního vykreslení grafu.
      /// </summary>
      /// <param name="KolekceZaznamu">Kolekce dat pro statistické zpracování a vykreslení do grafu</param>
      public void UvodniNastaveniCasovehoGrafu(ObservableCollection<Zaznam> KolekceZaznamu)
      {
         // Uložení dat určených ke zpracování do interní proměnné
         this.KolekceZaznamu = KolekceZaznamu;

         // Nastavení vykreslování grafu na první stránku
         CisloStrankyGrafu = 0;

         // Výchozí nastavení zvoleného zobrazení
         TypVykreslovanehoGrafu = TypGrafu.CasovyPrehled;
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Dny;
         PocetHodnotOsyX = 11;
         VykreslitPrijmy = true;
         VykreslitVydaje = true;

         // Nastavení počátečního výběru pro zobrazení
         VybranyMesic = DateTime.Now.Month;
         VybranyRok = DateTime.Now.Year;

         // Volání pomocných metod pro vytvoření úvodního zobrazení grafu
         VytvorNazevGrafu("Přehled financí");
         VytvorLegenduGrafu();
         VykresliOvladaciPrvky();
         VytvorBlokyGrafu();
         AktualizujVykresleniGrafu();
      }

      /// <summary>
      /// Úvodní nastavení třídy pro vykreslování grafu Přehled kategorií včetně úvodního vykreslení grafu.
      /// </summary>
      /// <param name="KolekceZaznamu">Kolekce dat pro statistické zpracování a vykreslení do grafu</param>
      public void UvodniNastaveniGrafuKategorii(ObservableCollection<Zaznam> KolekceZaznamu)
      {
         // Uložení dat určených ke zpracování do interní proměnné
         this.KolekceZaznamu = KolekceZaznamu;

         // Nastavení vykreslování grafu na první stránku
         CisloStrankyGrafu = 0;

         // Výchozí nastavení zvoleného zobrazení
         TypVykreslovanehoGrafu = TypGrafu.Kategorie;
         PocetHodnotOsyX = UrciPocetHodnotOsyX(Controller.VratPocetKategoriiZaznamu());
         VykreslitPrijmy = true;
         VykreslitVydaje = true;

         // Nastavení počátečního časového rozmezí pro výběr dat
         PocatecniDatum = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
         KoncoveDatum = DateTime.Now;

         // Volání pomocných metod pro vytvoření úvodního zobrazení grafu
         VytvorNazevGrafu("Zobrazení kategorií");
         VytvorLegenduGrafu();
         VykresliOvladaciPrvky();
         VytvorBlokyGrafu();
         AktualizujVykresleniGrafu();
      }



      /// <summary>
      /// Výpočet počtu prvků pro zobrazení 1 stránky grafu na základě celkového počtu prvků určených k zobrazení.
      /// </summary>
      /// <param name="CelkovyPocetPrvku">Celkový počet prvků určených k zobrazení</param>
      /// <returns>Počet prvků zobrazovaných na 1 stránce grafu</returns>
      private int UrciPocetHodnotOsyX(int CelkovyPocetPrvku)
      {
         // Pomocné proměnné pro výpočet počtu prvků zobrazených na 1 stránce dle celkového počtu prvků určených k zobrazení
         int MaxPrvku = 10;
         int PocetPrvku = MaxPrvku;
         int ZbytekPoDeleni = 0;


         for (int i = MaxPrvku; i >= 5; i--) 
         {
            // Pokud je zbytek po celočíselnémdělení nulový a zároveň je počet prvků větší než polovina maximálního počtu určí se počet prvků na stránku
            if (((CelkovyPocetPrvku % i) == 0) && (i >= (Math.Round(MaxPrvku / 2.0))))
            {
               PocetPrvku = i;
               return PocetPrvku;
            }

            // Nalezení nového největšího zbytku po celočíselném dělení
            if ((CelkovyPocetPrvku % i) > ZbytekPoDeleni)
            {
               ZbytekPoDeleni = CelkovyPocetPrvku % i;
               PocetPrvku = i;
            }
         }

         // Návratová hodnota
         return PocetPrvku;
      }

      /// <summary>
      /// Aktualizace vykreslení při změně dat nebo nastavení.
      /// </summary>
      public void AktualizujVykresleniGrafu()
      {
         VyberBlokyNaStrankuGrafu();
         VykresliGraf();
         VykresliOblastPodGrafem();
      }

      /// <summary>
      /// Metoda pro vytvoření grafického popisku s textem předaným v parametru.
      /// </summary>
      /// <param name="Nazev">Textová reprezentace názvu grafu</param>
      private void VytvorNazevGrafu(string Nazev)
      {
         // Uložení textového názve do interní proměnné
         this.Nazev = Nazev;

         // Vytvoření popisku s textem názvu grafu
         TextBlock nazev = new TextBlock
         {
            FontSize = 24,
            Foreground = Brushes.DarkBlue,
            Text = Nazev,
            TextDecorations = TextDecorations.Underline
         };
         GrafickyNazev = nazev;
      }

      /// <summary>
      /// Určení hodnot zobrazených na ose Y na základě maximální hodnoty prvků v grafu.
      /// </summary>
      private void NastavHodnotyNaOseY()
      {
         // Nalezení maximální hodnoty prvků pro možnost určení hodnot na ose Y
         NajdiMaximalniHodnotuPrvku();

         // Smazání kolekce hodnot pro možnost vytvoření nových hodnot na ose Y
         HodnotyOsyY.Clear();

         // Ochranná podmínka pro případ že na stránce v grafu nejsou žádná data, maximální hodnota je tedy 0
         if (MaximalniHodnota <= PocetHodnotOsyY)
         {
            for (int i = 1; i <= PocetHodnotOsyY; i++)
               HodnotyOsyY.Add(i);

            return;
         }

         // Pomocná proměnná pro zaokrouhlení na vyšší číslo s určitým počtem desetinných míst
         int Zaokrouhleni = MaximalniHodnota.ToString().Length - 1;

         // Určení nejvyššího čísla na ose Y, které je vyšší než maximální hodnota prvků v grafu
         double HorniHranice = Math.Ceiling(MaximalniHodnota / Zaokrouhleni) * Zaokrouhleni;

         // Výpočet hodnot zobrazovaných na ose Y dle zjištěné maximální zobrazené hodnoty a počtu hodnot na ose
         for (int i = 1; i <= PocetHodnotOsyY; i++)
         {
            HodnotyOsyY.Add((int)Math.Round(HorniHranice / (double)PocetHodnotOsyY * i));
         }
      }

      /// <summary>
      /// Metoda pro nalezení maximální hodnoty mezi zobrazovanými daty za účelem upravení hodnot na ose Y.
      /// </summary>
      private void NajdiMaximalniHodnotuPrvku()
      {
         // Smazání maximální hodnoty pro možnost opětovného nastavení
         MaximalniHodnota = 0;

         // Kolekce příjmů se projde pokud je určena k vykreslení
         if (VykreslitPrijmy)
         {
            // Nalezení nejvyšší hodnoty mezi příjmy
            foreach (BlokZaznamu blok in ZobrazeneBlokyPrijmu)
            {
               // Aktualizace maximální hodnoty v případě nalezení vyšší hodnoty než poslední nalezená maximální hodnota
               if (blok.CelkovaHodnota > MaximalniHodnota)
                  MaximalniHodnota = blok.CelkovaHodnota;
            }
         }

         // Kolekce výdajů se projde pokud je určena k vykreslení
         if (VykreslitVydaje)
         {
            // Nalezení nejvyšší hodnoty mezi výdaji
            foreach (BlokZaznamu blok in ZobrazeneBlokyVydaju)
            {
               // Aktualizace maximální hodnoty v případě nalezení vyšší hodnoty než poslední nalezená maximální hodnota
               if (blok.CelkovaHodnota > MaximalniHodnota)
                  MaximalniHodnota = blok.CelkovaHodnota;
            }
         }

         // Pokud je maximální hodnota menší než počet hodnot na ose Y nastaví se maximální hodnota právě na tento počet
         if (MaximalniHodnota < PocetHodnotOsyY)
            MaximalniHodnota = PocetHodnotOsyY;
      }

      /// <summary>
      /// Metoda pro vykreslení legendy grafu.
      /// </summary>
      private void VytvorLegenduGrafu()
      {
         // Vytvoření StackPanelů pro uložení jednotlivých prvků na potřebné pozice
         StackPanel legenda = new StackPanel { Orientation = Orientation.Vertical };
         StackPanel Prijem = new StackPanel { Orientation = Orientation.Horizontal };
         StackPanel Vydaj = new StackPanel { Orientation = Orientation.Horizontal };

         // Vytvoření konkrétních prvků legendy
         Ellipse OznaceniPrijmu = new Ellipse { Height = 20, Width = 20, Fill = BarvaZelena };
         Ellipse OznaceniVydaju = new Ellipse { Height = 20, Width = 20, Fill = BarvaCervena };
         Label PopisPrijmu = new Label { FontSize = 22, Content = " Příjmy" };
         Label PopisVydaju = new Label { FontSize = 22, Content = " Výdaje" };

         // Vytvoření řádků legendy
         Prijem.Children.Add(OznaceniPrijmu);
         Prijem.Children.Add(PopisPrijmu);
         Vydaj.Children.Add(OznaceniVydaju);
         Vydaj.Children.Add(PopisVydaju);

         // Přidání řádků do bloku legendy
         legenda.Children.Add(Prijem);
         legenda.Children.Add(Vydaj);

         // Uložení vytvořené legendy do interní proměnné
         Legenda = legenda;
      }


      /// <summary>
      /// Vykreslení panelu s ovládacími prvky grafu včetně legendy ke grafu.
      /// </summary>
      private void VykresliOvladaciPrvky()
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)PlatnoOvladacichPrvkuCanvas.Height;
         int SirkaCanvas = (int)PlatnoOvladacichPrvkuCanvas.Width;

         // Smazání plátna pro možnost nového vykreslení
         PlatnoOvladacichPrvkuCanvas.Children.Clear();

         // Vykrelení názvu grafu a legendy
         PlatnoOvladacichPrvkuCanvas.Children.Add(GrafickyNazev);
         PlatnoOvladacichPrvkuCanvas.Children.Add(Legenda);
         Canvas.SetLeft(GrafickyNazev, 0);
         Canvas.SetTop(GrafickyNazev, 100);
         Canvas.SetLeft(Legenda, 0);
         Canvas.SetTop(Legenda, 0);

         // Vykreslení ovládacích prvků pro Časový přehled
         if (TypVykreslovanehoGrafu == TypGrafu.CasovyPrehled)
         {
            VykresliOvladaciPrvkyCasovehoPrehledu();
         }

         // Vykreslení ovládacích prvků pro Přehled kategorií
         else if (TypVykreslovanehoGrafu == TypGrafu.Kategorie)
         {
            VykresliOvladaciPrvkyProGrafKategorii();
         }

         // Zaškrtávací pole pro možnost výběru zobrazení příjmů
         CheckBox VykresleniPrijmuCheck = new CheckBox
         {
            Height = 20,
            Width = 20,
            IsChecked = true
         };

         // Zaškrtávací pole pro možnost výběru zobrazení výdajů
         CheckBox VykresleniVydajuCheck = new CheckBox
         {
            Height = 20,
            Width = 20,
            IsChecked = true
         };

         // Popisek pro zaškrtávací pole
         Label vykreslitPrijmy = new Label
         {
            FontSize = 18,
            Foreground = BarvaZelena,
            Content = "Zobrazit příjmy"
         };

         // Popisek pro zaškrtávací pole
         Label vykreslitVydaje = new Label
         {
            FontSize = 18,
            Foreground = BarvaCervena,
            Content = "Zobrazit výdaje"
         };

         // Přidání události pro možnost reagovat na výběr dat pro zobrazení od uživatele
         VykresleniVydajuCheck.Click += VykresleniVydajuCheck_Click;
         VykresleniPrijmuCheck.Click += VykresleniPrijmuCheck_Click;

         // Přidání událostí pro možnost reagovat na pohyb kurzoru myši přes nápis
         vykreslitPrijmy.MouseMove += VykreslitPrijmy_MouseMove;
         vykreslitPrijmy.MouseLeave += VykreslitPrijmy_MouseLeave;
         vykreslitVydaje.MouseMove += VykreslitVydaje_MouseMove;
         vykreslitVydaje.MouseLeave += VykreslitVydaje_MouseLeave;

         // Vykreslení zaškrtávacích polí včetně popisků na plátno
         PlatnoOvladacichPrvkuCanvas.Children.Add(VykresleniPrijmuCheck);
         PlatnoOvladacichPrvkuCanvas.Children.Add(VykresleniVydajuCheck);
         PlatnoOvladacichPrvkuCanvas.Children.Add(vykreslitPrijmy);
         PlatnoOvladacichPrvkuCanvas.Children.Add(vykreslitVydaje);
         Canvas.SetLeft(VykresleniPrijmuCheck, 30);
         Canvas.SetTop(VykresleniPrijmuCheck, 410);
         Canvas.SetLeft(VykresleniVydajuCheck, 30);
         Canvas.SetTop(VykresleniVydajuCheck, 450);
         Canvas.SetLeft(vykreslitPrijmy, 50);
         Canvas.SetTop(vykreslitPrijmy, 400);
         Canvas.SetLeft(vykreslitVydaje, 50);
         Canvas.SetTop(vykreslitVydaje, 440);
      }

      /// <summary>
      /// Pomocná metoda pro vykreslení prvků určených k načtení vstupních dat od uživatele na plátno ovládacích prvků při zobrazení grafu Časový přehled.
      /// </summary>
      private void VykresliOvladaciPrvkyCasovehoPrehledu()
      {
         // Popisek pro výběr zobrazení
         Label popisekZobrazeni = new Label
         {
            FontSize = 16,
            Content = "Zobrazit: "
         };

         // Vytvoření výběrových polí pro možnost volby zobrazení
         RadioButton dny = new RadioButton { Content = " dny", FontSize = 24 };
         RadioButton tydny = new RadioButton { Content = " týdny", FontSize = 24 };
         RadioButton mesice = new RadioButton { Content = " měsíce", FontSize = 24 };
         RadioButton roky = new RadioButton { Content = " roky", FontSize = 24 };

         // Nastavení zaškrtnutí RadioButton při vykreslování dle zvolené možnosti zobrazení a vykreslení zadávacích polí
         switch (ZobrazeniPrehledu)
         {
            case ZvoleneZobrazeniPrehledu.Dny:
               dny.IsChecked = true;
               VykresliZadavaciPoleProGrafCasovyPrehled_DnyTydnyMesice();
               break;

            case ZvoleneZobrazeniPrehledu.Tydny:
               tydny.IsChecked = true;
               VykresliZadavaciPoleProGrafCasovyPrehled_DnyTydnyMesice();
               break;

            case ZvoleneZobrazeniPrehledu.Mesice:
               mesice.IsChecked = true;
               VykresliZadavaciPoleProGrafCasovyPrehled_DnyTydnyMesice();
               break;

            case ZvoleneZobrazeniPrehledu.Roky:
               roky.IsChecked = true;
               VykresliZadavaciPoleProGrafCasovyPrehled_Roku();
               break;

            default: break;
         }

         // Přidání událostí pro možnost reagovat na změnu volby 
         dny.Checked += Dny_Checked;
         tydny.Checked += Tydny_Checked;
         mesice.Checked += Mesice_Checked;
         roky.Checked += Roky_Checked;

         // Umístění výběru zobrazení na plátno
         PlatnoOvladacichPrvkuCanvas.Children.Add(popisekZobrazeni);
         PlatnoOvladacichPrvkuCanvas.Children.Add(dny);
         PlatnoOvladacichPrvkuCanvas.Children.Add(tydny);
         PlatnoOvladacichPrvkuCanvas.Children.Add(mesice);
         PlatnoOvladacichPrvkuCanvas.Children.Add(roky);
         Canvas.SetLeft(popisekZobrazeni, 10);
         Canvas.SetTop(popisekZobrazeni, 130);
         Canvas.SetLeft(dny, 10);
         Canvas.SetTop(dny, 160);
         Canvas.SetLeft(tydny, 130);
         Canvas.SetTop(tydny, 160);
         Canvas.SetLeft(mesice, 10);
         Canvas.SetTop(mesice, 200);
         Canvas.SetLeft(roky, 130);
         Canvas.SetTop(roky, 200);
      }

      /// <summary>
      /// Pomocná metoda pro vykreslení zadávacíh polí na plátno ovládacích prvků při zobrazení grafu Časová přehled.
      /// Metoda se volá v případě zvolení zobrazení dnů, týdnů, nebo měsíců.
      /// </summary>
      private void VykresliZadavaciPoleProGrafCasovyPrehled_DnyTydnyMesice()
      {
         // Popisek pro výběr měsíce
         Label MesicPopisek = new Label { FontSize = 14, Content = "Vyberte měsíc:" };

         // Popisek pro výběr roku
         Label RokPopisek = new Label { FontSize = 14, Content = "Vyberte rok:" };

         // Rozbalovací okno pro výběr měsíce
         ComboBox MesicVyber = new ComboBox { FontSize = 16, Width = 120, };

         // Rozbalovací okno pro výběr roku
         ComboBox RokVyber = new ComboBox { FontSize = 16, Width = 120, };

         // Přidání možností do rozbalovacího okna pro výběr měsíce
         for (int i = 1; i <= 12; i++)
         {
            ComboBoxItem item = new ComboBoxItem { Content = Hodiny.VratMesicTextove(i) };
            MesicVyber.Items.Add(item);
         }

         // Přidání možností do rozbalovacího okna pro výběr roku
         for (int i = 2019; i <= DateTime.Now.Year; i++)
         {
            ComboBoxItem item = new ComboBoxItem { Content = i.ToString() };
            RokVyber.Items.Add(item);
         }

         // Nastavení defaultního výběru rozbalovacích oken
         MesicVyber.SelectedIndex = VybranyMesic - 1;
         RokVyber.SelectedIndex = VybranyRok - 2019;

         // Přidání událostí pro možnost reagovat na změnu výběru
         MesicVyber.SelectionChanged += MesicVyber_SelectionChanged;
         RokVyber.SelectionChanged += RokVyber_SelectionChanged;

         // Vykreslení prvků pro výběr vstupních dat v případě vykreslování ovládání pro zobrazení měsíců
         if (ZobrazeniPrehledu == ZvoleneZobrazeniPrehledu.Mesice)
         {
            PlatnoOvladacichPrvkuCanvas.Children.Add(RokPopisek);
            PlatnoOvladacichPrvkuCanvas.Children.Add(RokVyber);
            Canvas.SetLeft(RokPopisek, 10);
            Canvas.SetTop(RokPopisek, 280);
            Canvas.SetLeft(RokVyber, 20);
            Canvas.SetTop(RokVyber, 330);
         }

         // Vykreslení prvků pro výběr vstupních dat v případě vykreslování ovládání pro zobrazení dnů nebo týdnů
         else
         {
            PlatnoOvladacichPrvkuCanvas.Children.Add(MesicPopisek);
            PlatnoOvladacichPrvkuCanvas.Children.Add(RokPopisek);
            PlatnoOvladacichPrvkuCanvas.Children.Add(MesicVyber);
            PlatnoOvladacichPrvkuCanvas.Children.Add(RokVyber);
            Canvas.SetLeft(MesicPopisek, 10);
            Canvas.SetTop(MesicPopisek, 250);
            Canvas.SetLeft(MesicVyber, 20);
            Canvas.SetTop(MesicVyber, 280);
            Canvas.SetLeft(RokPopisek, 10);
            Canvas.SetTop(RokPopisek, 330);
            Canvas.SetLeft(RokVyber, 20);
            Canvas.SetTop(RokVyber, 360);
         }
      }

      /// <summary>
      /// Pomocná metoda pro vykreslení zadávacíh polí na plátno ovládacích prvků při zobrazení grafu Časová přehled.
      /// Metoda se volá v případě zvolení zobrazení roků.
      /// </summary>
      private void VykresliZadavaciPoleProGrafCasovyPrehled_Roku()
      {
         // Popisky pro výběr období pro zobrazení grafu
         Label popisekRokPocatecni = new Label { FontSize = 14, Content = "Vyberte počáteční rok:" };
         Label popisekRokKoncovy = new Label { FontSize = 14, Content = "Vyberte koncový rok:" };

         // Rozbalovací okno pro výběr roku
         ComboBox RokPocatecniVyber = new ComboBox { FontSize = 16, Width = 120, };
         ComboBox RokKoncovyVyber = new ComboBox { FontSize = 16, Width = 120, };

         // Přidání možností do rozbalovacího okna pro výběr koncového roku
         for (int i = 2019; i <= DateTime.Now.Year; i++)
         {
            ComboBoxItem item = new ComboBoxItem { Content = i.ToString() };
            RokKoncovyVyber.Items.Add(item);
         }

         // Přidání možností do rozbalovacího okna pro výběr počátečního roku
         for (int i = 2019; i <= DateTime.Now.Year; i++)
         {
            ComboBoxItem item = new ComboBoxItem { Content = i.ToString() };
            RokPocatecniVyber.Items.Add(item);
         }

         // Nastavení defaultního výběru rozbalovacích oken (zobrzení aktuálního roku)
         VybranyKoncovyRok = VybranyRok;
         RokPocatecniVyber.SelectedIndex = VybranyRok - 2019;
         RokKoncovyVyber.SelectedIndex = VybranyKoncovyRok - 2019;

         // Přidání události pro možnot reagovat na výběr roku
         RokPocatecniVyber.SelectionChanged += RokVyber_SelectionChanged;
         RokKoncovyVyber.SelectionChanged += RokKoncovyVyber_SelectionChanged;

         // Vykreslení prvků na plátno
         PlatnoOvladacichPrvkuCanvas.Children.Add(popisekRokPocatecni);
         PlatnoOvladacichPrvkuCanvas.Children.Add(popisekRokKoncovy);
         PlatnoOvladacichPrvkuCanvas.Children.Add(RokPocatecniVyber);
         PlatnoOvladacichPrvkuCanvas.Children.Add(RokKoncovyVyber);
         Canvas.SetLeft(popisekRokPocatecni, 10);
         Canvas.SetTop(popisekRokPocatecni, 250);
         Canvas.SetLeft(RokPocatecniVyber, 20);
         Canvas.SetTop(RokPocatecniVyber, 280);
         Canvas.SetLeft(popisekRokKoncovy, 10);
         Canvas.SetTop(popisekRokKoncovy, 330);
         Canvas.SetLeft(RokKoncovyVyber, 20);
         Canvas.SetTop(RokKoncovyVyber, 360);
      }

      /// <summary>
      /// Pomocná metoda pro vykreslení prvků určených k načtení vstupních dat od uživatele na plátno ovládacích prvků při zobrazení grafu Přehled kategorií.
      /// </summary>
      private void VykresliOvladaciPrvkyProGrafKategorii()
      {
         // Popisek pro výběr počátečního data
         Label DatumPocatecniPopis = new Label
         {
            FontSize = 18,
            Content = "Vyberte počáteční datum:"
         };

         // Popisek pro výběr koncového data
         Label DatumKoncovePopis = new Label
         {
            FontSize = 18,
            Content = "Vyberte koncové datum:"
         };

         // Blok pro zadání počátečního data
         DatePicker DatumPocatecni = new DatePicker
         {
            FontSize = 16,
            SelectedDate = PocatecniDatum,
            Height = 30,
            Width = 150,
            IsTodayHighlighted = true
         };

         // Blok pro zadání koncového data
         DatePicker DatumKoncove = new DatePicker
         {
            FontSize = 16,
            SelectedDate = KoncoveDatum,
            Height = 30,
            Width = 150,
            IsTodayHighlighted = true
         };

         // Přidání událostí pro možnost reagovat na změnu hodnoty v zadávacím poli
         DatumPocatecni.SelectedDateChanged += DatumPocatecni_SelectedDateChanged;
         DatumKoncove.SelectedDateChanged += DatumKoncove_SelectedDateChanged;

         // Vykreslení zadávacích polí na plátno
         PlatnoOvladacichPrvkuCanvas.Children.Add(DatumPocatecniPopis);
         PlatnoOvladacichPrvkuCanvas.Children.Add(DatumKoncovePopis);
         PlatnoOvladacichPrvkuCanvas.Children.Add(DatumPocatecni);
         PlatnoOvladacichPrvkuCanvas.Children.Add(DatumKoncove);
         Canvas.SetLeft(DatumPocatecniPopis, 20);
         Canvas.SetTop(DatumPocatecniPopis, 180);
         Canvas.SetLeft(DatumPocatecni, 30);
         Canvas.SetTop(DatumPocatecni, 210);
         Canvas.SetLeft(DatumKoncovePopis, 20);
         Canvas.SetTop(DatumKoncovePopis, 280);
         Canvas.SetLeft(DatumKoncove, 30);
         Canvas.SetTop(DatumKoncove, 310);
      }


      /// <summary>
      /// Vytvoření bloků reprezentující data záznamů odpovídající zvoleným parametrům. 
      /// Metoda volá pomocné metody na základě typu vykreslovaného grafu a požadovaného zobrazení.
      /// </summary>
      private void VytvorBlokyGrafu()
      {
         // Smazání obsahu kolekcí pro možnost nového načtení dat
         KolekceBlokuZaznamu_Prijmy.Clear();
         KolekceBlokuZaznamu_Vydaje.Clear();

         // Je vykreslován graf Časový přehled
         if (TypVykreslovanehoGrafu == TypGrafu.CasovyPrehled)
         {
            // Vytvoření bloků na základě zvoleného zobrazení
            switch (ZobrazeniPrehledu)
            {
               case ZvoleneZobrazeniPrehledu.Dny:     VytvorBlokyProZobrazeniDnu();    break;
               case ZvoleneZobrazeniPrehledu.Tydny:   VytvorBlokyProZobrazeniTydnu();  break;
               case ZvoleneZobrazeniPrehledu.Mesice:  VytvorBlokyProZobrazeniMesicu(); break;
               case ZvoleneZobrazeniPrehledu.Roky:    VytvorBlokyProZobrazeniRoku();   break;
               default: break;
            }
         }

         // Je vykreslován graf Přehled kategorií
         else if (TypVykreslovanehoGrafu == TypGrafu.Kategorie)
         {
            VytvorBlokyProGrafKategorii();
         }


         // Určení počtu stran pro vykreslení na zázkladě počtu prvků v kolekci pro vykreslení
         MaximalniPocetStranGrafu = (int)Math.Ceiling((double)KolekceBlokuZaznamu_Vydaje.Count / (double)PocetHodnotOsyX);

         // Nastavení vykreslování na první stránku
         CisloStrankyGrafu = 0;
      }

      /// <summary>
      /// Vytvoření bloků obsahujících data potřebná pro vykreslení grafu Kategorie. 
      /// Metoda statisticky vyhodnotí všechny záznamy v určitém časovém období a roztřídí je dle kategorií do jednotlivých bloků.
      /// </summary>
      private void VytvorBlokyProGrafKategorii()
      {
         // Cyklus postupně projde všechny kategorie záznamů a vybere všechny záznamy z dané kategorie
         for (int i = 1; i < Controller.VratPocetKategoriiZaznamu(); i++)
         {
            // Vytvoření nových bloků
            BlokZaznamu BlokPrijmu = new BlokZaznamu();
            BlokZaznamu BlokVydaju = new BlokZaznamu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;

            // Přidání názvu kategorie
            BlokPrijmu.Nazev = Zaznam.NazvyKategorii[i];
            BlokVydaju.Nazev = Zaznam.NazvyKategorii[i];

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Uložení počátečního a koncového data do bloku
            BlokPrijmu.DatumPocatecni = PocatecniDatum;
            BlokPrijmu.DatumKoncove = KoncoveDatum;
            BlokVydaju.DatumPocatecni = PocatecniDatum;
            BlokVydaju.DatumKoncove = KoncoveDatum;

            // Uložení barvy bloku
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.Barva = BarvaCervena;


            // Cyklus postupně projde všechny záznamy pro nalezení všech záznamů z dané kategorie
            foreach (Zaznam zaznam in KolekceZaznamu)
            {
               // Záznam spadá do vybrané kategorie
               if (zaznam.kategorie == (Kategorie)i)
               {
                  // Kontrola zda záznam splňuje zadané časové období
                  if (zaznam.Datum <= KoncoveDatum && zaznam.Datum >= PocatecniDatum)
                  {
                     // Záznam spadá mezi příjmy
                     if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                     {
                        BlokPrijmu.PocetZaznamu++;
                        BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }

                     // Záznam spadá mezi výdaje
                     else
                     {
                        BlokVydaju.PocetZaznamu++;
                        BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }

                  }
               }

            }

            // Přidání bloku do kolekce
            KolekceBlokuZaznamu_Prijmy.Add(BlokPrijmu);
            KolekceBlokuZaznamu_Vydaje.Add(BlokVydaju);
         }
      }

      /// <summary>
      /// Vytvoření bloků reprezentující záznamy v 1 den.
      /// </summary>
      private void VytvorBlokyProZobrazeniDnu()
      {
         // Určení počtu prvků na ose X
         PocetHodnotOsyX = 11;

         // Vytvoření datumů dle zvoleného měsíce a roku
         DateTime Zacatek = new DateTime(VybranyRok, VybranyMesic, 1);
         DateTime Konec = new DateTime(VybranyRok, VybranyMesic, DateTime.DaysInMonth(VybranyRok, VybranyMesic));

         // Pokud je vybrán aktuální měsíc, vytvoří se bloky pouze od začátku měsíce do aktuálního dne
         if (DateTime.Now < Konec)
            Konec = DateTime.Now;

         // Cyklus pro vytvoření bloků reprezentující jednotlivé dny v zadaném období
         for (DateTime Datum = Zacatek; Datum <= Konec; Datum = Datum.AddDays(1))
         {
            // Vytvoření nových bloků
            BlokZaznamu BlokPrijmu = new BlokZaznamu();
            BlokZaznamu BlokVydaju = new BlokZaznamu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;
            BlokVydaju.Barva = BarvaCervena;

            // Přidání názvu bloku dne
            BlokPrijmu.Nazev = Hodiny.VratDenVTydnu(Datum.DayOfWeek);
            BlokVydaju.Nazev = Hodiny.VratDenVTydnu(Datum.DayOfWeek);

            // Přidání popisku dne
            BlokPrijmu.Popisek = Hodiny.VratDenVTydnu(Datum.DayOfWeek) + " " + Datum.Date.ToString("dd.MM.");
            BlokVydaju.Popisek = Hodiny.VratDenVTydnu(Datum.DayOfWeek) + " " + Datum.Date.ToString("dd.MM.");

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Uložení data
            BlokPrijmu.DatumPocatecni = Datum;
            BlokPrijmu.DatumKoncove = Datum;
            BlokVydaju.DatumPocatecni = Datum;
            BlokVydaju.DatumKoncove = Datum;

            // Cyklus projde všechny záznamy pro nalezení záznamů z daného dne
            foreach (Zaznam zaznam in KolekceZaznamu)
            {
               // Byl nalezen záznam v daném dni
               if (zaznam.Datum == Datum)
               {
                  // Záznam spadá mezi příjmy
                  if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                  {
                     BlokPrijmu.PocetZaznamu++;
                     BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                  }

                  // Záznam spadá mezi výdaje
                  else
                  {
                     BlokVydaju.PocetZaznamu++;
                     BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                  }
               }
            }

            // Přidání vytvořeného bloku do kolekce
            KolekceBlokuZaznamu_Prijmy.Add(BlokPrijmu);
            KolekceBlokuZaznamu_Vydaje.Add(BlokVydaju);
         }
      }

      /// <summary>
      /// Vytvoření bloků reprezentující záznamy v celém týdnu.
      /// </summary>
      private void VytvorBlokyProZobrazeniTydnu()
      {
         // Získání datumů pro první a poslední den v týdnu
         List<DateTime> DatumyTydnu = Tydny.GetWeek(VybranyMesic, VybranyRok);

         // Zjištění počtu týdnů v daném měsíci
         int PocetTydnuDanehoMesice = DatumyTydnu.Count / 2;

         // Určení počtu prvků na ose X (odpovídá počtu týdnu v zadaném měsíci)
         PocetHodnotOsyX = PocetTydnuDanehoMesice;

         // Postupné vytvoření potřebného počtu týdnů z závislosti na zadaném měsíci
         for (int tyden = 1; tyden <= PocetTydnuDanehoMesice; tyden++)
         {
            // Vytvoření nových bloků
            BlokZaznamu BlokPrijmu = new BlokZaznamu();
            BlokZaznamu BlokVydaju = new BlokZaznamu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;
            BlokVydaju.Barva = BarvaCervena;

            // Uložení názvu daného týdne
            BlokPrijmu.Nazev = Hodiny.VratMesicTextove(VybranyMesic) + ": " + tyden.ToString() + ". týden";
            BlokVydaju.Nazev = Hodiny.VratMesicTextove(VybranyMesic) + ": " + tyden.ToString() + ". týden";

            // Určení počátečního a koncového data týdne
            DateTime ZacatekTydne = DatumyTydnu[2 * (tyden - 1)];
            DateTime KonecTydne = DatumyTydnu[2 * (tyden - 1) + 1];

            // Uložení data
            BlokPrijmu.DatumPocatecni = ZacatekTydne;
            BlokPrijmu.DatumKoncove = KonecTydne;
            BlokVydaju.DatumPocatecni = ZacatekTydne;
            BlokVydaju.DatumKoncove = KonecTydne;

            // Uložení popisků týdne
            BlokPrijmu.Popisek = String.Format("{0}.{1}. - {2}.{3}.", ZacatekTydne.Day, ZacatekTydne.Month, KonecTydne.Day, KonecTydne.Month);
            BlokVydaju.Popisek = String.Format("{0}.{1}. - {2}.{3}.", ZacatekTydne.Day, ZacatekTydne.Month, KonecTydne.Day, KonecTydne.Month);

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Postupné procházení jednotlivých dní v daném týdnu
            for (DateTime den = ZacatekTydne; den <= KonecTydne; den = den.AddDays(1))
            {
               // Cyklus projde všechny záznamy pro nalezení záznamů z daného dne
               foreach (Zaznam zaznam in KolekceZaznamu)
               {
                  // Byl nalezen záznam v daném dni
                  if (zaznam.Datum == den)
                  {
                     // Záznam spadá mezi příjmy
                     if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                     {
                        BlokPrijmu.PocetZaznamu++;
                        BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }

                     // Záznam spadá mezi výdaje
                     else
                     {
                        BlokVydaju.PocetZaznamu++;
                        BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }
                  }
               }
            }

            // Přidání vytvořeného bloku do kolekce
            KolekceBlokuZaznamu_Prijmy.Add(BlokPrijmu);
            KolekceBlokuZaznamu_Vydaje.Add(BlokVydaju);
         }
      }

      /// <summary>
      /// Vytvoření bloků v zadaném roce, kde každý blok obsahuje informace o záznamech celého měsíce.
      /// </summary>
      private void VytvorBlokyProZobrazeniMesicu()
      {
         // Určení počtu prvků na ose X
         PocetHodnotOsyX = 12;

         // Určení posledního měsíce v roce (pokud se vykresluje aktuální rok, poslední vykreslovaný měsíc bude aktuální měsíc)
         int KoncovyMesic = DateTime.Now.Year > VybranyRok ? 12 : DateTime.Now.Month;

         // Zůžení osy X pro případ, že je vykreslován malý počet prvků (sloupců)
         if (KoncovyMesic < 10)
            PocetHodnotOsyX = KoncovyMesic + 1;

         // Cyklus pro vytvoření bloků reprezentující jednotlivé měsíce v zadaném roce
         for (int i = 1; i <= KoncovyMesic; i++)
         {
            // Vytvoření nových bloků
            BlokZaznamu BlokPrijmu = new BlokZaznamu();
            BlokZaznamu BlokVydaju = new BlokZaznamu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;
            BlokVydaju.Barva = BarvaCervena;

            // Přidání názvů měsíce
            BlokPrijmu.Nazev = Hodiny.VratMesicTextove(i);
            BlokVydaju.Nazev = Hodiny.VratMesicTextove(i);

            // Přidání popisku měíce
            BlokPrijmu.Popisek = Hodiny.VratMesicTextove(i);
            BlokVydaju.Popisek = Hodiny.VratMesicTextove(i);

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Uložení data
            BlokPrijmu.DatumPocatecni = new DateTime(VybranyRok, i, 1);
            BlokVydaju.DatumPocatecni = new DateTime(VybranyRok, i, 1);
            BlokPrijmu.DatumKoncove = new DateTime(VybranyRok, i, DateTime.DaysInMonth(VybranyRok, i));
            BlokVydaju.DatumKoncove = new DateTime(VybranyRok, i, DateTime.DaysInMonth(VybranyRok, i));

            // Postupné procházení všech dní v měsíci
            for (DateTime Den = BlokPrijmu.DatumPocatecni; Den <= BlokPrijmu.DatumKoncove; Den = Den.AddDays(1))
            {
               // Cyklus projde všechny záznamy pro nalezení záznamů z daného dne
               foreach (Zaznam zaznam in KolekceZaznamu)
               {
                  // Byl nalezen záznam v daném dni
                  if (zaznam.Datum == Den)
                  {
                     // Záznam spadá mezi příjmy
                     if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                     {
                        BlokPrijmu.PocetZaznamu++;
                        BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }

                     // Záznam spadá mezi výdaje
                     else
                     {
                        BlokVydaju.PocetZaznamu++;
                        BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }
                  }
               }
            }

            // Přidání vytvořeného bloku do kolekce
            KolekceBlokuZaznamu_Prijmy.Add(BlokPrijmu);
            KolekceBlokuZaznamu_Vydaje.Add(BlokVydaju);
         }
      }

      /// <summary>
      /// Vytvoření bloků uchovávající v sobě veškeré záznamy daného roku v zadánem intervalu let.
      /// </summary>
      private void VytvorBlokyProZobrazeniRoku()
      {
         // Určení počtu prvků na ose X
         PocetHodnotOsyX = (VybranyKoncovyRok - VybranyRok + 1) > 7 ? 5 : (VybranyKoncovyRok - VybranyRok + 1) + 1;

         // Cyklus pro vytvoření bloků reprezentující jednotlivé roky ve vybraném intervalu let
         for (int rok = VybranyRok; rok <= VybranyKoncovyRok; rok++)
         {
            // Vytvoření nových bloků
            BlokZaznamu BlokPrijmu = new BlokZaznamu();
            BlokZaznamu BlokVydaju = new BlokZaznamu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;
            BlokVydaju.Barva = BarvaCervena;

            // Uložení názvu roku
            BlokPrijmu.Nazev = rok.ToString();
            BlokVydaju.Nazev = rok.ToString();

            // Uložení popisku roků
            BlokPrijmu.Popisek = rok.ToString();
            BlokVydaju.Popisek = rok.ToString();

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Uložení data
            BlokPrijmu.DatumPocatecni = new DateTime(rok, 1, 1);
            BlokPrijmu.DatumKoncove = new DateTime(rok, 12, DateTime.DaysInMonth(rok, 12));
            BlokVydaju.DatumPocatecni = new DateTime(rok, 1, 1);
            BlokVydaju.DatumKoncove = new DateTime(rok, 12, DateTime.DaysInMonth(rok, 12));


            // Cyklus projde všechny záznamy pro nalezení záznamů z daného roku
            foreach (Zaznam zaznam in KolekceZaznamu)
            {
               // Byl nalezen záznam v daném roce
               if (zaznam.Datum.Year == rok)
               {
                  // Záznam spadá mezi příjmy
                  if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                  {
                     BlokPrijmu.PocetZaznamu++;
                     BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                  }

                  // Záznam spadá mezi výdaje
                  else
                  {
                     BlokVydaju.PocetZaznamu++;
                     BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                  }
               }
            }

            // Přidání vytvořeného bloku do kolekce
            KolekceBlokuZaznamu_Prijmy.Add(BlokPrijmu);
            KolekceBlokuZaznamu_Vydaje.Add(BlokVydaju);
         }
      }

      /// <summary>
      /// Výběr bloků dat zobrazovaných na 1 stránce grafu.
      /// </summary>
      private void VyberBlokyNaStrankuGrafu()
      {
         // Určení indexu bloku pro výběr bloku z kolekce na základě vykreslované stránky grafu
         int PrvniBlok = CisloStrankyGrafu * PocetHodnotOsyX;

         // Smazání kolekcí zobrazovaných dat pro možnost přidání nových bloků pro zobrazení
         ZobrazeneBlokyPrijmu.Clear();
         ZobrazeneBlokyVydaju.Clear();

         // Pokud je v kolekci více bloků, vybere se pouze požadovaný počet bloků k vykreslení
         if ((PrvniBlok + PocetHodnotOsyX) <= KolekceBlokuZaznamu_Vydaje.Count)
         {
            // Postupné přidání bloků z celkové kolekce do kolekce bloků určených k zobrazení
            for (int index = PrvniBlok; index < (PrvniBlok + PocetHodnotOsyX); index++)
            {
               ZobrazeneBlokyPrijmu.Add(KolekceBlokuZaznamu_Prijmy[index]);
               ZobrazeneBlokyVydaju.Add(KolekceBlokuZaznamu_Vydaje[index]);
            }
         }

         // Pokud v kolekci zbývá jen několik bloků, vyberou se všechny zbývající bloky pro vykreslení na poslední stránku grafu
         else
         {
            // Postupné přidání bloků z celkové kolekce do kolekce bloků určených k zobrazení
            for (int index = PrvniBlok; index < KolekceBlokuZaznamu_Vydaje.Count; index++)
            {
               ZobrazeneBlokyPrijmu.Add(KolekceBlokuZaznamu_Prijmy[index]);
               ZobrazeneBlokyVydaju.Add(KolekceBlokuZaznamu_Vydaje[index]);
            }
         }

         // Uložení popisků kategorií vybraných bloků pro možnost vykreslení popisků pod grafem
         VytvorPopiskyGrafu();
      }

      /// <summary>
      /// Uložení textové reprezentace vybraných bloků.
      /// </summary>
      public void VytvorPopiskyGrafu()
      {
         // Smazání obsahu kolekce pro opětovné vytvoření nových popisků při změně dat
         PopiskyOsyX.Clear();

         // Pomocné proměnné pro rozhodnutí, která z kolekcí obsahuje více prvků
         int PocetPrijmu = ZobrazeneBlokyPrijmu.Count;
         int PocetVydaju = ZobrazeneBlokyVydaju.Count;
         ObservableCollection<BlokZaznamu> SloupceGrafu = new ObservableCollection<BlokZaznamu>();

         // Načtení kolekce s více prvky
         if (PocetPrijmu <= PocetVydaju)
            SloupceGrafu = ZobrazeneBlokyVydaju;
         else
            SloupceGrafu = ZobrazeneBlokyPrijmu;

         // Uložení popisků pro graf Časový přehled
         if (TypVykreslovanehoGrafu == TypGrafu.CasovyPrehled)
         {
            // Uložení jednotlivých popisků do kolekce
            foreach (BlokZaznamu blok in SloupceGrafu)
            {
               PopiskyOsyX.Add(blok.Popisek);
            }
         }

         // Uložení popisků pro graf Přehled kategorií
         else if (TypVykreslovanehoGrafu == TypGrafu.Kategorie)
         {
            // Uložení jednotlivých názvů (kategorií) do kolekce
            foreach (BlokZaznamu blok in SloupceGrafu)
            {
               PopiskyOsyX.Add(blok.Nazev);
            }
         }
      }

      /// <summary>
      /// Vykreslení sloupcového grafu. 
      /// Na ose Y jsou hodnoty příjmů a výdajů v součtu za zvolené časové období. 
      /// Na ose X jsou časové úseky v rámci zvoleného časového intervalu. 
      /// </summary>
      private void VykresliGraf()
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int Vyska = (int)PlatnoGrafuCanvas.Height - 10;
         int Sirka = (int)PlatnoGrafuCanvas.Width;

         // Smazání obsahu pro možnost nového vykreslení
         PlatnoGrafuCanvas.Children.Clear();

         // Výběr bloků určených k zobrazení v grafu
         VyberBlokyNaStrankuGrafu();

         // Kontrolní podmínka že pro danou stránku nejsou vybrány žádné bloky
         if (ZobrazeneBlokyPrijmu.Count == 0 && ZobrazeneBlokyVydaju.Count == 0)
         {
            // Změna čísla stránky a opětovný výběr bloků
            CisloStrankyGrafu--;
            VyberBlokyNaStrankuGrafu();
         }

         // Výpočet hodnot pro určení hodnot na ose Y
         NastavHodnotyNaOseY();

         // Pomocná proměnná pro uchování šířky bloku obsahující popisky osy Y (začátek vykreslování samotného grafu)
         int Odsazeni = HodnotyOsyY[HodnotyOsyY.Count - 1].ToString().Length * 10;

         // Přičtení velikosti označení hodnoty (Kč za číslem)
         Odsazeni += 30;

         // Pomocná proměnná pro snadnější vykreslení hodnot na osu Y
         int mezeraY = (int)Math.Floor((double)Vyska / (double)HodnotyOsyY.Count);

         // Pomocná proměnná pro snadnější vykreslování prvků na osu X
         int mezeraX = (int)Math.Floor((double)(Sirka - Odsazeni) / (double)(PocetHodnotOsyX + 0.5));

         // Určení šířky sloupce reprezentující 1 blok dat
         int SirkaSloupce = (int)Math.Round((mezeraX - 10) / 3.0);

         // Uložení souřadnice prvního bloku pro možnost vykreslení popisu osy X na správné souřadnice
         SouradniceX_PrvniSloupec = Odsazeni + mezeraX;
         OdstupHodnotX = mezeraX;

         // Levé ohraničené oblasti grafu (osa Y)
         Rectangle LeveOhraniceni = new Rectangle
         {
            Width = 2,
            Height = Vyska + 10,
            Fill = BarvaCerna
         };

         // Spodní ohraničené oblasti grafu (osa X)
         Rectangle SpodniOhraniceni = new Rectangle
         {
            Width = Sirka - Odsazeni,
            Height = 2,
            Fill = BarvaCerna
         };

         // Přidání ohraničení na plátno
         PlatnoGrafuCanvas.Children.Add(LeveOhraniceni);
         PlatnoGrafuCanvas.Children.Add(SpodniOhraniceni);
         Canvas.SetLeft(LeveOhraniceni, Odsazeni - 2);
         Canvas.SetBottom(LeveOhraniceni, 0);
         Canvas.SetLeft(SpodniOhraniceni, Odsazeni);
         Canvas.SetBottom(SpodniOhraniceni, 0);

         // Přidání popisku osy Y včetně pomocných čar značících úroveň na ose Y pro lepší orientaci v grafu
         for (int i = 1; i <= HodnotyOsyY.Count; i++)
         {
            // Číselná hodnota na ose Y
            Label popisek = new Label
            {
               FontSize = 14,
               Content = HodnotyOsyY[i - 1].ToString() + " Kč"
            };

            // Pomocná čára
            Rectangle cara = new Rectangle
            {
               Width = Sirka - Odsazeni,
               Height = 1,
               Fill = Brushes.Gray
            };

            // Vyznačení hodnoty na ose
            Rectangle oznaceni = new Rectangle
            {
               Width = 10,
               Height = 1,
               Fill = BarvaCerna
            };

            // Přidání prvků a vykreslení na plátno
            PlatnoGrafuCanvas.Children.Add(popisek);
            PlatnoGrafuCanvas.Children.Add(cara);
            PlatnoGrafuCanvas.Children.Add(oznaceni);
            Canvas.SetLeft(popisek, 0);
            Canvas.SetBottom(popisek, (i * mezeraY) - 12);
            Canvas.SetLeft(cara, Odsazeni);
            Canvas.SetBottom(cara, (i * mezeraY));
            Canvas.SetLeft(oznaceni, Odsazeni - oznaceni.Width / 2 - 1);
            Canvas.SetBottom(oznaceni, (i * mezeraY));
         }

         // Přidání prvků na osu X včetně vykreslení grafických bloků reprezentující data 
         for (int i = 1; i <= PocetHodnotOsyX; i++)
         {
            // Vyznačení hodnoty na ose
            Rectangle oznaceni = new Rectangle
            {
               Width = 1,
               Height = 7,
               Fill = BarvaCerna
            };

            // Přidání prvků a vykreslení na plátno
            PlatnoGrafuCanvas.Children.Add(oznaceni);
            Canvas.SetLeft(oznaceni, Odsazeni + (i * mezeraX));
            Canvas.SetBottom(oznaceni, 0);

            // Pokud je blok v kolekci příjmů pro vykreslení, vytvoří se sloupec reprezentující jeho data -> vykreslení sloupců příjmů
            if (i <= ZobrazeneBlokyPrijmu.Count && VykreslitPrijmy == true)
            {
               // Výpočet výšky sloupce na základě jeho hodnoty
               int VyskaSloupce = (int)Math.Ceiling((ZobrazeneBlokyPrijmu[i - 1].CelkovaHodnota * Vyska) / MaximalniHodnota);

               // Pokud na vykreslované stránce nejsou žádné bloky k vykreslení, nastaví se výška sloupce na 0 pro možnost vykreslení prázdného grafu
               if (VyskaSloupce < 0)
                  VyskaSloupce = 0;

               // Vytvoření sloupce reprezentující blok příjmů
               Rectangle prijem = new Rectangle
               {
                  Width = SirkaSloupce,
                  Height = VyskaSloupce,
                  Fill = BarvaZelena
               };

               // Uložení indexu bloku do názvu sloupce pro možnost pozdější identifikace bloku ze sloupce
               prijem.Name = "sloupec" + (i - 1).ToString();

               // Přidání událoti pro možnost reagovat na pohyb myší na sloupci
               prijem.MouseMove += Prijem_MouseMove;
               prijem.MouseLeave += Prijem_MouseLeave;

               // Vykreslení sloupce na plátno
               PlatnoGrafuCanvas.Children.Add(prijem);
               Canvas.SetLeft(prijem, Odsazeni + (i * mezeraX) - 5 - SirkaSloupce);
               Canvas.SetBottom(prijem, 2);
            }

            // Pokud je blok v kolekci výdajů pro vykreslení, vytvoří se sloupec reprezentující jeho data -> vykreslení sloupců výdajů
            if (i <= ZobrazeneBlokyVydaju.Count && VykreslitVydaje == true)
            {
               // Výpočet výšky sloupce na základě jeho hodnoty
               int VyskaSloupce = (int)Math.Ceiling((ZobrazeneBlokyVydaju[i - 1].CelkovaHodnota * Vyska) / MaximalniHodnota);

               // Pokud na vykreslované stránce nejsou žádné bloky k vykreslení, nastaví se výška sloupce na 0 pro možnost vykreslení prázdného grafu
               if (VyskaSloupce < 0)
                  VyskaSloupce = 0;

               // Vytvoření sloupce reprezentující blok příjmů
               Rectangle vydaj = new Rectangle
               {
                  Width = SirkaSloupce,
                  Height = VyskaSloupce,
                  Fill = BarvaCervena
               };

               // Uložení indexu bloku do názvu sloupce pro možnost pozdější identifikace bloku ze sloupce
               vydaj.Name = "sloupec" + (i - 1).ToString();

               // Přidání událoti pro možnost reagovat na pohyb myší na sloupci
               vydaj.MouseMove += Vydaj_MouseMove;
               vydaj.MouseLeave += Vydaj_MouseLeave;

               // Vykreslení sloupce na plátno
               PlatnoGrafuCanvas.Children.Add(vydaj);
               Canvas.SetLeft(vydaj, Odsazeni + (i * mezeraX) + 5);
               Canvas.SetBottom(vydaj, 2);
            }
         }

      }

      /// <summary>
      /// Metoda pro vykreslení oblasti pod grafem, tedy popisky osy X a ovládací šipky pro pohyb mezi stránkami grafu.
      /// </summary>
      private void VykresliOblastPodGrafem()
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)PlatnoInfoOblastiCanvas.Height;
         int SirkaCanvas = (int)PlatnoInfoOblastiCanvas.Width;

         // Smazání obsahu plátna pro možnost nového vykreslení
         PlatnoInfoOblastiCanvas.Children.Clear();

         // Vytvoření a vykreslení šipek na plátno pokud je k dispozici více stránek grafu pro vykreslení
         if (MaximalniPocetStranGrafu > 1)
         {
            // Není vykreslena první stránka (je možnost posunout se doleva v grafu)
            if (CisloStrankyGrafu > 0)
            {
               // Levá šipka pro možnost měnit číslo stránky grafu (pohyb v grafu)
               Path LevaSipka = new Path
               {
                  Stroke = BarvaZluta,
                  Fill = BarvaCervena,
                  StrokeThickness = 5,
                  Data = Geometry.Parse("m0,40 l40,-40 l0,20 l50,0 l0,40 l-50,0 l0,20 l-40,-40")
               };

               // Přidání události pro možnost kliknutí na šipku
               LevaSipka.MouseDown += LevaSipkaProStrankyGrafu_MouseDown;

               // Vykreslení šipky na plátno
               PlatnoInfoOblastiCanvas.Children.Add(LevaSipka);
               Canvas.SetRight(LevaSipka, 140);
               Canvas.SetBottom(LevaSipka, 20);
            }

            // Není vykreslena poslední stránka (je možnot posunout se doprava v grafu)
            if (CisloStrankyGrafu < MaximalniPocetStranGrafu - 1) 
            {
               // Pravá šipka pro možnost měnit číslo stránky grafu (pohyb v grafu)
               Path PravaSipka = new Path
               {
                  Stroke = BarvaZluta,
                  Fill = BarvaCervena,
                  StrokeThickness = 5,
                  Data = Geometry.Parse("m0,40 l0,20 l50,0 l0,20 l40,-40 l-40,-40 l0,20 l-50,0 l0,20")
               };

               // Přidání události pro možnost kliknutí na šipku
               PravaSipka.MouseDown += PravaSipkaProStrankyGrafu_MouseDown;

               // Vykreslení šipky na plátno
               PlatnoInfoOblastiCanvas.Children.Add(PravaSipka);
               Canvas.SetRight(PravaSipka, 20);
               Canvas.SetBottom(PravaSipka, 20);
            } 
         }

         // Vypsání popisků osy X 
         for (int i = 1; i <= PopiskyOsyX.Count; i++)
         {
            // Vytvoření popisku
            Label popisek = new Label
            {
               FontSize = 18,
               Content = PopiskyOsyX[i - 1],
               Foreground = Brushes.DarkRed
            };

            // Naklonění textu
            RotateTransform rotace = new RotateTransform(45);
            popisek.RenderTransform = rotace;

            // Vykreslení popisku na plátno
            PlatnoInfoOblastiCanvas.Children.Add(popisek);
            Canvas.SetLeft(popisek, SouradniceX_PrvniSloupec + ((i - 1) * OdstupHodnotX));
            Canvas.SetTop(popisek, 0);
         }
      }

      /// <summary>
      /// Vykreslení informační bubliny pro vybraný sloupec reprezentující shrnutá data určitých záznamů.
      /// </summary>
      /// <param name="Sloupec">Vybraný sloupec, jehož data jsou vykreslena</param>
      private void VykresliInfoBublinu(BlokZaznamu Sloupec)
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)PlatnoInfoOblastiCanvas.Height;
         int SirkaCanvas = (int)PlatnoInfoOblastiCanvas.Width;
         int SirkaNazev = Sloupec.Nazev.Length * 10;

         // Určení barvy pro vykreslení informační tabulky dle toho, zda se jedná o příjmy či výdaje
         Brush Barva;
         if (Sloupec.kategorie == KategoriePrijemVydaj.Prijem)
            Barva = BarvaZelena;
         else
            Barva = BarvaCervena;

         // Smazání obsahu plátna pro možnost nového vykreslení
         PlatnoInfoOblastiCanvas.Children.Clear();

         // Pomocná proměnná
         string slovo = "";
         if (Sloupec.kategorie == KategoriePrijemVydaj.Prijem)
            slovo = "příjmů";
         else
            slovo = "výdajů";


         // Okraje
         Rectangle okraje = new Rectangle
         {
            Fill = Barva,
            Width = SirkaCanvas,
            Height = VyskaCanvas
         };

         // Pozadí informačního okna
         Rectangle pozadi = new Rectangle
         {
            Fill = Controller.BarvaPozadi,
            Width = SirkaCanvas - 2,
            Height = VyskaCanvas - 2
         };

         // Oddělení názvu
         Rectangle deliciCara = new Rectangle
         {
            Fill = Barva,
            Width = SirkaCanvas,
            Height = 1
         };

         // Přepážky
         Rectangle prepazka = new Rectangle { Fill = Barva, Width = 1, Height = VyskaCanvas / 3 };
         Rectangle prepazka2 = new Rectangle { Fill = Barva, Width = 1, Height = VyskaCanvas / 3 };

         // Název kategorie
         Label nazev = new Label
         {
            Content = Sloupec.Nazev,
            FontSize = 24
         };

         // Popisek informující zda se jedná o příjem nebo o výdaj
         Label PrijemVydaj = new Label { FontSize = 24 };

         // Rozmezí datumů pro který je graf vykreslen
         Label datum = new Label
         {
            Content = String.Format("\t{0}.{1}.{2}\t\t->\t{3}.{4}.{5}", Sloupec.DatumPocatecni.Day, Sloupec.DatumPocatecni.Month,
            Sloupec.DatumPocatecni.Year, Sloupec.DatumKoncove.Day, Sloupec.DatumKoncove.Month, Sloupec.DatumKoncove.Year),
            FontSize = 20
         };

         // Pokud se vykresluje informační okno pro záznamy z 1 dne vypíše se pouze toto datum (nikoli interval datumů)
         if (Sloupec.DatumPocatecni == Sloupec.DatumKoncove)
         {
            datum.Content = String.Format("\t\t{0}.{1}.{2}", Sloupec.DatumPocatecni.Day, Sloupec.DatumPocatecni.Month, Sloupec.DatumPocatecni.Year);
         }

         // Informace o celkové hodnotě
         Label hodnota = new Label
         {
            Content = "Celková hodnota " + slovo + " je " + Sloupec.CelkovaHodnota.ToString() + " Kč.",
            FontSize = 22
         };

         // Počet záznamů v bloku 
         Label pocet = new Label
         {
            Content = "Počet záznamů: " + Sloupec.PocetZaznamu.ToString(),
            FontSize = 18
         };


         // Určení popisku a barvy na základě zda se jedná o příjem nebo o výdaj
         if (Sloupec.kategorie == KategoriePrijemVydaj.Prijem)
         {
            PrijemVydaj.Content = "PŘÍJMY";
            PrijemVydaj.Foreground = BarvaZelena;
         }
         else
         {
            PrijemVydaj.Content = "VÝDAJE";
            PrijemVydaj.Foreground = BarvaCervena;
         }


         // Přidání okrajů na plátno
         PlatnoInfoOblastiCanvas.Children.Add(okraje);
         Canvas.SetLeft(okraje, 0);
         Canvas.SetTop(okraje, 0);

         // Přidání pozadí na plátno
         PlatnoInfoOblastiCanvas.Children.Add(pozadi);
         Canvas.SetLeft(pozadi, 1);
         Canvas.SetTop(pozadi, 1);

         // Přidání dělící čáry na plátno
         PlatnoInfoOblastiCanvas.Children.Add(deliciCara);
         Canvas.SetLeft(deliciCara, 0);
         Canvas.SetTop(deliciCara, VyskaCanvas / 3 - 1);

         // Přidání názvu na plátno
         PlatnoInfoOblastiCanvas.Children.Add(nazev);
         Canvas.SetLeft(nazev, 4);
         Canvas.SetTop(nazev, 6);

         // Přidání data na plátno
         PlatnoInfoOblastiCanvas.Children.Add(datum);
         Canvas.SetLeft(datum, SirkaNazev + 135);
         Canvas.SetTop(datum, 6);

         // Přidání popisku na plátno
         PlatnoInfoOblastiCanvas.Children.Add(PrijemVydaj);
         Canvas.SetRight(PrijemVydaj, 11);
         Canvas.SetTop(PrijemVydaj, 6);

         // Přidání přepážky na plátno
         PlatnoInfoOblastiCanvas.Children.Add(prepazka);
         PlatnoInfoOblastiCanvas.Children.Add(prepazka2);
         Canvas.SetLeft(prepazka, SirkaNazev + 34);
         Canvas.SetTop(prepazka, 1);
         Canvas.SetRight(prepazka2, 200);
         Canvas.SetTop(prepazka2, 1);

         // Přidání celkové hodnoty na plátno
         PlatnoInfoOblastiCanvas.Children.Add(hodnota);
         Canvas.SetLeft(hodnota, 24);
         Canvas.SetTop(hodnota, VyskaCanvas / 3 + 20);

         // Přidání počtu záznamů na plátno
         PlatnoInfoOblastiCanvas.Children.Add(pocet);
         Canvas.SetRight(pocet, 16);
         Canvas.SetBottom(pocet, 6);
      }

      /// <summary>
      /// Metoda pro vykreslení informativního výpisu o celkových příjmech nebo výdajích.
      /// </summary>
      /// <param name="PrijemNeboVydaj">Rozhodnutí zda vykreslit informace o celkových příjmech nebo výdajích</param>
      private void VykresliInfoCelkovehoPrehledu(KategoriePrijemVydaj PrijemNeboVydaj)
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)PlatnoInfoOblastiCanvas.Height;
         int SirkaCanvas = (int)PlatnoInfoOblastiCanvas.Width;

         // Smazání obsahu plátna pro možnost nového vykreslení
         PlatnoInfoOblastiCanvas.Children.Clear();

         // Vykreslení informativních vět o celkových příjmech nebo výdajích
         if (PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
         {
            // Popisek pro přímy v zobrazené v grafu
            Label ZobrazenePrijmy = new Label
            {
               Content = String.Format("Celkové příjmy zobrazené v grafu: {0} Kč", VratCelkovouHodnotuBloku(ZobrazeneBlokyPrijmu, KategoriePrijemVydaj.Prijem)),
               FontSize = 30,
               Foreground = Brushes.Green
            };

            // Popisek pro přímy ve zvoleném období
            Label CelkovePrijmy = new Label
            {
               Content = String.Format("Celkové příjmy ve vybraném období: {0} Kč", VratCelkovouHodnotuBloku(KolekceBlokuZaznamu_Prijmy, KategoriePrijemVydaj.Prijem)),
               FontSize = 30,
               Foreground = Brushes.Green
            };

            // Vykreslení popisků na plátno
            PlatnoInfoOblastiCanvas.Children.Add(ZobrazenePrijmy);
            PlatnoInfoOblastiCanvas.Children.Add(CelkovePrijmy);
            Canvas.SetLeft(ZobrazenePrijmy, 20);
            Canvas.SetTop(ZobrazenePrijmy, 30);
            Canvas.SetLeft(CelkovePrijmy, 20);
            Canvas.SetTop(CelkovePrijmy, 80);
         }
         else
         {
            // Popisek pro výdaje v zobrazené v grafu
            Label ZobrazeneVydaje = new Label
            {
               Content = String.Format("Celkové výdaje zobrazené v grafu: {0} Kč", VratCelkovouHodnotuBloku(ZobrazeneBlokyVydaju, KategoriePrijemVydaj.Vydaj)),
               FontSize = 30,
               Foreground = Brushes.Red
            };

            // Popisek pro výdaje ve zvoleném období
            Label CelkoveVydaje = new Label
            {
               Content = String.Format("Celkové výdaje ve vybraném období: {0} Kč", VratCelkovouHodnotuBloku(KolekceBlokuZaznamu_Vydaje, KategoriePrijemVydaj.Vydaj)),
               FontSize = 30,
               Foreground = Brushes.Red
            };

            // Vykreslení popisků na plátno
            PlatnoInfoOblastiCanvas.Children.Add(ZobrazeneVydaje);
            PlatnoInfoOblastiCanvas.Children.Add(CelkoveVydaje);
            Canvas.SetLeft(ZobrazeneVydaje, 20);
            Canvas.SetTop(ZobrazeneVydaje, 30);
            Canvas.SetLeft(CelkoveVydaje, 20);
            Canvas.SetTop(CelkoveVydaje, 80);
         }
      }

      /// <summary>
      /// Metoda pro výpočet celkové hodnoty bloků v kolekci.
      /// </summary>
      /// <param name="KolekceBloku">Kolekce bloků, jejíhž hodota se počítá</param>
      /// <param name="PrijmyNeboVydaje">Zvolení zda se sčítají příjmy nebo výdaje</param>
      /// <returns></returns>
      private double VratCelkovouHodnotuBloku(ObservableCollection<BlokZaznamu> KolekceBloku, KategoriePrijemVydaj PrijmyNeboVydaje)
      {
         // Pomocná proměnná
         double CelkovaHodnota = 0;

         // Postupné projití všech bloků pro sečtení hodnoty
         foreach (BlokZaznamu blok in KolekceBloku)
         {
            // Pokud je záznam zvolené kategorie, přičte se její hodnota k celkové sčítané hodnotě
            if (blok.kategorie == PrijmyNeboVydaje)
               CelkovaHodnota += blok.CelkovaHodnota;
         }

         // Návratová hodnota
         return CelkovaHodnota;
      }



      /// <summary>
      /// Obsluha události při změně zaškrtávacího pole pro možnost nastavení nebo zrušení zobrazení příjmů v grafu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VykresleniPrijmuCheck_Click(object sender, RoutedEventArgs e)
      {
         // Převedení zpět na původní datový typ
         CheckBox checkBox = sender as CheckBox;

         // Nastavení příznakové proměnné dle toho, zda je zaškrtávací políčko zatrhnuto
         if (checkBox.IsChecked == true)
            VykreslitPrijmy = true;
         else
            VykreslitPrijmy = false;

         // Aktualizace vykreslení grafu
         AktualizujVykresleniGrafu();
      }

      /// <summary>
      /// Obsluha události při změně zaškrtávacího pole pro možnost nastavení nebo zrušení zobrazení výdajů v grafu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VykresleniVydajuCheck_Click(object sender, RoutedEventArgs e)
      {
         // Převedení zpět na původní datový typ
         CheckBox checkBox = sender as CheckBox;

         // Nastavení příznakové proměnné dle toho, zda je zaškrtávací políčko zatrhnuto
         if (checkBox.IsChecked == true)
            VykreslitVydaje = true;
         else
            VykreslitVydaje = false;

         // Aktualizace vykreslení grafu
         AktualizujVykresleniGrafu();
      }

      /// <summary>
      /// Obsluha události při kliknutí na pravou šipku. 
      /// Zvýší se číslo stránky a aktualizuje se vykreslení grafu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PravaSipkaProStrankyGrafu_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Pokud je vykreslena poslední stránka grafu, událot se neobslouží
         if (CisloStrankyGrafu == MaximalniPocetStranGrafu - 1)
            return;

         // Změna čísla stránky s aktualizací vykreslení grafu
         CisloStrankyGrafu++;
         AktualizujVykresleniGrafu();
         return;
      }

      /// <summary>
      /// Obsluha události při kliknutí na levou šipku. 
      /// Sníží se číslo stránky a aktualizuje se vykreslení grafu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void LevaSipkaProStrankyGrafu_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Pokud je vykreslena první stránka grafu, událost se neobslouží
         if (CisloStrankyGrafu == 0)
            return;

         // Změna čísla stránky s aktualizací vykreslení grafu
         CisloStrankyGrafu--;
         AktualizujVykresleniGrafu();
         return;
      }


      /// <summary>
      /// Obsluha události při odjetí kurzoru myši ze sloupce.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Vydaj_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno)
         {
            // Vykreslení spodní oblasti grafu na plátno
            PlatnoInfoOblastiCanvas.Children.Clear();
            VykresliOblastPodGrafem();

            // Nastavení příznakové proměnné pro možnost dalšího vykreslení
            InfoVykresleno = false;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na vybraný sloupec.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Vydaj_MouseMove(object sender, MouseEventArgs e)
      {
         if (!InfoVykresleno)
         {
            // Převedení objektu na obdélník
            Rectangle sloupec = sender as Rectangle;

            // Zjištění indexu vybraného sloupce odpovídajícího bloku v kolekci bloků
            int index = int.Parse(sloupec.Name.Substring(7));

            // Zobrazení informačního okna pro vybraný objekt (sloupec)
            VykresliInfoBublinu(ZobrazeneBlokyVydaju[index]);

            // Nastavení příznakové proměnné pro možnost dalšího vykreslení
            InfoVykresleno = true;
         }
      }

      /// <summary>
      /// Obsluha události při odjetí kurzoru myši ze sloupce.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Prijem_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno)
         {
            // Vykreslení spodní oblasti grafu na plátno
            PlatnoInfoOblastiCanvas.Children.Clear();
            VykresliOblastPodGrafem();

            // Nastavení příznakové proměnné pro možnost dalšího vykreslení
            InfoVykresleno = false;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na vybraný sloupec.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Prijem_MouseMove(object sender, MouseEventArgs e)
      {
         if (!InfoVykresleno)
         {
            // Převedení objektu na obdélník
            Rectangle sloupec = sender as Rectangle;

            // Zjištění indexu vybraného sloupce odpovídajícího bloku v kolekci bloků
            int index = int.Parse(sloupec.Name.Substring(7));

            // Zobrazení informačního okna pro vybraný objekt (sloupec)
            VykresliInfoBublinu(ZobrazeneBlokyPrijmu[index]);

            // Nastavení příznakové proměnné pro možnost dalšího vykreslení
            InfoVykresleno = true;
         }
      }

      /// <summary>
      /// Obsluha události při odjetí kurzoru myši na textový popisek Zobrazit výdaje.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VykreslitVydaje_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InformativniVetyVykresleny)
         {
            // Vykreslení oblasti pod grafem
            VykresliOblastPodGrafem();

            // Nastavení příznakové proměnné pro možnost dalšího vykreslení
            InformativniVetyVykresleny = false;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na textový popisek Zobrazit výdaje.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VykreslitVydaje_MouseMove(object sender, MouseEventArgs e)
      {
         if (!InformativniVetyVykresleny)
         {
            // Vykreslení informativního okna o celkových výdajích
            VykresliInfoCelkovehoPrehledu(KategoriePrijemVydaj.Vydaj);

            // Nastavení příznakové proměnné pro možnost dalšího vykreslení
            InformativniVetyVykresleny = true;
         }
      }

      /// <summary>
      /// Obsluha události při odjetí kurzoru myši na textový popisek Zobrazit příjmy.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VykreslitPrijmy_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InformativniVetyVykresleny)
         {
            // Vykreslení oblasti pod grafem
            VykresliOblastPodGrafem();

            // Nastavení příznakové proměnné pro možnost dalšího vykreslení
            InformativniVetyVykresleny = false;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na textový popisek Zobrazit příjmy.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VykreslitPrijmy_MouseMove(object sender, MouseEventArgs e)
      {
         if (!InformativniVetyVykresleny)
         {
            // Vykreslení informativního okna o celkových příjmech
            VykresliInfoCelkovehoPrehledu(KategoriePrijemVydaj.Prijem);

            // Nastavení příznakové proměnné pro možnost dalšího vykreslení
            InformativniVetyVykresleny = true;
         }
      }

      /// <summary>
      /// Aktualizace vykreslení grafu pro zvolené zobrazení při výběru zobrazení roků.
      /// </summary>
      /// <param name="sender">Zvolená objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Roky_Checked(object sender, RoutedEventArgs e)
      {
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Roky;
         VykresliOvladaciPrvky();
         VytvorBlokyGrafu();
         AktualizujVykresleniGrafu();
      }

      /// <summary>
      /// Aktualizace vykreslení grafu pro zvolené zobrazení při výběru zobrazení měsíců.
      /// </summary>
      /// <param name="sender">Zvolená objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Mesice_Checked(object sender, RoutedEventArgs e)
      {
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Mesice;
         VykresliOvladaciPrvky();
         VytvorBlokyGrafu();
         AktualizujVykresleniGrafu();
      }

      /// <summary>
      /// Aktualizace vykreslení grafu pro zvolené zobrazení při výběru zobrazení týdnů.
      /// </summary>
      /// <param name="sender">Zvolená objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Tydny_Checked(object sender, RoutedEventArgs e)
      {
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Tydny;
         VykresliOvladaciPrvky();
         VytvorBlokyGrafu();
         AktualizujVykresleniGrafu();
      }

      /// <summary>
      /// Aktualizace vykreslení grafu pro zvolené zobrazení při výběru zobrazení dnů.
      /// </summary>
      /// <param name="sender">Zvolená objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Dny_Checked(object sender, RoutedEventArgs e)
      {
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Dny;
         VykresliOvladaciPrvky();
         VytvorBlokyGrafu();
         AktualizujVykresleniGrafu();
      }

      /// <summary>
      /// Obsluha události při změně vybraného roku.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void RokKoncovyVyber_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení zvoleného objektu na původní datový typ
            ComboBox Box = sender as ComboBox;

            // Kontrola zda počáteční rok je menší než koncový
            if ((Box.SelectedIndex + 2019) < VybranyRok)
            {
               Box.SelectedIndex = VybranyKoncovyRok - 2019;
               throw new ArgumentException("Koncový rok musí být větší než počáteční!");
            }

            // Načtení roku dle zvoleného indexu v rozbalovacím okně
            VybranyKoncovyRok = Box.SelectedIndex + 2019;

            // Aktualizace vykreslení grafu s novými daty dle výběru
            VytvorBlokyGrafu();
            AktualizujVykresleniGrafu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      /// <summary>
      /// Obsluha události při změně vybraného roku.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void RokVyber_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení zvoleného objektu na původní datový typ
            ComboBox Box = sender as ComboBox;

            // Kontrola zda je počáteční rok je menší než koncový v případě zobrazení roků v grafu
            if (((Box.SelectedIndex + 2019) > VybranyKoncovyRok) && (ZobrazeniPrehledu == ZvoleneZobrazeniPrehledu.Roky))
            {
               Box.SelectedIndex = VybranyRok - 2019;
               throw new ArgumentException("Počáteční rok nesmí být větší než koncový!");
            }

            // Načtení roku dle zvoleného indexu v rozbalovacím okně
            VybranyRok = Box.SelectedIndex + 2019;

            // Aktualizace vykreslení grafu s novými daty dle výběru
            VytvorBlokyGrafu();
            AktualizujVykresleniGrafu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      /// <summary>
      /// Obsluha události při změně vybraného měsíce.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void MesicVyber_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení zvoleného objektu na původní datový typ
            ComboBox Box = sender as ComboBox;

            // Kontrolní podmínka pro případ, že uživatel zadá budoucí měsíc
            if (VybranyRok == DateTime.Now.Year && (Box.SelectedIndex + 1) > DateTime.Now.Month)
            {
               Box.SelectedIndex = VybranyMesic - 1;
               throw new ArgumentException("Nelze zobrazit data z budoucnosti!");
            }

            // Načtení měsíce dle zvoleného indexu v rozbalovacím okně
            VybranyMesic = Box.SelectedIndex + 1;

            // Aktualizace vykreslení grafu s novými daty dle výběru
            VytvorBlokyGrafu();
            AktualizujVykresleniGrafu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      /// <summary>
      /// Obsluha události při změně zadaného koncového data pro výběr dat k zobrazení grafu Kategorie.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumKoncove_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení vybraného objektu na původní datový typ
            DatePicker Datum = sender as DatePicker;

            // Načtení zadaného do pomocné proměnné
            DateTime NacteneDatum = Validace.NactiDatum(Datum.SelectedDate);

            // Ošetření pro případ že uživatel zadá koncové datum menší než je počáteční
            if (NacteneDatum < PocatecniDatum)
            {
               Datum.SelectedDate = KoncoveDatum;
               throw new ArgumentException("Koncové datum je menší než počáteční!");
            }
            else
            {
               // Načtení zadaného data do interní proměnné
               KoncoveDatum = NacteneDatum;
            }

            // Nastavení grafu na první stránku
            CisloStrankyGrafu = 0;

            // Aktualizace vykreslení grafu s novým koncovým datem pro výběr dat k zobrazení
            VytvorBlokyGrafu();
            AktualizujVykresleniGrafu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Obsluha události při změně zadaného počátečního data pro výběr dat k zobrazení grafu Kategorie.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumPocatecni_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení vybraného objektu na původní datový typ
            DatePicker Datum = sender as DatePicker;

            // Načtení zadaného do pomocné proměnné
            DateTime NacteneDatum = Validace.NactiDatum(Datum.SelectedDate);

            // Ošetření pro případ že uživatel zadá počáteční datum větší než je koncové
            if (NacteneDatum > KoncoveDatum)
            {
               Datum.SelectedDate = PocatecniDatum;
               throw new ArgumentException("Počáteční datum je větší než koncové!");
            }
            else
            {
               // Načtení zadaného data do interní proměnné
               PocatecniDatum = NacteneDatum;
            }

            // Nastavení grafu na první stránku
            CisloStrankyGrafu = 0;

            // Aktualizace vykreslení grafu s novým počátečním datem pro výběr dat k zobrazení
            VytvorBlokyGrafu();
            AktualizujVykresleniGrafu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

   }
}
