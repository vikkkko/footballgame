using FootballGameServer.lib;
using FootballGameServer.RPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Security.Cryptography;

namespace FootballGameServer.Controllers
{
    public class Api
    {
        httpHelper hh = new httpHelper();
        mongoHelper mh = new mongoHelper();

        public Api() {
        }

        private JArray getJAbyKV(string key, object value)
        {
            return  new JArray
                        {
                            new JObject
                            {
                                {
                                    key,
                                    value.ToString()
                                }
                            }
                        };
        }

        private JArray getJAbyJ(JObject J)
        {
            return new JArray
                        {
                            J
                        };
        }

        public object getRes(JsonRPCrequest req,string reqAddr)
        {
            JArray result = new JArray();
            string resultStr = string.Empty;
            string findFliter = string.Empty;
            string sortStr = string.Empty;
            try
            {
                switch (req.method)
                {
                    case "signup":
                        {
                            string password = (string)req.@params[0];
                            ThinNeo.NEP6.NEP6Wallet nep6wallet = new ThinNeo.NEP6.NEP6Wallet();
                            nep6wallet.CreateAccount(password);
                            result = new JArray() { JObject.Parse(nep6wallet.GetWallet().ToString()) };
                        }
                        break;
                    case "signin":
                        {
                            string wif = (string)req.@params[0];
                            string str_wallet = (string)req.@params[1];
                            if (!string.IsNullOrEmpty(wif))
                            {
                                byte[] bytes_prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(wif);
                                string prikey = ThinNeo.Helper.Bytes2HexString(bytes_prikey);
                                byte[] bytes_pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(bytes_prikey);
                                string pubkey = ThinNeo.Helper.Bytes2HexString(bytes_pubkey);
                                string address = ThinNeo.Helper.GetAddressFromPublicKey(bytes_pubkey);
                                JObject Jo = new JObject();
                                Jo[address] = prikey;
                                result = new JArray() { Jo };
                            }
                            else if (!string.IsNullOrEmpty(str_wallet))
                            {
                                MyJson.JsonNode_Object wallet = MyJson.Parse(str_wallet.Replace("\r\n", "")).AsDict();
                                ThinNeo.NEP6.NEP6Wallet nep6wallet = new ThinNeo.NEP6.NEP6Wallet(wallet);
                                JArray Ja = new JArray();
                                foreach (var account in nep6wallet.accounts)
                                {
                                    JObject jObject = new JObject();
                                    jObject[ThinNeo.Helper.GetAddressFromPublicKey(account.Value.ScriptHash)] = account.Value.nep2key;
                                    Ja.Add(jObject);
                                }
                                result = Ja;
                            }
                        }
                        break;
                    case "getassets"://获取资产余额
                        {
                            string prikey = (string)req.@params[0];
                            JArray jArray = new JArray();
                            JObject jObject = new JObject();
                            jObject["id"] = "neo";
                            jObject["name"] = "小蚁股";
                            jObject["value"] = 123;
                            jArray.Add(jObject);
                            jObject = new JObject();
                            jObject["id"] = "gas";
                            jObject["name"] = "小蚁币";
                            jObject["value"] = 321;
                            jArray.Add(jObject);

                            result = jArray;
                        }
                        break;
                    case "chargemoney"://充钱
                        {
                            string prikey = (string)req.@params[0];
                            Int64 value = (Int64)req.@params[1];
                            result = getJAbyKV("result","success");
                        }
                        break;
                    case "getbackmoney"://退钱
                        {
                            string prikey = (string)req.@params[0];
                            Int64 value = (Int64)req.@params[1];
                            result = getJAbyKV("result", "success");
                        }
                        break;
                    case "bet"://下注
                        {
                            string prikey = (string)req.@params[0];
                            string seasonid = (string)req.@params[1];
                            string gameid = (string)req.@params[2];
                            Int64 value = (Int64)req.@params[3];
                            result = getJAbyKV("result", "success");
                        }
                        break;
                    case "take"://领奖
                        {
                            string prikey = (string)req.@params[0];
                            string seasonid = (string)req.@params[1];
                            string gameid = (string)req.@params[2];
                            result = getJAbyKV("result", "success");
                        }
                        break;
                    case "getseasoninfo": //获得赛季信息
                        {
                            string seasonid = (string)req.@params[0];
                            if (string.IsNullOrEmpty(seasonid))
                            {

                            }
                            else
                            {

                            }
                            JObject jObject = new JObject();
                            jObject["seasonid"] = "1";
                            JArray jArray = new JArray();
                            for (var i = 1; i < 16; i++)
                            {
                                JObject jObject2 = new JObject();
                                jObject2["gameid"] = i.ToString();
                                jObject2["team1"] = i;
                                jObject2["team2"] = i + 1;
                                jObject2["winner"] = i;
                                jArray.Add(jObject2);
                            }
                            jObject["info"] = jArray;
                            result = new JArray() { jObject };
                        }
                        break;
                    case "getgamestate"://获取比赛状态
                        {
                            Random rd = new Random();
                            var i = rd.Next(0, 17);
                            result = getJAbyKV("result", i);

                        }
                        break;
                }
                if (result.Count == 0)
                {
                    JsonPRCresponse_Error resE = new JsonPRCresponse_Error(req.id, -1, "No Data", "Data does not exist");

                    return resE;
                }
            }
            catch (Exception e)
            {
                JsonPRCresponse_Error resE = new JsonPRCresponse_Error(req.id, -100, "Parameter Error", e.Message);

                return resE;

            }

            JsonPRCresponse res = new JsonPRCresponse();
            res.jsonrpc = req.jsonrpc;
            res.id = req.id;
            res.result = result;

            return res;
        }
    }
}
