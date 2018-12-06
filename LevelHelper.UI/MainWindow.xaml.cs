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
using LevelHelper.Core.Util;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LevelHelper.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _SETTINGS__FILE = Directory.GetCurrentDirectory() + @"\settings.json";
        private Core.LevelHelper helper;
        private string _accountName;
        private string _pth;
        private string _chName;
        private string _chClass;
        private string _level;
        private string _zone;
        private List<string> _currentLevelMessagesStringList = new List<string>();
        private List<string> _currentZoneMessagesStringList = new List<string>();
        private string _currentFlaskMessageString;
        private List<string> _currentQuestMessagesStringList;
        
        private LevelMessage _flaskMessage;
        private List<LevelMessage> _currentLevelMessages = new List<LevelMessage>();
        private List<AreaMessage> _currentZoneMessages = new List<AreaMessage>();
        private List<AreaMessage> _currentQuestMessages = new List<AreaMessage>();

        bool _startClicked = false;
        bool _locked = true;

        public MainWindow()
        {
            InitializeComponent();
        }



        private void LoadSettings()
        {
            if (!File.Exists(_SETTINGS__FILE))
                SaveSettings();
            else
            {
                using (StreamReader sr = File.OpenText(_SETTINGS__FILE))
                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    JObject jo = (JObject)JToken.ReadFrom(reader);
                    _pth = jo.GetValue("install_path").ToString();
                    path.Text = _pth;
                    _accountName = jo.GetValue("account_name").ToString();
                    account.Text = _accountName;
                }
            }

        }

        private void SaveSettings()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("install_path");
                writer.WriteValue(path.Text);
                writer.WritePropertyName("account_name");
                writer.WriteValue(account.Text);
//                writer.WriteEnd();
                writer.WriteEndObject();
            }
            File.WriteAllText(_SETTINGS__FILE, sw.ToString());
        }

        private void OnStartClick(object sender, RoutedEventArgs args)
        {
            Start();
        }

        private void Start()
        {
            _accountName = account.Text;
            _pth = path.Text;
            LoadSettings();
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
                SaveSettings();
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

            helper.GetLevelMessages(_level, out _flaskMessage, out _currentLevelMessages);
//            helper.GetLevelMessages2(_level, out _currentFlaskMessage, out _currentLevelMessages);
            SetLevelContent();
        }

        private void OnHelperZoneChange(object sender, InterpretEventArgs args)
        {
            _zone = args.Zone;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.lbl_zone.Content = _zone;
            }));
            helper.GetZoneMessages(_zone, out _currentQuestMessages, out _currentZoneMessages);
            //            helper.GetZoneMessages2(_zone, out _currentQuestMessagesStringList, out _currentZoneMessagesStringList);
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
                    /*
                    foreach (string questMsg in _currentQuestMessagesStringList)
                    {
                        this.pnl_zone.Children.Add(new TextBlock()
                        {
                            Text = questMsg,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0),
                            TextWrapping = TextWrapping.Wrap,
                        });
                    }*/
                    foreach( IMessage msg in _currentQuestMessages)
                    {
                        this.pnl_zone.Children.Add(new TextBlock()
                        {
                            Text = msg.ToString(false),
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0),
                            TextWrapping = TextWrapping.Wrap,
                        });
                    }
                }
                if (_currentZoneMessages != null)
                {
                    //foreach (string chrAreaMsg in _currentZoneMessagesStringList)
                    //{
                    //    this.pnl_zone.Children.Add(new TextBlock()
                    //    {
                    //        Text = chrAreaMsg,
                    //        VerticalAlignment = VerticalAlignment.Top,
                    //        Margin = new Thickness(0),
                    //        TextWrapping = TextWrapping.Wrap,
                    //    });
                    //}
                    foreach (IMessage msg in _currentZoneMessages)
                    {
                        this.pnl_zone.Children.Add(new TextBlock()
                        {
                            Text = msg.ToString(),
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
                this.lbl_flask.Content = _flaskMessage.ToString();
                int counter = 0;
                this.pnl_lvl.Children.Clear();
                foreach (IMessage msg in _currentLevelMessages)
                {
                    this.pnl_lvl.Children.Add(new TextBlock()
                    {
                        //Text = string.Join(Environment.NewLine, msg.GetMessages()) + " ( " + (float)Mathfx.Sinerp(.5, 1, Math.Min(1, Double.Parse(_level) / Double.Parse(msg.Identifier()))) + " ) ",
                        Text = msg.ToString(true),
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0),
                        TextWrapping = TextWrapping.Wrap,

                        // TODO: Make better function for the gradient
                        Foreground = new SolidColorBrush(new Color()
                        {
                            ScR = 0,
                            ScG = 0,
                            ScB = 0,
                            ScA = (float)Mathfx.Sinerp(.5, 1, Math.Min(1,Double.Parse(msg.Identifier()) / Double.Parse(_level)))
                        }),
                        
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
            helper.GetLevelMessages(_level, out _flaskMessage, out _currentLevelMessages);
//            helper.GetLevelMessages2(_level, out _currentFlaskMessageString, out _currentLevelMessagesStringList);
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
                LockInput();
                Start();
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }
    }
}
