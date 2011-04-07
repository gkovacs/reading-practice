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
        public MainPage mainPage;

        public WordDictionary wordDictionary
        {
            get
            {
                return mainPage.wordDictionary;
            }
        }

        /// <summary>
        /// do not add code to the constructor. Put things you want to occur on startup in performOnStartup
        /// </summary>
        public RightSidebarControl()
        {
            InitializeComponent();
        }

        public void performOnStartup()
        {
            LanguageSelector.Items.Add("Select Language");
            LanguageSelector.Items.Add("Mandarin Chinese (Simplified)");
            LanguageSelector.SelectedIndex = 0;
            this.readSentencesTab.mainPage = mainPage;
            this.contributeSentencesTab.mainPage = mainPage;
        }
    }
}
