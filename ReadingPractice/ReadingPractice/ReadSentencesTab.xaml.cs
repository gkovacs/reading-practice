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
            this.Warnings.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            this.Warnings.FontWeight = FontWeights.Bold;
            this.sentencesToBeAdded = new LinkedList<string>(this.sentenceDictionary.getSentences(StudyFocus, q => true).Where(sent => !isAlreadyPresent(sent)).OrderBy(q => sentenceDictionary.getWords(q).Select(v => wordDictionary.translateToEnglish(v) != "" && !mainPage.LeftSidebar.isDisplayed(q) ? 1 : 0).Sum()));
            if (StudyFocus == "")
            {
                if (sentencesToBeAdded.Count > 0)
                {
                    this.Warnings.Content = "Please select words that can be displayed in sentences";
                    this.FetchNextSentenceButton.IsEnabled = true;
                    return;
                }
                else
                {
                    this.Warnings.Content = "No more sentences are available";
                    this.FetchNextSentenceButton.IsEnabled = false;
                    return;
                }

            }
            if (sentencesToBeAdded.Count == 0)
            {
                this.Warnings.Content = "No more sentences are available containing " + StudyFocus;
                this.FetchNextSentenceButton.IsEnabled = false;
            }
            else
            {
                this.Warnings.Content = "Remaining sentences containing " + StudyFocus + " contain words you may not know";
                this.FetchNextSentenceButton.IsEnabled = true;
            }
        }

        private void haveSentencesAvailable()
        {
            this.FetchNextSentenceButton.IsEnabled = true;
            this.Warnings.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            this.Warnings.Content = "";
        }

        private bool isAlreadyPresent(string sentence)
        {
            foreach (var x in SentenceListViewer.Children.Skip(2))
            {
                SentenceView sen = (SentenceView)x;
                if (sentence == sen.nativeSentence)
                    return true;
            }
            return false;
        }

        public void insertSentence(string nativeSentence)
        {
            string tranlatedSentence = sentenceDictionary.translateToEnglish(nativeSentence);
            SentenceView sentview = new SentenceView(nativeSentence, tranlatedSentence, mainPage);
            sentview.removeButton.Click += (o, e) =>
            {
                this.SentenceListViewer.Children.Remove(sentview);
                serverCommunication.sendRmSentence(nativeSentence);
            };
            SentenceListViewer.Children.Insert(2, sentview);
        }

        private void FetchNextSentenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sentencesToBeAdded.Count == 0)
                return;
            string sent = sentencesToBeAdded.First();
            sentencesToBeAdded.RemoveFirst();
            insertSentence(sent);
            serverCommunication.sendAddSentence(sent);
            if (sentencesToBeAdded.Count == 0)
            {
                noMoreSentencesAvailable();
            }
        }
    }
}
