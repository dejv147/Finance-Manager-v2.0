using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Calculator;       // Jmený prostor obsahující projekt Kalkulacka
using Microsoft.Win32;  // Jmenný prostor pro možnost uložení dat do souboru

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
   /// Třída představuje Controller aplikace Správce Financí.
   /// Controller slouží pro propojení logických tříd se třídami obsluhující okenní formuláře čímž zajišťuje strukturu MVC architektury aplikace.
   /// Třída funguje jako Singleton, nelze tedy vytvářet její instance. Instance existuje pouze 1 a lze ji získat statickou metodou VratInstanciControlleru().
   /// </summary>
   class SpravceFinanciController
   {
      /// <summary>
      /// Interní proměnná pro uložení nastavené barvy pozadí.
      /// </summary>
      private Brush BarvaPozadiAplikace;

      /// <summary>
      /// Barva pozadí pro nastavení pozadí všech oken aplikace.
      /// </summary>
      public Brush BarvaPozadi
      {
         get
         {
            return BarvaPozadiAplikace;
         }
         set
         {
            // Načtení nastavované hodnoty
            BarvaPozadiAplikace = value;

            // Aktualizace pozadí hlavního okna
            HlavniOkno.HlavniOknoGrid.Background = BarvaPozadiAplikace;
         }
      }

      /// <summary>
      /// Instance třídy pro metody volané při obsluze událostí vyvolaných v oknech aplikace.
      /// </summary>
      public ObsluhyUdalosti obsluhyUdalosti { get; private set; }

      /// <summary>
      /// Instance hlavního okna aplikace.
      /// </summary>
      public MainWindow HlavniOkno { get; private set; }

      /// <summary>
      /// Instance časovače pro vyvolání metody v určitém časovém intervalu.
      /// </summary>
      private DispatcherTimer CasovacSpousteniCasu;

      /// <summary>
      /// Instance třídy pro práci s grafickými prvky aplikace.
      /// </summary>
      private GrafickePrvky grafickePrvky;

      /// <summary>
      /// Instance třídy spravující důležité funce a data pro chod aplikace.
      /// </summary>
      private SpravceAplikace spravceAplikace;

      /// <summary>
      /// Instance třídy pro práci s daty (načítání a ukládání dat do souborů)
      /// </summary>
      private ZpracovaniDat zpracovaniDat;

      /// <summary>
      /// Interní proměnná uchovávající referenci na záznam, který je označen uživatelem.
      /// </summary>
      private Zaznam VybranyZaznam;

      /// <summary>
      /// Interní proměnná uchovávající referenci na položku, která je označena uživatelem.
      /// </summary>
      private Polozka VybranaPolozka;

      /// <summary>
      /// Instance třídy pro grafické zpracování kolekce záznamů pro možnost vykreslení těchto záznamů do požadovaného okna aplikace.
      /// </summary>
      private GrafickeZaznamy GrafickySeznamZaznamu;

      /// <summary>
      /// Instance třídy pro grafické zpracování kolekce položek pro možnost vykreslení těchto položek do požadovaného okna aplikace.
      /// </summary>
      private GrafickePolozky GrafickySeznamPolozek;

      /// <summary>
      /// Interní kolekce záznamů pro možnost zobrazení v otevřených oknech aplikace.
      /// </summary>
      private ObservableCollection<Zaznam> AktualneZobarovaneZaznamy;

      /// <summary>
      /// Interní kolekce položek pro možnost zobrazení v otevřených oknech aplikace.
      /// </summary>
      private ObservableCollection<Polozka> AktualneZobrazovanePolozky;



      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Nastavení kontroléru jako Singleton */

      /// <summary>
      /// Privátní konstruktor třídy pro zamezení vytváření více instancí Conrolleru.
      /// Konstruktor provádí inicializaci interních proměnných včetně všech tříd, které spravuje.
      /// </summary>
      private SpravceFinanciController() { }
      
      /// <summary>
      /// Statická proměnná pro uložení instance této třídy
      /// </summary>
      private static SpravceFinanciController instance = null;

      /// <summary>
      /// Statická metoda pro předání instance třídy Controller
      /// </summary>
      /// <returns>Instance třídy</returns>
      public static SpravceFinanciController VratInstanciControlleru()
      {
         // Pokud instance nebyla dosud vytvořena (první volání funkce) vytvoří se nová instance
         if (instance == null)
            instance = new SpravceFinanciController();
            
         // Předání instance třídy
         return instance;
      }



      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro správu aplikace */

      /// <summary>
      /// Metoda pro úvodní inicializaci vlastností kontroléru.
      /// </summary>
      /// <param name="HlavniOkno">Instance hlavního okna</param>
      public void UvodniNastaveniKontroleru(MainWindow HlavniOkno)
      {
         // Uložení instance hlavního okna do interní proměnné
         this.HlavniOkno = HlavniOkno;

         // Inicializace pomocných tříd pro správu aplikace
         zpracovaniDat = new ZpracovaniDat();
         spravceAplikace = new SpravceAplikace();
         zpracovaniDat = new ZpracovaniDat();
         grafickePrvky = new GrafickePrvky();
         obsluhyUdalosti = new ObsluhyUdalosti();

         // Inicializace interních proměnných
         VybranyZaznam = null;
         AktualneZobarovaneZaznamy = new ObservableCollection<Zaznam>();
         AktualneZobrazovanePolozky = new ObservableCollection<Polozka>();
         
         // Načtení barvy pozadí do interní proměnné
         BarvaPozadi = HlavniOkno.HlavniOknoGrid.Background;
      }

      /// <summary>
      /// Metoda pro odhlášení uživatele při ukončování aplikace.
      /// </summary>
      public void UkonceniAplikace()
      {
         try
         {
            // Uložení dat do souboru a zrušení přihlášeného uživatele
            spravceAplikace.OdhlaseniUzivatele();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }



      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro správu hlavního okna aplikace */

      /// <summary>
      /// Metoda pro aktualizaci data a času zobrazované v hlavním okně aplikace.
      /// </summary>
      public void AktualizujDatumCas()
      {
         // Aktualizace digitálního zobrazení data a času v hlavním okně aplikace
         HlavniOkno.DigitalniHodinyLabel.Content = Hodiny.VratAktualniCas(0);
         HlavniOkno.DatumLabel.Content = Hodiny.VratAktualniDatum();

         // Aktualizace polohy ručiček analogových hodin zobrazovaných v hlavním okně aplikace
         HlavniOkno.VterinovaRucickaRotace.Angle = Hodiny.NastavUhelVterinoveRucicky();
         HlavniOkno.MinutovaRucickaRotace.Angle = Hodiny.NastavUhelMinutoveRucicky();
         HlavniOkno.HodinovaRucickaRotace.Angle = Hodiny.NastavUhelHodinoveRucicky();
      }

      /// <summary>
      /// Nastavení prvotního zobrazení při spuštění aplikace. 
      /// Tato metoda je volána při úvodní inicializaci hlavního okna aplikace.
      /// </summary>
      public void NastavUvodniZobrazeni()
      {
         try
         {
            // Inicializace časovače s nastavením spouštěního intervalu (vyvolá obsluhu události každých 50 ms)
            CasovacSpousteniCasu = new DispatcherTimer
            {
               Interval = TimeSpan.FromMilliseconds(50)
            };

            // Přiřazení obsluhy události časovači
            CasovacSpousteniCasu.Tick += obsluhyUdalosti.CasovacSpousteniCasu_Tick;

            // Spuštění časovače
            CasovacSpousteniCasu.Start();

            // Otevření přihlašovacího okna
            Prihlaseni_Window prihlaseni = new Prihlaseni_Window();
            prihlaseni.ShowDialog();

            // Kontrola přihlášení uživatele (pokud uživatel není přihlášen vykreslí se anonymní obrazovka)
            if (!spravceAplikace.KontrolaPrihlaseniUzivatele(VratJmenoPrihlasenehoUzivatele()))
            {
               OdhlasUzivatele();
               return;
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Vykreslení postranních MENU v hlavním okně při úvodním zobrazení (obě menu jsou sbalené).
      /// </summary>
      public void VykresliPostranniMenuHlavnihoOkna()
      {
         // Vykreslí se postranní MENU se skrytím ovládacích prvků
         VykresliLevePostraniMENU(true);
         VykresliPravePostraniMENU(true);
      }

      /// <summary>
      /// Vykreslení levého postranního MENU v hlavním okně aplikace.
      /// </summary>
      /// <param name="SkrytOvladaciPrvky">TRUE - Vykreslení menu se skrytými ovládacími prvky, FALSE - Vykreslí se menu včetně ovládacích prvků</param>
      public void VykresliLevePostraniMENU(bool SkrytOvladaciPrvky)
      {
         grafickePrvky.VykresliLeveMENU(HlavniOkno.LeveMENU_Canvas, HlavniOkno.Height, !SkrytOvladaciPrvky);
      }

      /// <summary>
      /// Vykreslení pravého postranního MENU v hlavním okně aplikace.
      /// </summary>
      /// <param name="SkrytOvladaciPrvky">TRUE - Vykreslení menu se skrytými ovládacími prvky, FALSE - Vykreslí se menu včetně ovládacích prvků</param>
      public void VykresliPravePostraniMENU(bool SkrytOvladaciPrvky)
      {
         grafickePrvky.VykresliPraveMENU(HlavniOkno.PraveMENU_Canvas, HlavniOkno.Height, !SkrytOvladaciPrvky);
      }

      /// <summary>
      /// Vykreslení informačního přehledu příjmů a výdajů v aktuálním měsíci.
      /// </summary>
      /// <param name="PlatnoIfnoBloku">Plátno pro vykreslení přehledu</param>
      public void VykresliInformacniBlokMesicnihoPrehledu(Canvas PlatnoIfnoBloku)
      {
         // Pomocné proměnné pro výpočet celkových příjmů a výdajů z aktuálního měsíce
         double Prijmy_hodnota = 0;
         double Vydaje_hodnota = 0;

         // Výpočet celkových příjmů a výdajů sečtením hodnot ze všech záznamů z aktuálního měsíce
         foreach (Zaznam zaznam in Vyhledavani.VratZaznamyAktualnihoMesice(spravceAplikace.KolekceDatPrihlasenehoUzivatele)) 
         {
            // Přičtení hodonty aktuálního záznamu k celkové hodnotě příjmů nebo výdajů (dle kategorie záznamu)
            if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
               Prijmy_hodnota += zaznam.Hodnota_PrijemVydaj;
            else
               Vydaje_hodnota += zaznam.Hodnota_PrijemVydaj;
         }

         // Vykreslení bloku přehledu
         grafickePrvky.VykresliInformacniPrehled(PlatnoIfnoBloku, Prijmy_hodnota, Vydaje_hodnota);
      }

      /// <summary>
      /// Vykreslení záznamů z aktuálního měsíce v hlavním okně aplikace.
      /// </summary>
      /// <param name="InstanceGrafickychZaznamu">Instance třídy pro správu grafické reprezentace záznamů v hlavním okně</param>
      public void VykresliSeznamZaznamuAktualnihoMesiceDoHlavnihoOkna(GrafickeZaznamy InstanceGrafickychZaznamu)
      {
         // Vytvoření nové instance třídy pro práci s grafickým seznamem záznamů
         GrafickySeznamZaznamu = InstanceGrafickychZaznamu;

         // Nastavení záznamů určených k zobrazení
         AktualneZobarovaneZaznamy = Vyhledavani.VratZaznamyAktualnihoMesice(spravceAplikace.KolekceDatPrihlasenehoUzivatele);

         // Vykreslení seznamu záznamů
         VykresliSeznamZaznamu();
      }




      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro práci s položkami */

      /// <summary>
      /// Přidání nové položky do interní kolekce aktuálně zpracovávaných položek. Včetně aktualizace vykreslení seznamu položek.
      /// </summary>
      /// <param name="polozka">Nová položka</param>
      public void PridejPolozkuDoInterniKolekce(Polozka polozka)
      {
         // Přidání položky
         AktualneZobrazovanePolozky.Add(polozka);

         // Aktualizace vykreslení sezanmu položek
         VykresliSeznamPolozek();
      }

      /// <summary>
      /// Odebrání vybrané položky z kolekce aktuálně zobrazovaných položek včetně aktualizace vykreslení seznamu položek.
      /// </summary>
      public void OdeberPolozkuZeZaznamu()
      {
         // Kontrola zda byla vybrána nějaká položka
         if (VybranaPolozka == null)
            throw new ArgumentException("Vyberte položku!");

         // Odebrání položky
         AktualneZobrazovanePolozky.Remove(VybranaPolozka);

         // Zrušení reference na již neexistující položku
         VybranaPolozka = null;

         // Aktualizace vykreslení seznamu položek
         VykresliSeznamPolozek();
      }

      /// <summary>
      /// Předání položek v interní pomocné kolekci zpracovávaných položek do kolekce položek vybraného záznamu.
      /// </summary>
      public void PridejZobrazovanePolozkyDoVybranehoZaznamu()
      {
         foreach (Polozka polozka in AktualneZobrazovanePolozky)
         {
            // Přidání nové položky do seznamu položek vybraného záznamu (pokud tato položky již v seznamu neexistuje -> nová položka)
            if (!VybranyZaznam.SeznamPolozek.Contains(polozka))
               VybranyZaznam.SeznamPolozek.Add(polozka);
         }
      }

      /// <summary>
      /// Nalezení vybrané položky v pomocné kolekci zpracovávaných položek.
      /// </summary>
      /// <param name="polozka">Zvolená položka</param>
      public void VyberPolozku(Polozka polozka)
      {
         foreach (Polozka p in AktualneZobrazovanePolozky)
         {
            if (p.Equals(polozka))
               VybranaPolozka = p;
         }
      }

      /// <summary>
      /// Zrušení označení vybrané položky pro zamezení nechtěného smazání.
      /// </summary>
      public void ZrusOznaceniPolozky()
      {
         VybranaPolozka = null;
      }

      /// <summary>
      /// Vrátí počet položek v interní kolekci určené k zobrazení položek do seznamu. 
      /// Interní kolekce představuje položky, které budou přidány vybranému záznamu.
      /// </summary>
      /// <returns>Počet aktuálně zpracovávaných položek</returns>
      public int VratPocetZobrazovanychPolozek()
      {
         return AktualneZobrazovanePolozky.Count;
      }

      /// <summary>
      /// Vykreslení seznamu položek v grafické podobě. 
      /// Metoda vykreslí položky v kolekci položek určených pro aktuální zobrazení na plátno předané v konstruktoru třídy spravující grafickou reprezentaci položek (GrafickePolozek).
      /// </summary>
      public void VykresliSeznamPolozek()
      {
         // Zrušení označení vybrané položky
         ZrusOznaceniPolozky();

         // Obnovení kolekce dat určených ke grafickému zpracování
         GrafickySeznamPolozek.ObnovKolekciPolozek(AktualneZobrazovanePolozky);

         // Aktualizace vykreslení graficky zpracovaných položek
         GrafickySeznamPolozek.AktualizujVykreslovanouStranu();
      }



      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro otevírání oken aplikace a práci s nimi */

      /// <summary>
      /// Otevření okna pro možnost přidat nebo upravit stávající položky vybraného záznamu.
      /// </summary>
      /// <param name="PridatNeboUpravit">1 - Přidat nové položky, 0 - Upravit stávající položky</param>
      public void OtevriOknoPridatUpravitPolozky(byte PridatNeboUpravit)
      {
         try
         {
            // Vytvoření instance okna pro přidání a upravení záznamu
            PridatUpravitPolozky_Window pridatUpravitPolozky = new PridatUpravitPolozky_Window();

            // Vytvoření třídy pro správu graficky reprezentovaných položek
            GrafickySeznamPolozek = new GrafickePolozky(pridatUpravitPolozky.SeznamPolozekCanvas, pridatUpravitPolozky.InfoPolozkaCanvas);

            // Úvodní nastavení okna pro přidání nových položek
            if (PridatNeboUpravit == 1)
            {
               // Pokud je okno otevřeno v řežimu přidání nových položek, vytvoří se nová kolekce položek 
               VybranyZaznam.SeznamPolozek = new ObservableCollection<Polozka>();
               AktualneZobrazovanePolozky = new ObservableCollection<Polozka>();

               // Úvodní nastavení okna  
               pridatUpravitPolozky.UvodniNastaveni(true);

               // Otevření okna
               pridatUpravitPolozky.ShowDialog();
            }

            // Úvodní nastavení okna pro úpravu stávajících položek položek
            else
            {
               // Načtení položek k úpravě do interní pomocné kolekce
               AktualneZobrazovanePolozky = VybranyZaznam.SeznamPolozek;

               // Úvodní nastavení okna 
               pridatUpravitPolozky.UvodniNastaveni(false);

               // Úvodní vykreslení seznamu položek
               VykresliSeznamPolozek();

               // Otevření ona pro přidání nebo úpravu záznamu
               pridatUpravitPolozky.ShowDialog();
            } 
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Otevření okna pro úpravu poznámky.
      /// </summary>
      /// <param name="PoznamkovyBlok">Poznámkový blok pro nastavení textu poznámky</param>
      public void OtevriOknoPoznamky(TextBox PoznamkovyBlok)
      {
         try
         {
            PoznamkaWindow poznamkaWindow = new PoznamkaWindow(PoznamkovyBlok);
            poznamkaWindow.ShowDialog();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Zobrazení okna pro možnost upravit nebo přidat nový záznam.
      /// </summary>
      /// <param name="PridatNeboUpravit">1 - Přidat záznam, 0 - Upravit záznam</param>
      public void OtevriOknoPridatUpravitZaznam(byte PridatNeboUpravit)
      {
         try
         {
            // Vytvoření instance okna pro přidání a upravení záznamu
            PridatUpravitZaznam_Window pridatUpravitWindow = new PridatUpravitZaznam_Window();

            // Úvodní nastavení okna pro práci se záznamem
            if (PridatNeboUpravit == 1)
            {
               // Pokud je okno otevřeno v řežimu přidání nového záznamu, vytvoří se nový záznam 
               VybranyZaznam = new Zaznam();

               // Úvodní nastavení okna  
               pridatUpravitWindow.UvodniNastaveniRezimuPridavani();

               // Otevření ona pro přidání nového záznamu
               pridatUpravitWindow.ShowDialog();

               // Přidání nově vytvořeného (nastaveného) záznamu do kolekce dat přihlášeného uživatele
               PridatVybranyZaznamJakoNovy(); 
            }
            else
            {
               // Úvodní nastavení okna včetně načtení parametrů upravovaného záznamu
               pridatUpravitWindow.UvodniNastaveniRezimuUpravovani(VybranyZaznam);

               // Otevření ona pro úpravu záznamu
               pridatUpravitWindow.ShowDialog();
            } 
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Zobrazení okna zobrazující grafy statisticky zpracovaných záznamů.
      /// </summary>
      public void ZobrazStatistiku()
      {
         try
         {
            // Vytvoření instance okna Statistika
            Statistika_Window statistika = new Statistika_Window();

            // Úvodní nastavení prvků pro zobrazení statistických grafů
            statistika.UvodniNastaveniGrafu();

            // Vytvoření instancí třídy pro zpracování a vykreslení grafů
            Statistika CasovyPrehled = new Statistika(statistika.CasovyPrehledCanvasGraf, statistika.CasovyPrehledCanvasOvladaciPrvky, statistika.CasovyPrehledCanvasInfo);
            Statistika PrehledKategorii = new Statistika(statistika.PrehledKategoriiCanvasGraf, statistika.PrehledKategoriiCanvasOvladaciPrvky, statistika.PrehledKategoriiCanvasInfo);

            // Úvodní vykreslení grafů
            CasovyPrehled.UvodniNastaveniCasovehoGrafu(spravceAplikace.KolekceDatPrihlasenehoUzivatele);
            PrehledKategorii.UvodniNastaveniGrafuKategorii(spravceAplikace.KolekceDatPrihlasenehoUzivatele);

            // Otevření okna Statistika
            statistika.ShowDialog();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Zobrazení okna pro možnost vyhledávání záznamů dle určitých kritérií.
      /// </summary>
      public void OtevriOknoVyhledat()
      {
         try
         {
            // Vytvoření instance okna Vyhledat
            Vyhledat_Window vyhledat_Window = new Vyhledat_Window();

            // Zrušení označení vybraného záznamu
            ZrusOznaceniZaznamu();

            // Uvedení pomocné kolekce záznamů k zobrazení do úvodního stavu
            AktualizujZobrazovaneZaznamy();

            // Vytvoření nové instance třídy pro práci s grafickým seznamem záznamů
            GrafickySeznamZaznamu = new GrafickeZaznamy(vyhledat_Window.SeznamZaznamuCANVAS, vyhledat_Window.InfoBublinaCanvas);

            // Inicializace úvodního nastavení okna
            vyhledat_Window.UvodniNastaveni();

            // Otevření okna Vyhledat
            vyhledat_Window.ShowDialog();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }  
      }

      /// <summary>
      /// Otevření okna zobrazující všechny příjmy nebo výdaje.
      /// </summary>
      /// <param name="PrijmyVydaje">Kategorie určující zda se zobrazí příjmy nebo výdaje</param>
      public void OtevriOknoZobrazitPrijmyVydaje(KategoriePrijemVydaj PrijmyVydaje)
      {
         try
         {
            // Vytvoření instance okna ZobrazitPrijmyVydaje
            ZobrazitPrijmyVydaje_Window prijmyVydaje_Window = new ZobrazitPrijmyVydaje_Window(PrijmyVydaje == KategoriePrijemVydaj.Prijem ? (byte)1 : (byte)0);

            // Zrušení označení vybraného záznamu
            ZrusOznaceniZaznamu();

            // Uvedení pomocné kolekce záznamů k zobrazení do úvodního stavu
            AktualizujZobrazovaneZaznamy();

            // Načtení příjmů nebo výdajů, dle požadovaného zobrazení
            AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(PrijmyVydaje);

            // Vytvoření nové instance třídy pro práci s grafickým seznamem záznamů
            GrafickySeznamZaznamu = new GrafickeZaznamy(prijmyVydaje_Window.SeznamZaznamuCANVAS, prijmyVydaje_Window.InfoBublinaCanvas);

            // Inicializace úvodního nastavení okna
            prijmyVydaje_Window.UvodniNastaveni();

            // Otevření okna pro zobrazení příjmů nebo výdajů
            prijmyVydaje_Window.ShowDialog();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Otevření okna pro exportování záznamů do souboru.
      /// </summary>
      public void OtevriOknoExportDat()
      {
         try
         {
            // Vytvoření instance okna pro export dat
            ImportExportDat_Window ExportWindow = new ImportExportDat_Window(0);

            // Zrušení označení vybraného záznamu
            ZrusOznaceniZaznamu();

            // Uvedení pomocné kolekce záznamů k zobrazení do úvodního stavu
            AktualizujZobrazovaneZaznamy();

            // Vytvoření nové instance třídy pro práci s grafickým seznamem záznamů
            GrafickySeznamZaznamu = new GrafickeZaznamy(ExportWindow.SeznamZaznamuProExportCanvas, null);

            // Úvodní vykreslení záznamů
            VykresliSeznamZaznamu();

            // Otevření okna pro export dat
            ExportWindow.ShowDialog();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda pro spuštění okenního formuláře představujícího jednoduchou kalkulačku.
      /// </summary>
      public void OtevriKalkulacku()
      {
         try
         {
            // Vytvoření instance projektu Calculator a otevření okna s kalkulačkou
            Calculator.MainWindow Kalkulacka = new Calculator.MainWindow();
            Kalkulacka.ShowDialog();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }



      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro práci s kategoriemi položek i záznamů */

      /// <summary>
      /// Nastavení jednotlivých prvků do rozbalovací nabídky. 
      /// Metoda vypíše všechny kategorie do rozbalovací nabídky. 
      /// </summary>
      /// <param name="RozbalovaciNabidka">ComboBox do kterého e vypíší všechny kategorie</param>
      public void NastavKategorieDoComboBoxu(ComboBox RozbalovaciNabidka)
      {
         // Smazání obsahu rozbalovací nabídky
         RozbalovaciNabidka.Items.Clear();

         // Nastavení jednotlivých prvků v rozbalovací nabídce
         for (int i = 1; i < VratPocetKategoriiZaznamu(); i++)
         {
            RozbalovaciNabidka.Items.Add(Zaznam.NazvyKategorii[i]);
         }
      }

      /// <summary>
      /// Zjištění počtu kategorií. Číslování začíná od 0, proto číslo kategorie = Počet - 1.
      /// </summary>
      /// <returns>Počet kategorií</returns>
      public int VratPocetKategoriiZaznamu()
      {
         return Enum.GetValues(typeof(Kategorie)).Length;
      }



      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro práci se záznamy */

      /// <summary>
      /// Vykreslení seznamu záznamů v grafické podobě. 
      /// Metoda vykreslí záznamy v kolekci záznamu určených pro aktuální zobrazení na plátno předané v konstruktoru třídy spravující grafickou reprezentaci záznamů (GrafickeZaznamy).
      /// </summary>
      public void VykresliSeznamZaznamu()
      {
         // Zrušení označení vybraného záznamu
         ZrusOznaceniZaznamu();

         // Obnovení kolekce dat určených ke grafickému zpracování
         GrafickySeznamZaznamu.ObnovKolekciZaznamu(AktualneZobarovaneZaznamy);

         // Aktualizace vykreslení seznamu záznamů
         GrafickySeznamZaznamu.AktualizujVykreslovanouStranu();
      }

      /// <summary>
      /// Metoda pro aktualizaci zobrazovaných dat do úvodního nastavení. 
      /// Kolekce dat určených k ozbrazení obsahuje všechny záznamy přihlášeného uživatele.
      /// </summary>
      public void AktualizujZobrazovaneZaznamy()
      {
         AktualneZobarovaneZaznamy = spravceAplikace.KolekceDatPrihlasenehoUzivatele;
      }

      /// <summary>
      /// Vyhledání záznamů s určitým názvem
      /// </summary>
      /// <param name="Nazev">Název záznamů</param>
      public void AktualizujZobrazovaneZaznamyDleNazvu(string Nazev)
      {
         // Načtení záznamů s požadovaným názvem
         AktualneZobarovaneZaznamy = Vyhledavani.VratZaznamyDleNazvu(AktualneZobarovaneZaznamy, Nazev);

         // Pokud není nalezen žádný odpovídající záznam, obnoví se kolekce zobrazovaných záznamů do úvodního nastavení
         if (AktualneZobarovaneZaznamy.Count == 0)
         {
            AktualizujZobrazovaneZaznamy();
            throw new ArgumentException("Nebyly nalezeny žádné záznamy se zadaným jménem!");
         }
      }

      /// <summary>
      /// Vyhledání záznamů v požadovaném období
      /// </summary>
      /// <param name="MIN">Počáteční datum</param>
      /// <param name="MAX">Koncové datum</param>
      public void AktualizujZobrazovaneZaznamyDleData(DateTime MIN, DateTime MAX)
      {
         // Načtení záznamů v zadaném časovém období
         AktualneZobarovaneZaznamy = Vyhledavani.VratZaznamyVCasovemRozmezi(AktualneZobarovaneZaznamy, MIN, MAX);

         // Pokud není nalezen žádný odpovídající záznam, obnoví se kolekce zobrazovaných záznamů do úvodního nastavení
         if (AktualneZobarovaneZaznamy.Count == 0)
         {
            AktualizujZobrazovaneZaznamy();
            throw new ArgumentException("Nebyly nalezeny žádné záznamy v požadovaném období!");
         }
      }

      /// <summary>
      /// Vyhledání záznamů s hodnotou v zadaném intervalu
      /// </summary>
      /// <param name="MIN">Minimální hodnota</param>
      /// <param name="MAX">Maximální hodnota</param>
      public void AktualizujZobrazovaneZaznamyDleHodnoty(double MIN, double MAX)
      {
         // Načtení záznamů s hodnotou v požadovaném rozmezí
         AktualneZobarovaneZaznamy = Vyhledavani.VratZaznamySHodnotouVRozmezi(AktualneZobarovaneZaznamy, MIN, MAX);

         // Pokud není nalezen žádný odpovídající záznam, obnoví se kolekce zobrazovaných záznamů do úvodního nastavení
         if (AktualneZobarovaneZaznamy.Count == 0)
         {
            AktualizujZobrazovaneZaznamy();
            throw new ArgumentException("Nebyly nalezeny žádné záznamy s požadovanou hodnotou!");
         }
      }

      /// <summary>
      /// Vyhledání záznamů v určité kategorii
      /// </summary>
      /// <param name="kategorie">Kategorie požadovaných záznamů</param>
      public void AktualizujZobrazovaneZaznamyDleKategorie(Kategorie kategorie)
      {
         // Načtení záznamů s požadovanou kategorií
         AktualneZobarovaneZaznamy = Vyhledavani.VratZaznamyDleKategorie(AktualneZobarovaneZaznamy, kategorie);

         // Pokud není nalezen žádný odpovídající záznam, obnoví se kolekce zobrazovaných záznamů do úvodního nastavení
         if (AktualneZobarovaneZaznamy.Count == 0)
         {
            AktualizujZobrazovaneZaznamy();
            throw new ArgumentException("Nebyly nalezeny žádné záznamy v požadované kategorii!");
         }
      }

      /// <summary>
      /// Vyhledání všech příjmů nebo výdajů
      /// </summary>
      /// <param name="PrijemVydaj">Kategorie příjmů nebo výdajů</param>
      public void AktualizujZobrazovaneZaznamyDleKategoriePrijmuVydaju(KategoriePrijemVydaj PrijemVydaj)
      {
         // Načtení příjmů nebo výdajů
         AktualneZobarovaneZaznamy = PrijemVydaj == KategoriePrijemVydaj.Prijem ? Vyhledavani.VratPrijmy(AktualneZobarovaneZaznamy) : Vyhledavani.VratVydaje(AktualneZobarovaneZaznamy);

         // Pokud není nalezen žádný odpovídající záznam, obnoví se kolekce zobrazovaných záznamů do úvodního nastavení
         if (AktualneZobarovaneZaznamy.Count == 0)
         {
            AktualizujZobrazovaneZaznamy();
            string slovo = PrijemVydaj == KategoriePrijemVydaj.Prijem ? "příjmy" : "výdaje";
            throw new ArgumentException(String.Format("Nebyly nalezeny žádné {0}!", slovo));
         }
      }

      /// <summary>
      /// Vyhledání záznamů s požadovaným počtem položek
      /// </summary>
      /// <param name="MIN">Minimální počet položek</param>
      /// <param name="MAX">Maximální počet položek</param>
      public void AktualizujZobrazovaneZaznamyDlePoctuPolozek(int MIN, int MAX)
      {
         // Načtení záznamů s požadovaným počtem položek
         AktualneZobarovaneZaznamy = Vyhledavani.VratZaznamyDlePoctuPolozek(AktualneZobarovaneZaznamy, MIN, MAX);

         // Pokud není nalezen žádný odpovídající záznam, obnoví se kolekce zobrazovaných záznamů do úvodního nastavení
         if (AktualneZobarovaneZaznamy.Count == 0)
         {
            AktualizujZobrazovaneZaznamy();
            throw new ArgumentException("Nebyly nalezeny žádné záznamy s požadovaným počtem položek!");
         }
      }

      /// <summary>
      /// Vrátí počet záznamů v kolekci dat určené k aktuálnímu zobrazení v určitém okně.
      /// </summary>
      /// <returns>Počet aktuálně zobrazovaných záznamů</returns>
      public int VratPocetZobrazovanychZaznamu()
      {
         return AktualneZobarovaneZaznamy.Count();
      }

      /// <summary>
      /// Výpočet celkových příjmů a výdajů všech aktuálně zobrazovatelných záznamů.
      /// </summary>
      /// <returns>(Celkové příjmy, Celkové výdaje)</returns>
      public (double, double) VratPrijmyVydajeZobrazovanychZaznamu()
      {
         // Pomocné proměnné
         double Prijmy = 0;
         double Vydaje = 0;

         // Sečtení všech příjmů a výdajů v kolekci aktuálně zobrazovaných příjmů
         foreach (Zaznam zaznam in AktualneZobarovaneZaznamy)
         {
            if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
               Prijmy += zaznam.Hodnota_PrijemVydaj;
            else
            {
               Vydaje += zaznam.Hodnota_PrijemVydaj;
            }
         }

         // Návratová hodnota
         return (Prijmy, Vydaje);
      }


      /// <summary>
      /// Nalezení vybraného záznamu v kolekci dat přihlášeného uživatele na základě porovnání s předaným záznamem.
      /// </summary>
      /// <param name="zaznam">Vybraný záznam</param>
      public void VyberZaznam(Zaznam zaznam)
      {
         foreach (Zaznam z in spravceAplikace.KolekceDatPrihlasenehoUzivatele)
         {
            if (z.Equals(zaznam))
               VybranyZaznam = z;
         }
      }

      /// <summary>
      /// Zrušení označení vybraného záznamu pro zamezení nechtěného smazání.
      /// </summary>
      public void ZrusOznaceniZaznamu()
      {
         VybranyZaznam = null;
      }

      /// <summary>
      /// Smazání vybraného (dříve označeného) záznamu z kolekce dat přihlášeného uživatele.
      /// </summary>
      public void SmazZaznam()
      {
         try
         {
            // Kontrola zda byl vybrán nějaký zánam
            if (VybranyZaznam == null)
               throw new ArgumentException("Vyberte záznam!");

            // Zobrazení varovného okna s načtením zvolené volby
            MessageBoxResult VybranaVolba = MessageBox.Show("Opravdu chcete vybraný záznam odstranit?", "Upozornění", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Smazání vybrané položky v případě stisku tlačíka YES
            switch (VybranaVolba)
            {
               case MessageBoxResult.Yes:
                  spravceAplikace.SmazatZaznam(VybranyZaznam);    // Smazání záznamu
                  VybranyZaznam = null;                           // Zrušení reference na již neexistující záznam
                  break;

               case MessageBoxResult.No:
                  break;
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Přidání vybraného záznamu do kolekce dat přihlášenéh uživatele jako nový záznam.
      /// </summary>
      public void PridatVybranyZaznamJakoNovy()
      {
         // Přidání vybraného záznamu jako nový
         spravceAplikace.KolekceDatPrihlasenehoUzivatele.Add(VybranyZaznam);

         // Odstranění reference na nový záznam v interní proměnné (zrušení označení nového záznamu)
         ZrusOznaceniZaznamu();
      }

      /// <summary>
      /// Metoda přepíše parametry vybraného záznamu parametry předanými v konstruktoru.
      /// </summary>
      /// <param name="Nazev">Nový název vybraného záznamu</param>
      /// <param name="Datum">Nové datum vybraného záznamu</param>
      /// <param name="Hodnota">Nová hodnota vybraného záznamu</param>
      /// <param name="PrijemNeboVydaj">Přepis zda se jedná o příjem nebo výdaj vybraného záznamu</param>
      /// <param name="Poznamka">Nová poznámka vybraného záznamu</param>
      /// <param name="kategorie">Nová kategorie vybraného záznamu</param>
      public void UpravZaznam(string Nazev, DateTime Datum, double Hodnota, KategoriePrijemVydaj PrijemNeboVydaj, string Poznamka, Kategorie kategorie)
      {
         VybranyZaznam.Nazev = Nazev;
         VybranyZaznam.Datum = Datum;
         VybranyZaznam.Hodnota_PrijemVydaj = Hodnota;
         VybranyZaznam.PrijemNeboVydaj = PrijemNeboVydaj;
         VybranyZaznam.Poznamka = Poznamka;
         VybranyZaznam.kategorie = kategorie;
      }




      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro práci s uživateli */

      /// <summary>
      /// Metoda pro registraci nového uživatele do aplikace
      /// </summary>
      /// <param name="Jmeno">Jméno registrovaného uživatele</param>
      /// <param name="Heslo">Heslo registrovaného uživatele</param>
      /// <returns>TRUE - registrace proběhla úspěšně, FALSE - registrace neproběhla</returns>
      public bool RegistrujUzivatele(string Jmeno, string Heslo)
      {
         try
         {
            // Vytvoření nového uživatele
            spravceAplikace.VytvorNovehoUzivatele(Jmeno, Heslo);

            // Zobrazení informativního okna o úšpěšném provedení registrace nového uživatele do systému
            MessageBox.Show("Nový uživatel byl registrován.", "Registrace proběhla úspěšně", MessageBoxButton.OK, MessageBoxImage.Information);

            // Návratová hodnota informující o úspěšné registraci nového uživatele
            return true;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
         } 
      }

      /// <summary>
      /// Metoda pro přihlášení uživatele do aplikace. 
      /// Spustí se přihlašovací metoda ve správci aplikace, která načte požadovaného uživatele do interních proměnných pro možnost práce s jeho daty.
      /// </summary>
      /// <param name="Jmeno">Jméno uživatele</param>
      /// <param name="Heslo">Heslo uživatele</param>
      /// /// <returns>TRUE - přihlášení proběhlo úspěšně, FALSE - přihlášení neproběhlo</returns>
      public bool PrihlasUzivatele(string Jmeno, string Heslo)
      {
         try
         {
            // Přihlášení uživatele do aplikace
            spravceAplikace.PrihlaseniUzivatele(Jmeno, Heslo);

            // Zrušení plátna pro nepřihlášeného uživatele
            HlavniOkno.ResizeMode = ResizeMode.CanResize;
            HlavniOkno.NeprihlasenyUzivatel_Canvas.Children.Clear();
            HlavniOkno.NeprihlasenyUzivatel_Canvas.Visibility = Visibility.Collapsed;

            // Návratová hodnota informující o úspěšném přihlášení uživatele
            return true;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
         }
      }

      /// <summary>
      /// Metoda pro odhlášení uživatele z aplikace včetně vykreslení obrazovky pro nepřihlášeného uživatele.
      /// </summary>
      public void OdhlasUzivatele()
      {
         try
         {
            // Odhlášení uživatele z aplikace
            spravceAplikace.OdhlaseniUzivatele();

            // Nastavení anonymního módu hlavního okna (vykreslení obrazovky pro nepřihlášeného uživatele)
            HlavniOkno.ResizeMode = ResizeMode.NoResize;
            grafickePrvky.VykresliObrazovkuNeprihlasenehoUzivatele(HlavniOkno.NeprihlasenyUzivatel_Canvas, HlavniOkno.Width, HlavniOkno.Height);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda pro získání jména přihlášeného uživatele v textové formě.
      /// </summary>
      /// <returns>Jméno příhlášeného uživatele</returns>
      public string VratJmenoPrihlasenehoUzivatele()
      {
         return spravceAplikace.JmenoPrihlasenehoUzivatele;
      }


      /// <summary>
      /// Metoda pro kontrolu bezpečnosti hesla a vykreslení ukazatele síly hesla.
      /// </summary>
      /// <param name="Heslo">Heslo určené ke kontrole</param>
      /// <param name="UkazatelHesla">Plátno pro vykreslení síly hesla</param>
      public void ZkontrolujSiluHesla(string Heslo, Canvas UkazatelHesla)
      {
         bool BezpecnostSplnena;
         int PocetSplnenychPodminek;
         string BezpecnostniZprava;

         // Kontrola bezpečnosti hesla
         (BezpecnostSplnena, PocetSplnenychPodminek, BezpecnostniZprava) = Validace.KontrolaMinimalniBezpecnostiHesla(Heslo);

         // Vykreslení ukazatele síly hesla
         if (UkazatelHesla != null)
            grafickePrvky.VykresliUkazatelHesla(UkazatelHesla, PocetSplnenychPodminek, 6, 200, 20);

         // Pokud není splněna minimální bezpečnost hesla, vyvolá se vyjímka informující o nedostatku bezpečnosti hesla
         if (BezpecnostSplnena == false)
            throw new ArgumentException("Zadané heslo nesplňuje bezpečnostní požadavky! " + BezpecnostniZprava);
      }



      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro práci s poznámkovým blokem a pro nastavení poznámky záznamů */

      /// <summary>
      /// Nastavení poznámkového bloku
      /// </summary>
      /// <param name="Zobrazit">Příznakový bit: 0 - nevykreslovat, 1 - vykreslit</param>
      public void NastavPoznamkovyBlok()
      {
         // Pokud je poznámkový blok určen k zobrazení, provede se jeho inicializace
         if (VratZobrazeniPoznamky() == 1)
         {
            // Nastavení viditelnosti poznámkového bloku
            HlavniOkno.PoznamkovyBlokStackPanel.Visibility = Visibility.Visible;

            // Nastavení barvy poznámkového bloky dle zvolené barvy pozadí
            HlavniOkno.PoznamkovyBlokStackPanel.Background = BarvaPozadi;

            // Zobrazení textu poznámky přihlášeného uživatele do poznámkového bloku
            HlavniOkno.PoznamkovyBlokTextBox.Text = spravceAplikace.VratPoznamkuUzivatele();

            // Přidání události pro možnost reagovat na změnu textu v poznámkovém bloku
            HlavniOkno.PoznamkovyBlokTextBox.TextChanged += obsluhyUdalosti.PoznamkovyBlokTextBox_TextChanged;
         }
         else
            HlavniOkno.PoznamkovyBlokStackPanel.Visibility = Visibility.Collapsed;

      }

      /// <summary>
      /// Příznakový bit informující o nastavení viditelnosti daného uživatele
      /// </summary>
      /// <returns>1 - poznámky zobrazit, 0 - poznámky skrýt</returns>
      public byte VratZobrazeniPoznamky()
      {
         return spravceAplikace.VratZobrazeniPoznamky();
      }

      /// <summary>
      /// Nastavení příznakového bitu informující o nastavení viditelnosti poznámkového bloku přihlášeného uživatele.
      /// </summary>
      /// <param name="Volba">1 - poznámky zobrazit, 0 - poznámky skrýt</param>
      public void NastavZobrazeniPoznamky(byte Volba)
      {
         spravceAplikace.NastavZobrazeniPoznamky(Volba);
      }

      /// <summary>
      /// Nastavení textu poznámky přihlášeného uživatele
      /// </summary>
      /// <param name="TextPoznamky">Poznámka uživatele</param>
      public void NastavTextPoznamkyUzivateli(string TextPoznamky)
      {
         spravceAplikace.NastavPoznamkuUzivatele(TextPoznamky);
      }


      

      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /* Metody pro práci s daty (uložení, načítání, import, export) */

      /// <summary>
      /// Metoda pro přidání záznamů do kolekce přihlášeného uživatele načtených ze souboru ve formátu XML.
      /// Metoda načte záznamy ze souboru a jednotlivé záznamy porovná s již existujícími. Záznamy, které ještě neexistují se přidají do kolekce přihlášeného uživatele.
      /// </summary>
      /// <param name="NahraditStavajici">TRUE - Původní záznamy budou smazány a do kolekce budou přidány nové záznmy, FALSE - záznamy se přidají jako nové mezi stávající záznamy</param>
      public void ImportujZaznamy(bool NahraditStavajici)
      {
         // Pomocná kolekce pro načtené záznamy ze souboru
         ObservableCollection<Zaznam> NacteneZaznamy = new ObservableCollection<Zaznam>();

         try
         {
            // Vytvoření funkce pro načtení dat ze souboru s nastavením nabídky formátů pro uložení
            OpenFileDialog dialog = new OpenFileDialog
            {
               Filter = "Data files (*.xml)|*.xml|All files (*.*)|*.*"
            };

            // Zobrazení dialogu pro vybrání cesty k uložneí s návratovou hodnotou určující zda bylo uložení potvrzeno
            if (dialog.ShowDialog() == true)
            {
               // Kontrola zda byl zadán název souboru pro uložení
               if (string.IsNullOrWhiteSpace(dialog.FileName))
                  throw new ArgumentException("Zadejte název souboru!");

               // Volání metody pro načtení dat ze souboru
               NacteneZaznamy = zpracovaniDat.NactiDataUzivatele_XML(dialog.FileName);

               // Vytvoření pomocné proměnné reprezentující čítač úspěšně přidaných záznamů
               int CitacPridanychZaznamu = 0;

               // Smazání původních záznamů v kolekci přihlášeného uživatele pokud uživatel zvolil přepsání stávajících dat
               if (NahraditStavajici)
                  spravceAplikace.KolekceDatPrihlasenehoUzivatele.Clear();

               // Přidání nových záznamů do kolekce přihlášeného uživatele
               foreach (Zaznam zaznam in NacteneZaznamy)
               {
                  // Pokud přidávající záznam ještě neexistuje, přidá se do kolekce záznamů jako nový záznam
                  if (!spravceAplikace.KontrolaExistujicihoZaznamu(zaznam)) 
                  {
                     spravceAplikace.KolekceDatPrihlasenehoUzivatele.Add(zaznam);
                     CitacPridanychZaznamu++;
                  }
               }
               
               // Zobrazení informačního okna o výsledku přidání nových záznamů do kolekce včetně zobrazení počtu úspěšně přidaných záznamů
               MessageBox.Show(String.Format("Bylo přidáno {0} záznamů z celkových {1}. \nZáznamy, které již existují byly přeskočeny.", CitacPridanychZaznamu, NacteneZaznamy.Count),
                               "Import dat dokončen", MessageBoxButton.OK, MessageBoxImage.Information);

               // Aktualizace vykreslení hlavního okna aplikace
               HlavniOkno.AktualizujUvodniObrazovku();
            }

         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda pro export dat v kolekci aktuálně zobrazovaných dat (vybraná data v okně Vyhledat) do souboru ve zvolené formátu.
      /// </summary>
      public void ExportujZaznamy()
      {
         // Vytvoření funkce pro uložení dat do souboru s nastavením nabídky formátů pro uložení
         SaveFileDialog dialog = new SaveFileDialog
         {
            Filter = "Text files (*.txt)|*.txt|Separated files (*.csv)|*.csv|Data files (*.xml)|*.xml"
         };

         // Zobrazení dialogu pro vybrání cesty k uložneí s návratovou hodnotou určující zda bylo uložení potvrzeno
         if (dialog.ShowDialog() == true)
         {
            try
            {
               // Kontrola zda byl zadán název souboru pro uložení
               if (string.IsNullOrWhiteSpace(dialog.FileName))
                  throw new ArgumentException("Zadejte název souboru!");

               // Uložení souboru ve zvoleném formátu
               switch (dialog.FilterIndex)
               {
                  case 1: zpracovaniDat.UlozDataUzivatele_TXT(dialog.FileName, AktualneZobarovaneZaznamy); break;
                  case 2: zpracovaniDat.UlozDataUzivatele_CSV(dialog.FileName, AktualneZobarovaneZaznamy); break;
                  case 3: zpracovaniDat.UlozDataUzivatele_XML(dialog.FileName, AktualneZobarovaneZaznamy); break;
                  default: break;
               }
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
         }
      }

      /// <summary>
      /// Uložení dat aplikace do souboru.
      /// </summary>
      public void ProvedUlozeniDat()
      {
         spravceAplikace.UlozDataAplikace();
      }

      /// <summary>
      /// Metoda pro uložení dat aplikace do souboru XML.
      /// </summary>
      /// <param name="kolekce">Kolekce dat k uložení</param>
      public void UlozDataDoSouboru(ObservableCollection<Uzivatel> kolekce)
      {
         zpracovaniDat.UlozDataUzivateluDoSouboru(kolekce);
      }

      /// <summary>
      /// Metoda pro načtení dat aplikace ze souboru XML.
      /// </summary>
      /// <returns>Kolekce dat načtených ze souboru</returns>
      public ObservableCollection<Uzivatel> NactiDataUzivateluZeSouboru()
      {
         return zpracovaniDat.NactiDataUzivateluZeSouboru();
      }

   }
}
