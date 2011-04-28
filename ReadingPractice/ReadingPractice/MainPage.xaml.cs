using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using System.IO;

namespace ReadingPractice
{
    [ScriptableType]
    public partial class MainPage : UserControl
    {
        public WordDictionary wordDictionary;
        public SentenceDictionary sentenceDictionary;
        public Languages language
        {
            get
            {
                return Languages.SimplifiedMandarin;
            }
        }
        private Popup _popup;
        public Popup popup
        {
            get
            {
                return _popup;
            }
            set
            {
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                }
                _popup = value;
            }
        }
        public Textbooks textbooks;
        private string server = "gkovacs.xvm.mit.edu";
        private string folder = "reading-practice";
        public string baseurl
        {
            get
            {
                return "http://" + server + "/" + folder + "/";
            }
        }

        public string username
        {
            get
            {
                return "gkovacs";
            }
        }

        public MainPage()
        {
            WebClient wc1 = new WebClient();
            wc1.OpenReadCompleted += (o1, webReadEventArgs1) =>
            {
                WordDictionary wordDict = new WordDictionarySimplifiedMandarin(webReadEventArgs1.Result);
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += (o, webReadEventArgs) =>
                {
                    sentenceDictionary = new SentenceDictionarySimplifiedMandarin(webReadEventArgs.Result, wordDict);
                    performOnStartup();
                };
                wc.OpenReadAsync(new Uri(baseurl + "listsentences.aspx?listMe=yes"));
                //wc.OpenReadAsync(new Uri("http://mit.edu/~gkovacs/www/reading-practice/sentences.txt"));
            };
            //wc1.OpenReadAsync(new Uri(baseurl + "listwords.aspx?listMe=yes"));
            wc1.OpenReadAsync(new Uri("http://mit.edu/~gkovacs/www/reading-practice/words.txt"));
        }

        public void performOnStartup()
        {
            wordDictionary = sentenceDictionary.wordDictionary;
            textbooks = new Textbooks();
            InitializeComponent();
            this.LeftSidebar.mainPage = this;
            this.RightSidebar.mainPage = this;
            this.LeftSidebar.performOnStartup();
            this.RightSidebar.performOnStartup();
            MouseButtonEventHandler closePopupIfClickedOutside = (s, e) =>
            {
                if (popup == null)
                    return;
                Point position = e.GetPosition(popup);
                if (position.X >= 0 && position.Y >= 0)
                {
                    if (position.X <= popup.ActualWidth && position.Y <= popup.ActualHeight)
                    {
                        return;
                    }
                }
                popup = null;
            };
            this.AddHandler(UIElement.MouseLeftButtonDownEvent, closePopupIfClickedOutside, true);
            PopupShield.AddHandler(UIElement.MouseLeftButtonDownEvent, closePopupIfClickedOutside, true);
            //HtmlPage.RegisterScriptableObject("mainPage", this);
            //HtmlPage.RegisterScriptableObject("sentenceDictionary", sentenceDictionary);
            //HtmlPage.RegisterScriptableObject("wordDictionary", wordDictionary);
            WebClient wc2 = new WebClient();
            wc2.OpenReadCompleted += (o2, e2) =>
            {
                StreamReader studyFocusHistory = new StreamReader(e2.Result);
                while (!studyFocusHistory.EndOfStream)
                {
                    string currentWord = studyFocusHistory.ReadLine();
                    LeftSidebar.StudyFocusForeignWord.Items.Add(currentWord);
                }
                WebClient wc3 = new WebClient();
                wc3.OpenReadCompleted += (o3, e3) =>
                {
                    StreamReader studyFocusReader = new StreamReader(e3.Result);
                    while (!studyFocusReader.EndOfStream)
                    {
                        LeftSidebar.StudyFocus = studyFocusReader.ReadLine();
                        break;
                    }
                    LeftSidebar.focusWordChanged += sendNewStudyFocus;
                    LeftSidebar.Opacity = 100.0;
                    RightSidebar.Opacity = 100.0;
                };
                wc3.OpenReadAsync(new Uri(baseurl + "getStudyFocus.cgi.py?userName=" + username));
            };
            wc2.OpenReadAsync(new Uri(baseurl + "getStudyHistory.cgi.py?userName="+username));
        }

        public void sendMessage(string message)
        {
            WebClient wc = new WebClient();
            //wc.OpenReadCompleted += (o, e) => {};
            wc.OpenReadAsync(new Uri(baseurl + message));
        }

        public void sendNewStudyFocus(string text)
        {
            sendMessage("setStudyFocus.cgi.py?userName=" + username + ";studyFocus=" + text);
        }

        /*
        public void htmlMouseDown()
        {
            System.Diagnostics.Debug.WriteLine("stuff pressed");
            popup = null;
        }
        */
        public void setStudyFocus(string text)
        {
            LeftSidebar.StudyFocus = text;
        }
    }
}
