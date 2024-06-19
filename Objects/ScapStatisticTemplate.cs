using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcheck.Statistic.Items;
using wcheck.Statistic.Nodes;
using wcheck.Statistic.Styles;
using wcheck.Statistic.Templates;
using pwither.formatter;

namespace tview.scapproc.shellv1.Objects
{
    [BitSerializable]
    public class ScapStatisticTemplate : IStatisticTemplate
    {
        public List<IStatisticNode> Nodes { get; }

        public TextNodeStyle HeaderStyle => new TextNodeStyle
        {
            SpacingBetweenLines = 2,
            FontSize = 12,
            IsBold = true,
            WpfFontSize = 14,
            Aligment = wcheck.Statistic.Enums.TextAligment.Center
        };

        public string? Header => "Результаты проведения аудита и инвентаризации ПО";

        public bool UseBreakAfterTemplate => true;

        public ScapStatisticTemplate(Defenition[] critical, Defenition[] high, Defenition[] mid, Defenition[] low, Defenition[] inv, string hostName)
        {
            var pies = new List<PieItem>();
            if(critical.Length > 0)
            {
                pies.Add(new PieItem("Критический УР", new[] { critical.Length }, "#976565"));
            }
            if (high.Length > 0)
            {
                pies.Add(new PieItem("Высокий УР", new[] { high.Length }, "#ff9090"));
            }
            if (mid.Length > 0)
            {
                pies.Add(new PieItem("Средний УР", new[] { mid.Length }, "#ffd691"));
            }
            if (low.Length > 0)
            {
                pies.Add(new PieItem("Низкий УР", new[] { low.Length }, "#b3ff91"));
            }
            Nodes = new List<IStatisticNode>
            {
                new TextStatisticNode(),
                new TextStatisticNode($"Проверяемый объект: {hostName}", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Right,
                    FontSize = 10,
                    WpfFontSize= 12,
                    IsItalic = true,
                }),
                new TextStatisticNode(),
                new TextStatisticNode($"Краткая статистика", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize= 14,
                    IsItalic = true,
                }),
                new PieStatisticNode(pies),
                new TextStatisticNode(),
                new TextStatisticNode($"В результате аудита объекта, было выявлено:"
                , new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Both,
                    FontSize = 12,
                    WpfFontSize= 14,
                    IsBold = true,
                    WpfMargin = new WpfThinkness(5, 0, 5 ,5),
                    SpacingBetweenLines = 1.5
                }),
                new TextStatisticNode(
                $"Уязвимостей с критическим уровнем риска: {critical.Length};",
                new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Both,
                    FontSize = 12,
                    WpfFontSize= 14,
                    IsBold = true,
                    WpfMargin = new WpfThinkness(5, 0, 5 ,5),
                    SpacingBetweenLines = 1.5
                }),
                new TextStatisticNode(
                $"Уязвимостей с высоким уровнем риска: {high.Length};",
                new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Both,
                    FontSize = 12,
                    WpfFontSize= 14,
                    IsBold = true,
                    WpfMargin = new WpfThinkness(5, 0, 5 ,5),
                    SpacingBetweenLines = 1.5
                }),
                new TextStatisticNode(
                $"Уязвимостей с средним уровнем риска: {mid.Length};",
                new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Both,
                    FontSize = 12,
                    WpfFontSize= 14,
                    IsBold = true,
                    WpfMargin = new WpfThinkness(5, 0, 5 ,5),
                    SpacingBetweenLines = 1.5
                }),
                new TextStatisticNode(
                $"Уязвимостей с низким уровнем риска: {low.Length};",
                new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Both,
                    FontSize = 12,
                    WpfFontSize= 14,
                    IsBold = true,
                    WpfMargin = new WpfThinkness(5, 0, 5 ,5),
                    SpacingBetweenLines = 1.5
                }),
                new TextStatisticNode($"Количество инвентаризируемого ПО: {inv.Length}.",
                new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Both,
                    FontSize = 12,
                    WpfFontSize= 14,
                    IsBold = true,
                    WpfMargin = new WpfThinkness(5, 0, 5 ,5),
                    SpacingBetweenLines = 1.5
                }),
                new TextStatisticNode(),
            };
            if (critical.Length > 0)
            {
                Nodes.Add(new TextStatisticNode($"Уязвимости с критическим уровнем риска", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Right,
                    FontSize = 12,
                    WpfFontSize = 14,
                    IsItalic = true,
                    WpfMargin = new WpfThinkness(5, 0, 5, 5),
                    SpacingBetweenLines = 1.5
                }));
                int row = 1;
                var ceils = new List<CeilItem>();
                ceils.Add(new CeilItem("Название", 0, 0, "#976565", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("Описание", 1, 0, "#976565", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("Доп. информация и способы решения", 2, 0, "#976565", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                foreach (var crit in critical)
                {
                    ceils.Add(new CeilItem(crit.Title, 0, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(crit.Description, 1, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(crit.Hyperlink, 2, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                        IsHyperlink = true,
                        HyperlinkUrl = crit.Hyperlink,
                    }));
                    row++;
                }
                Nodes.Add(new TableStatisticNode(ceils));
                Nodes.Add(new TextStatisticNode());
            }
            if (high.Length > 0)
            {
                Nodes.Add(new TextStatisticNode($"Уязвимости с высоким уровнем риска", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Right,
                    FontSize = 12,
                    WpfFontSize = 14,
                    IsItalic = true,
                    WpfMargin = new WpfThinkness(5, 0, 5, 5),
                    SpacingBetweenLines = 1.5
                }));
                int row = 1;
                var ceils = new List<CeilItem>();
                ceils.Add(new CeilItem("Название", 0, 0, "#ff9090", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("Описание", 1, 0, "#ff9090", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("Доп. информация и способы решения", 2, 0, "#ff9090", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                foreach (var crit in high)
                {
                    ceils.Add(new CeilItem(crit.Title, 0, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(crit.Description, 1, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(crit.Hyperlink, 2, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                        IsHyperlink = true,
                        HyperlinkUrl = crit.Hyperlink,
                    }));
                    row++;
                }
                Nodes.Add(new TableStatisticNode(ceils));
                Nodes.Add(new TextStatisticNode());
            }
            if (mid.Length > 0)
            {
                Nodes.Add(new TextStatisticNode($"Уязвимости с средним уровнем риска", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Right,
                    FontSize = 12,
                    WpfFontSize = 14,
                    IsItalic = true,
                    WpfMargin = new WpfThinkness(5, 0, 5, 5),
                    SpacingBetweenLines = 1.5
                }));
                int row = 1;
                var ceils = new List<CeilItem>();
                ceils.Add(new CeilItem("Название", 0, 0, "#ffd691", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("Описание", 1, 0, "#ffd691", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("Доп. информация и способы решения", 2, 0, "#ffd691", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                foreach (var crit in mid)
                {
                    ceils.Add(new CeilItem(crit.Title, 0, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(crit.Description, 1, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(crit.Hyperlink, 2, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                        IsHyperlink = true,
                        HyperlinkUrl = crit.Hyperlink,
                    }));
                    row++;
                }
                Nodes.Add(new TableStatisticNode(ceils));
                Nodes.Add(new TextStatisticNode());
            }
            if (low.Length > 0)
            {
                Nodes.Add(new TextStatisticNode($"Уязвимости с низким уровнем риска", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Right,
                    FontSize = 12,
                    WpfFontSize = 14,
                    IsItalic = true,
                    WpfMargin = new WpfThinkness(5, 0, 5, 5),
                    SpacingBetweenLines = 1.5
                }));
                int row = 1;
                var ceils = new List<CeilItem>();
                ceils.Add(new CeilItem("Название", 0, 0, "#b3ff91", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("Описание", 1, 0, "#b3ff91", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("Доп. информация и способы решения", 2, 0, "#b3ff91", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                foreach (var crit in low)
                {
                    ceils.Add(new CeilItem(crit.Title, 0, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(crit.Description, 1, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(crit.Hyperlink, 2, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                        IsHyperlink = true,
                        HyperlinkUrl = crit.Hyperlink,
                    }));
                    row++;
                }
                Nodes.Add(new TableStatisticNode(ceils));
                Nodes.Add(new TextStatisticNode());
            }
            if (inv.Length > 0)
            {
                Nodes.Add(new TextStatisticNode($"Инвентаризация ПО", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Right,
                    FontSize = 12,
                    WpfFontSize = 14,
                    IsItalic = true,
                    WpfMargin = new WpfThinkness(5, 0, 5, 5),
                    SpacingBetweenLines = 1.5
                }));
                int row = 1;
                var ceils = new List<CeilItem>();
                ceils.Add(new CeilItem("По", 0, 0, "#91b2ff", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("OVALdb ID", 1, 0, "#91b2ff", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                foreach (var crit in inv)
                {
                    ceils.Add(new CeilItem(crit.Title, 0, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem($"https://ovaldbru.altx-soft.ru/Definition.aspx?id={crit.Id}", 1, row, new TextNodeStyle
                    {
                        Aligment = wcheck.Statistic.Enums.TextAligment.Left,
                        FontSize = 12,
                        WpfFontSize = 14,
                        IsHyperlink = true,
                        HyperlinkUrl = $"https://ovaldbru.altx-soft.ru/Definition.aspx?id={crit.Id}"
                    }));
                    row++;
                }
                Nodes.Add(new TableStatisticNode(ceils));
                Nodes.Add(new TextStatisticNode());
            }
        }
    }
}
