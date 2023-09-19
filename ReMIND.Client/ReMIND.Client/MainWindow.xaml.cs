using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shell;

using Microsoft.AspNetCore.SignalR.Client;
using ReMIND.Client.AnalyticsTab;
using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;
using ReMIND.Client.TitleBar;

namespace ReMIND.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Person User { get; set; }

        public MainWindow(Person user)
        {
            User = user;
            InitializeComponent();

            SettingsPopupBorder.mainWindow = this;
            SettingsPopupBorder.Refresh();
            Utility.mainWindow = this;
            InitializeSignalR();

            Thickness resizeBorderThickness = WindowChrome.GetWindowChrome(this).ResizeBorderThickness;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - resizeBorderThickness.Top - resizeBorderThickness.Bottom;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - resizeBorderThickness.Left - resizeBorderThickness.Right;
        }

        #region Load & Close
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Maximized)
                WindowState = WindowState.Maximized;
            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;

            TabGroupControl.mainWindow = this;
            TabGroupControl.ActivateIndex(0);

            setUpTabs();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Left = Left;
            Properties.Settings.Default.Top = Top;
            Properties.Settings.Default.Height = Height;
            Properties.Settings.Default.Width = Width;
            Properties.Settings.Default.Maximized = WindowState == WindowState.Maximized;
        }
        #endregion

        #region Tabs
        public void setUpTabs()
        {
            //za sada proveravam da li je user bas onaj jedan admin kada se napravi kontroler za testiranje title cu menjam
            if (Utility.UserTittle == TitleType.Leader)
            {
                TabGroupControl.setupLeader();

            }
            else if (Utility.UserTittle == TitleType.Admin)
            {
                TabGroupControl.setupAdmin();
            }
            //TabGroupControl.setupTabs();
        }
        private void TabGroup_OnTabChanged(object sender, UIElement e)
        {
            TabPageViewbox.Child = e;
        }
        #endregion

        #region Notifications & Popup
        public void RefreshNotifications()
        {
            SettingsPopupBorder.Refresh();
        }
        public void showNotificationIndicator()
        {
            PopupButton.Foreground = new BrushConverter().ConvertFromString("#D0480E") as SolidColorBrush;
            PopupButton.Content = "!";
        }
        public void hideNotificationIndicator()
        {
            PopupButton.Foreground = new BrushConverter().ConvertFromString("#0000") as SolidColorBrush;
            PopupButton.Content = "";
        }
        private void Popup_OnPopupOutsideClicked(object sender, bool e)
        {
            DoubleAnimation closePopup = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(80));
            PopupScale.BeginAnimation(ScaleTransform.ScaleXProperty, closePopup);
            PopupScale.BeginAnimation(ScaleTransform.ScaleYProperty, closePopup);
            PopupButton.IsChecked = false;
        }
        #endregion

        #region SignalR
        HubConnection connection;
        private async void InitializeSignalR()
        {
            try
            {
                connection = new HubConnectionBuilder()
                            .WithUrl("ws://localhost:5000/notificationHub", options => options.Headers.Add("SessionID", Utility.User.SessionID))
                            .Build();

                #region snippet_ClosedRestart
                connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connection.StartAsync();
                };
                #endregion

                #region snippet_ConnectionOn
                connection.On("ReloadJobs", () =>
                {

                    this.Dispatcher.Invoke(async () =>
                    {

                        await Task.Delay(1000);
                        Utility.ReloadTasksData();
                    });
                });
                connection.On("ReloadNotifications", () =>
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        this.RefreshNotifications();
                    });
                });

                #endregion

                try
                {
                    await connection.StartAsync();
                    //lstListBox.Items.Add("Connection started");
                    //connectButton.IsEnabled = false;
                    //btnSend.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    //lstListBox.Items.Add(ex.Message);
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
        #region Titlebar Buttons
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        double oldWidth, oldHeight, oldLeft, oldTop;
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            //WindowState = WindowState == WindowState.Maximized ?
            //    WindowState.Normal : WindowState.Maximized;

            HandleMaximize();
        }
        private void HandleMaximize()
        {
            if (Width==SystemParameters.WorkArea.Width)
            {
                Width = oldWidth;
                Height = oldHeight;
                Left = oldLeft;
                Top = oldTop;
            }
            else
            {
                oldWidth = Width;
                oldHeight = Height;
                oldLeft = Left;
                oldTop = Top;
                Width = SystemParameters.WorkArea.Width;
                Height = SystemParameters.WorkArea.Height - 1;
                Left = 0;
                Top = 0;
            }
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if(e.ClickCount == 2)
                {
                    HandleMaximize();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        #endregion

        #region Blur
        //dodao sam blurBuffer zato sto:
        //otvori se jedan prozor koji bluruje, otvori se drugi.
        //oba pozovu blur, ostaje mainWindow blurred.
        //medjutim, ako se jedan od njih zatvori a drugi ostane,
        //Unblur() ce svakako da se pozove,
        //ostavljajuci jedan prozor bez blura na mainWindow.
        //ovime pravimo buffer koji iskljucuje blur tek kad nema vise prozora koji traze blur
        int blurBuffer = 0;
        public void Blur()
        {
            blurBuffer++;
            WindowBlur.Radius = 8;
            BlurBorder.Visibility = Visibility.Visible;
        }
        public void Unblur()
        {
            if(--blurBuffer == 0)
            {
                WindowBlur.Radius = 0;
                BlurBorder.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

    }
}
