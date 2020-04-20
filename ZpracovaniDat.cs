using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
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
   /// Třída pro práci s daty aplikace a ukládání do souborů
   /// </summary>
   class ZpracovaniDat
   {
      /// <summary>
      /// Cestak souboru pro uložení kolekce obsahující seznam položek v daném záznamu
      /// </summary>
      private string cesta = "SpravaFinanciData.xml";

      /// <summary>
      /// Proměnná pro výběr místa uložení dat aplikace. 
      /// TRUE - data jsou uložena ve složce aplikace, tedy ve složce odkud je spouštěn program. 
      /// FALSE - data jsou ukládána v systémové složce AppData na konkrétním počítači.
      /// </summary>
      private bool UkladatDataDoSlozkyAplikace = false;

      /// <summary>
      /// Konstruktor třídy pro práci s daty
      /// </summary>
      public ZpracovaniDat()
      {

      }

      /// <summary>
      /// Metoda pro uložení záznamů do textového souboru ve formátu TXT.
      /// </summary>
      /// <param name="CestaSouboru">Cesta k souboru pro uložení</param>
      /// <param name="KolekceDat">Kolekce záznamů určených k uložení</param>
      public void UlozDataUzivatele_TXT(string CestaSouboru, ObservableCollection<Zaznam> KolekceDat)
      {
         try
         {
            // Vytvoření nového textového souboru pro uložení dat v textové formě
            using (StreamWriter sw = new StreamWriter(CestaSouboru))
            {
               // Cyklus pro postupné zapisování jednotlivých záznamů do textového souboru
               foreach (Zaznam zaznam in KolekceDat)
               {
                  // Vypsání údajů záznamu
                  sw.WriteLine(zaznam.Nazev + " (" + zaznam.PrijemNeboVydaj.ToString() + "): " + zaznam.Hodnota_PrijemVydaj + " Kč (" + zaznam.kategorie + ")");
                  sw.WriteLine(zaznam.Datum.ToShortDateString() + "\t Pozn.: " + zaznam.Poznamka + ".");

                  // Oddělení položek od záznamu
                  sw.WriteLine("...");

                  // Vypsání položek daného záznamu
                  foreach (Polozka polozka in zaznam.SeznamPolozek)
                  {
                     sw.WriteLine(polozka.Nazev + " (" + polozka.Popis + "): " + polozka.Cena + " Kč (" + polozka.KategoriePolozky + ");");
                  }

                  // Oddělení jednotlivých záznamů
                  sw.WriteLine("\n\n\n");
               }

               // Vyprázdnění paměti po zápisu do souboru
               sw.Flush();
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda pro uložení záznamů do separovaného textového dokumentu ve formátu CSV.
      /// </summary>
      /// <param name="CestaSouboru">Cesta k souboru pro uložení</param>
      /// <param name="KolekceDat">Kolekce záznamů určených k uložení</param>
      public void UlozDataUzivatele_CSV(string CestaSouboru, ObservableCollection<Zaznam> KolekceDat)
      {
         try
         {
            // Vytvoření nového souboru ve formátu CSV pro zápis dat
            using (StreamWriter sw = new StreamWriter(CestaSouboru))
            {
               // Cyklus pro postupné zapisování jednotlivých záznamů do souboru
               foreach (Zaznam zaznam in KolekceDat)
               {
                  // Vytvoření pole textových řetězců reprezentující jednotlivé parametry a hodnoty daného záznamu
                  string[] parametry = { zaznam.Nazev.Replace(';', ' '), zaznam.PrijemNeboVydaj.ToString(), zaznam.Hodnota_PrijemVydaj.ToString(),
                                      zaznam.kategorie.ToString(), zaznam.Datum.ToShortDateString(), zaznam.Poznamka.Replace(';', ' ') };

                  // Seskupení pole parametrů do jednoho textového řetězce se středníkem jako separátor pro oddělení jednotlivých parametrů
                  string radek = String.Join(";", parametry);

                  // Zapsání řádku reprezentující daný záznam do souboru
                  sw.WriteLine(radek);

                  // Vytvoření řádku pro výpis položek
                  string radekPolozky = "";

                  // Postupné vypsání jednotlivých položek do souboru pod daný záznam
                  foreach (Polozka polozka in zaznam.SeznamPolozek)
                  {
                     // Vytvoření pole textových řetězců reprezentující jednotlivé parametry a hodnoty daného záznamu
                     string[] parametryPolozky = { polozka.Nazev.Replace(';', ' '), polozka.Popis.Replace(';', ' '), polozka.Cena.ToString(), polozka.KategoriePolozky.ToString() };

                     // Seskupení pole parametrů do jednoho textového řetězce se středníkem jako separátor pro oddělení jednotlivých parametrů
                     string radekJednePolozky = String.Join(";", parametryPolozky) + ";";

                     // Přidání položky do řádku položek
                     radekPolozky += radekJednePolozky;
                  }

                  // Zapsání řádku reprezentující daný záznam do souboru
                  sw.WriteLine(radekPolozky);
               }

               // Vyprázdnění paměti po zápisu do souboru
               sw.Flush();
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda pro uložení vybraných záznamů do souboru ve formátu XML.
      /// </summary>
      /// <param name="CestaSouboru">Cesta k souboru pro uložení</param>
      /// <param name="KolekceDat">Kolekce záznamů určených k uložení</param>
      public void UlozDataUzivatele_XML(string CestaSouboru, ObservableCollection<Zaznam> KolekceDat)
      {
         try
         {
            // Vytvoření serializátoru s předáním typu kolekce pro uložení
            XmlSerializer serializer = new XmlSerializer(KolekceDat.GetType());

            // Funkce pro zápis dat do souboru. Blok using zajistí automatické zavření souboru. 
            using (StreamWriter sw = new StreamWriter(CestaSouboru))
            {
               // Volání metody pro serializaci kolekce k uložení
               serializer.Serialize(sw, KolekceDat);
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda pro načtení záznamů ze souboru ve formátu XML.
      /// </summary>
      /// <param name="cesta">Cesta k souboru pro načtení záznamů ze souboru</param>
      /// <returns>Kolekce záznamů (načtená data)</returns>
      public ObservableCollection<Zaznam> NactiDataUzivatele_XML(string cesta)
      {
         // Vytvoření dočasné kolekce pro načtené jednotlivých záznamů ze souboru
         ObservableCollection<Zaznam> NacteneZaznamy = new ObservableCollection<Zaznam>();

         try
         {
            // Vytvoření serializátoru s předáním typu kolekce pro uložení
            XmlSerializer serializer = new XmlSerializer(NacteneZaznamy.GetType());

            // Pokud soubor existuje do kolekce se načtou data ze souboru
            if (File.Exists(cesta))
            {
               // Funkce pro čtení dat ze souboru. Blok using zajistí automatické zavření souboru. 
               using (StreamReader sr = new StreamReader(cesta))
               {
                  // Provedení deserializace -> načtení objektů ze souboru do vnitřní kolekce (včetně přetypování)
                  NacteneZaznamy = (ObservableCollection<Zaznam>)serializer.Deserialize(sr);
               }
            }

            // Navrácení kolekce načtených dat (záznamů)
            return NacteneZaznamy;
         }
         catch (Exception ex) // V případě neúspěchu v bloku TRY dojde k vyvolání vyjímky a načtení se zruší
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
         }
      }



      /// <summary>
      /// Metoda pro uložení dat aplikace do souboru XML uložený ve složce AppData.
      /// K uložení dat je využita třída XmlSerializer, která uloží obsah do formátu XML automaticky. Tato třída ukládá pouze veřejné vlastnosti položek v kolekci.
      /// </summary>
      public void UlozDataUzivateluDoSouboru(ObservableCollection<Uzivatel> KolekceDat)
      {
         try
         {
            // Nastavení cesty k souboru pro uložení dat do složky AppData (dle zvolene možnosti složky pro uložení)
            if (!UkladatDataDoSlozkyAplikace)
            {
               // Uložení cesty ke složce v AppData do textového řetězce
               cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpravaFinanci_v2");

               // Pokud složka neexistuje, vytvoří se nová složka
               if (!Directory.Exists(cesta))
                  Directory.CreateDirectory(cesta);

               // Sloučení cesty do složky a samotného souboru
               cesta = Path.Combine(cesta, "DataUzivatelu.xml");
            }
           
            // Vytvoření serializátoru s předáním typu kolekce pro uložení
            XmlSerializer serializer = new XmlSerializer(KolekceDat.GetType());

            // Funkce pro zápis dat do souboru. Blok using zajistí automatické zavření souboru. 
            using (StreamWriter sw = new StreamWriter(cesta))
            {
               // Volání metody pro serializaci kolekce k uložení
               serializer.Serialize(sw, KolekceDat);
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda pro načtení uložených dat ze souboru XML uložený ve složce AppData.
      /// Využívá se třída XmlSerializer, která je využita i pro uložení těchto dat. Tato třída ukládá pouze veřejné vlastnosti položek v kolekci.
      /// </summary>
      public ObservableCollection<Uzivatel> NactiDataUzivateluZeSouboru()
      {
         // Vytvoření kolekce dat pro uložení načtených dat
         ObservableCollection<Uzivatel> KolekceDat = new ObservableCollection<Uzivatel>();

         try
         {
            // Nastavení cesty k souboru pro načtení dat ze složky AppData (dle zvolene možnosti složky pro uložení)
            if (!UkladatDataDoSlozkyAplikace)
            {
               // Uložení cesty ke složce v AppData do textového řetězce
               cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpravaFinanci_v2");

               // Pokud složka neexistuje, vytvoří se nová složka
               if (!Directory.Exists(cesta))
                  Directory.CreateDirectory(cesta);

               // Sloučení cesty do složky a samotného souboru
               cesta = Path.Combine(cesta, "DataUzivatelu.xml");
            }
               
            // Vytvoření serializátoru s předáním typu kolekce pro uložení
            XmlSerializer serializer = new XmlSerializer(KolekceDat.GetType());

            // Pokud soubor existuje do kolekce aplikace se načtou data ze souboru
            if (File.Exists(cesta))
            {
               // Funkce pro čtení dat ze souboru. Blok using zajistí automatické zavření souboru. 
               using (StreamReader sr = new StreamReader(cesta))
               {
                  // Provedení deserializace -> načtení objektů ze souboru do vnitřní kolekce (včetně přetypování)
                  KolekceDat = (ObservableCollection<Uzivatel>)serializer.Deserialize(sr);
               }
            }

            else  // Pokud soubor neexistuje, vytvoří se prázdná kolekce
            {
               KolekceDat = new ObservableCollection<Uzivatel>();
            }

         }
         catch (Exception ex)    // V případě neúspěchu v bloku TRY dojde k vyvolání vyjímky a načtení se zruší
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }

         // Návratová hodnota - kolekce dat
         return KolekceDat;
      }

   }
}
