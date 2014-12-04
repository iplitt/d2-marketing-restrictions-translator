using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Lib;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

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

        private double totalSeconds = 0;

        private async Task<List<MyResult>> GetRestrictionsAsync_Old(List<string> upcs, string countryCode)
        {
            var list = new List<MyResult>();

            totalSeconds = 0;
            Stopwatch swElapsed = new Stopwatch();
            swElapsed.Start();
            Logger.LogInfo("Starting...");

            for (int i = 0; i < upcs.Count; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

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

                sw.Stop();
                Logger.LogInfo(i.ToString() + " " + sw.Elapsed.TotalSeconds.ToString());
                totalSeconds += sw.Elapsed.TotalSeconds;

            }

            swElapsed.Stop();
            Logger.LogInfo("Total time waiting on all tasks: " + totalSeconds.ToString() + " Total Elapsed Time: " + swElapsed.Elapsed.TotalSeconds.ToString());
            return list;
        }

        private List<RestrictionsResult> GetRestrictionsAsync_New(List<string> upcs, string countryCode, int maxThreads)
        {
            var list = new List<RestrictionsResult>();

            // break up into batches.
            var batches = GetBatches(upcs, maxThreads);

            foreach (var batch in batches)
            {
                var result = GetRestrictionsAsync_New(batch, countryCode);
                list.AddRange(result);
            }

            return list;
        }
        public static List<List<T>> GetBatches<T>(List<T> inputList, int batchSize)
        {
            List<List<T>> batches = new List<List<T>>();

            int batchCount = 0;
            List<T> list = new List<T>();

            foreach (var item in inputList)
            {
                if (batchCount == 0)
                {
                    list = new List<T>();
                    batches.Add(list);
                }

                list.Add(item);
                batchCount++;

                if (batchCount == batchSize)
                    batchCount = 0;
            }

            return batches;
        }
        private List<RestrictionsResult> GetRestrictionsAsync_New(List<string> upcs, string countryCode)
        {
            var list = new List<RestrictionsResult>();

            totalSeconds = 0;
            Stopwatch swElapsed = new Stopwatch();
            swElapsed.Start();
            Logger.LogInfo("Starting...");

            var workerDict = new Dictionary<string, RestrictionsWorker>();
            var threadDict = new Dictionary<string, Thread>();

            for (int i = 0; i < upcs.Count; i++)
            {
                var upc = upcs[i];
                Logger.LogInfo("Starting " + i.ToString());

                var worker = new RestrictionsWorker(i, upc, countryCode);
                var thread = new Thread(new ThreadStart(worker.Go));
                workerDict.Add(upc, worker);
                threadDict.Add(upc, thread);
                thread.Start();
            }

            foreach (var key in threadDict.Keys)
            {
                var thread = threadDict[key];
                thread.Join();
                var worker = workerDict[key];
                list.Add(worker.Result);
            }

            totalSeconds = list.Sum(x => x.TotalSeconds);

            swElapsed.Stop();
            Logger.LogInfo("Total time waiting on all tasks: " + totalSeconds.ToString() + " Total Elapsed Time: " + swElapsed.Elapsed.TotalSeconds.ToString());
            return list;
        }

        private async Task<MyResult> GetMyResultAsync(int i, string upc, string countryCode)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
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

            myResult.Restrictions = myResult.Exists ? string.Join(" ", finalSentenceList.ToArray()) : "Not found.";

            sw.Stop();
            totalSeconds += sw.Elapsed.TotalSeconds;
            Logger.LogInfo(i.ToString() + " " + sw.Elapsed.TotalSeconds.ToString());

            return myResult;
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

            var response = GetRestrictionsAsync_Old(list, "US");
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

            var response = GetRestrictionsAsync_Old(list, "US");
            Assert.AreEqual(50, response.Result.Count);
        }

        [TestMethod]
        public void GenerateRestrictionsReportFor100Items_Old()
        {
            var inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/Results.txt");

            var lines = UtilFile.GetTextLines(inputPath);
            var response = GetRestrictionsAsync_Old(lines.Take(100).ToList(), "US");

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

        [TestMethod]
        public void GenerateRestrictionsReportFor100Items()
        {
            var maxThreads = int.Parse(ConfigurationManager.AppSettings["MaxThreads"]);

            var inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/Results.txt");

            var lines = UtilFile.GetTextLines(inputPath);
            var response = GetRestrictionsAsync_New(lines.Take(100).ToList(), "US", maxThreads);

            using (var sw = new StreamWriter(outputPath))
            {
                foreach (var item in response)
                {
                    sw.WriteLine(item.Upc + "\t" + item.Restrictions);
                }
                sw.Flush();
                sw.Close();
            }

        }


        [TestMethod]
        public void GenerateFinalReport()
        {
            var inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/UPCs.txt");
            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Files/Results.txt");

            var lines = UtilFile.GetTextLines(inputPath);
            var response = GetRestrictionsAsync_Old(lines, "US");

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
