using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Net.Http;
using Newtonsoft.Json;
using Solamis.Models.Result;
using System.Net.Http.Json;

namespace Solamis
{
    public class SmartContract
    {
        public SmartContract()
        {
            Root ReturnModel = new Root();
        }
        public async Task<Boolean> Check(string input)
        {
            var link = $"https://api.dexscreener.com/latest/dex/tokens/{input}";
            Boolean ret;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(link);
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and output the response content as a string
                        ret = true;
                        var ResponseString = await response.Content.ReadAsStringAsync();
                        Convert(ResponseString);
                    }
                    else
                    {
                        ret = false;
                        Console.WriteLine($"Error: {response.StatusCode}");
                    }
                }
                catch (Exception)
                {
                    ret = false;
                    throw;
                }
            }
            return ret;
        }
        public List<string> ConvertToList(string input)
        {
            List<string> ListString = new List<string>();
            var SplitString = input.Split(' ');
            foreach (var str in SplitString)
            {
                if (str.Length >= 20)
                {
                    ListString.Add(str);
                }
            }
            return ListString;
        }
        private void Convert(string input)
        {
            try
            {
                Root root = JsonConvert.DeserializeObject<Root>(input);

                if (root != null && root.pairs != null)
                {
                    foreach (var pair in root.pairs)
                    {
                        if (pair.dexId == "raydium" && pair.quoteToken.symbol =="SOL")
                        {
                            Console.WriteLine($"Pair Address: {pair.pairAddress}");
                            Console.WriteLine($"Base Token Address: {pair.baseToken?.address}");
                            Console.WriteLine($"Base Token Name: {pair.baseToken?.name}");
                            Console.WriteLine($"Base Token Symbol: {pair.baseToken?.symbol}");
                            Console.WriteLine($"Quote Token Address: {pair.quoteToken?.address}");
                            Console.WriteLine($"Quote Token Name: {pair.quoteToken?.name}");
                            Console.WriteLine($"Quote Token Symbol: {pair.quoteToken?.symbol}");
                            Console.WriteLine($"Chain Id: {pair.chainId}");
                            Console.WriteLine($"Dex Id: {pair.dexId}");
                            Console.WriteLine($"URL: {pair.url}");
                            Console.WriteLine();
                        }

                    }
                }
                else
                {
                    Console.WriteLine("Error: Root or Pairs are null.");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            }
        }
    }
}