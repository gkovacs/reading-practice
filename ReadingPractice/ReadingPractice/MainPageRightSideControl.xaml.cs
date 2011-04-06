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

        public RightSidebarControl()
        {
            InitializeComponent();
            LanguageSelector.Items.Add("Select Language");
            LanguageSelector.Items.Add("Mandarin Chinese (Simplified)");
            LanguageSelector.SelectedIndex = 0;
        }
    }
}
