using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

namespace Wox.Plugin.PasswordGenerator
{
    public class Main : IPlugin
    {
        private readonly Dictionary<String, String> Seed = new Dictionary<string, string>
        {
            { "n","1234567890"},
            { "w","abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"},
            { "s","!@#$%^&*()_+=-"}
        };

        private static readonly Regex RegNumber = new Regex(@"\d+", RegexOptions.Compiled);

        private readonly int COUNT = 5;

        public void Init(PluginInitContext context)
        {
        }
        public List<Result> Query(Query query)
        {
            string sl = RegNumber.Match(query.Search).Value;
            int length = 16;
            if (sl != "")
                length = int.Parse(sl);

            List<Result> results = new List<Result>()
            {
                new Result(){
                    Title = "Password Generator | 帮助：",
                    SubTitle = "pwd nws16lug >>> n 数字；w 字母；s 特殊字符；g 生成GUID； 16 生成的密码长度；l 小写；u 大写",
                    IcoPath = "Images\\app.png"
                }
            };
            string p = "";
            for (int i = 0; i < COUNT; i++)
            {
                p = GenRandomPassword(length, query.Search);
                Result result = new Result()
                {
                    Title = (i + 1) + "、" + p,
                    // TODO 密码强度
                    // SubTitle = "",
                    IcoPath = "Images\\app.png",
                    Action = c =>
                    {
                        Clipboard.SetText(p);
                        return true;
                    }
                };
                results.Add(result);
            }


            return results;
        }

        private string GenRandomPassword(int length, string types)
        {
            string s = Seed["n"] + Seed["w"];
            bool flag = true;
            foreach (var t in types.ToCharArray())
            {
                string type = t.ToString();
                if (Seed.ContainsKey(type))
                {
                    if (flag)
                    {
                        s = "";
                        flag = false;
                    }
                    s += Seed[type];
                }
            }
            char[] cs = s.ToCharArray();
            string result = "";
            if (types.IndexOf('g') >= 0)
            {
                result = Guid.NewGuid().ToString();
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    byte[] buffer = Guid.NewGuid().ToByteArray();
                    int iSeed = BitConverter.ToInt32(buffer, 0);
                    Random rd = new Random(iSeed);
                    result += cs[rd.Next(cs.Length)].ToString();
                }
            }
            if (types.IndexOf('l') >= 0)
            {
                result = result.ToLower();
            }
            else if (types.IndexOf('u') >= 0)
            {
                result = result.ToUpper();
            }
            return result;
        }
    }
}
