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
                    break;
                case ScapState.LoadDefenitions:
                    this.Invoke(() =>
                    {
                        uiCircleInit.Fill = _inComplete;
                        uiCircleLoad.Fill = _inProgress;
                    });
                    break;
                case ScapState.Eval:
                    this.Invoke(() =>
                    {
                        uiCircleLoad.Fill = _inComplete;
                        uiCircleEval.Fill = _inProgress;
                    });
                    break;
                case ScapState.GetInfo:
                    this.Invoke(() =>
                    {
                        uiCircleEval.Fill = _inComplete;
                        uiCircleGetList.Fill = _inProgress;
                        uiCircleGetList.Fill = _inComplete;
                        uiCircleGetInfo.Fill = _inProgress;
                    });
                    break;
                case ScapState.Result:
                    this.Invoke(() =>
                    {
                        uiCircleGetInfo.Fill = _inComplete;
                        uiCircleStop.Fill = _inProgress;

                        if (Processor.CriticalDefenitions.Length > 0)
                        {
                            uiExpanderCritical.Visibility = Visibility.Visible;
                            uiDataGridCritical.ItemsSource = Processor.CriticalDefenitions;
                            uiTextCritical.Text += $" [{Processor.CriticalDefenitions.Length}]";
                        }
                        if (Processor.HighDefenitions.Length > 0)
                        {
                            uiExpanderHigh.Visibility = Visibility.Visible;
                            uiDataGridHigh.ItemsSource = Processor.HighDefenitions;
                            uiTextHigh.Text += $" [{Processor.HighDefenitions.Length}]";
                        }
                        if (Processor.MediumDefenitions.Length > 0)
                        {
                            uiExpanderMedium.Visibility = Visibility.Visible;
                            uiDataGridMedium.ItemsSource = Processor.MediumDefenitions;
                            uiTextMedium.Text += $" [{Processor.MediumDefenitions.Length}]";
                        }
                        if (Processor.LowDefenitions.Length > 0)
                        {
                            uiExpanderLow.Visibility = Visibility.Visible;
                            uiDataGridLow.ItemsSource = Processor.LowDefenitions;
                            uiTextLow.Text += $" [{Processor.LowDefenitions.Length}]";
                        }
                        if (Processor.InventoryDefenitions.Length > 0)
                        {
                            uiExpanderInventory.Visibility = Visibility.Visible;
                            uiDataGridInventory.ItemsSource = Processor.InventoryDefenitions;
                            uiTextInventory.Text += $" [{Processor.InventoryDefenitions.Length}]";
                        }

                        uiGridCaption.Visibility = Visibility.Collapsed;

                        uiCircleStop.Fill = _inComplete;
                    });
                    break;
            }
        }

        public void StartTask()
        {
            Processor.StartAsync(Shell.CancellationToken);
        }

        private void uiCloseTab_Click(object sender, RoutedEventArgs e)
        {

        }

        private void uiDataGridCritical_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var destination = e.Uri.OriginalString;
            ProcessStartInfo psInfo = new ProcessStartInfo
            {
                FileName = $"https://ovaldbru.altx-soft.ru/Definition.aspx?id={destination}",
                UseShellExecute = true
            };
            Process.Start(psInfo);
            e.Handled = true;
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            textBlock.Foreground = "#fca577".GetBrush();
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            textBlock.Foreground = "#dfdfdf".GetBrush();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var destination = textBlock.Text.Replace("Больше информации и способы решения: ", "");
            ProcessStartInfo psInfo = new ProcessStartInfo
            {
                FileName = destination,
                UseShellExecute = true
            };
            Process.Start(psInfo);
        }
    }
}
