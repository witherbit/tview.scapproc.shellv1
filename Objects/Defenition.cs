using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tview.scapproc.shellv1.Objects
{
    public class Defenition
    {
        public string Id {  get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public string Risk { get; set; }

        public string Description { get; set; }

        public string FstecReference { get; set; }

        public string? Hyperlink { get; set; }
    }
}
