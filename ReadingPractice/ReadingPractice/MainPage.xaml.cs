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
    public partial class MainPage : UserControl
    {
        public WordDictionary wordDictionary;

        public MainPage()
        {
            InitializeComponent();
            wordDictionary = new WordDictionary();
            this.LeftSidebar.wordDictionary = wordDictionary;
            this.RightSidebar.wordDictionary = wordDictionary;
            this.LeftSidebar.StudyFocusForeignWord.Content = "食";
            this.LeftSidebar.StudyFocusReading.Content = wordDictionary.getReading(this.LeftSidebar.StudyFocusForeignWord.Content.ToString(), Languages.SimplifiedMandarin);
            this.LeftSidebar.StudyFocusTranslation.Content = wordDictionary.translateToEnglish(this.LeftSidebar.StudyFocusForeignWord.Content.ToString(), Languages.SimplifiedMandarin);
        }
    }
}
