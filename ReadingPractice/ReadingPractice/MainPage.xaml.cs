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
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

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
        private string folder = "reading-practice/secret-backend-path";
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
        private Queue<string> sendMessageQueue = new Queue<string>();
        private Queue<IEnumerable<string>> dataQueue = new Queue<IEnumerable<string>>();

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
            wc1.OpenReadAsync(new Uri(baseurl + "listwords.aspx?listMe=yes"));
            //wc1.OpenReadAsync(new Uri("http://mit.edu/~gkovacs/www/reading-practice/words.txt"));
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
            
            getDisplayedWords();
        }

        private void getDisplayedWords()
        {
            Debug.WriteLine("getting displayed words");
            WebClient wc5 = new WebClient();
            wc5.OpenReadCompleted += (o5, e5) =>
            {
                Debug.WriteLine("reading displayed words complete");
                StreamReader dispWords = new StreamReader(e5.Result);
                LeftSidebar.batchChanges = true;
                while (!dispWords.EndOfStream)
                {
                    string currentWord = dispWords.ReadLine();
                    
                    LeftSidebar.allowWord(currentWord, false);
                    //System.Diagnostics.Debug.WriteLine(currentWord);
                }
                LeftSidebar.batchChanges = false;
                //LeftSidebar.updateDisplayedWords();
                getStudyHistory();
            };
            wc5.OpenReadAsync(new Uri(baseurl + "getDisplayedWords.aspx?userName=" + username));
        }

        private void getStudyHistory()
        {
            Debug.WriteLine("getting study history");
            WebClient wc2 = new WebClient();
            wc2.OpenReadCompleted += (o2, e2) =>
            {
                Debug.WriteLine("reading study focus history complete");
                StreamReader studyFocusHistory = new StreamReader(e2.Result);
                while (!studyFocusHistory.EndOfStream)
                {
                    string currentWord = studyFocusHistory.ReadLine();
                    LeftSidebar.StudyFocusForeignWord.Items.Add(currentWord);
                }
                WebClient wc3 = new WebClient();
                wc3.OpenReadCompleted += (o3, e3) =>
                {
                    Debug.WriteLine("reading study focus complete");
                    StreamReader studyFocusReader = new StreamReader(e3.Result);
                    while (!studyFocusReader.EndOfStream)
                    {
                        LeftSidebar.StudyFocus = studyFocusReader.ReadLine();
                        break;
                    }
                    LeftSidebar.focusWordChanged += sendNewStudyFocus;
                    finishedDownloading();
                };
                wc3.OpenReadAsync(new Uri(baseurl + "getStudyFocus.cgi.py?userName=" + username));
            };
            wc2.OpenReadAsync(new Uri(baseurl + "getStudyHistory.cgi.py?userName=" + username));
        }

        public void finishedDownloading()
        {
            Debug.WriteLine("finished downloading");
            LeftSidebar.Opacity = 100.0;
            RightSidebar.Opacity = 100.0;
            new Thread(() =>
            {
                while (true)
                {
                    if (sendMessageQueue.Count > 0)
                    {
                        string curval = null;
                        IEnumerable<string> data = null;
                        lock (sendMessageQueue)
                        {
                            curval = sendMessageQueue.Dequeue();
                            if (curval[0] == '@')
                                data = dataQueue.Dequeue();
                        }
                        if (data == null)
                        {
                            sendMessageActual(curval);
                        }
                        else
                        {
                            sendMessageWithDataActual(curval.Substring(1), data);
                        }

                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }).Start();
        }

        public void sendMessageActual(string message)
        {
            Debug.WriteLine("sending message " + message);
            bool lockVar = false;
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (o, e) => { lockVar = true; };
            wc.OpenReadAsync(new Uri(baseurl + message));
            while (!lockVar)
            {
                Thread.Sleep(50);
            }
            Debug.WriteLine("done sending message " + message);
        }

        public void sendMessageWithDataActual(string message, IEnumerable<string> data)
        {
            Debug.WriteLine("sending message with data " + message);
            bool lockVar = false;
            WebClient wc = new WebClient();
            wc.OpenWriteCompleted += (o, e) =>
            {
                using (StreamWriter st = new StreamWriter(e.Result))
                {
                    foreach (string x in data)
                    {
                        st.WriteLine(x);
                    }
                }
                lockVar = true;
            };
            wc.OpenWriteAsync(new Uri(baseurl + message));
            while (!lockVar)
            {
                Thread.Sleep(200);
            }
            Debug.WriteLine("done sending message with data " + message);
        }

        public void sendMessage(string message)
        {
            lock (sendMessageQueue)
            {
                sendMessageQueue.Enqueue(message);
            }
        }

        public void sendMessageWithData(string message, IEnumerable<string> data)
        {
            lock (sendMessageQueue)
            {
                sendMessageQueue.Enqueue("@" + message);
                dataQueue.Enqueue(data);
            }
        }

        public void sendNewStudyFocus(string text)
        {
            sendMessage("setStudyFocus.cgi.py?userName=" + username + "&studyFocus=" + text);
        }

        public void sendAllowWord(string text)
        {
            sendMessage("addDisplayedWord.cgi.py?userName=" + username + "&displayedWord=" + text);
        }

        public void sendAllowAllWord()
        {
            sendMessage("addAllDisplayedWord.aspx?userName=" + username);
        }

        public void sendBanWord(string text)
        {
            sendMessage("rmDisplayedWord.cgi.py?userName=" + username + "&displayedWord=" + text);
        }

        public void sendBanAllWord()
        {
            sendMessage("rmAllDisplayedWord.aspx?userName=" + username);
        }

        public void sendAllowWordGroup(IEnumerable<string> words)
        {
            sendMessageWithData("addDisplayedWordMany.cgi.py?userName=" + username, words);
            //sendMessage("postpage.cgi.py?userName=" + username + "&page=addDisplayedWordMany.cgi.py&postdata=" + String.Join("%0A", words));
        }

        public void sendBanWordGroup(IEnumerable<string> words)
        {
            sendMessageWithData("rmDisplayedWordMany.cgi.py?userName=" + username, words);
            //sendMessage("postpage.cgi.py?userName=" + username + "&page=rmDisplayedWordMany.cgi.py&postdata=" + String.Join("%0A", words));
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
