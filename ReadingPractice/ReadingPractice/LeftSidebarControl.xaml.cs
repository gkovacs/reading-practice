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
    public partial class LeftSidebarControl : UserControl
    {
        public MainPage mainPage;
        public event Action<string> focusWordChanged;
        public event Action displayedListChanged;

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

        /// <summary>
        /// do not add code to the constructor. Put things you want to occur on startup in performOnStartup
        /// </summary>
        public LeftSidebarControl()
        {
            InitializeComponent();
        }

        public void performOnStartup()
        {
            StudyFocus = "地震";
            foreach (string word in wordDictionary.listWords().Take(50)) // first 50 words in dictionary
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = word;
                this.AllVocabList.Items.Add(checkbox);
            }

        }

        /// <summary>
        /// getter returns empty string if no study focus is set (reviewing mode), returns study focus in
        /// the foreign language being studied otherwise.
        /// setter sets the study focus and updates GUI appropriately
        /// </summary>
        ///
        private string _studyFocus;
        public string StudyFocus
        {
            get
            {
                return _studyFocus;
            }
            set
            {
                _studyFocus = value;
                StudyFocusForeignWord.Content = _studyFocus;
                StudyFocusReading.Content = wordDictionary.getReading(_studyFocus);
                StudyFocusTranslation.Content = wordDictionary.translateToEnglish(_studyFocus);
                if (focusWordChanged != null)
                    focusWordChanged(_studyFocus);
            }
        }

        /// <summary>
        /// TODO
        /// returns true if this word (given in the foreign language) is allowed to be displayed in sentences
        /// </summary>
        public bool isDisplayed(string foreignWord)
        {
            return true;
        }
    }
}
