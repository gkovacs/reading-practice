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

        private LoginScreen loginScreen;
        private WaitingScreen waitingScreen;
        private bool isLoggedIn = false;
        private ServerCommunication _serverCommunication;
        public ServerCommunication serverCommunication
        {
            get
            {
                return _serverCommunication;
            }
        }
        public string baseurl
        {
            get
            {
                return _serverCommunication.baseurl;
            }
        }
        public string username
        {
            get
            {
                return _serverCommunication.username;
            }
        }

        public void closeLoginScreen()
        {
            if (this.mainPageContents.Children.Contains(loginScreen))
                this.mainPageContents.Children.Remove(loginScreen);
            if (!this.mainPageContents.Children.Contains(waitingScreen))
                this.mainPageContents.Children.Add(waitingScreen);
        }

        public MainPage()
        {
            InitializeComponent();
            _serverCommunication = new ServerCommunication();
            loginScreen = new LoginScreen(this);
            waitingScreen = new WaitingScreen();
            loginScreen.userLoggedIn += () =>
            {
                this.RightSidebar.userLoggedIn(username);
                closeLoginScreen();
                this.isLoggedIn = true;
            };
            this.mainPageContents.Children.Add(loginScreen);
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
            wc1.OpenReadAsync(new Uri(baseurl + "words.txt"));
            //wc1.OpenReadAsync(new Uri(baseurl + "listwords.aspx?listMe=yes"));
            //wc1.OpenReadAsync(new Uri("http://mit.edu/~gkovacs/www/reading-practice/words.txt"));
        }

        public void performOnStartup()
        {
            wordDictionary = sentenceDictionary.wordDictionary;
            textbooks = new Textbooks();
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

            App.Current.Host.Content.Resized += new EventHandler(Content_Resized);

            //getDisplayedWords();
            new Thread(() =>
            {
                while (!isLoggedIn)
                {
                    Thread.Sleep(10);
                }
                this.Dispatcher.BeginInvoke(() =>
                {
                    getDisplayedWords();
                });
            }).Start();
        }

        private void Content_Resized (object sender, EventArgs e)
        {
            double height = App.Current.Host.Content.ActualHeight;
            double width = App.Current.Host.Content.ActualWidth;

            Debug.WriteLine("MainPage.xaml.cs, Content_Resized():");
            Debug.WriteLine(height);
            Debug.WriteLine(width);
            this.LeftSidebar.Resize(height,width);
            this.RightSidebar.Resize(height,width);
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
                        tmpstudyfocus = studyFocusReader.ReadLine();
                        LeftSidebar.setStudyFocusNoChanges(tmpstudyfocus);
                        //LeftSidebar.StudyFocus = studyFocusReader.ReadLine();
                        break;
                    }
                    //LeftSidebar.focusWordChanged += serverCommunication.sendNewStudyFocus;
                    populateSentenceLists();
                };
                wc3.OpenReadAsync(new Uri(baseurl + "getStudyFocus.cgi.py?userName=" + username));
            };
            wc2.OpenReadAsync(new Uri(baseurl + "getStudyHistory.cgi.py?userName=" + username));
        }

        private string tmpstudyfocus = "";

        private void populateSentenceLists()
        {
            serverCommunication.getSentences(sents =>
            {
                foreach (string x in sents)
                {
                    RightSidebar.readSentencesTab.insertSentence(x);
                }
                populateClosedSentenceLists();
            });
        }

        private void populateClosedSentenceLists()
        {
            serverCommunication.getClosedSentences(sents =>
            {
                foreach (string x in sents)
                {
                    RightSidebar.closedSentencesTab.insertSentence(x);
                }
                populateContribSentenceLists();
            });
        }

        private void populateContribSentenceLists()
        {
            serverCommunication.getContribSentences(sents =>
            {
                foreach (string x in sents)
                {
                    RightSidebar.contributeSentencesTab.insertSentence(x);
                }
                finishedDownloading();
            });
        }

        private void finishedDownloading()
        {
            LeftSidebar.StudyFocus = tmpstudyfocus;
            LeftSidebar.focusWordChanged += serverCommunication.sendNewStudyFocus;
            this.mainPageContents.Children.Remove(waitingScreen);
            LeftSidebar.Visibility = Visibility.Visible;
            RightSidebar.Visibility = Visibility.Visible;
        }
    }
}
