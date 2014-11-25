using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Lib;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetTextTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var lines = UtilFile.GetTextLines(path);
            Assert.AreEqual(1789, lines.Count);
        }

        [TestMethod]
        public void ProductTermTest()
        {
            var response = Helper.GetProductRestrictionsAsync("00000000000000", "US");
            Assert.AreEqual(true, response.Result.ProductExists);
        }


        private async Task<List<string>> GetUpcsAsync(List<string> upcs, string countryCode)
        {
            var list = new List<string>();

            foreach (var upc in upcs)
            {
                try
                {
                    var response = await Helper.GetProductRestrictionsAsync(upc, countryCode);
                    list.Add(upc + " " + response.ProductExists.ToString());
                }
                catch (Exception ex)
                {
                    list.Add(upc + " " + ex.Message);
                }
            }
            return list;
        }

        [TestMethod]
        public void Get10UpcsTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var lines = UtilFile.GetTextLines(path);
            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(lines[i]);
            }

            var response = GetUpcsAsync(list, "US");
            Assert.AreEqual(10, response.Result.Count);
        }

        [TestMethod]
        public void Get50UpcsTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var lines = UtilFile.GetTextLines(path);
            var list = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                list.Add(lines[i]);
            }

            var response = GetUpcsAsync(list, "US");
            Assert.AreEqual(50, response.Result.Count);
        }

        [TestMethod]
        public void Get100UpcsTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var lines = UtilFile.GetTextLines(path);
            var list = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(lines[i]);
            }

            var response = GetUpcsAsync(list, "US");
            Assert.AreEqual(100, response.Result.Count);
        }

        [TestMethod]
        public void Get500UpcsTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var lines = UtilFile.GetTextLines(path);
            var list = new List<string>();
            for (int i = 0; i < 500; i++)
            {
                list.Add(lines[i]);
            }

            var response = GetUpcsAsync(list, "US");
            Assert.AreEqual(500, response.Result.Count);
        }

        [TestMethod]
        public void ExceptionUpc00015707827720Test()
        {
            var upc = "00015707827720";
            var countryCode = "US";

            var success = false;
            var message = "";
            try
            {
                var response = Helper.GetProductRestrictions(upc, countryCode);
                success = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(false, success);
            Assert.AreEqual("Product does not exist.", message);

        }

        private class MyResult
        {
            public int Index;
            public string Upc;
            public ProductRestrictionItems ProductItems;
            public PartnerRestrictionItems PartnerItems;
            public bool Exists;
            public string Restrictions;
            public string Note;
        }

        private async Task<List<MyResult>> GetRestrictionsAsync(List<string> upcs, string countryCode)
        {
            var list = new List<MyResult>();

            for (int i = 0; i < upcs.Count; i++)
            {
                var upc = upcs[i];
                var myResult = new MyResult { Index = i, Upc = upc, Exists = false, Restrictions = "", Note = "" };
                
                try
                {
                    var productResponse = await Helper.GetProductRestrictionsAsync(upc, countryCode);
                    myResult.ProductItems = D2MappingHelper.GetMappedProductRestrictions(productResponse);
                    myResult.Exists = true;
                    // Use only the last note.
                    if (productResponse.DeliveryCommentsList.Count > 0)
                        myResult.Note = productResponse.DeliveryCommentsList.Last();
                }
                catch
                {
                    // Do nothing.
                }
                try
                {
                    var partnerResponse = await Helper.GetPartnerRestrictionsAsync(upc, countryCode);
                    myResult.PartnerItems = D2MappingHelper.GetMappedPartnerRestrictions(partnerResponse);
                }
                catch
                {
                    // Do nothing.
                }

                // Build final sentence.
                var finalSentenceList = new List<string>();
                finalSentenceList.Add(D2MappingHelper.CombineProductRestrictions(myResult.ProductItems));
                finalSentenceList.Add(D2MappingHelper.CombinePartnerRestrictions(myResult.PartnerItems));
                finalSentenceList.Add(myResult.Note);

                finalSentenceList = finalSentenceList.Where(x => x != "").ToList();

                myResult.Restrictions = myResult.Exists? string.Join(" ", finalSentenceList.ToArray()) : "Not found.";

                list.Add(myResult);

            }
            return list;
        }

        [TestMethod]
        public void Get10MappedResultsTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var lines = UtilFile.GetTextLines(path);
            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(lines[i]);
            }

            var response = GetRestrictionsAsync(list, "US");
            Assert.AreEqual(10, response.Result.Count);
        }

        [TestMethod]
        public void Get50MappedResultsTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var lines = UtilFile.GetTextLines(path);
            var list = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                list.Add(lines[i]);
            }

            var response = GetRestrictionsAsync(list, "US");
            Assert.AreEqual(50, response.Result.Count);
        }

        [TestMethod]
        public void GenerateFinalReport()
        {
            var inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/Results.txt");

            var lines = UtilFile.GetTextLines(inputPath);
            var response = GetRestrictionsAsync(lines, "US");

            using (var sw = new StreamWriter(outputPath))
            {
                foreach (var item in response.Result)
                {
                    sw.WriteLine(item.Upc + "\t" + item.Restrictions);
                }
                sw.Flush();
                sw.Close();
            }

        }


    }
}
