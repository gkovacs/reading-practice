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

        public void insertSentence(string nativeSentence)
        {
            allClosedSentences.Add(nativeSentence);
            string translatedSentence = sentenceDictionary.translateToEnglish(nativeSentence);
            serverCommunication.sendAddClosedSentence(nativeSentence);
            SentenceView sentview = new SentenceView(nativeSentence, translatedSentence, mainPage);
            sentview.removeButton.Content = "Restore";
            sentview.removeButton.Click += (o, e) =>
            {
                this.SentenceListViewer.Children.Remove(sentview);
                serverCommunication.sendRmClosedSentence(nativeSentence);
                mainPage.RightSidebar.readSentencesTab.insertSentence(nativeSentence);
            };
            this.SentenceListViewer.Children.Insert(1, sentview);
        }
/// <summary>
/// NEW CODE BY ANDREW
/// </summary>
        public IList<string> allClosedSentences = new List<string>();

        public IList<string> newMatches;
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
                        SentenceView sv = new SentenceView(sentence, translatedSentence, mainPage);
                        sv.removeButton.Content = "Restore";
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
                           
                            SentenceView sv = new SentenceView(sentence, translatedSentence, mainPage);
                            sv.removeButton.Content = "Restore";
                            //SentenceListViewer.Dispatcher.BeginInvoke(() =>
                            //{
                                SentenceListViewer.Children.Insert(1, sv);
                            //});
                        }
                    }
                }

            }

        private void SearchClosedSentences_TextChanged(object sender, TextChangedEventArgs e)
        {
            // schedule a thread and run findClosedSentencesAsynch in the thread
            
            string searchText = SearchClosedSentences.Text.Trim();
            Debug.WriteLine(searchText);
            //Thread t = new Thread(() =>
            //{
                findClosedSentencesAsynch(searchText);
            //});
            //t.Start();

            
        }

    }
}
