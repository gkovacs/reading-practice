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

using System.Diagnostics;

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
                colorBackgrounds();
                serverCommunication.sendRmContribSentence(nativeSentence);
            };
            this.ContributedSentenceListViewer.Children.Insert(1, sentview);
            colorBackgrounds();
        }

        private void colorBackgrounds()
        {
            int i = 0;
            foreach (var x in ContributedSentenceListViewer.Children.Skip(1))
            {
                SentenceView sentview = x as SentenceView;
                if (i % 2 == 1)
                    sentview.rootStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 224, 251, 255));
                else
                    sentview.rootStackPanel.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                i += 1;
            }
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

        internal void Resize (double height,double width)
        {
            //this.NativeSentenceTextBox.Width = width / 2;
            //this.TranslatedSentenceTextBox.Width = this.NativeSentenceTextBox.Width;
            //this.ContributeButton.Width = this.NativeSentenceTextBox.Width;
//            this.ContributedSentenceListViewer.Width = width / 2;
//            this.LayoutRoot.Width = width / 2;
        }
    }
}
