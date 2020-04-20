using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
   /// Třída spravuje metody pro nastavení a úpravu grafických prvků. 
   /// Obsahuje metody pro nastavení tlačítek, grafické zpracování a vykreslení určitých částí aplikace včetně obrazovky pro nepřihlášeného uživatele.
   /// </summary>
   class GrafickePrvky
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      SpravceFinanciController Controller;

      /// <summary>
      /// Konstruktor třídy pro správu grafických prvků.
      /// </summary>
      public GrafickePrvky()
      {
         Controller = SpravceFinanciController.VratInstanciControlleru();
      }



      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Uložit
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoULOZIT(Button tlacitko)
      {
         tlacitko.Content = "Uložit";
         tlacitko.FontSize = 27;
         tlacitko.Width = 120;
         tlacitko.Height = 60;
         tlacitko.FontWeight = FontWeights.Bold;
         tlacitko.Background = Brushes.LimeGreen;
         tlacitko.BorderBrush = Brushes.Blue;
         tlacitko.IsDefault = true;
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Zrušit
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoZRUSIT(Button tlacitko)
      {
         tlacitko.Content = "Zrušit";
         tlacitko.FontSize = 27;
         tlacitko.Width = 120;
         tlacitko.Height = 60;
         tlacitko.FontWeight = FontWeights.Bold;
         tlacitko.Background = Brushes.OrangeRed;
         tlacitko.BorderBrush = Brushes.Blue;
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Přihlásit se
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení základních parametrů</param>
      public void NastavTlacitkoPRIHLASIT(Button tlacitko)
      {
         tlacitko.Content = "Přihlásit se";
         tlacitko.FontSize = 20;
         tlacitko.Width = 150;
         tlacitko.Height = 40;
         tlacitko.FontWeight = FontWeights.Bold;
         tlacitko.Background = Brushes.Aqua;
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Registrovat se
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoREGISTROVAT(Button tlacitko)
      {
         tlacitko.Content = "Registrovat se";
         tlacitko.FontSize = 16;
         tlacitko.Width = 130;
         tlacitko.Height = 30;
         tlacitko.FontWeight = FontWeights.Medium;
         tlacitko.Background = new SolidColorBrush(Color.FromRgb(0xDA, 0, 0));
         tlacitko.Foreground = Brushes.White;
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Zobrazit heslo
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoZOBRAZITHESLO(Button tlacitko)
      {
         tlacitko.Content = "Zobrazit heslo";
         tlacitko.FontSize = 16;
         tlacitko.FontWeight = FontWeights.Medium;
         tlacitko.Width = 130;
         tlacitko.Height = 27;
         tlacitko.Foreground = new SolidColorBrush(Color.FromRgb(191, 0, 26));
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Přidat
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoPRIDAT(Button tlacitko)
      {
         tlacitko.Content = "Přidat";
         tlacitko.FontSize = 28;
         tlacitko.Width = 100;
         tlacitko.Height = 50;
         tlacitko.Background = Brushes.GreenYellow;
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Odebrat
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoODEBRAT(Button tlacitko)
      {
         tlacitko.Content = "Odebrat";
         tlacitko.FontSize = 24;
         tlacitko.Width = 100;
         tlacitko.Height = 50;
         tlacitko.Background = new SolidColorBrush(Color.FromRgb(0xF1, 0x5C, 0x78));
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Přidat položky
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoPRIDATPOLOZKY(Button tlacitko)
      {
         tlacitko.Content = "Přidat \npoložky";
         tlacitko.FontSize = 24;
         tlacitko.HorizontalContentAlignment = HorizontalAlignment.Center;
         tlacitko.VerticalContentAlignment = VerticalAlignment.Center;
         tlacitko.Height = 100;
         tlacitko.Width = 150;
         tlacitko.Background = new SolidColorBrush(Color.FromRgb(0xF1, 0x5C, 0x78));
         tlacitko.BorderBrush = Brushes.Yellow;
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Přidat poznámku
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoPRIDATPOZNAMKU(Button tlacitko)
      {
         tlacitko.Content = "Přidat \npoznámku";
         tlacitko.FontSize = 22;
         tlacitko.HorizontalContentAlignment = HorizontalAlignment.Center;
         tlacitko.VerticalContentAlignment = VerticalAlignment.Center;
         tlacitko.Height = 100;
         tlacitko.Width = 150;
         tlacitko.Background = new SolidColorBrush(Color.FromRgb(0xF1, 0x5C, 0x78));
         tlacitko.BorderBrush = Brushes.Yellow;
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Upravit položky
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoUPRAVITPOLOZKY(Button tlacitko)
      {
         NastavTlacitkoPRIDATPOLOZKY(tlacitko);
         tlacitko.Content = "Upravit \npoložky";
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Upravit poznámku
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoUPRAVITPOZNAMKU(Button tlacitko)
      {
         NastavTlacitkoPRIDATPOZNAMKU(tlacitko);
         tlacitko.Content = "Upravit \npoznámku";
      }

      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocení stylů všech tlačítek se stejnou funkcí.
      /// Nastavení tlačítka: Info
      /// </summary>
      /// <param name="tlacitko">Tlačítko pro nastavení parametrů</param>
      public void NastavTlacitkoINFO(Button tlacitko)
      {
         tlacitko.Width = 70;
         tlacitko.Height = 30;
         tlacitko.FontSize = 18;
         tlacitko.FontStyle = FontStyles.Italic;
         tlacitko.Content = "Info";
         tlacitko.HorizontalContentAlignment = HorizontalAlignment.Center;
         tlacitko.Background = Brushes.DarkSalmon;
      }


      /// <summary>
      /// Nastavení základních parametrů tlačítka pro sjednocený styl a rozměr tlačitek v postranním menu. 
      /// Metoda nastaví rozměry tlačítka a velikost písma pro popisek tlačítka.
      /// </summary>
      /// <param name="Tlacitko">Tlačítko určené k nastavení parametrů</param>
      public void UpravTlacitkoProMENU(Button Tlacitko)
      {
         Tlacitko.Width = 100;
         Tlacitko.Height = 30;
         Tlacitko.FontSize = 18;
         Tlacitko.HorizontalContentAlignment = HorizontalAlignment.Center;
      }


      /// <summary>
      /// Vytvoření popisku s pro rozbalovací postranní menu ve zbaleném stavu. 
      /// Popisek obsahuje horizontální text MENU.
      /// </summary>
      /// <returns>Popisek s horizontálním textem MENU</returns>
      public Label VytvorPopisekMENU()
      {
         // Vytvoření popisku s horizontálním textem MENU
         Label popisek = new Label
         {
            Content = "M\nE\nN\nU",
            FontSize = 18,
            FontStyle = FontStyles.Oblique,
            FontWeight = FontWeights.SemiBold,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top
         };

         // Návratová hodnota
         return popisek;
      }


      /// <summary>
      /// Vykreslení levého postranního MENU.
      /// </summary>
      /// <param name="PlatnoLevehoMENU">Plátno pro vykreslení rozbalovacího postranního menu</param>
      /// <param name="VyskaPostrannihoMENU">Výška okna ve kterém je menu vykreslováno</param>
      /// <param name="ZobrazitOvladaciPrvky">TRUE - vykreslení rozbaleného menu včetně všech tlačítek, FALSE - vykreslení zbaleného menu</param>
      public void VykresliLeveMENU(Canvas PlatnoLevehoMENU, double VyskaPostrannihoMENU, bool ZobrazitOvladaciPrvky)
      {
         // Smazání obsahu plátna
         PlatnoLevehoMENU.Children.Clear();

         // Vykreslení levého postranního MENU se zobrazenými ovládacími prvky (rozbalené menu)
         if (ZobrazitOvladaciPrvky)
         {
            // Přidání události pro možnost reagovat na odjetí kurzoru myši z oblasti rozbaleného postranního menu
            PlatnoLevehoMENU.MouseLeave += Controller.obsluhyUdalosti.LeveMENU_MouseLeave;

            // Vytvoření obdélníku pro vykreslení pozadí rozbaleného menu
            Rectangle pozadi = new Rectangle
            {
               Fill = Brushes.Green,
               Width = 120,
               Height = VyskaPostrannihoMENU
            };

            // Vykreslení pozadí na plátno
            PlatnoLevehoMENU.Children.Add(pozadi);
            Canvas.SetLeft(pozadi, 0);
            Canvas.SetTop(pozadi, 0);

            // Vytvoření bloku pro vykreslení jména uživatele
            StackPanel prihlasenyUzivatel = new StackPanel
            {
               Orientation = Orientation.Vertical
            };

            // Titulek pro jméno uživatele
            Label titulek = new Label
            {
               Content = "Přihlášený uživatel: ",
               FontSize = 12,
               HorizontalContentAlignment = HorizontalAlignment.Center
            };

            // Jméno uživatele
            Label jmeno = new Label
            {
               Content = Controller.VratJmenoPrihlasenehoUzivatele(),
               FontSize = 16,
               FontWeight = FontWeights.Bold,
               Foreground = Brushes.LightGoldenrodYellow,
               HorizontalContentAlignment = HorizontalAlignment.Center
            };

            // Přidání prvků do bloku a vykreslení na plátno
            prihlasenyUzivatel.Children.Add(titulek);
            prihlasenyUzivatel.Children.Add(jmeno);
            PlatnoLevehoMENU.Children.Add(prihlasenyUzivatel);
            Canvas.SetLeft(prihlasenyUzivatel, 0);
            Canvas.SetTop(prihlasenyUzivatel, 5);


            // Vytvoření tlačítka pro přidání nového záznamu
            Button Pridat = new Button { Content = "Přidat", Background = Brushes.DarkSalmon };
            UpravTlacitkoProMENU(Pridat);

            // Vytvoření tlačítka pro odebrání záznamu 
            Button Odebrat = new Button { Content = "Odebrat", Background = Brushes.Red };
            UpravTlacitkoProMENU(Odebrat);

            // Vytvoření tlačítka pro vyhledávání záznamů
            Button Vyhledat = new Button { Content = "Vyhledat", Background = Brushes.DarkSalmon };
            UpravTlacitkoProMENU(Vyhledat);

            // Vytvoření tlačítka pro zobrazení příjmů
            Button Prijmy = new Button { Content = "Příjmy", Background = Brushes.LimeGreen };
            UpravTlacitkoProMENU(Prijmy);

            // Vytvoření tlačítka pro zobrazení výdajů
            Button Vydaje = new Button { Content = "Výdaje", Background = Brushes.Crimson };
            UpravTlacitkoProMENU(Vydaje);

            // Vytvoření tlačítka pro zobrazení statistických grafů
            Button Statistika = new Button { Content = "Statistika", Background = Brushes.DarkTurquoise };
            UpravTlacitkoProMENU(Statistika);


            // Přidání událostí pro možnost reagovat na stisk tlačítek zobrazených v levém rozbaleném postranním menu
            Pridat.Click += Controller.obsluhyUdalosti.PridatZaznam_Click;
            Odebrat.Click += Controller.obsluhyUdalosti.OdebratZaznam_Click;
            Vyhledat.Click += Controller.obsluhyUdalosti.Vyhledat_Click;
            Prijmy.Click += Controller.obsluhyUdalosti.ZobrazPrijmy_Click;
            Vydaje.Click += Controller.obsluhyUdalosti.ZobrazVydaje_Click;
            Statistika.Click += Controller.obsluhyUdalosti.Statistika_Click;


            // Vytvoření pomocných proměnných pro vykreslení tlačítek na plátno
            int VyskaTlacitka = (int)Pridat.Height;
            int Mezera = 15;

            // Vykreslení tlačítka Přidat na plátno
            PlatnoLevehoMENU.Children.Add(Pridat);
            Canvas.SetLeft(Pridat, 10);
            Canvas.SetTop(Pridat, 100);

            // Vykreslení tlačítka Odebrat na plátno
            PlatnoLevehoMENU.Children.Add(Odebrat);
            Canvas.SetLeft(Odebrat, 10);
            Canvas.SetTop(Odebrat, 100 + (VyskaTlacitka + Mezera));

            // Vykreslení tlačítka Vyhledat na plátno
            PlatnoLevehoMENU.Children.Add(Vyhledat);
            Canvas.SetLeft(Vyhledat, 10);
            Canvas.SetTop(Vyhledat, 100 + 2 * VyskaTlacitka + 4 * Mezera);

            // Vykreslení tlačítka Příjmy na plátno
            PlatnoLevehoMENU.Children.Add(Prijmy);
            Canvas.SetLeft(Prijmy, 10);
            Canvas.SetTop(Prijmy, 100 + 3 * VyskaTlacitka + 7 * Mezera);

            // Vykreslení tlačítka Výdaje na plátno
            PlatnoLevehoMENU.Children.Add(Vydaje);
            Canvas.SetLeft(Vydaje, 10);
            Canvas.SetTop(Vydaje, 100 + 4 * VyskaTlacitka + 8 * Mezera);

            // Vykreslení tlačítka Statistika na plátno
            PlatnoLevehoMENU.Children.Add(Statistika);
            Canvas.SetLeft(Statistika, 10);
            Canvas.SetTop(Statistika, 100 + 5 * VyskaTlacitka + 10 * Mezera);
         }

         // Vykreslení levého postranního MENU se skrytými ovládacími prvky (zbalené menu)
         else
         {
            // Přidání události pro možnost reagovat na najetí kurzoru myši na oblast zbaleného postranního menu
            PlatnoLevehoMENU.MouseMove += Controller.obsluhyUdalosti.LeveMENU_MouseMove;

            // Vykreslení zbaleného postranního menu včetně popisku
            VykresliSkryteMENU(PlatnoLevehoMENU, VyskaPostrannihoMENU, 1);
         }

      }

      /// <summary>
      /// Vykreslení pravého postranního MENU.
      /// </summary>
      /// <param name="PlatnoPravehoMENU">Plátno pro vykreslení rozbalovacího postranního menu</param>
      /// <param name="VyskaPostrannihoMENU">Výška okna ve kterém je menu vykreslováno</param>
      /// <param name="ZobrazitOvladaciPrvky">TRUE - vykreslení rozbaleného menu včetně všech tlačítek, FALSE - vykreslení zbaleného menu</param>
      public void VykresliPraveMENU(Canvas PlatnoPravehoMENU, double VyskaPostrannihoMENU, bool ZobrazitOvladaciPrvky)
      {
         // Smazání obsahu plátna
         PlatnoPravehoMENU.Children.Clear();

         // Vykreslení pravého postranního MENU se zobrazenými ovládacími prvky (rozbalené menu)
         if (ZobrazitOvladaciPrvky)
         {
            // Přidání události pro možnost reagovat na odjetí kurzoru myši z oblasti rozbaleného postranního menu
            PlatnoPravehoMENU.MouseLeave += Controller.obsluhyUdalosti.PraveMENU_MouseLeave;

            // Vytvoření obdélníku pro vykreslení pozadí rozbaleného menu
            Rectangle pozadi = new Rectangle
            {
               Fill = Brushes.Green,
               Width = 120,
               Height = VyskaPostrannihoMENU
            };

            // Vykreslení pozadí na plátno
            PlatnoPravehoMENU.Children.Add(pozadi);
            Canvas.SetRight(pozadi, 0);
            Canvas.SetTop(pozadi, 0);

            // Vytvoření tlačítka pro otevření kalkulačky
            Button kalkulacka = new Button { Content = "Kalkulačka", Background = Brushes.DeepPink };
            UpravTlacitkoProMENU(kalkulacka);

            // Vytvoření tlačítka pro import dat ze souboru
            Button import = new Button { Content = "Import dat", Background = Brushes.DarkSalmon };
            UpravTlacitkoProMENU(import);

            // Vytvoření tlačítka pro export dat
            Button export = new Button { Content = "Export dat", Background = Brushes.DarkSalmon };
            UpravTlacitkoProMENU(export);

            // Vytvoření tlačítka pro uložení zpracovávaných dat
            Button ulozit = new Button { Content = "Uložit", Background = Brushes.OrangeRed };
            UpravTlacitkoProMENU(ulozit);

            // Vytvoření tlačítka pro změnu nastavení aplikace
            Button nastaveni = new Button { Content = "Nastavení", Background = Brushes.DarkSalmon };
            UpravTlacitkoProMENU(nastaveni);
            nastaveni.FontSize = 16;

            // Vytvoření tlačítka pro otevření informačního okna
            Button Info = new Button();
            NastavTlacitkoINFO(Info);
            UpravTlacitkoProMENU(Info);

            // Vytvoření tlačítka pro odhlášení uživatele
            Button Odhlasit = new Button { Content = "Odhlásit se", Background = Brushes.DarkRed };
            UpravTlacitkoProMENU(Odhlasit);


            // Přidání událostí pro možnost reagoavt na stisk tlačítek
            kalkulacka.Click += Controller.obsluhyUdalosti.KalkulackaButton_Click;
            import.Click += Controller.obsluhyUdalosti.ImportDat_Click;
            export.Click += Controller.obsluhyUdalosti.ExportDat_Click;
            ulozit.Click += Controller.obsluhyUdalosti.Ulozit_Click;
            nastaveni.Click += Controller.obsluhyUdalosti.Nastaveni_Click;
            Info.Click += Controller.obsluhyUdalosti.InformaceButton_Click;
            Odhlasit.Click += Controller.obsluhyUdalosti.Odhlasit_Click;


            // Vytvoření pomocných proměnných pro vykreslení tlačítek na plátno
            int VyskaTlacitka = (int)kalkulacka.Height;
            int Mezera = 15;

            // Vykreslení tlačítka Kalkulačka na plátno
            PlatnoPravehoMENU.Children.Add(kalkulacka);
            Canvas.SetRight(kalkulacka, 10);
            Canvas.SetTop(kalkulacka, 100);

            // Vykreslení tlačítka Import na plátno
            PlatnoPravehoMENU.Children.Add(import);
            Canvas.SetRight(import, 10);
            Canvas.SetTop(import, 100 + VyskaTlacitka + Mezera);

            // Vykreslení tlačítka Export na plátno
            PlatnoPravehoMENU.Children.Add(export);
            Canvas.SetRight(export, 10);
            Canvas.SetTop(export, 100 + 2 * VyskaTlacitka + 2 * Mezera);

            // Vykreslení tlačítka Uložit na plátno
            PlatnoPravehoMENU.Children.Add(ulozit);
            Canvas.SetRight(ulozit, 10);
            Canvas.SetTop(ulozit, 100 + 3 * VyskaTlacitka + 3 * Mezera);

            // Vykreslení tlačítka Nastavení na plátno
            PlatnoPravehoMENU.Children.Add(nastaveni);
            Canvas.SetRight(nastaveni, 10);
            Canvas.SetBottom(nastaveni, 30 + 2 * VyskaTlacitka + 2 * Mezera);

            // Vykreslení tlačítka Info na plátno
            PlatnoPravehoMENU.Children.Add(Info);
            Canvas.SetRight(Info, 10);
            Canvas.SetBottom(Info, 30 + VyskaTlacitka + Mezera);

            // Vykreslení tlačítka Odhlášení na plátno
            PlatnoPravehoMENU.Children.Add(Odhlasit);
            Canvas.SetRight(Odhlasit, 10);
            Canvas.SetBottom(Odhlasit, 30);
         }

         // Vykreslení pravého postranního MENU se skrytými ovládacími prvky (zbalené menu)
         else
         {
            // Přidání události pro možnost reagovat na najetí kurzoru myši na oblast zbaleného postranního menu
            PlatnoPravehoMENU.MouseMove += Controller.obsluhyUdalosti.PraveMENU_MouseMove;

            // Vykreslení zbaleného postranního menu včetně popisku
            VykresliSkryteMENU(PlatnoPravehoMENU, VyskaPostrannihoMENU, 0);
         }

      }

      /// <summary>
      /// Metoda pro vykreslení zbaleného postranního MENU včetně popisku.
      /// </summary>
      /// <param name="PlatnoMENU">Plátno pro vykreslení postranního menu</param>
      /// <param name="VyskaMENU">Výška postranního menu</param>
      /// <param name="LevePraveUmisteni">1 - MENU umístěno vlevo, 0 - MENU umístěno vpravo</param>
      private void VykresliSkryteMENU(Canvas PlatnoMENU, double VyskaMENU, byte LevePraveUmisteni)
      {
         // Vytvoření obdélníku reprezentující zbalené postranní menu
         Rectangle pozadi = new Rectangle
         {
            Fill = Brushes.DarkGreen,
            Width = 30,
            Height = VyskaMENU
         };

         // Vykreslení obdélníku na plátno
         PlatnoMENU.Children.Add(pozadi);
         Canvas.SetTop(pozadi, 0);

         // Vytvoření popisku s horizontálním textem MENU
         Label popisek = VytvorPopisekMENU();

         // Vykreslení popisku na plátno
         PlatnoMENU.Children.Add(popisek);
         Canvas.SetTop(popisek, Math.Floor(VyskaMENU / 3));

         // Umístění na levou nebo pravou stranu okna
         if (LevePraveUmisteni == 1)
         {
            Canvas.SetLeft(pozadi, 0);
            Canvas.SetLeft(popisek, 0);
         }
         else
         {
            Canvas.SetRight(pozadi, 0);
            Canvas.SetRight(popisek, 0);
         }
      }

      /// <summary>
      /// Vykreslení informační bloku přehledu financí aktuálního měsíce.
      /// </summary>
      /// <param name="PlatnoPrehledu">Plátno pro vykreslení</param>
      /// <param name="CelkovePrijmy">Celkové příjmy v aktuálním měsíci</param>
      /// <param name="CelkoveVydaje">Celkové výdaje v aktuálním měsíci</param>
      public void VykresliInformacniPrehled(Canvas PlatnoPrehledu, double CelkovePrijmy, double CelkoveVydaje)
      {
         // Smazání obsahu plátna
         PlatnoPrehledu.Children.Clear();

         // Rozměry zobrazovaného okna po odečtení okrajů
         int Sirka = (int)PlatnoPrehledu.Width - 2;
         int Vyska = (int)PlatnoPrehledu.Height - 2;

         // Proměnná pro nastavení výšky textového popisku
         int VyskaTextu = 40;

         // Výpočet bilance mezi příjmy a výdaji
         double Bilance_hodnota = CelkovePrijmy - CelkoveVydaje;

         // Inicializace proměnných uchovávající v sobě data k zobrazení v textové podobě
         string AktualniMesic = Hodiny.VratMesicTextove(DateTime.Now.Month);
         string AktualniDen = Hodiny.VratDenVTydnu(DateTime.Now.DayOfWeek);
         string PocetDniDoKonceMesice = (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day).ToString();
         string VydajeCelkem = CelkoveVydaje.ToString() + " Kč";
         string PrijmyCelekm = CelkovePrijmy.ToString() + " Kč";
         
         // Inicializace proměnné uchovávající v sobě bilanci mezi příjmy a výdaji v textové podobě
         string Bilance = Bilance_hodnota > 0 ? "+" : "";   // Nastavení prefixu hodnoty -> Pokud je bilance kladná přidá se před ní znak + (plus), pokud ne tak záporné číso již obsahuje znak - (minus)
         Bilance += Bilance_hodnota.ToString() + " Kč";     // Nastavení hodnoty včetně měny (Kč)


         // Vytvoření obdélníku pro efekt okrajů
         Rectangle okraje = new Rectangle
         {
            Fill = Brushes.DarkBlue,
            Width = PlatnoPrehledu.Width,
            Height = PlatnoPrehledu.Height
         };

         // Pozadí informačního bloku
         Rectangle pozadi = new Rectangle
         {
            Fill = Controller.BarvaPozadi,
            Width = Sirka,
            Height = Vyska
         };

         // Oddělovací blok
         Rectangle oddeleni = new Rectangle
         {
            Fill = Brushes.BlueViolet,
            Width = 1,
            Height = Vyska - 2 - 2 * VyskaTextu
         };

         // Dělící čára
         Rectangle deliciCara = new Rectangle
         {
            Fill = Brushes.BlueViolet,
            Width = Sirka,
            Height = 1
         };

         // Aktuální měsíc
         Label mesic_popisek = new Label { Content = "Tento měsíc:  ", FontSize = 16 };
         Label mesic = new Label { Content = AktualniMesic, FontSize = 22 };

         // Aktuální den
         Label den = new Label { Content = AktualniDen, FontSize = 18 };

         // Výdaje
         Label vydaje_popisek = new Label { Content = "Výdaje:  ", FontSize = 14 };
         Label vydaje = new Label { Content = VydajeCelkem, FontSize = 18, Foreground = Brushes.Red };

         // Příjmy
         Label prijmy_popisek = new Label { Content = "Příjmy:  ", FontSize = 14 };
         Label prijmy = new Label { Content = PrijmyCelekm, FontSize = 18, Foreground = Brushes.Green };

         // Bilance
         Label Bilance_popisek = new Label { Content = "Bilance:  ", FontSize = 16 };
         Label bilance = new Label { Content = Bilance, FontSize = 20 };

         // Nastavení barvy písma bilance na základě zda se jedná o kladnou či zápornou bilanci
         bilance.Foreground = Bilance_hodnota > 0 ? Brushes.Green : Brushes.Red;

         // Nastavení černé barvy pro případ, že bilance je nulová
         if (Bilance_hodnota == 0)
            bilance.Foreground = Brushes.Black;

         // Informace o počtu dní do konce měsíce
         Label konec = new Label { Content = "Do konce měsíce zbývá " + PocetDniDoKonceMesice + " dní.", FontSize = 14 };


         // Vykreslení okrajů
         PlatnoPrehledu.Children.Add(okraje);
         Canvas.SetLeft(okraje, 0);
         Canvas.SetTop(okraje, 0);

         // Vykreslení pozadí
         PlatnoPrehledu.Children.Add(pozadi);
         Canvas.SetLeft(pozadi, 1);
         Canvas.SetTop(pozadi, 1);

         // Vykreslení dělících čar
         PlatnoPrehledu.Children.Add(oddeleni);
         PlatnoPrehledu.Children.Add(deliciCara);
         Canvas.SetRight(oddeleni, 101);
         Canvas.SetTop(oddeleni, 1);
         Canvas.SetLeft(deliciCara, 1);
         Canvas.SetTop(deliciCara, Vyska - 2 - 2 * VyskaTextu);

         // Vykrelsení aktuálního měsíce
         PlatnoPrehledu.Children.Add(mesic_popisek);
         PlatnoPrehledu.Children.Add(mesic);
         Canvas.SetLeft(mesic_popisek, 1);
         Canvas.SetTop(mesic_popisek, 1);
         Canvas.SetLeft(mesic, 1 + 95);
         Canvas.SetTop(mesic, -2);

         // Vykreslení aktuálního dne
         PlatnoPrehledu.Children.Add(den);
         Canvas.SetRight(den, 6);
         Canvas.SetTop(den, 0);

         // Vykreslení výdajů
         PlatnoPrehledu.Children.Add(vydaje_popisek);
         PlatnoPrehledu.Children.Add(vydaje);
         Canvas.SetLeft(vydaje_popisek, 1);
         Canvas.SetTop(vydaje_popisek, Vyska - 1 - 2 * VyskaTextu + 10);
         Canvas.SetLeft(vydaje, 1 + 55);
         Canvas.SetTop(vydaje, Vyska - 2 * VyskaTextu + 5);

         // Vykreslení příjmů
         PlatnoPrehledu.Children.Add(prijmy_popisek);
         PlatnoPrehledu.Children.Add(prijmy);
         Canvas.SetLeft(prijmy_popisek, 1);
         Canvas.SetTop(prijmy_popisek, Vyska - 1 - VyskaTextu + 10);
         Canvas.SetLeft(prijmy, 1 + 55);
         Canvas.SetTop(prijmy, Vyska - VyskaTextu + 5);

         // Vykreslení Bilance
         PlatnoPrehledu.Children.Add(Bilance_popisek);
         PlatnoPrehledu.Children.Add(bilance);
         Canvas.SetLeft(Bilance_popisek, Sirka / 2);
         Canvas.SetTop(Bilance_popisek, Vyska - 1 - 2 * VyskaTextu + 10);
         Canvas.SetLeft(bilance, (Sirka / 2) + 60);
         Canvas.SetTop(bilance, Vyska - 2 * VyskaTextu + 5);

         // Vykrelení věty o počtu dní do konce měsíce
         PlatnoPrehledu.Children.Add(konec);
         Canvas.SetRight(konec, 6);
         Canvas.SetBottom(konec, 3);
      }


      /// <summary>
      /// Vykreslení anonymní obrazovky, tedy obrazovky pro nepřihlášeného uživatele.
      /// Obrazovka překry celý obsah okna a má k dispozici pouze tlačítka pro registraci a přihlášení a informační okno.
      /// </summary>
      /// <param name="Platno">Plátno pro vykreslení anonymního okna</param>
      /// <param name="Sirka">Šířka vykreslovaného plátna</param>
      /// <param name="Vyska">Výška vykreslovaného plátna</param>
      public void VykresliObrazovkuNeprihlasenehoUzivatele(Canvas Platno, double Sirka, double Vyska)
      {
         // Nastavení viditelnosti plátna
         Platno.Visibility = Visibility.Visible;

         // Smazání původního obsahu plátna
         Platno.Children.Clear();


         // Vykreslení pozadí pro plné překrytí okna
         Rectangle pozadi = new Rectangle
         {
            Width = Sirka,
            Height = Vyska,
            Fill = Controller.BarvaPozadi
         };

         // Umístění pozadí na plátno
         Platno.Children.Add(pozadi);
         Canvas.SetLeft(pozadi, 0);
         Canvas.SetTop(pozadi, 0);


         // Vytvoření tlačítka pro možnost přihlášení uživatele
         Button PrihlasitButton = new Button();

         // Nastavení stylu tlačítka
         NastavTlacitkoPRIHLASIT(PrihlasitButton);

         // Úprava přihlašovacího tlačítka
         PrihlasitButton.Width = Sirka / 5;
         PrihlasitButton.Height = Vyska / 5;
         PrihlasitButton.IsDefault = true;

         // Přidání události pro možnost reagovat na kliknutí na tlačítko
         PrihlasitButton.Click += Controller.obsluhyUdalosti.PrihlasitButton_Click;

         // Vykreslení tlačítka na plátno
         Platno.Children.Add(PrihlasitButton);
         Canvas.SetLeft(PrihlasitButton, Sirka / 2 - PrihlasitButton.Width / 2);
         Canvas.SetTop(PrihlasitButton, Vyska / 2 - PrihlasitButton.Height / 2);


         // Vytvoření tlačítka pro možnost registrace nového uživatele
         Button RegistraceButton = new Button();

         // Nastavení tylu tlačítka
         NastavTlacitkoREGISTROVAT(RegistraceButton);

         // Přidání události pro možnost reagovat na kliknutí na tlačítko
         RegistraceButton.Click += Controller.obsluhyUdalosti.RegistrovatButton_Click;

         // Vykreslení tlačítka na plátno
         Platno.Children.Add(RegistraceButton);
         Canvas.SetLeft(RegistraceButton, 20);
         Canvas.SetTop(RegistraceButton, Vyska - 3 * RegistraceButton.Height);


         // Vytvoření tlačítka pro možnost zobrazení informačního okna
         Button InfoButton = new Button();

         // Nastavení stylu tlačítka
         NastavTlacitkoINFO(InfoButton);

         // Přidání události pro možnost reagovat na kliknutí na tlačítko
         InfoButton.Click += Controller.obsluhyUdalosti.InformaceButton_Click;

         // Vykreslení tlačítka na plátno
         Platno.Children.Add(InfoButton);
         Canvas.SetLeft(InfoButton, Sirka - InfoButton.Width - 40);
         Canvas.SetTop(InfoButton, Vyska - 3 * InfoButton.Height);
      }


      /// <summary>
      /// Metoda pro vykreslení ukazatele síly hesla.
      /// Na základě celkového počtu podmínek a počtu splněných podmínek graficky zobrazí sílu zadaného hesla.
      /// </summary>
      /// <param name="PlatnoHesla">Plátno pro vykreslení ukazatele hesla</param>
      /// <param name="PocetSplnenychPodminek">Počet splněných podmínek</param>
      /// <param name="CelkovyPocetPodminek">Celkový počet podmínek</param>
      /// <param name="Sirka">Šířka ukazatele hesla</param>
      /// <param name="Vyska">Výška ukazatele hesla</param>
      public void VykresliUkazatelHesla(Canvas PlatnoHesla, int PocetSplnenychPodminek, int CelkovyPocetPodminek, int Sirka, int Vyska)
      {
         PlatnoHesla.Children.Clear();          // Smazání původního obsahu plátna
         Brush BarvaOkraje = Brushes.Black;     // Nastavení barvy okraje ukazatele
         int VyskaUkazatele = Vyska;            // Nastavení výšky ukazatele
         int SirkaUkazatele = Sirka;            // Nastavení šířky ukazatele


         // Vytvoření levého okraje ukazatele a nastavení počátečních souřadnic včetně nastavení barvy
         Rectangle LevyOkraj = new Rectangle
         {
            Height = VyskaUkazatele,
            Width = 1
         };
         LevyOkraj.Fill = BarvaOkraje;          // Nastavení barvy okraje
         PlatnoHesla.Children.Add(LevyOkraj);   // Přidání levého okraje do canvasu
         Canvas.SetLeft(LevyOkraj, 0);          // Nastavení počátečních souřadnic
         Canvas.SetTop(LevyOkraj, 0);           // Nastavení počátečních souřadnic


         // Cyklus pro vytvoření okrajů následně vykreslovaných obdélníků
         for (int i = 0; i < CelkovyPocetPodminek; i++)
         {
            // Vytvoření nového obdélníku
            Rectangle OkrajovyObdelnik = new Rectangle
            {
               Height = VyskaUkazatele,
               Width = (SirkaUkazatele - 1) / CelkovyPocetPodminek
            };
            OkrajovyObdelnik.Fill = BarvaOkraje;         // Nastavení barvy dle arvy okraje
            PlatnoHesla.Children.Add(OkrajovyObdelnik);  // Přidání okrajového obdélníku do canvasu

            // Nastavení souřadnic pro vykreslení okrajových obdélníků
            Canvas.SetLeft(OkrajovyObdelnik, i * OkrajovyObdelnik.Width + 1);
            Canvas.SetTop(OkrajovyObdelnik, 0);
         }


         // Cyklus pro vytvoření požadovaného počtu obdélníku zarovnaných do canvasu vedle sebe s mezerou 1p
         for (int i = 0; i < CelkovyPocetPodminek; i++)
         {
            // Vytvoření nového obdélníku
            Rectangle obdelnik = new Rectangle
            {
               Height = VyskaUkazatele - 2,                                                  // Nastavení výšky zobrazovaných bloků 
               Width = (SirkaUkazatele - 1 - CelkovyPocetPodminek) / CelkovyPocetPodminek    // Určení šířky 1 obdélníku podle celkové šířky ukazatele 
            };

            // Nastavení barvy obdélníku (počet zelených obdélníku je dán počtem splněných podmínek)
            obdelnik.Fill = PocetSplnenychPodminek > i ? Brushes.Green : Brushes.Red;

            // Přidání vytvořeného obdélníku mezi potomky vykreslované na canvas
            PlatnoHesla.Children.Add(obdelnik);

            // Nastavení počátečních souřadnic obdélníku pro vykreslení na požadované místo
            Canvas.SetLeft(obdelnik, i * (obdelnik.Width + 1) + 1);
            Canvas.SetTop(obdelnik, 1);  // Posunutí o 1 p dolů -> horní okraj
         }
      }

   }
}
