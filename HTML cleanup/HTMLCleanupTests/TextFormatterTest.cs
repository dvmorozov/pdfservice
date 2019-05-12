using HtmlCleanup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlCleanupTests
{
    /// <summary>
    ///     This is a test class for TextFormatterTest and is intended
    ///     to contain all TextFormatterTest Unit Tests
    /// </summary>
    [TestClass]
    public class TextFormatterTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get;
            set;
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
        ///     A test for Process
        /// </summary>
        [TestMethod]
        public void ProcessTest()
        {
            var target = new BaseHtmlCleaner.TextFormatter(null);
            string text = "    К концу XX века многие заговорили об упадке железнодорожного сообщения, выте" +
                          "сняемого более популярными видами транспорта, прежде всего авиационным и автомоб" +
                          "ильным. Однако мрачные прогнозы не оправдались: в 2000-х годах интерес к поездам" +
                          " как среди клиентов, уставших от автомобильных пробок, так и на государственном " +
                          "уровне, где решаются вопросы развития существующей сети железнодорожных путей, с" +
                          "тал расти. Впрочем, Россию <железнодорожный бум> обходит стороной: несмотря на в" +
                          "се разговоры, высокоскоростные магистрали в нашей стране пока так и не появились" +
                          ".";
            string expected =
                "    К концу XX века многие заговорили об упадке железнодорожного сообщения, \r\n" +
                "вытесняемого более популярными видами транспорта, прежде всего авиационным и \r\n" +
                "автомобильным. Однако мрачные прогнозы не оправдались: в 2000-х годах интерес к \r\n" +
                "поездам как среди клиентов, уставших от автомобильных пробок, так и на \r\n" +
                "государственном уровне, где решаются вопросы развития существующей сети \r\n" +
                "железнодорожных путей, стал расти. Впрочем, Россию <железнодорожный бум> обходит \r\n" +
                "стороной: несмотря на все разговоры, высокоскоростные магистрали в нашей стране \r\n" +
                "пока так и не появились.\r\n";
            string actual;
            target.Delimiters = new char[]{' ', ',', '.', '-', '!', '?', ';'};
            actual = target.Process(text);
            Assert.AreEqual(expected, actual);

            text =
                "    Экономика России вырастет в этом году не более чем на 3,4%, прогнозирует Между" +
                "народный валютный фонд. Январский прогноз фонда был оптимистичнее на 0,3%. Повод д" +
                "ля снижения прогноза - дешевеющая нефть и связанное с этим замедление внутреннего " +
                "спроса. Глобальная экономика также будет замедляться: прогноз по мировому ВВП пони" +
                "жен до 3,3%. ";
            expected =
                "    Экономика России вырастет в этом году не более чем на 3,4%, прогнозирует \r\n" +
                "Международный валютный фонд. Январский прогноз фонда был оптимистичнее на 0,3%. \r\n" +
                "Повод для снижения прогноза - дешевеющая нефть и связанное с этим замедление \r\n" +
                "внутреннего спроса. Глобальная экономика также будет замедляться: прогноз по \r\n" +
                "мировому ВВП понижен до 3,3%. \r\n";

            target = new BaseHtmlCleaner.TextFormatter(null)
            {
                Delimiters = new char[] { ' ', ',', '.', '-', '!', '?', ';' }
            };
            actual = target.Process(text);
            Assert.AreEqual(expected, actual);
        }
    }
}