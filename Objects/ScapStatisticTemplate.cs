﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcheck.Statistic.Nodes;
using wcheck.Statistic.Styles;
using wcheck.Statistic.Templates;

namespace tview.scapproc.shellv1.Objects
{
    public class ScapStatisticTemplate : IStatisticTemplate
    {
        public List<IStatisticNode> Nodes { get; }

        public TextNodeStyle HeaderStyle { get; }

        public string? Header { get; set; }

        public bool UseBreakAfterTemplate { get; set; }
    }
}
