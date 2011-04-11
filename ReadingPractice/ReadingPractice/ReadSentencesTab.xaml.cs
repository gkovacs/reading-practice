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
            this.FetchNextSentenceButton.IsEnabled = false;
            this.Warnings.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            this.Warnings.FontWeight = FontWeights.Bold;
            if (StudyFocus == "")
            {
                this.Warnings.Content = "You must select words that can be displayed in sentences";
            }
            else
            {
                this.Warnings.Content = "No more sentences are available containing " + StudyFocus;
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

        private void FetchNextSentenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sentencesToBeAdded.Count == 0)
                return;
            else if (sentencesToBeAdded.Count == 1)
            {
                noMoreSentencesAvailable();
            }
            string sent = sentencesToBeAdded.First();
            string tranlatedSentence = sentenceDictionary.translateToEnglish(sent);
            sentencesToBeAdded.RemoveFirst();
            SentenceListViewer.Children.Insert(2, new SentenceView(sent, tranlatedSentence, mainPage));
        }
    }
}
