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

        private bool isAlreadyPresent(string sentence)
        {
            foreach (var x in SentenceListViewer.Children.Skip(1))
            {
                SentenceView sen = (SentenceView)x;
                if (sentence == sen.NativeLanguageSentence.Text)
                    return true;
            }
            return false;
        }

        private void FetchNextSentenceButton_Click(object sender, RoutedEventArgs e)
        {
            IList<string> sentences = sentenceDictionary.getSentences(StudyFocus, language, mainPage.LeftSidebar.isDisplayed);
            System.Diagnostics.Debug.WriteLine(sentences.Count);
            foreach (string sent in sentences)
            {
                System.Diagnostics.Debug.WriteLine(sent);
                if (!isAlreadyPresent(sent))
                {
                    SentenceListViewer.Children.Insert(1, new SentenceView(sent, mainPage));
                    System.Diagnostics.Debug.WriteLine(sent+"added");
                    return;
                }
            }
            //int sentenceNum = SentenceListViewer.Children.Count;
            //SentenceListViewer.Children.Insert(1, new SentenceView("native sentence " + sentenceNum, "translated sentence " + sentenceNum));

        }
    }
}
