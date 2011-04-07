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
    public partial class WordHelpPopup : UserControl
    {
        string foreignWord;
        MainPage mainPage;
        public WordDictionary wordDictionary
        {
            get
            {
                return mainPage.wordDictionary;
            }
        }
        public Languages language
        {
            get
            {
                return mainPage.language;
            }
        }

        public WordHelpPopup(string foreignWord, MainPage mainPage)
        {
            InitializeComponent();
            this.mainPage = mainPage;
            this.foreignWord = foreignWord;
            this.displayForeignWord.Content = foreignWord;
            this.displayReading.Content = wordDictionary.getReading(foreignWord, language);
            this.displayDefinition.Content = wordDictionary.translateToEnglish(foreignWord, language);
            this.displayedCheckbox.IsChecked = mainPage.LeftSidebar.isDisplayed(foreignWord);
        }
    }
}
