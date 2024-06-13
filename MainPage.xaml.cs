using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wcheck.wshell.Enums;
using wcheck.wshell.Objects;
using wcheck;
using wshell.Enums;
using wcheck.wcontrols;
using Newtonsoft.Json;
using tview.scapproc.shellv1.Objects;
using System.IO;
using wcheck.Utils;
using System.Collections;
using System.ComponentModel;
using wshell.Utils;
using static MaterialDesignThemes.Wpf.Theme;
using tview.scapproc.shellv1.Enums;
using System.Diagnostics;
using tview.scapproc.shellv1.Controls;


namespace tview.scapproc.shellv1
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private static Brush _inProgress = "#77cefc".GetBrush();
        private static Brush _inComplete = "#fca577".GetBrush();
        public ScapProcessor Processor { get; private set; }
        public ScapShell Shell { get; set; }
        public bool CanClose { get; private set; }
        public MainPage(ScapShell shell)
        {
            InitializeComponent();
            Shell = shell;
            var xsdPath = (string)Shell.RequestSettingsProperty(SettingsParamConsts.ParameterPath.p_PathToXds).Value;
            var defsPath = Shell.Settings.GetValue<string>("pOvalDef");
            Processor = new ScapProcessor(xsdPath, defsPath);
            Processor.StateChanged += OnStateChanged;
            Processor.OvalInfoLoaded += OnOvalInfoLoaded;
            uiCircleWait.Fill = _inProgress;
        }

        private void OnOvalInfoLoaded(object? sender, string e)
        {
            this.Invoke(new Action(() =>
            {
                uiTextCaption.Text = e;
            }));
            Processor.OutputRedirect.InvokeEventAsync("output", e);
        }
        private void OnStateChanged(object? sender, ScapState e)
        {
            switch (e)
            {
                case ScapState.None:
                    break;
                case ScapState.Initialize:
                    this.Invoke(() =>
                    {
                        uiCircleWait.Fill = _inComplete;
                        uiCircleInit.Fill = _inProgress;
                    });
                    Processor.OutputRedirect.InvokeEventAsync("state", "Инициализация SCAP [2/7]");
                    break;
                case ScapState.LoadDefenitions:
                    this.Invoke(() =>
                    {
                        uiCircleInit.Fill = _inComplete;
                        uiCircleLoad.Fill = _inProgress;
                    });
                    Processor.OutputRedirect.InvokeEventAsync("state", "Загрузка OVAL определений [3/7]");
                    break;
                case ScapState.Eval:
                    this.Invoke(() =>
                    {
                        uiCircleLoad.Fill = _inComplete;
                        uiCircleEval.Fill = _inProgress;
                    });
                    Processor.OutputRedirect.InvokeEventAsync("state", "Выолнение OVAL определений [4/7]");
                    break;
                case ScapState.GetInfo:
                    this.Invoke(() =>
                    {
                        uiCircleEval.Fill = _inComplete;
                        uiCircleGetList.Fill = _inProgress;
                        uiCircleGetList.Fill = _inComplete;
                        uiCircleGetInfo.Fill = _inProgress;
                    });
                    Processor.OutputRedirect.InvokeEventAsync("state", "Анализ найденных уязвимостей [6/7]");
                    break;
                case ScapState.Result:
                    this.Invoke(() =>
                    {
                        uiCircleGetInfo.Fill = _inComplete;
                        uiCircleStop.Fill = _inProgress;

                        Processor.OutputRedirect.InvokeEventAsync("state", "Завершение задания [7/7]");

                        SetResult();
                    });
                    break;
            }
        }
        public void StartTask()
        {
            Processor.OutputRedirect = Shell.GetEventRedirect();
            Processor.OutputRedirect.InvokeEventAsync("state", "Запуск задания [1/7]");
            Processor.StartAsync(Shell.CancellationToken);
        }
        private void uiCloseTab_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetResult()
        {
            if (Processor.CriticalDefenitions.Length > 0)
            {
                uiExpanderCritical.Visibility = Visibility.Visible;
                AddRangeDefenitions(uiStackPanelCritical, Processor.CriticalDefenitions);
                uiTextCritical.Text += $" [{Processor.CriticalDefenitions.Length}]";
            }
            if (Processor.HighDefenitions.Length > 0)
            {
                uiExpanderHigh.Visibility = Visibility.Visible;
                AddRangeDefenitions(uiStackPanelHigh, Processor.HighDefenitions);
                uiTextHigh.Text += $" [{Processor.HighDefenitions.Length}]";
            }
            if (Processor.MediumDefenitions.Length > 0)
            {
                uiExpanderMedium.Visibility = Visibility.Visible;
                AddRangeDefenitions(uiStackPanelMedium, Processor.MediumDefenitions);
                uiTextMedium.Text += $" [{Processor.MediumDefenitions.Length}]";
            }
            if (Processor.LowDefenitions.Length > 0)
            {
                uiExpanderLow.Visibility = Visibility.Visible;
                AddRangeDefenitions(uiStackPanelLow, Processor.LowDefenitions);
                uiTextLow.Text += $" [{Processor.LowDefenitions.Length}]";
            }
            if (Processor.InventoryDefenitions.Length > 0)
            {
                uiExpanderInventory.Visibility = Visibility.Visible;
                AddRangeDefenitions(uiStackPanelInventory, Processor.InventoryDefenitions);
                uiTextInventory.Text += $" [{Processor.InventoryDefenitions.Length}]";
            }

            uiGridCaption.Visibility = Visibility.Collapsed;

            uiCircleStop.Fill = _inComplete;
            Processor.OutputRedirect.InvokeEventAsync("state", "Выполнено");
            Processor.OutputRedirect.InvokeEventAsync("complete", new ScapStatisticTemplate());
        }

        private void AddRangeDefenitions(StackPanel panel, IEnumerable<Defenition> defenitions)
        {
            foreach (var def in defenitions)
            {
                panel.Children.Add(new ScapControl(def)
                {
                    Margin = new Thickness(5, 5, 5, 0)
                });
            }
        }
    }
}
