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
            this.displayReading.Content = wordDictionary.getReading(foreignWord);
            this.displayDefinition.Content = wordDictionary.translateToEnglish(foreignWord);
            this.displayedCheckbox.IsChecked = mainPage.LeftSidebar.isDisplayed(foreignWord);
            if (mainPage.LeftSidebar.StudyFocus == foreignWord)
            {
                this.makeStudyFocus.IsEnabled = false;
                this.displayedCheckbox.IsEnabled = false;
            }
        }

        private void makeStudyFocus_Click(object sender, RoutedEventArgs e)
        {
            mainPage.LeftSidebar.StudyFocus = this.foreignWord;
            this.makeStudyFocus.IsEnabled = false;
            this.displayedCheckbox.IsChecked = true;
            this.displayedCheckbox.IsEnabled = false;
            if (mainPage.LeftSidebar.wordMakeStudyFocusButtons.ContainsKey(this.foreignWord))
            {
                mainPage.LeftSidebar.wordMakeStudyFocusButtons[foreignWord].IsEnabled = false;
            }
            if (mainPage.LeftSidebar.wordAllowedCheckboxes.ContainsKey(this.foreignWord))
            {
                mainPage.LeftSidebar.wordAllowedCheckboxes[foreignWord].IsChecked = true;
                mainPage.LeftSidebar.wordAllowedCheckboxes[foreignWord].IsEnabled = false;
            }
        }

        private void displayedCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (!displayedCheckbox.IsChecked.HasValue)
                return;
            if (displayedCheckbox.IsChecked.Value)
                mainPage.LeftSidebar.allowWord(this.foreignWord);
            else
                mainPage.LeftSidebar.banWord(this.foreignWord);
        }
    }
}
