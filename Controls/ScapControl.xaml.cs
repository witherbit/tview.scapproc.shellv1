using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using tview.scapproc.shellv1.Enums;
using tview.scapproc.shellv1.Objects;
using wcheck.wcontrols;

namespace tview.scapproc.shellv1.Controls
{
    /// <summary>
    /// Логика взаимодействия для ScapControl.xaml
    /// </summary>
    ///
    public partial class ScapControl : UserControl
    {
        static Brush _critBg => "#6f3838".GetBrush();
        static Brush _hiBg => "#ff8888".GetBrush();
        static Brush _midBg => "#ffc887".GetBrush();
        static Brush _loBg => "#a3ff87".GetBrush();
        static Brush _invBg => "#87c0ff".GetBrush();
        public Defenition Defenition { get; }
        public ScapControl(Defenition defenition)
        {
            InitializeComponent();
            Defenition = defenition;
            uiTextId.Text = defenition.Id;
            uiTextTitle.Text = defenition.Title;
            uiTextDescription.Text = defenition.Description;
            switch (defenition.Risk)
            {
                case "Критический":
                    uiBorderId.Background = _critBg;
                    SetHyperlink();
                    break;
                case "Высокий":
                    uiBorderId.Background = _hiBg;
                    SetHyperlink();
                    break;
                case "Средний":
                    uiBorderId.Background = _midBg;
                    SetHyperlink();
                    break;
                case "Низкий":
                    uiBorderId.Background = _loBg;
                    SetHyperlink();
                    break;
                case "Информация":
                    uiBorderId.Background = _invBg;
                    uiBorderLink.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void SetHyperlink()
        {
            uiTextLink.ToolTip = (uiTextLink.ToolTip as string) + "\r\nURL: " + Defenition.Hyperlink;
        }

        private void uiTextLink_MouseEnter(object sender, MouseEventArgs e)
        {
            uiTextLink.Foreground = "#7000ff".GetBrush();
        }

        private void uiTextLink_MouseLeave(object sender, MouseEventArgs e)
        {
            uiTextLink.Foreground = "#dfdfdf".GetBrush();
        }

        private void uiTextLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ProcessStartInfo psInfo = new ProcessStartInfo
            {
                FileName = Defenition.Hyperlink,
                UseShellExecute = true
            };
            Process.Start(psInfo);
            e.Handled = true;
        }

        private void uiTextId_MouseEnter(object sender, MouseEventArgs e)
        {
            uiTextId.Foreground = "#7000ff".GetBrush();
        }

        private void uiTextId_MouseLeave(object sender, MouseEventArgs e)
        {
            uiTextId.Foreground = "#1f1f1f".GetBrush();
        }

        private void uiTextId_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ProcessStartInfo psInfo = new ProcessStartInfo
            {
                FileName = $"https://ovaldbru.altx-soft.ru/Definition.aspx?id={Defenition.Id}",
                UseShellExecute = true
            };
            Process.Start(psInfo);
            e.Handled = true;
        }
    }
}
