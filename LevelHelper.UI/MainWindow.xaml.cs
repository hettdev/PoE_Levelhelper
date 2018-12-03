using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LevelHelper.Core;
using LevelHelper.Core.Interpreter;
using LevelHelper.Core.Messages;
using System.IO;

namespace LevelHelper.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core.LevelHelper helper;
        private string _accountName;
        private string _pth;
        private string _chName;
        private string _chClass;
        private string _level;
        private string _zone;
        private List<string> _currentLevelMessages = new List<string>();
        private List<string> _currentZoneMessages = new List<string>();
        private string _currentFlaskMessage;
        private List<string> _currentQuestMessages;

        bool _startClicked = false;
        bool _locked = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnStartClick(object sender, RoutedEventArgs args)
        {
            Start();
        }

        private void Start()
        {
            _accountName = account.Text;
            _pth = path.Text;
            Console.WriteLine("{0} {1}", _accountName, _pth);
            this.btn_start.Content = "Started...";
            _startClicked = true;
            InitializeHelper();
        }

        private void OnInputLocked(object sender, RoutedEventArgs args)
        {
            LockInput();
        }

        private void LockInput()
        {
            if (!_locked)
            {
                account.IsEnabled = false;
                path.IsEnabled = false;
                this.btn_lock.Content = "Unlock Input";

            }
            else
            {
                account.IsEnabled = true;
                path.IsEnabled = true;
                this.btn_lock.Content = "Lock Input";
            }
            _locked = !_locked;
        }

        private void InitializeHelper()
        {
            helper = new Core.LevelHelper(_accountName, _pth);
            helper.InitializedEvent += OnHelperInitialized;
            helper.GameRunningEvent += OnHelperGameRunning;
            helper.WebCrawlEvent    += OnHelperWebCrawl;
            helper.LevelUpEvent     += OnHelperLevelUp;
            helper.ZoneChangeEvent  += OnHelperZoneChange;
            helper.Initialize();
        }

        private void OnHelperLevelUp(object sender, InterpretEventArgs args)
        {
            _level = args.Level;

//            helper.GetLevelMessages(_level, out flaskMessage, out charLvlMessage);
            helper.GetLevelMessages2(_level, out _currentFlaskMessage, out _currentLevelMessages);
            SetLevelContent();
        }

        private void OnHelperZoneChange(object sender, InterpretEventArgs args)
        {
            _zone = args.Zone;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.lbl_zone.Content = _zone;
            }));
            helper.GetZoneMessages2(_zone, out _currentQuestMessages, out _currentZoneMessages);
            SetZoneContent();
        }

        private void SetZoneContent()
        {

            // TODO: wire up zone changes
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.pnl_zone.Children.Clear();
                if (_currentQuestMessages != null)
                {
                    foreach (string questMsg in _currentQuestMessages)
                    {
                        this.pnl_zone.Children.Add(new TextBlock()
                        {
                            Text = questMsg,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0),
                            TextWrapping = TextWrapping.Wrap,
                        });
                    }
                }
                if (_currentZoneMessages != null)
                {
                    foreach (string chrAreaMsg in _currentZoneMessages)
                    {
                        this.pnl_zone.Children.Add(new TextBlock()
                        {
                            Text = chrAreaMsg,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0),
                            TextWrapping = TextWrapping.Wrap,
                        });
                    }
                }
            }));
        }

        private void SetLevelContent()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.lbl_level.Content = "Level " + _level;
                this.lbl_flask.Content = _currentFlaskMessage;
                int counter = 0;
                this.pnl_lvl.Children.Clear();
                foreach (string chrLvlMsg in _currentLevelMessages)
                {
                    this.pnl_lvl.Children.Add(new TextBlock()
                    {
                        Text = chrLvlMsg,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0),
                        TextWrapping = TextWrapping.Wrap,
                        // TODO: Make better function for the gradient
                        Foreground = new SolidColorBrush(new Color()
                        {
                            ScR = 0,
                            ScG = 0,
                            ScB = 0,
                            ScA = Math.Max(0.5f, ((float)(_currentLevelMessages.Count - counter) / ((float)_currentLevelMessages.Count)))
                        })
                    });
                    ++counter;
                }
            }));
        }

        private void SetClassImage()
        {
            
            this.Dispatcher.Invoke(new Action(() =>
            {
                string uriString = Directory.GetCurrentDirectory() + @"\Resources\ClassImages\" + _chClass + ".png";
                Console.WriteLine(uriString);
                BitmapImage classBmp = new BitmapImage(new Uri(uriString));
                double bmpRatio = classBmp.Width / classBmp.Height;
                if (this.img_class.Width > this.img_class.Height)
                {
                    this.img_class.Height = this.img_class.Width/bmpRatio;
                }
                else
                {
                    this.img_class.Width = this.img_class.Height * bmpRatio;
                }
                this.img_class.Source = classBmp;
            }));
        }

        private void OnHelperWebCrawl(object sender, InterpretEventArgs args)
        {
            _chName = args.CharName;
            _chClass = args.CharClass;
            _level = args.Level;

            // this crashes, find out how to set labelContent
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.lbl_name.Content = _chName;
                this.lbl_class.Content = _chClass;
                this.lbl_level.Content = "Level " + _level;
            }));
            helper.ReadMesageFiles();
            helper.StartLevelingMonitoring();
//            helper.GetLevelMessages(_level, out flaskMessage, out charLvlMessage);
            helper.GetLevelMessages2(_level, out _currentFlaskMessage, out _currentLevelMessages);
            SetLevelContent();
            SetClassImage();
        }

        private void OnHelperGameRunning(object sender, InterpretEventArgs args)
        {
            if (args.GameRunning)
            {
                Console.WriteLine("OnHelperGameRunning");
                helper.StartWebCrawling();
            }
        }

        private void OnHelperInitialized(object sender, InterpretEventArgs args)
        {
        }

        private void OnPathTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_startClicked)
            {

            }
        }

        private void Account_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                Start();
                LockInput();
            }
        }
    }
}
