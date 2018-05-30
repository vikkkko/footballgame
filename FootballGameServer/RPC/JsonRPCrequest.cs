using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FootballGameServer.RPC
{
    public class JsonRPCrequest
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public object[] @params { get; set; }
        public long id { get; set; }
    }
}
