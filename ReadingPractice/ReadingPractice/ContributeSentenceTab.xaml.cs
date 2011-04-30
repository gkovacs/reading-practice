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
    public partial class ContributeSentenceTab : UserControl
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

        public ContributeSentenceTab()
        {
            InitializeComponent();
        }

        public void performOnStartup()
        {
            ContributeButton.IsEnabled = false;
        }

        public void insertSentence(string nativeSentence)
        {
            string translatedSentence = sentenceDictionary.translateToEnglish(nativeSentence);
            serverCommunication.sendAddContribSentence(nativeSentence, translatedSentence);
            SentenceView sentview = new SentenceView(nativeSentence, translatedSentence, mainPage);
            sentview.removeButton.Click += (o, e) =>
            {
                this.ContributedSentenceListViewer.Children.Remove(sentview);
                serverCommunication.sendRmContribSentence(nativeSentence);
            };
            this.ContributedSentenceListViewer.Children.Insert(1, sentview);
        }

        private void ContributeButton_Click(object sender, RoutedEventArgs e)
        {
            string nativeSentence = this.NativeSentenceTextBox.Text;
            string translatedSentence = this.TranslatedSentenceTextBox.Text;
            sentenceDictionary.addSentence(nativeSentence, translatedSentence);
            insertSentence(nativeSentence);
            serverCommunication.sendAddContribSentence(nativeSentence, translatedSentence);
            mainPage.LeftSidebar.updateDisplayedWords();
            this.NativeSentenceTextBox.Text = "";
            this.TranslatedSentenceTextBox.Text = "";
        }

        private void NativeSentenceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NativeSentenceTextBox.Text != "" && TranslatedSentenceTextBox.Text != "")
            {
                ContributeButton.IsEnabled = true;
            }
            else
            {
                ContributeButton.IsEnabled = false;
            }
        }

        private void TranslatedSentenceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NativeSentenceTextBox.Text != "" && TranslatedSentenceTextBox.Text != "")
            {
                ContributeButton.IsEnabled = true;
            }
            else
            {
                ContributeButton.IsEnabled = false;
            }
        }
    }
}
