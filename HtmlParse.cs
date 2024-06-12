using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tview.scapproc.shellv1.Enums;
using wcheck.Utils;

namespace tview.scapproc.shellv1
{
    public static class HtmlParse
    {
        public static RiskLevel GetRiskLevel(this string id)
        {

            HtmlWeb web = new HtmlWeb();

            
           // Logger.Log(new LogContent($"Try parse {id} OVAL defenition info", web));

            var htmlDoc = web.Load($"https://ovaldbru.altx-soft.ru/Definition.aspx?id={id}");

            var node = htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/form[1]/div[3]/div[1]/div[1]/table[1]/tr[1]/td[6]/a[1]");

           // Logger.Log(new LogContent($"Parse {id} info complete: {node.InnerText}", node));
            var text = node.InnerText.ToLower();
            if(text == "критическая")
            {
                return RiskLevel.Critical;
            }
            else if (text == "высокая")
            {
                return RiskLevel.High;
            }
            else if (text == "средняя")
            {
                return RiskLevel.Medium;
            }
            else if (text == "средняя")
            {
                return RiskLevel.Medium;
            }
            else if (text == "низкая")
            {
                return RiskLevel.Low;
            }
            else if (text == "информация")
            {
                return RiskLevel.Info;
            }
            else
            {
                return RiskLevel.None;
            }
        }
    }
}
