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

        public ClosedSentenceTab()
        {
            InitializeComponent();
        }

        public void performOnStartup()
        {

        }

        public void insertSentence(string nativeSentence)
        {
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


        void findClosedSentencesAsynch()
        {
            // lock ()
            // {
            // List newMatches = new List<>()
            // global list of ALL closed sentences: allClosedSentences
            // for each sentence in allClosedSentences
            //    if the sentence matches the search query
            //    then add the sentence to newMatches
            // prevSentenceMatches = newMatches


            // for existing search, if the user simply entered more keys,
            // then u can narrow the search by searching through the previous search
            // you'll need some sort of previousSentenceMatches
            // search through the list of prevSentenceMatches
            // for each sentence in prevSentenceMatches
            // remove the nonmatching sentences from prevSentenceMatches


            // draw(prevSentenceMatches)
            // }
        }

        private void SearchClosedSentences_TextChanged(object sender, TextChangedEventArgs e)
        {
            // schedule a thread and run findClosedSentencesAsynch in the thread

            
        }

    }
}
