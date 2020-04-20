using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
   /// Třída pro vyhledávání konkrétních záznamů dle požadovaných filtračních parametrů. 
   /// Kolekce ve které se záznamy hledají je předávána vždy v parametru příslušné funkce.
   /// Pro vyhledávání jsou používány LINQ dotazy a návratová kolekce je vždy přetypována na původní datový typ, tedy Zaznam.
   /// </summary>
   public static class Vyhledavani
   {

      /// <summary>
      /// Výběr záznamů aktuálního měsíce z celkové kolekce záznamů
      /// </summary>
      /// <param name="KolekceZazanamu">Celková kolekce záznamů</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public static ObservableCollection<Zaznam> VratZaznamyAktualnihoMesice(ObservableCollection<Zaznam> KolekceZazanamu)
      {
         // LINQ dotaz pro výběr záznamů pouze z aktuálního měsíce. Záznamy jsou seřazeny sestupně dle data 
         var VybraneZaznamy = from zaznam in KolekceZazanamu
                              where (DateTime.Now.Month == zaznam.Datum.Month)
                              orderby zaznam.Datum descending
                              select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in VybraneZaznamy)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr všech záznamů v kategorii příjmy.
      /// </summary>
      /// <param name="KolekceZazanamu">Celková kolekce záznamů</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public static ObservableCollection<Zaznam> VratPrijmy(ObservableCollection<Zaznam> KolekceZazanamu)
      {
         // LINQ dotaz pro výběr všech záznamů spadajících do kategorie Příjmy
         var Prijmy = from zaznam in KolekceZazanamu
                      where (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                      orderby zaznam.Datum descending
                      select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in Prijmy)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr všech záznamů v kategorii výdaje.
      /// </summary>
      /// <param name="KolekceZazanamu">Celková kolekce záznamů</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public static ObservableCollection<Zaznam> VratVydaje(ObservableCollection<Zaznam> KolekceZazanamu)
      {
         // LINQ dotaz pro výběr všech záznamů spadajících do kategorie Výdaje
         var Vydaje = from zaznam in KolekceZazanamu
                      where (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Vydaj)
                      orderby zaznam.Datum descending
                      select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in Vydaje)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů podle názvu
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="Nazev">Textový řetězec pro porovnání jednotlivých záznamů</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public static ObservableCollection<Zaznam> VratZaznamyDleNazvu(ObservableCollection<Zaznam> Zaznamy, string Nazev)
      {
         // LINQ dotaz pro výběr záznamů se shodným jménem
         var ZaznamyNazev = from zaznam in Zaznamy
                            where zaznam.Nazev == Nazev
                            select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamyNazev)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů spadajících do kategorie v parametru
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="kategorie">Kategorie záznamu pro vyhledání</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public static ObservableCollection<Zaznam> VratZaznamyDleKategorie(ObservableCollection<Zaznam> Zaznamy, Kategorie kategorie)
      {
         // LINQ dotaz pro výběr záznamů spadajících do konkrétní kategorie
         var ZaznamyKategorie = from zaznam in Zaznamy
                                where zaznam.kategorie == kategorie
                                select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamyKategorie)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů s datem v zadaném časovém období
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="PocatecniDatum">Spodní hranice pro vyhledání</param>
      /// <param name="KoncoveDatum">Horní hranice pro vyhledání</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public static ObservableCollection<Zaznam> VratZaznamyVCasovemRozmezi(ObservableCollection<Zaznam> Zaznamy, DateTime PocatecniDatum, DateTime KoncoveDatum)
      {
         // LINQ dotaz pro výběr záznamů s datem v zadaném časovém období
         var ZaznamyVCase = from zaznam in Zaznamy
                            where ((zaznam.Datum >= PocatecniDatum) && (zaznam.Datum <= KoncoveDatum))
                            select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamyVCase)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů s hodnotou v zadaném rozmezí
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="MIN">Minimální hodnota pro vyhledání</param>
      /// <param name="MAX">Maximální hodnota pro vyhledání</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public static ObservableCollection<Zaznam> VratZaznamySHodnotouVRozmezi(ObservableCollection<Zaznam> Zaznamy, double MIN, double MAX)
      {
         // LINQ dotaz pro výběr záznamů s datem v zadaném časovém období
         var ZaznamyVHodnote = from zaznam in Zaznamy
                               where (zaznam.Hodnota_PrijemVydaj >= MIN && zaznam.Hodnota_PrijemVydaj <= MAX)
                               select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamyVHodnote)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů s počtem položek dle zadaného rozmezí
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="PocetMIN">Minimální počet položek</param>
      /// <param name="PocetMAX">Maximální počet položek</param>
      /// <returns></returns>
      public static ObservableCollection<Zaznam> VratZaznamyDlePoctuPolozek(ObservableCollection<Zaznam> Zaznamy, int PocetMIN, int PocetMAX)
      {
         // LINQ dotaz pro výběr záznamu dle zadaného rozpětí počtu položek
         var ZaznamySPoctemPolozek = from zaznam in Zaznamy
                                     where (PocetMIN <= zaznam.SeznamPolozek.Count && PocetMAX >= zaznam.SeznamPolozek.Count)
                                     select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamySPoctemPolozek)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Metoda pro sestupné seřazení kolekce záznamů.
      /// </summary>
      /// <param name="Zaznamy">Kolekce záznamů</param>
      /// <returns>Seřazená kolekce záznamů</returns>
      public static ObservableCollection<Zaznam> SeradZaznamy(ObservableCollection<Zaznam> Zaznamy)
      {
         // LINQ dotaz pro seřazení záznamů 
         var Razeni = from zaznam in Zaznamy
                      orderby zaznam.Datum descending
                      select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in Razeni)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

   }
}
