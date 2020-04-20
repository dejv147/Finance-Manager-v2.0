using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpravceFinanci_v2;

/// <summary>
/// Jmenný prostor sdružující všechny třídy realizující testy projektu SpravceFinanci.
/// Lze jej použít i pro testování dalších verzí tohoto projektu.
/// </summary>
namespace SpravceFinanci_Tests
{
   /// <summary>
   /// Třída pro provedení testů metod ve statické třídě Validace, která slouží k načítání dat od uživatele.
   /// </summary>
   [TestClass]
   public class ValidaceTests
   {
      /// <summary>
      /// Metoda, která se spustí před každým testem. Slouží k úvodnímu nastavení a definici parametrů pro zajištění stejných podmínek každého testu.
      /// </summary>
      [TestInitialize]
      public void Initialize()
      {

      }



      /// <summary>
      /// Test správných výsledků metody NactiCislo ve třídě Validace.
      /// </summary>
      [TestMethod]
      public void NactiCislo()
      {
         Assert.AreEqual(155, Validace.NactiCislo("155"));
         Assert.AreEqual(155, Validace.NactiCislo("1 55 "));
         Assert.AreEqual(3.14, Validace.NactiCislo("3,14"));
         Assert.AreEqual(6.25, Validace.NactiCislo("6.25"));
         Assert.AreEqual(-28, Validace.NactiCislo("- 28"));
         Assert.AreEqual(0, Validace.NactiCislo("000"));
      }

      /// <summary>
      /// Test špatných vstupů metody NactiCislo ve třídě Validace.
      /// </summary>
      [TestMethod]
      [ExpectedException(typeof(ArgumentException))]
      public void NactiCisloVyjimka()
      {
         Validace.NactiCislo("");
         Validace.NactiCislo(" ");
         Validace.NactiCislo("a");
         Validace.NactiCislo(".");
         Validace.NactiCislo("&");
         Validace.NactiCislo("+");
         Validace.NactiCislo("-");
         Validace.NactiCislo("*");
         Validace.NactiCislo("/");
         Validace.NactiCislo("\\");
         Validace.NactiCislo("\"");
      }

      /// <summary>
      /// Test správných výsledků metody NactiDatum ve třídě Validace.
      /// </summary>
      [TestMethod]
      public void NactiDatum()
      {
         Assert.AreEqual(DateTime.Now.Date, Validace.NactiDatum(DateTime.Now));
      }

      /// <summary>
      /// Test špatných vstupů metody NactiDatum ve třídě Validace.
      /// </summary>
      [TestMethod]
      [ExpectedException(typeof(ArgumentException))]
      public void NactiDatumVyjimka()
      {
         Validace.NactiDatum(null);
      }

      /// <summary>
      /// Test správných výsledků metody SlozSlovaDoTextu ve třídě Validace.
      /// </summary>
      [TestMethod]
      public void SlozSlovaDoTextu()
      {
         string[] slova = { "Máma", "mele", "maso" };
         Assert.AreEqual("Máma mele maso", Validace.SlozSlovaDoTextu(slova));
      }

      /// <summary>
      /// Test správných výsledků metody SmazPosledniZnak ve třídě Validace.
      /// </summary>
      [TestMethod]
      public void SmazPosledniZnak()
      {
         Assert.AreEqual("David", Validace.SmazPosledniZnak("David;"));
         Assert.AreEqual("", Validace.SmazPosledniZnak(""));
         Assert.AreEqual("", Validace.SmazPosledniZnak(";"));
         Assert.AreEqual(" ", Validace.SmazPosledniZnak("  "));
      }








      /// <summary>
      /// Metoda, která se spustí po každém testu. Slouží k ukončení všech závislostí po provedení testu.
      /// </summary>
      [TestCleanup]
      public void CleanUp()
      {

      }

   }
}
