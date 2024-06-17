using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wcheck.Utils;
using wcheck.wcontrols;
using wcheck.wshell.Enums;
using wcheck.wshell.Objects;
using wshell.Abstract;
using wshell.Net.Nodes;
using wshell.Utils;

namespace tview.scapproc.shellv1
{
    public class ScapShell : ShellBase
    {
        public MainPage Page { get; private set; }
        public ClientContext ClientContext { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        private CancellationTokenSource _cts { get; set; }
        public ScapShell() : base(new wshell.Objects.ShellInfo("SCAP", "1.0.0", new Guid("ee3de59e-00e8-40e9-bfcd-cba116b9a81d"), wshell.Enums.ShellType.TaskView, "SCAP процессор для интерпритации и эвалюации OVAL", "Артем И.С."))
        {
            Settings = ShellSettings.Load(ShellInfo.Id.ToString(), new List<SettingsObject>
            {
                //new SettingsObject
                //{
                //    Name = "pCheckBox",
                //    Value = true,
                //    ViewName = "CheckBox parameter"
                //},
                new SettingsObject
                {
                    Name = "pOvalDef",
                    Value = @"C:\ProgramData\Witherbit\wcheck\tmp\ee3de59e-00e8-40e9-bfcd-cba116b9a81d\oval.xml",
                    ViewAdditional = "Default C:\\ProgramData\\Witherbit\\wcheck\\tmp\\ee3de59e-00e8-40e9-bfcd-cba116b9a81d\\oval.xml",
                    ViewName = "Путь к файлу OVAL-определений"
                },
                //new SettingsObject
                //{
                //    Name = "pComboBox",
                //    Value = 0,
                //    ViewName = "Combo box parameter",
                //    ViewAdditional = "Item 1;Item 2;Item 3"
                //},
            });
        }

        public override Schema OnHostCallback(Schema schema)
        {
            Page.Invoke(() =>
            {
                Logger.Log(new LogContent($"Contract request handling: {JsonConvert.SerializeObject(schema, Formatting.Indented)}", this));
            });
            switch (schema.Type)
            {
                case CallbackType.StartTaskView:
                    Page.Invoke(() =>
                    {
                        Logger.Log(new LogContent($"Handle request: StartTaskView", this));
                    });
                    Page.StartTask();
                    break;
                case CallbackType.RedirectNetRequest:
                    var node = schema.GetProviding<Node>();
                    Page.Invoke(() =>
                    {
                        Logger.Log(new LogContent($"Handle request: RedirectNetRequest: {JsonConvert.SerializeObject(node, Formatting.Indented)}", this));
                    });
                    if (node.GetAttribute("type") == "start task")
                    {
                        ClientContext = new ClientContext(node, this);
                        ClientContext.StartTask();
                        Page.Invoke(() =>
                        {
                            Logger.Log(new LogContent($"Handle request: RedirectNetRequest: {ClientContext.Ip}", this));
                        });
                    }
                    return new Schema(CallbackType.RedirectNetResponse).SetProviding(new Node("redirect response", new Dictionary<string, string> { { "code", "200" } }));
            }
            return new Schema(CallbackType.EmptyResponse);
        }

        public override void OnPause()
        {

        }

        public override void OnResume()
        {

        }

        public override void OnRun()
        {
            //registering page
            _cts = new CancellationTokenSource();
            CancellationToken = _cts.Token;
            Page = new MainPage(this);
            Callback.Invoke(this, new Schema(CallbackType.RegisterPage).SetProviding(Page));
        }

        public override void OnStop()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            Callback.Invoke(this, new Schema(CallbackType.UnregisterPage).SetProviding(Page));
            Page = null;
        }

        public override void OnSettingsEdit(SettingsObject obj, PropertyEventArgs propertyEventArgs)
        {
            var setting = Settings.Get(obj.Name);
            if (propertyEventArgs.Type == PropertyType.TextBox)
            {
                setting.Value = propertyEventArgs.Text;
            }
            else if (propertyEventArgs.Type == PropertyType.CheckBox)
            {
                setting.Value = propertyEventArgs.Checked;
            }
            else if (propertyEventArgs.Type == PropertyType.ComboBox)
            {
                setting.Value = propertyEventArgs.SelectedIndex;
            }
            Settings.Save();
            this.InvokeSettingsPropertyChanged("b4877dc5-a5b5-4b7e-b08b-1b1995e8c8d8");
        }
    }
}
