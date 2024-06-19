using Newtonsoft.Json;
using pwither.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using tview.scapproc.shellv1.Objects;
using wcheck;
using wcheck.Statistic.Templates;
using wcheck.Utils;
using wcheck.wcontrols;
using wcheck.wshell.Utils;
using wshell.Abstract;
using wshell.Net;
using wshell.Net.Nodes;
using wshell.Objects;
using wshell.Utils;

namespace tview.scapproc.shellv1
{
    public class ClientContext
    {
        public string TaskId {  get; }
        public string Ip { get; }
        public ShellClientProviding ClientProviding { get; set; }
        public ScapOutputRedirect EventRedirect { get; }
        private CancellationTokenSource _cancellationTokenSource;
        public ScapShell Shell { get; }

        public const string TargetId = "b4877dc5-a5b5-4b7e-b08b-1b1995e8c8d8";

        public ClientContext(Node node, ScapShell shell)
        {
            Shell = shell;
            TaskId = node.GetAttribute("task id");
            Ip = node.GetAttribute("from");
            EventRedirect = new ScapOutputRedirect(shell);
            Shell.SetEventRedirect(EventRedirect);
            EventRedirect.Output += OnOutput;
            EventRedirect.Complete += OnComplete;
            EventRedirect.State += OnState;
        }

        private void OnState(string obj)
        {
            ClientProviding.GetRedirectAsync(Ip, TargetId, new Node(null, obj)
                .SetAttribute("type", "task state")
                .SetAttribute("task id", TaskId));
        }

        private async void OnComplete(IStatisticTemplate template)
        {
            var tempPath = (string)Shell.RequestSettingsProperty(SettingsParamConsts.ParameterPath.p_PathToTemp).Value;
            var fileId = Guid.NewGuid().ToString().Replace("-", "");
            var fileName = tempPath + $@"\{fileId}.wuniversal";
            var node = new Node("file", NetHandleStatisticTemplate.ConvertToNetworkingTemplate(template))
                .SetAttribute("vuln-crit", Shell.Page.Processor.CriticalDefenitions.Length.ToString())
                .SetAttribute("vuln-high", Shell.Page.Processor.HighDefenitions.Length.ToString())
                .SetAttribute("vuln-mid", Shell.Page.Processor.MediumDefenitions.Length.ToString())
                .SetAttribute("vuln-low", Shell.Page.Processor.LowDefenitions.Length.ToString())
                .SetAttribute("inv", Shell.Page.Processor.InventoryDefenitions.Length.ToString());
            System.IO.File.WriteAllBytes(fileName, node.Pack(new SocketInitializeParameters { AuthType = wshell.Enums.SocketAuthType.Aes, UseEncryption = false}));
            File file = new File(fileName, 4096);
            var count = file.GetPartsCount().ToString();
            ClientProviding.GetRedirectAsync(Ip, TargetId, new Node(null)
                .SetAttribute("type", "task complete")
                .SetAttribute("task id", TaskId)
                .SetAttribute("content id", fileId)
                .SetAttribute("content count", count));
        }

        private void OnOutput(string obj)
        {
            ClientProviding.GetRedirectAsync(Ip, TargetId, new Node(null, obj)
                .SetAttribute("type", "task output")
                .SetAttribute("task id", TaskId));
        }

        public void StartTask()
        {
            ClientProviding = Shell.RequestClientProviding();
            Shell.Page.StartTask();
        }
    }
}
