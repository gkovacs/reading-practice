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
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
namespace ReadingPractice
{
    public partial class ClosedSentenceTab : UserControl
    {
        public MainPage mainPage;

        public ServerCommunication serverCommunication
        {
            get
            {
                return mainPage.serverCommunication;
            }
        }
        public SentenceDictionary sentenceDictionary
        {
            get
            {
                return mainPage.sentenceDictionary;
            }
        }
        public WordDictionary wordDictionary
        {
            get
            {
                return mainPage.wordDictionary;
            }
        }

        public ClosedSentenceTab()
        {
            InitializeComponent();
        }

        public void performOnStartup()
        {

        }

        public void rmSentence(string nativeSentence)
        {
            if (!presentWords.ContainsKey(nativeSentence))
                return;
            SentenceView sentview = presentWords[nativeSentence];
            presentWords.Remove(nativeSentence);
            allClosedSentences.Remove(nativeSentence);
            this.SentenceListViewer.Children.Remove(sentview);
            serverCommunication.sendRmClosedSentence(nativeSentence);
            colorBackgrounds();
        }

        public void insertSentence(string nativeSentence)
        {
            if (presentWords.ContainsKey(nativeSentence))
            {
                rmSentence(nativeSentence);
            }
            allClosedSentences.Add(nativeSentence);
            string translatedSentence = sentenceDictionary.translateToEnglish(nativeSentence);
            serverCommunication.sendAddClosedSentence(nativeSentence);
            SentenceView sentview = new SentenceView(nativeSentence, translatedSentence, mainPage, true);
            presentWords[nativeSentence] = sentview;
            sentview.restoreButton.Click += (o, e) =>
            {
                rmSentence(nativeSentence);
                mainPage.RightSidebar.readSentencesTab.insertSentence(nativeSentence);
            };
            sentview.removeButton.Click += (o, e) =>
            {
                rmSentence(nativeSentence);
            };
            this.SentenceListViewer.Children.Insert(1, sentview);
            colorBackgrounds();
        }
/// <summary>
/// NEW CODE BY ANDREW
/// </summary>
        public IList<string> allClosedSentences = new List<string>();
        public Dictionary<string, SentenceView> presentWords = new Dictionary<string, SentenceView>();
        void findClosedSentencesAsynch(string searchText)
        {
                //SentenceListViewer.Dispatcher.BeginInvoke(() =>
                //{
            for (int i = 1; i < SentenceListViewer.Children.Count; ++i)
            {
                SentenceListViewer.Children.RemoveAt(i);
            }
                //});
                
                if (searchText == ""){
                    foreach (string sentence in allClosedSentences){
                        string translatedSentence = sentenceDictionary.translateToEnglish(sentence);
                        SentenceView sv = new SentenceView(sentence, translatedSentence, mainPage, true);
                        //SentenceListViewer.Dispatcher.BeginInvoke(() =>
                        //{
                            SentenceListViewer.Children.Insert(1, sv);
                        //});
                        
                    }
                }
                else {
                    foreach (string sentence in allClosedSentences){
                        string translatedSentence = sentenceDictionary.translateToEnglish(sentence);
                        bool matchingPinyin = false;
                        foreach (string word in sentenceDictionary.getWords(sentence))
                        {
                            if (wordDictionary.getReading(word).Contains(searchText) || wordDictionary.translateToEnglish(word).Contains(searchText))
                            {
                                matchingPinyin = true;
                                break;
                            }
                        }
                        if (matchingPinyin || sentence.Contains(searchText) || translatedSentence.Contains(searchText))
                        {
                           
                            SentenceView sv = new SentenceView(sentence, translatedSentence, mainPage, true);
                            //SentenceListViewer.Dispatcher.BeginInvoke(() =>
                            //{
                                SentenceListViewer.Children.Insert(1, sv);
                            //});
                        }
                    }
                }
                colorBackgrounds();

            }

        private void colorBackgrounds()
        {
            int i = 0;
            foreach (var x in SentenceListViewer.Children.Skip(1))
            {
                SentenceView sentview = x as SentenceView;
                if (i % 2 == 1)
                    sentview.rootStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 224, 251, 255));
                else
                    sentview.rootStackPanel.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                i += 1;
            }
        }

        private void SearchClosedSentences_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchClosedSentences.Text.Trim();
            Debug.WriteLine(searchText);
                findClosedSentencesAsynch(searchText);
        }

    }
}
