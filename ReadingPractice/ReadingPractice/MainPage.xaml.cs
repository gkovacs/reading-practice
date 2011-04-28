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
        private string baseurl
        {
            get
            {
                return "http://" + server + "/" + folder + "/";
            }
        }

        public MainPage()
        {
            WebClient wc1 = new WebClient();
            wc1.OpenReadCompleted += (o1, webReadEventArgs1) =>
            {
                WordDictionarySimplifiedMandarin wordDict = new WordDictionarySimplifiedMandarin(webReadEventArgs1.Result);
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += (o, webReadEventArgs) =>
                {
                    sentenceDictionary = new SentenceDictionarySimplifiedMandarin(webReadEventArgs.Result, wordDict);
                    performOnStartup();
                };
                wc.OpenReadAsync(new Uri(baseurl + "listsentences.aspx?listMe=yes"));
            };
            wc1.OpenReadAsync(new Uri(baseurl + "listwords.aspx?listMe=yes"));
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
            HtmlPage.RegisterScriptableObject("mainPage", this);
            HtmlPage.RegisterScriptableObject("sentenceDictionary", sentenceDictionary);
            HtmlPage.RegisterScriptableObject("wordDictionary", wordDictionary);
        }

        public void htmlMouseDown()
        {
            System.Diagnostics.Debug.WriteLine("stuff pressed");
            popup = null;
        }

        public void setStudyFocus(string text)
        {
            LeftSidebar.StudyFocus = text;
        }
    }
}
