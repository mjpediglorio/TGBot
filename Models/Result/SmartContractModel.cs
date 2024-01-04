using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solamis.Models.Result
{
    public class BaseToken
    {
        public string address { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class QuoteToken
    {
        public string address { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class Pair
    {
        public string chainId { get; set; }
        public string dexId { get; set; }
        public string url { get; set; }
        public string pairAddress { get; set; }
        public BaseToken baseToken { get; set; }
        public QuoteToken quoteToken { get; set; }
    }

    public class Root
    {
        public string schemaVersion { get; set; }
        public List<Pair> pairs { get; set; }
    }
}