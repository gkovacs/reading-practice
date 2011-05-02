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

namespace ReadingPractice
{
    public partial class ReadSentencesTab : UserControl
    {
        public MainPage mainPage;

        private LinkedList<string> sentencesToBeAdded = new LinkedList<string>();

        public string StudyFocus
        {
            get
            {
                return mainPage.LeftSidebar.StudyFocus;
            }
            set
            {
                mainPage.LeftSidebar.StudyFocus = value;
            }
        }

        public Languages language
        {
            get
            {
                return mainPage.language;
            }
        }
        public WordDictionary wordDictionary
        {
            get
            {
                return mainPage.wordDictionary;
            }
        }

        public SentenceDictionary sentenceDictionary
        {
            get
            {
                return mainPage.sentenceDictionary;
            }
        }
        public ServerCommunication serverCommunication
        {
            get
            {
                return mainPage.serverCommunication;
            }
        }

        public ReadSentencesTab()
        {
            InitializeComponent();
        }

        public void performOnStartup()
        {
            noMoreSentencesAvailable();
            Action<string> newSentences = (focusWord) =>
            {
                this.sentencesToBeAdded = new LinkedList<string>(
                    sentenceDictionary.getSentences(StudyFocus, mainPage.LeftSidebar.isDisplayed)
                    .Where(sent => !isAlreadyPresent(sent))
                );
                if (this.sentencesToBeAdded.Count == 0)
                {
                    noMoreSentencesAvailable();
                }
                else
                {
                    haveSentencesAvailable();
                }
            };
            mainPage.LeftSidebar.displayedListChanged += () =>
            {
                newSentences(StudyFocus);
            };
            mainPage.LeftSidebar.focusWordChanged += newSentences;
            newSentences(StudyFocus);
        }

        private void noMoreSentencesAvailable()
        {
            this.WarningsCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            this.Warnings.FontWeight = FontWeights.Bold;
            this.sentencesToBeAdded = new LinkedList<string>(this.sentenceDictionary.getSentences(StudyFocus, q => true).Where(sent => !isAlreadyPresent(sent)).OrderBy(q => sentenceDictionary.getWords(q).Select(v => wordDictionary.translateToEnglish(v) != "" && !mainPage.LeftSidebar.isDisplayed(q) ? 1 : 0).Sum()));
            if (StudyFocus == "")
            {
                if (sentencesToBeAdded.Count > 0)
                {
                    this.Warnings.Text = "Please select words that can be displayed in sentences";
                    this.FetchNextSentenceButton.IsEnabled = true;
                    return;
                }
                else
                {
                    noMoreSentencesAvailableTryClosedSentences();
                    return;
                }

            }
            if (sentencesToBeAdded.Count == 0)
            {
                noMoreSentencesAvailableTryClosedSentences();
            }
            else
            {
                this.Warnings.Text = "Remaining sentences containing " + StudyFocus + " contain words you may not know";
                this.FetchNextSentenceButton.IsEnabled = true;
            }
        }

        private void noMoreSentencesAvailableTryClosedSentences()
        {
            if (StudyFocus == "")
            {
                sentencesToBeAdded = new LinkedList<string>(mainPage.RightSidebar.closedSentencesTab.allClosedSentences.OrderBy(q => sentenceDictionary.getWords(q).Select(v => wordDictionary.translateToEnglish(v) != "" && !mainPage.LeftSidebar.isDisplayed(q) ? 1 : 0).Sum()));
                if (sentencesToBeAdded.Count > 0)
                {
                    this.Warnings.Text = "Remaining sentences have previously been seen";
                    this.FetchNextSentenceButton.IsEnabled = true;
                }
                else
                {
                    this.Warnings.Text = "No more sentences are available";
                    this.FetchNextSentenceButton.IsEnabled = false;
                }
            }
            else
            {
                sentencesToBeAdded = new LinkedList<string>(mainPage.RightSidebar.closedSentencesTab.allClosedSentences.Where(sent => sentenceDictionary.getWords(sent).Contains(StudyFocus)).OrderBy(q => sentenceDictionary.getWords(q).Select(v => wordDictionary.translateToEnglish(v) != "" && !mainPage.LeftSidebar.isDisplayed(q) ? 1 : 0).Sum()));
                if (sentencesToBeAdded.Count > 0)
                {
                    this.Warnings.Text = "Remaining sentences containing " + StudyFocus + " have previously been seen";
                    this.FetchNextSentenceButton.IsEnabled = true;
                }
                else
                {
                    this.Warnings.Text = "No more sentences are available containing " + StudyFocus;
                    this.FetchNextSentenceButton.IsEnabled = false;
                }
            }
        }

        private void haveSentencesAvailable()
        {
            this.FetchNextSentenceButton.IsEnabled = true;
            this.WarningsCanvas.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            this.Warnings.Text = "";
        }

        private bool isAlreadyPresent(string sentence)
        {
            if (mainPage.RightSidebar.closedSentencesTab.presentWords.ContainsKey(sentence))
                return true;
            if (presentWords.ContainsKey(sentence))
                return true;
            return false;
        }

        public Dictionary<string, SentenceView> presentWords = new Dictionary<string, SentenceView>();

        public void rmSentence(string nativeSentence)
        {
            if (!presentWords.ContainsKey(nativeSentence))
                return;
            SentenceView sentview = presentWords[nativeSentence];
            this.SentenceListViewer.Children.Remove(sentview);
            serverCommunication.sendRmSentence(nativeSentence);
            colorBackgrounds();
        }

        public void insertSentence(string nativeSentence)
        {
            if (presentWords.ContainsKey(nativeSentence))
            {
                rmSentence(nativeSentence);
            }
            string tranlatedSentence = sentenceDictionary.translateToEnglish(nativeSentence);
            SentenceView sentview = new SentenceView(nativeSentence, tranlatedSentence, mainPage);
            presentWords[nativeSentence] = sentview;
            sentview.removeButton.Content = "Close";
            sentview.removeButton.Click += (o, e) =>
            {
                rmSentence(nativeSentence);
                mainPage.RightSidebar.closedSentencesTab.insertSentence(nativeSentence);
            };
            SentenceListViewer.Children.Insert(2, sentview);
            colorBackgrounds();
        }

        private void colorBackgrounds()
        {
            int i = 0;
            foreach (var x in SentenceListViewer.Children.Skip(2))
            {
                SentenceView sentview = x as SentenceView;
                if (i % 2 == 1)
                    sentview.rootStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 224, 251, 255));
                else
                    sentview.rootStackPanel.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                i += 1;
            }
        }

        private void FetchNextSentenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sentencesToBeAdded.Count == 0)
                return;
            string sent = sentencesToBeAdded.First();
            sentencesToBeAdded.RemoveFirst();
            mainPage.RightSidebar.closedSentencesTab.rmSentence(sent);
            insertSentence(sent);
            serverCommunication.sendAddSentence(sent);
            if (sentencesToBeAdded.Count == 0)
            {
                noMoreSentencesAvailable();
            }
        }

    }
}
