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
   /// Třída pro grafické zpracování položek včetně vykreslení pomocných prvků.
   /// Třída zpracovává kolekci položek a vytváří grafickou stránku pro vykreslení na plátno. 
   /// </summary>
   class GrafickePolozky
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      
      /// <summary>
      /// Interní kolekce pro zpracování dat.
      /// </summary>
      public ObservableCollection<Polozka> SeznamPolozek { get; private set; }

      /// <summary>
      /// Kolekce vybraných dat vykreslovaných na 1 stránku seznamu.
      /// </summary>
      public ObservableCollection<Polozka> PolozkyNaJedneStrance { get; private set; }

      /// <summary>
      /// Konstanta udávající počet vykreslených položek, které se vejdou na 1 stránku.
      /// </summary>
      public const int MaxPolozekNaStranku = 5;

      /// <summary>
      /// Grafický prvek zobrazující vykreslenou celou stránku seznamu, tedy určitý počet položek na 1 stránce.
      /// </summary>
      public StackPanel StrankaPolozek { get; private set; }

      /// <summary>
      /// Plátno pro vykreslení grafické reprezentace seznamu položek.
      /// </summary>
      public Canvas PlatnoPolozek { get; set; }

      /// <summary>
      /// Plátno pro vykreslení informační bubliny vybrané položky.
      /// Plátno je předáváno v konstruktoru, tedy instance uchovává plátno v okně, ve kterém byla instance třídy vytvořena.
      /// </summary>
      public Canvas InfoCanvas { get; set; }

      /// <summary>
      /// Pomocná proměnná pro uchování minulého vybraného bloku ze seznamu pro možnost zrušit jeho označení při výběru jiného bloku.
      /// </summary>
      private StackPanel OznacenyBlok;

      /// <summary>
      /// Interní proměnná pro identifikaci vykreslované stránky
      /// </summary>
      private int CisloStranky;

      /// <summary>
      /// Interní proměnná pro uložení celkového počtu stran v závislosti na počtu dat v kolekci.
      /// </summary>
      private int MaximalniPocetStran;

      /// <summary>
      /// Příznakový bit informující zda je vykresleno informační okno pro vybranou položku.
      /// </summary>
      private byte InfoVykresleno;



      /// <summary>
      /// Konstrukto pro grafické zpracování položek.
      /// </summary>
      /// <param name="PlatnoProSeznamPolozek">Plátno pro vykreslení seznamu položek</param>
      /// <param name="InfoBublina">Plátno pro vykreslení informační bubliny</param>
      public GrafickePolozky(Canvas PlatnoProSeznamPolozek, Canvas InfoBublina)
      {
         // Načtení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Načtení plátna pro vykreslení do interních proměnných
         PlatnoPolozek = PlatnoProSeznamPolozek;
         InfoCanvas = InfoBublina;

         // Úvodní inicializace interních proměnných
         // Úvodní inicializace interních kolekcí a proměných
         CisloStranky = 0;
         MaximalniPocetStran = 1;
         InfoVykresleno = 0;
         this.StrankaPolozek = new StackPanel();
         this.SeznamPolozek = new ObservableCollection<Polozka>();
         PolozkyNaJedneStrance = new ObservableCollection<Polozka>();
         OznacenyBlok = new StackPanel();
      }



      /// <summary>
      /// Aktualizace dat v interní kolekci položek.
      /// </summary>
      /// <param name="KolekcePolozek">Položky určené ke grafickému zpracování</param>
      public void ObnovKolekciPolozek(ObservableCollection<Polozka> KolekcePolozek)
      {
         // Načtení kolekce položek určených k vykreslení do interní proměnné
         SeznamPolozek = KolekcePolozek;

         // Určení počtu stran pro výpis dat z kolekce
         MaximalniPocetStran = (int)Math.Ceiling((double)this.SeznamPolozek.Count / (double)MaxPolozekNaStranku);
      }

      /// <summary>
      /// Opětovné vykreslení určité stránky seznamu položek v grafické podobě.
      /// </summary>
      public void AktualizujVykreslovanouStranu()
      {
         // Kontrola proti překročení minimálního čísla stránky (číslo stránky pro vykreslení bude menší než 0)
         if (CisloStranky < 0)
            CisloStranky = 0;

         // Zrušení označení vybrané položky
         Controller.ZrusOznaceniPolozky();

         // Výběr určitého počtu položek pro vykreslení 1 stránky seznamu
         VyberPolozkyNaStranku();

         // Kontrola zda jsou na zadané stránce alespoň nějaké položky k vykreslení (zabezpečení změny stránky v případě vymazání všech položek uživatelem na zobrazované stránce)
         if (PolozkyNaJedneStrance.Count == 0)
         {
            this.CisloStranky--;
            VyberPolozkyNaStranku();
         }

         // Vytvoření grafické reprezentace položek na 1 stránce
         VytvorGrafickouStrankuPolozek();

         // Vykreslení vytvořené stránky na plátno
         PlatnoPolozek.Children.Clear();
         PlatnoPolozek.Children.Add(StrankaPolozek);
         Canvas.SetLeft(StrankaPolozek, 0);
         Canvas.SetTop(StrankaPolozek, 0);
      }


      /// <summary>
      /// Metoda pro vytvoření interní kolekce obsahující pouze položky vykreslované na konkrétní stránku.
      /// </summary>
      private void VyberPolozkyNaStranku()
      {
         // Určení indexu položky pro výběr na základě počtu již vykreslených stran
         int StartIndex = CisloStranky * MaxPolozekNaStranku;

         // Smazání kolekce pro možnost přidání nových dat
         PolozkyNaJedneStrance.Clear();

         // Pokud v kolekci je více položek než se vejde na 1 stránku, vybere se pouze maximální počet na stránku
         if ((StartIndex + MaxPolozekNaStranku) <= SeznamPolozek.Count)
         {
            // Postupné přidání určitého počtu položek do kolkece pro vykreslení 1 strany
            for (int index = StartIndex; index < (StartIndex + MaxPolozekNaStranku); index++)
            {
               PolozkyNaJedneStrance.Add(SeznamPolozek[index]);
            }
         }
         // Pokud v kolekci zbýva jen několik položek pro vykreslení, vybere se daný počet na poslední stránku
         else
         {
            // Postupné přidání určitého počtu položek do kolkece pro vykreslení 1 strany
            for (int index = StartIndex; index < SeznamPolozek.Count; index++)
            {
               PolozkyNaJedneStrance.Add(SeznamPolozek[index]);
            }
         }
      }

      /// <summary>
      /// Vytvoření grafické reprezentace položek na 1 stránce a seskupení těchto položek do interní kolekce.
      /// </summary>
      private void VytvorGrafickouStrankuPolozek()
      {
         // Smazání obsahu stránky za účelem vykreslení nových dat
         this.StrankaPolozek.Children.Clear();

         // Vytvoření StackPanel pro seskupení grafických prvků na 1 stránce
         StackPanel Stranka = new StackPanel { Orientation = Orientation.Vertical };

         // Postupné vytvoření grafické reprezentace položky a přidání do kolkece grafických položek
         foreach (Polozka polozka in PolozkyNaJedneStrance)
         {
            // Vytvoření bloku reprezentující 1 položku na stránce
            StackPanel GrafPolozka = new StackPanel
            {
               Orientation = Orientation.Vertical,
               Background = Controller.BarvaPozadi
            };

            // Název položky
            Label nazev = new Label
            {
               Content = polozka.Nazev + ":",
               FontSize = 17
            };

            // Hodnota položky
            Label cena = new Label
            {
               Content = polozka.Cena.ToString() + " Kč",
               FontSize = 18
            };

            // Oddělovací čára
            Rectangle deliciCara = new Rectangle
            {
               Width = 200,
               Height = 2,
               Fill = Brushes.DarkBlue
            };

            // Přidání prvků do bloku položky
            GrafPolozka.Children.Add(nazev);
            GrafPolozka.Children.Add(cena);
            GrafPolozka.Children.Add(deliciCara);

            // Uložení indexu položky v seznamu pro následnou identifikaci konrétní položky
            GrafPolozka.Name = "obr" + PolozkyNaJedneStrance.IndexOf(polozka).ToString();

            // Přidání událostí pro grafický blok určený pro vykreslení stránky položek
            GrafPolozka.MouseDown += GrafPolozka_MouseDown;
            GrafPolozka.MouseMove += GrafPolozka_MouseMove;
            GrafPolozka.MouseLeave += GrafPolozka_MouseLeave;

            // Přidání bloku na stránku
            Stranka.Children.Add(GrafPolozka);
         }


         /* Doplnění stránky prázdnými bloky pro zajištění stálé velikosti stránky z důvodu pohodlnějšího ovládání kolečkem myši */

         // Počet prázdných bloků potřebných pro doplnění stránky
         int PocetPrazdnychBloku = MaxPolozekNaStranku - PolozkyNaJedneStrance.Count;

         // Pomocná proměnná pro uchování výšky vytvořeného bloku
         int VyskaPrazdnehoBloku = (int)Math.Floor(360.0 / MaxPolozekNaStranku);

         // Přidání určitého počtu prázdných bloků pro dpolnění stránky na celkovou velikost
         for (int i = 0; i < PocetPrazdnychBloku; i++)
         {
            StackPanel prazdnyBlok = new StackPanel
            {
               Background = Controller.BarvaPozadi,
               Height = VyskaPrazdnehoBloku,
               Width = 200
            };
            Stranka.Children.Add(prazdnyBlok);
         }


         // Vytvoření popisku pro označení aktuální stránky
         Label OznaceniStranky = new Label
         {
            FontSize = 12,
            Content = "Strana " + (CisloStranky + 1).ToString() + " z " + MaximalniPocetStran.ToString(),
            Foreground = Brushes.Red
         };

         // Přidání popisu do bloku stránky
         Stranka.Children.Add(OznaceniStranky);

         // Uložení indexu stránky pro identifikaci dat a orientaci v celkovém seznamu
         Stranka.Name = "str" + CisloStranky.ToString();

         // Přidání události pro stránku graficky vykreslených dat
         Stranka.MouseWheel += Stranka_MouseWheel;

         // Uložení grafické reprezentace stránky do interní proměnné
         this.StrankaPolozek.Children.Add(Stranka);
      }

      /// <summary>
      /// Metoda pro vykreslení informační bubliny předané položky.
      /// </summary>
      /// <param name="polozka">Položka pro vykreslení</param>
      private void VykresliInfoBublinu(Polozka polozka)
      {
         // Uložení rozměrů plátna do pomocných proměných
         double Vyska = InfoCanvas.Height;
         double Sirka = InfoCanvas.Width;

         //Smazání předchozího obsahu
         InfoCanvas.Children.Clear();

         // Vytvoření bloku reprezentující informační okno 1 položky
         StackPanel GrafPolozkaInfo = new StackPanel
         {
            Orientation = Orientation.Vertical,
            Background = Brushes.Ivory,
            Width = Sirka,
            Height = Vyska
         };

         // Kategorie položky
         Label kategorie = new Label
         {
            Content = Zaznam.NazvyKategorii[(int)polozka.KategoriePolozky],
            FontSize = 17
         };

         // Podtržení 
         Rectangle cara = new Rectangle
         {
            Width = Sirka,
            Height = 1,
            Fill = Brushes.Red
         };

         // Popis položky
         TextBox popis = new TextBox
         {
            Text = polozka.Popis,
            FontSize = 15,
            Background = Brushes.Ivory,
            TextWrapping = TextWrapping.Wrap,
            Width = Sirka - 2,
            Height = Vyska - 20
         };

         // Pokud popis neobsahuje text, zobrazí se informativní popisek
         if (popis.Text.Length == 0)
            popis.Text = "Žádný popis";

         // Přidání prvku do informačního bloku položky
         GrafPolozkaInfo.Children.Add(kategorie);
         GrafPolozkaInfo.Children.Add(cara);
         GrafPolozkaInfo.Children.Add(popis);

         // Vykreslení informačního bloku na plátno
         InfoCanvas.Children.Add(GrafPolozkaInfo);
         Canvas.SetLeft(GrafPolozkaInfo, 0);
         Canvas.SetTop(GrafPolozkaInfo, 0);
      }


      /// <summary>
      /// Událost vyvolaná při pohybu kolečka myši pro celou stránku.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Stranka_MouseWheel(object sender, MouseWheelEventArgs e)
      {
         // Převedení vybraného objektu zpět na StackPanel
         StackPanel stranka = sender as StackPanel;

         // Odstranění prefixu "str" z názvu bloku
         string IndexStrany = stranka.Name.Substring(3);

         // Identifikace aktuálně vykreslené strany seznamu na základě čísla uloženého v názvu StackPanelu
         int AktualniStranka = int.Parse(IndexStrany);

         // Pokud je vykreslena první stránka seznamu a uživatel pohne kolečkem myši nahoru, nic se nestane
         if (AktualniStranka == 0 && (e.Delta > 0))
         {
            return;
         }

         // Pokud je vykreslena první stránka seznamu a uživatel pohne kolečkem myši dolů, 
         // změní se číslo vykreslované stránky a aktualizuje se vykreslení (vykreslí se nová stránka)
         else if (AktualniStranka == 0 && (e.Delta <= 0))
         {
            CisloStranky++;
            AktualizujVykreslovanouStranu();
            return;
         }

         // Pokud je vykreslena poslední stránka seznamu a uživatel pohne kolečkem myši dolů, nic se nestane
         if ((AktualniStranka + 1) == MaximalniPocetStran && (e.Delta <= 0))
         {
            return;
         }

         // Pokud je vykreslena poslední stránka seznamu a uživatel pohne kolečkem myši nahorů, 
         // změní se číslo vykreslované stránky a aktualizuje se vykreslení (vykreslí se nová stránka)
         else if ((AktualniStranka + 1) == MaximalniPocetStran && (e.Delta > 0))
         {
            CisloStranky--;
            AktualizujVykreslovanouStranu();
            return;
         }

         // Pokud uživatel pohnul kolečkem myši nahorů vykreslí se nová stránka v reakci na změnu čísla stránky
         if (e.Delta > 0)
         {
            CisloStranky--;
            AktualizujVykreslovanouStranu();
            return;
         }

         // Pokud uživatel pohnul kolečkem myši dolů vykreslí se nová stránka v reakci na změnu čísla stránky
         else if (e.Delta <= 0)
         {
            CisloStranky++;
            AktualizujVykreslovanouStranu();
            return;
         }
      }

      /// <summary>
      /// Obsluha události při odjetí myši z konkrétního bloku reprezentující 1 položku na stránce.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void GrafPolozka_MouseLeave(object sender, MouseEventArgs e)
      {
         // Ošetření pro případ že infobulbina není určena k vykreslení
         if (InfoCanvas == null)
            return;

         // Smazání informační bubliny
         if (InfoVykresleno == 1)
         {
            // Smazání interní proměnné uchovávající informační bublinu
            InfoCanvas.Children.Clear();

            // Nastavení příznakové proměnné pro možnost dalšího vykreslování
            InfoVykresleno = 0;
         }
      }

      /// <summary>
      /// Obsluha události při najetí myši na konkrétní blok reprezentující 1 položku na stránce.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void GrafPolozka_MouseMove(object sender, MouseEventArgs e)
      {
         // Ošetření pro případ že infobulbina není určena k vykreslení
         if (InfoCanvas == null)
            return;

         // Vykreslení informační bubliny
         if (InfoVykresleno == 0)
         {
            // Převedení zvoleného objektu zpět na StackPanel
            StackPanel blok = sender as StackPanel;

            // Odstranění prefixu "obr" z názvu bloku
            string IndexPolozky = blok.Name.Substring(3);

            // Identifikace položky na základě indexu objektu (Zjištění o jakou položku se jedná) a vytvoření informační buliny dané položky
            VykresliInfoBublinu(PolozkyNaJedneStrance[(int)Validace.NactiCislo(IndexPolozky)]);

            // Nastavení příznakové proměnné pro zamezení opětovného vykreslování
            InfoVykresleno = 1;
         }
      }

      /// <summary>
      /// Obsluha události při kliknutí na konkrétní blok reprezentující 1 položku na stránce.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void GrafPolozka_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Převedení zvoleného objektu zpět na StackPanel
         StackPanel blok = sender as StackPanel;

         // Barevné vyznačení vybraného objektu
         blok.Background = Brushes.Orange;

         // Zrušení barevného vyznačení předchozího vybraného objektu
         OznacenyBlok.Background = Controller.BarvaPozadi;

         // Uložení nově označeného objektu do pomocné proměnné pro možnost následného zrušení jeho označení při označení jiného objektu
         OznacenyBlok = blok;

         // Odstranění prefixu "obr" z názvu bloku
         string IndexPolozky = blok.Name.Substring(3);

         // Identifikace položky na základě indexu objektu -> Zjištění o jakou položku se jedná (přiřazení do VybranaPolozka)
         Controller.VyberPolozku(PolozkyNaJedneStrance[(int)Validace.NactiCislo(IndexPolozky)]);
      }

   }
}
