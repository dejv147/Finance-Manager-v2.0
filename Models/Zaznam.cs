using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
   /// Kategorie podle kterých se záznamy rozdělí do různých kolkecí.
   /// 0 značí nevybráno, tudíž kategorie jsou číslovány od 1.
   /// </summary>
   public enum Kategorie
   {
      Nevybrano, Alkohol, Auto, Brigada, Cestovani, Dar, Divadlo, DomaciMazlicek, Domacnost, Domov,
      Drogerie, Elektronika, Hobby, Inkaso, Jidlo, Kino, Kosmetika, Kultura, Najem, Napoje,
      Nezarazeno, Obleceni, Partner, PC, Restaurace, Rodina, Sport, Skola, Telefon, Vyplata,
      Vzdelani, Zamestnani, Zdravi
   };

   /// <summary>
   /// Výčtový typ pro určení zda záznam představuje výdaj, nebo příjem
   /// </summary>
   public enum KategoriePrijemVydaj { Prijem, Vydaj };


   /// <summary>
   /// Třída reprezentující určitou finanční transakci (příjem/výdaj). 
   /// Uchovává v sobě potřebná data včetně seznamu položek.
   /// </summary>
   public class Zaznam
   {
      /// <summary>
      /// Statický atribut pro uchování názvů kategorií v textové podobě včetně diakritiky. 
      /// První kategorie (s indexem = 0) je "Nevybráno" pro možnost úvodního nastavení, názvy kategorií jsou tedy indexovány od 1.
      /// </summary>
      public static string[] NazvyKategorii = { "Nevybráno", "Alkohol", "Auto", "Brigáda", "Cestování", "Dar", "Divadlo", "Domácí mazlíček",
                                                "Domácnot", "Domov", "Drogerie", "Elektronika", "Hobby", "Inkaso", "Jídlo", "Kino", "Kosmetika",
                                                "Kultura", "Nájem", "Nápoje", "Nezařazeno", "Oblečení", "Partner / Partnerka", "Pc",
                                                "Restaurace", "Rodina", "Sport", "Škola", "Telefon / Mobil", "Výplata", "Vzdělání",
                                                "Zaměstnání / Práce", "Zdraví" };

      /// <summary>
      /// Datum vytvoření záznamu.
      /// </summary>
      public DateTime DatumZapisu { get; set; }

      /// <summary>
      /// Datum uskutečnění transakce v tomto záznamu. 
      /// Jedná se o datum koupě/prodeje produktů.
      /// </summary>
      public DateTime Datum { get; set; }

      /// <summary>
      /// Název vytvořeného záznamu
      /// </summary>
      public string Nazev { get; set; }

      /// <summary>
      /// Celková částka příjmu nebo výdaje v daném záznamu.
      /// </summary>
      public double Hodnota_PrijemVydaj { get; set; }

      /// <summary>
      /// Poznámka k záznamu
      /// </summary>
      public string Poznamka { get; set; }

      /// <summary>
      /// Kategorie do které záznam spadá.
      /// </summary>
      public Kategorie kategorie { get; set; }

      /// <summary>
      /// Výčtový typ k rozdělení záznamů na příjmy a výdaje.
      /// </summary>
      public KategoriePrijemVydaj PrijemNeboVydaj { get; set; }

      /// <summary>
      /// Seznam položek v daném záznamu o změně financí
      /// </summary>
      public ObservableCollection<Polozka> SeznamPolozek { get; set; }

      

      /// <summary>
      /// Konstruktor třídy pro vytvoření nového záznamu s nastavením všech parametrů předaných v parametru.
      /// </summary>
      /// <param name="Nazev">Název záznamu</param>
      /// <param name="Datum">Datum záznamu</param>
      /// <param name="Hodnota">Celkový příjem/výdaj</param>
      /// <param name="PrijemNeboVydaj">Rozdělení záznamu na příjem a výdaj</param>
      /// <param name="Poznamka">Textová poznámka</param>
      /// <param name="kategorie">Kategorie záznamu</param>
      /// <param name="SeznamPolozek">Kolekce položek</param>
      public Zaznam(string Nazev, DateTime Datum, double Hodnota, KategoriePrijemVydaj PrijemNeboVydaj, string Poznamka, Kategorie kategorie, ObservableCollection<Polozka> SeznamPolozek)
      {
         // Načtení hodnot z parametru do interních proměnných
         DatumZapisu = DateTime.Now;               // Datum zápisu je aktuální datum při vytvoření záznamu
         this.Nazev = Nazev;
         this.Poznamka = Poznamka;
         this.SeznamPolozek = SeznamPolozek;
         this.Hodnota_PrijemVydaj = Hodnota;
         this.PrijemNeboVydaj = PrijemNeboVydaj;
         this.Datum = Datum;
         this.kategorie = kategorie;
      }

      /// <summary>
      /// Bezparametrický konstruktor třídy pro možnost ukládání dat do souboru
      /// </summary>
      public Zaznam()
      {

      }

      /// <summary>
      /// Metoda pro vytvoření textového řetězce obsahující veškeré potřebné údaje o záznamu pro možnost výpisu.
      /// </summary>
      /// <returns>Textový řetězec obsahující potřebné údaje</returns>
      public override string ToString()
      {
         // Textový řetězec pro uložení parametrů záznamu
         string Zaznam = "";
         
         Zaznam += Nazev + "; ";
         Zaznam += "vytvořen " + Datum.ToString("dd.MM.yyyy");
         Zaznam += ". hodnota: " + Hodnota_PrijemVydaj + " Kč \n";

         // Vypsání všech položek do textového řetězce
         foreach (Polozka p in SeznamPolozek)
         {
            Zaznam += p + "; ";
         }
         return Zaznam;
      }
   }
}
