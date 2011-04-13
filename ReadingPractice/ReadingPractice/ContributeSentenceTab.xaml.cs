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

        public ContributeSentenceTab()
        {
            InitializeComponent();
        }

        public void performOnStartup()
        {
            ContributeButton.IsEnabled = false;
        }

        private void ContributeButton_Click(object sender, RoutedEventArgs e)
        {
            string nativeSentence = this.NativeSentenceTextBox.Text;
            string translatedSentence = this.TranslatedSentenceTextBox.Text;
            this.ContributedSentenceListViewer.Children.Insert(1, new SentenceView(nativeSentence, translatedSentence, mainPage));
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
