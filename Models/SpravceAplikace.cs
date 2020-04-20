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
using Calculator;

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
   /// Třída obsahující funkce a metody pro správu a řízení celé aplikace. 
   /// </summary>
   class SpravceAplikace
   {
      /// <summary>
      /// Instance kontroléru aplikace
      /// </summary>
      private SpravceFinanciController Controller;

      /// <summary>
      /// Instance přihlášeného uživatele
      /// </summary>
      private Uzivatel PrihlasenyUzivatel;

      /// <summary>
      /// Jméno přihlášeného uživatele
      /// </summary>
      public string JmenoPrihlasenehoUzivatele { get; private set; }

      /// <summary>
      /// Kolekce pro uložení dat všech uživatelů v aplikaci.
      /// </summary>
      private ObservableCollection<Uzivatel> KolekceUzivatelu;

      /// <summary>
      /// Kolekce dat, která se zpracovává při běhu programu. 
      /// Kolekce obsahuje pouze data, která patří přihlášenému uživateli.
      /// </summary>
      public ObservableCollection<Zaznam> KolekceDatPrihlasenehoUzivatele
      {
         get
         {
            return PrihlasenyUzivatel.SeznamZaznamuUzivatele;
         }
         set
         {
            PrihlasenyUzivatel.SeznamZaznamuUzivatele = value;
         }
      }


      /// <summary>
      /// Konstruktor třídy pro správu aplikace
      /// </summary>
      public SpravceAplikace()
      {
         // Inicializace proměnných
         JmenoPrihlasenehoUzivatele = "";
         PrihlasenyUzivatel = null;

         // Uložení instance kontroléru
         Controller = SpravceFinanciController.VratInstanciControlleru();

         // Načtení dat aplikace ze souboru
         KolekceUzivatelu = Controller.NactiDataUzivateluZeSouboru();
      }


      /// <summary>
      /// Smazání zvoleného záznamu z kolekce záznamů přihlášeného uživatele.
      /// </summary>
      /// <param name="zaznam">Záznam, který bude smazán</param>
      public void SmazatZaznam(Zaznam zaznam)
      {
         KolekceDatPrihlasenehoUzivatele.Remove(zaznam);
      }

      /// <summary>
      /// Metoda pro kontrolu zda v kolekci uživatele již existuje stejný záznam jako záznam předaný v parametru.
      /// </summary>
      /// <param name="zaznam">Záznam ke kontrole</param>
      /// <returns>True - záznam již existuje, False - záznam neexistuje</returns>
      public bool KontrolaExistujicihoZaznamu(Zaznam zaznam)
      {
         // Čítač podobností při porovnání záznamů
         int citac = 0;

         // Postupné projití všech záznamů v kolekci přihlášeného uživatele pro kontrolu existujícího záznamu
         foreach (Zaznam InterniZaznam in KolekceDatPrihlasenehoUzivatele)
         {
            // Kontrola názvu
            if (zaznam.Nazev == InterniZaznam.Nazev)
               citac++;

            // Kontrola data
            if (zaznam.Datum == InterniZaznam.Datum)
               citac++;

            // Kontrola hodnoty
            if (zaznam.Hodnota_PrijemVydaj == InterniZaznam.Hodnota_PrijemVydaj)
               citac++;

            // Kontrola poznámky
            if (zaznam.Poznamka == InterniZaznam.Poznamka)
               citac++;

            // Kontrola kategorie
            if (zaznam.kategorie == InterniZaznam.kategorie)
               citac++;

            // Kontrola zda se jedná o příjem či výdaj
            if (zaznam.PrijemNeboVydaj == InterniZaznam.PrijemNeboVydaj)
               citac++;

            // Pokud jsou všechny parametry stejné, nastaví se návratová hodnota na TRUE a ukončí se porovnávání. 
            // Pokud se nějaká hodnota liší, čítač se vynuluje a pokračuje se v porovnávání dalšího záznamu.
            if (citac > 5)
               return true;
            else
               citac = 0;
         }

         // Pokud předaný záznam v kolekci záznamů uživatele neexistuje nastaví se návratová hodnota na false
         return false;
      }

      /// <summary>
      /// Metoda pro vytvoření nového uživatele a přidání do kolekce uživatelů.
      /// </summary>
      /// <param name="Jmeno">Jméno uživatele</param>
      /// <param name="Heslo">Heslo uživatele</param>
      public void VytvorNovehoUzivatele(string Jmeno, string Heslo)
      {
         // Kontrola zda již neexistuje uživatel se stejným jménem
         if (KontrolaExistujicihoJmenaUzivatele(Jmeno))
            throw new ArgumentException("Uživatel se zadaným jménem již existuje!");

         // Vytvoření nového uživatele
         Uzivatel NovyUzivatel = new Uzivatel(Jmeno, Heslo);

         // Přidání nového uživatele do kolekce uživatelů (data aplikace)
         KolekceUzivatelu.Add(NovyUzivatel);
         Controller.UlozDataDoSouboru(KolekceUzivatelu);
      }

      /// <summary>
      /// Metoda zkontroluje zda již neexistuje uživatel se stejným jménem.
      /// Pokud uživatel se stejným jménem již existuje vrátí TRUE a pokud je uživatelské jméno k dispozici vrátí FALSE.
      /// </summary>
      /// <param name="JmenoNovehoUzivatele">Zadané jméno</param>
      /// <returns>TRUE - uživatel již existuje, FALSE - uživatel ještě neexistuje</returns>
      private bool KontrolaExistujicihoJmenaUzivatele(string JmenoNovehoUzivatele)
      {
         // Cyklus postupně projde všechny existující uživatele a při první shodě jmen vrátí TRUE. 
         foreach (Uzivatel uzivatel in KolekceUzivatelu)
         {
            if (uzivatel.Jmeno == JmenoNovehoUzivatele)
               return true;
         }
         // Pokud ke shodě nedojde a cyklus se ukončí vrátí se FALSE
         return false;
      }

      /// <summary>
      /// Metoda pro kontrolu přihlášeného uživatele.
      /// Pokud je jméno předané v parametru shodné se jménem přihlášeného uživatele vrátí se TRUE.
      /// </summary>
      /// <param name="JmenoUzivatele">Jméno kontrolovaného uživatele</param>
      /// <returns>TRUE - je přihlášen kontrolovaný uživatel, FALSE - kontrolovaný uživatel není přihlášen</returns>
      public bool KontrolaPrihlaseniUzivatele(string JmenoUzivatele)
      {
         if ((PrihlasenyUzivatel == null) || !(PrihlasenyUzivatel.Jmeno == JmenoUzivatele))
            return false;
         else
            return true;
      }

      /// <summary>
      /// Metoda pro přihlášení uživatele do aplikace.
      /// V případě úspěšného přihlášení načte vybraného uživatele do interní proměnné pro zpracování jeho dat.
      /// Metoda hledá v kolekci jméno uživatele a v přpaděnalezení provede kontrolu hesla.
      /// </summary>
      /// <param name="JmenoUzivatele">Zadané jméno uživatele</param>
      /// <param name="HesloUzivatele">Zadané heslo uživatele</param>
      public void PrihlaseniUzivatele(string JmenoUzivatele, string HesloUzivatele)
      {
         // Cyklus projde všechny uživatele v kolekci a v případě nalezené shody jmen zkontroluje zadané heslo
         foreach (Uzivatel uzivatel in KolekceUzivatelu)
         {
            // Nalezení jména uživatele
            if (uzivatel.Jmeno == JmenoUzivatele)
            {
               // Kontrola hesla 
               if (uzivatel.Heslo == HesloUzivatele)
               {
                  // Přihlášení uživatele -> uložení nalezeného uživatele do interní proměnné pro práci s jeho daty
                  PrihlasenyUzivatel = uzivatel;
                  JmenoPrihlasenehoUzivatele = uzivatel.Jmeno;
                  return;
               }
               // Zadané heslo není správně
               else
               {
                  throw new ArgumentException("Zadali jste špatné heslo!");
               }

            }

         }

         // Uživatel nebyl nalezen
         throw new ArgumentException("Uživatel se zadaným jménem nebyl nalezen.");
      }

      /// <summary>
      /// Metoda pro odhlášení uživatele.
      /// Nejprve uloží data do souboru a poté zpracovávaná data odstraní z interních proměnných.
      /// </summary>
      public void OdhlaseniUzivatele()
      {
         // Uložení dat přihlášeného uživatele do souboru
         Controller.UlozDataDoSouboru(KolekceUzivatelu);

         // Zrušení přihlášeného uživatele (smazání jeho dat z aplikace)
         PrihlasenyUzivatel = null;
         JmenoPrihlasenehoUzivatele = "";
      }

      /// <summary>
      /// Příznakový bit informující o nastavení viditelnosti daného uživatele
      /// </summary>
      /// <returns>1 - poznámky zobrazit, 0 - poznámky skrýt</returns>
      public byte VratZobrazeniPoznamky()
      {
         return PrihlasenyUzivatel.PoznamkaZobrazena;
      }

      /// <summary>
      /// Nastavení příznakového bitu informující o nastavení viditelnosti poznámkového bloku přihlášeného uživatele.
      /// </summary>
      /// <param name="Volba">1 - poznámky zobrazit, 0 - poznámky skrýt</param>
      public void NastavZobrazeniPoznamky(byte Volba)
      {
         PrihlasenyUzivatel.PoznamkaZobrazena = Volba;
      }

      /// <summary>
      /// Vrácení textu poznámky přihlášeného uživatele
      /// </summary>
      /// <returns>Poznámka uživatele</returns>
      public string VratPoznamkuUzivatele()
      {
         return PrihlasenyUzivatel.Poznamka;
      }

      /// <summary>
      /// Nastavení textu poznámky přihlášeného uživatele
      /// </summary>
      /// <param name="TextPoznamky">Poznámka uživatele</param>
      public void NastavPoznamkuUzivatele(string TextPoznamky)
      {
         PrihlasenyUzivatel.Poznamka = TextPoznamky;
      }

      /// <summary>
      /// Uložení všech dat do souboru uložených dat aplikace.
      /// </summary>
      public void UlozDataAplikace()
      {
         Controller.UlozDataDoSouboru(KolekceUzivatelu);
      }

   }
}
