using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.CardNumbers
{
    public class CardNumbersResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<string> Data { get; set; }
    }
}
