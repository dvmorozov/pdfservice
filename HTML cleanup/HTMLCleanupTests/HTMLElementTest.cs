using HTMLCleanup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HTMLCleanupTests
{
    
    
    /// <summary>
    ///This is a test class for HTMLElementTest and is intended
    ///to contain all HTMLElementTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HTMLElementTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for HTMLElement Constructor
        ///</summary>
        [TestMethod()]
        public void HTMLElementConstructorTest()
        {
            /*
            string startTag = string.Empty; // TODO: Initialize to an appropriate value
            string endTag = string.Empty; // TODO: Initialize to an appropriate value
            string text = string.Empty; // TODO: Initialize to an appropriate value
            HTMLElement target = new HTMLElement(startTag, endTag, text);
            Assert.Inconclusive("TODO: Implement code to verify target");
             */
        }

        /// <summary>
        ///A test for FindNext
        ///</summary>
        [TestMethod()]
        public void FindNextTest()
        {
            /*
            string startTag = string.Empty; // TODO: Initialize to an appropriate value
            string endTag = string.Empty; // TODO: Initialize to an appropriate value
            string text = string.Empty; // TODO: Initialize to an appropriate value
            HTMLElement target = new HTMLElement(startTag, endTag, text); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.FindNext();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
             */
        }

        /// <summary>
        ///A test for GetAttr
        ///</summary>
        [TestMethod()]
        public void GetAttrTest()
        {
            /*
            string startTag = string.Empty; // TODO: Initialize to an appropriate value
            string endTag = string.Empty; // TODO: Initialize to an appropriate value
            string text = string.Empty; // TODO: Initialize to an appropriate value
            HTMLElement target = new HTMLElement(startTag, endTag, text); // TODO: Initialize to an appropriate value
            string attrName = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetAttr(attrName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
             */
        }

        /// <summary>
        ///A test for GetText
        ///</summary>
        [TestMethod()]
        public void GetTextTest()
        {
            /*
            string startTag = string.Empty; // TODO: Initialize to an appropriate value
            string endTag = string.Empty; // TODO: Initialize to an appropriate value
            string text = string.Empty; // TODO: Initialize to an appropriate value
            HTMLElement target = new HTMLElement(startTag, endTag, text); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetText();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
             */
        }

        /// <summary>
        ///A test for RemoveTags
        ///</summary>
        [TestMethod()]
        public void RemoveTagsTest()
        {
            /*
            string startTag = string.Empty; // TODO: Initialize to an appropriate value
            string endTag = string.Empty; // TODO: Initialize to an appropriate value
            string text = string.Empty; // TODO: Initialize to an appropriate value
            HTMLElement target = new HTMLElement(startTag, endTag, text); // TODO: Initialize to an appropriate value
            target.RemoveTags();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
             */
        }

        /// <summary>
        ///A test for RemoveTagsWithText
        ///</summary>
        [TestMethod()]
        public void RemoveTagsWithTextTest()
        {
            string startTag = "<a";
            string endTag = "</a>";
            string text = "<a href=\"http://top.mail.ru/jump?from=8821\"><img src=\"http://d2.c2.b0.a0.top.mail.ru/counter?js=na;id=8821;t=109\" style=\"border:0;\" height=\"18\" width=\"88\" alt=\"Рейтинг@Mail.ru\" /></a>";
            HTMLElement target = new HTMLElement(startTag, endTag, text);
            target.FindNext();
            target.RemoveTagsWithText();
            Assert.AreEqual(target.Text, String.Empty);
        }

        /// <summary>
        ///A test for ReplaceTagsWithText
        ///</summary>
        [TestMethod()]
        public void ReplaceTagsWithTextTest()
        {
            /*
            string startTag = string.Empty; // TODO: Initialize to an appropriate value
            string endTag = string.Empty; // TODO: Initialize to an appropriate value
            string text = string.Empty; // TODO: Initialize to an appropriate value
            HTMLElement target = new HTMLElement(startTag, endTag, text); // TODO: Initialize to an appropriate value
            string text1 = string.Empty; // TODO: Initialize to an appropriate value
            target.ReplaceTagsWithText(text1);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
             */
        }

        /// <summary>
        ///A test for Text
        ///</summary>
        [TestMethod()]
        public void TextTest()
        {
            /*
            string startTag = string.Empty; // TODO: Initialize to an appropriate value
            string endTag = string.Empty; // TODO: Initialize to an appropriate value
            string text = string.Empty; // TODO: Initialize to an appropriate value
            HTMLElement target = new HTMLElement(startTag, endTag, text); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Text;
            Assert.Inconclusive("Verify the correctness of this test method.");
             */
        }
    }
}
