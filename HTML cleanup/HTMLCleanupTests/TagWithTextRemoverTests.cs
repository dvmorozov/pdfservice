using Microsoft.VisualStudio.TestTools.UnitTesting;
using HtmlCleanup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlCleanup.Tests
{
    [TestClass()]
    public class TagWithTextRemoverTests
    {
        [TestMethod()]
        public void TagWithTextRemoverTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void ProcessTest()
        {
            var original = System.IO.File.ReadAllText(@"..\..\..\..\HtmlCleanupTests\TestFiles\original.html");
            var inputRemoved = System.IO.File.ReadAllText(@"..\..\..\..\HtmlCleanupTests\TestFiles\input_removed.html");
            var remover = new BaseHtmlCleaner.TagRemover(null)
            {
                Tags = new List<BaseHtmlCleaner.Tag>(new BaseHtmlCleaner.Tag[] {
                    //  Closing tag should be empty string.
                    new BaseHtmlCleaner.Tag( "<input", "" )
                })
            };

            var removerResult = remover.Process(original);
            var removerResultLen = removerResult.Length;
            var inputRemovedLen = inputRemoved.Length;
            
            var writer = new System.IO.StreamWriter(@"..\..\..\..\HtmlCleanupTests\TestFiles\remover_result.html");
            writer.WriteLine(removerResult);

            Assert.IsTrue(removerResult == inputRemoved);
        }
    }
}