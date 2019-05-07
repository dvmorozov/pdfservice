using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTMLCleanup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTMLCleanup.Tests
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
            var original = System.IO.File.ReadAllText(@"..\..\..\..\HTMLCleanupTests\TestFiles\original.html");
            var inputRemoved = System.IO.File.ReadAllText(@"..\..\..\..\HTMLCleanupTests\TestFiles\input_removed.html");
            var remover = new BaseHTMLCleaner.TagWithTextRemover(null)
            {
                Tags = new List<BaseHTMLCleaner.TagToRemove>(new BaseHTMLCleaner.TagToRemove[] {
                    //  Closing tag should be empty string.
                    new BaseHTMLCleaner.TagToRemove( "<input", "" )
                })
            };

            var removerResult = remover.Process(original);
            var removerResultLen = removerResult.Length;
            var inputRemovedLen = inputRemoved.Length;
            
            var writer = new System.IO.StreamWriter(@"..\..\..\..\HTMLCleanupTests\TestFiles\remover_result.html");
            writer.WriteLine(removerResult);

            Assert.IsTrue(removerResult == inputRemoved);
        }
    }
}