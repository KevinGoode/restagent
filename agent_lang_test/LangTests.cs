using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kge
{
    namespace Agent
    {
        namespace Lang
        {
            [TestClass]
            public class LangTests
            {
                [TestMethod]
                public void TestEnglish()
                {
                    string mess = LanguageHelper.Resolve("CORE_ERROR_GENERAL_EXCEPTION", "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestJapanese()
                {
                    //Need to modify test if localisation supported
                    string mess = Messages.GetLocalisedMessage(new LocalMessage("jp", "CORE_ERROR_GENERAL_EXCEPTION"), "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestGerman()
                {
                    //Need to modify test if localisation supported
                    string mess = Messages.GetLocalisedMessage(new LocalMessage("de", "CORE_ERROR_GENERAL_EXCEPTION"), "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestFrench()
                {
                    //Need to modify test if localisation supported
                    string mess = Messages.GetLocalisedMessage(new LocalMessage("fr", "CORE_ERROR_GENERAL_EXCEPTION"), "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestSpanish()
                {
                    //Need to modify test if localisation supported
                    string mess = Messages.GetLocalisedMessage(new LocalMessage("es", "CORE_ERROR_GENERAL_EXCEPTION"), "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestRussian()
                {
                    //Need to modify test if localisation supported
                    string mess = Messages.GetLocalisedMessage(new LocalMessage("rs", "CORE_ERROR_GENERAL_EXCEPTION"), "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestSimplifiedChinese()
                {
                    //Need to modify test if localisation supported
                    string mess = Messages.GetLocalisedMessage(new LocalMessage("zh-chs", "CORE_ERROR_GENERAL_EXCEPTION"), "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestTraditionalChinese()
                {
                    //Need to modify test if localisation supported
                    string mess = Messages.GetLocalisedMessage(new LocalMessage("zh-cht", "CORE_ERROR_GENERAL_EXCEPTION"), "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestUnsupportedLanguage()
                {
                    string mess = Messages.GetLocalisedMessage(new LocalMessage("babel", "CORE_ERROR_GENERAL_EXCEPTION"), "some error message");
                    Assert.AreEqual(mess, "General error: some error message");
                }
                [TestMethod]
                public void TestUnknownMessage()
                {
                    string mess = LanguageHelper.Resolve("somemistakeinid", "some error message");
                    Assert.AreEqual(mess, "Failed to find message");
                }
                [TestMethod]
                [ExpectedException(typeof(FormatException))]
                public void TestNoParamsWhenOneExpected()
                {
                    string mess = LanguageHelper.Resolve("CORE_ERROR_GENERAL_EXCEPTION");
                    Assert.AreEqual(mess, "Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }
    }
}
