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
    public partial class RightSidebarControl : UserControl
    {
        public RightSidebarControl()
        {
            InitializeComponent();
            LanguageSelector.Items.Add("Select Language");
            LanguageSelector.Items.Add("Mandarin Chinese (Simplified)");
            LanguageSelector.SelectedIndex = 0;
        }

        private void FetchNextSentenceButton_Click(object sender, RoutedEventArgs e)
        {
            int sentenceNum = SentenceListViewer.Children.Count;
            SentenceListViewer.Children.Insert(1, new SentenceView("native sentence " + sentenceNum, "translated sentence " + sentenceNum));
        }
    }
}
