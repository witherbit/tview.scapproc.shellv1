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
            Shell.Page.Invoke(() =>
            {
                Logger.Log(new LogContent($"Client Context task: TaskId = {TaskId}", this));
            });
            Ip = node.GetAttribute("from");
            Shell.Page.Invoke(() =>
            {
                Logger.Log(new LogContent($"Client Context task: Ip = {Ip}", this));
            });
            EventRedirect = new ScapOutputRedirect(shell);
            Shell.Page.Invoke(() =>
            {
                Logger.Log(new LogContent($"Client Context task: EV = {EventRedirect}", this));
            });
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

        private void OnComplete(IStatisticTemplate template)
        {
            ClientProviding.GetRedirectAsync(Ip, TargetId, new Node(null)
                .SetAttribute("type", "task complete")
                .SetAttribute("task id", TaskId)
                .SetAttribute("vuln-crit", Shell.Page.Processor.CriticalDefenitions.Length.ToString())
                .SetAttribute("vuln-high", Shell.Page.Processor.HighDefenitions.Length.ToString())
                .SetAttribute("vuln-mid", Shell.Page.Processor.MediumDefenitions.Length.ToString())
                .SetAttribute("vuln-low", Shell.Page.Processor.LowDefenitions.Length.ToString())
                .SetAttribute("inv", Shell.Page.Processor.InventoryDefenitions.Length.ToString()));
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
            Shell.Page.Invoke(() =>
            {
                Logger.Log(new LogContent($"Client Context task: Client = {ClientProviding}", this));
            });
            Shell.Page.Invoke(() =>
            {
                Logger.Log(new LogContent($"Start Client Context task", this));
            });
            Shell.Page.StartTask();
        }
    }
}
