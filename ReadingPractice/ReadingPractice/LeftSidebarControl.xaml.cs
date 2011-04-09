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
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

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

            Label showVocab = new Label();
            showVocab.Height = 20.0;
            showVocab.Content = "show all vocab";
            IList<string> allWords = wordDictionary.listWords();
            IList<UIElement> elementList = new List<UIElement>();
            showVocab.SetValue(Canvas.LeftProperty, 0.0);
            showVocab.SetValue(Canvas.TopProperty, 0.0);
            elementList.Add(showVocab);
            for (int i = 0; i < allWords.Count; ++i)
            {
                string word = allWords[i];
                CheckBox newVocab = new CheckBox();
                newVocab.Height = 20.0;
                newVocab.Content = word;
                newVocab.SetValue(Canvas.LeftProperty, 10.0);
                newVocab.SetValue(Canvas.TopProperty, 20.0 * i + 20.0);
                elementList.Add(newVocab);
            }
            VocabSelectionCanvas.Children.Add(showVocab);
            Action<int, int> drawItemsInRange = (firstItemVisible, lastItemVisible) =>
            {
                VocabSelectionCanvas.Children.Clear();
                for (int i = Math.Max(firstItemVisible-2, 0); i < Math.Min(lastItemVisible+2, elementList.Count-1); ++i)
                {
                    UIElement currentElement = elementList[i];
                    VocabSelectionCanvas.Children.Add(currentElement);
                }
            };
            bool vocabShownClicked = false;
            showVocab.MouseLeftButtonDown += (s, e) =>
            {
                vocabShownClicked = true;
                VocabSelectionCanvas.Height = 20.0 + 20.0 * allWords.Count;
                int firstItemVisible = (int) (VocabSelectionScrollViewer.VerticalOffset / 20.0);
                int lastItemVisible = (int) ((VocabSelectionScrollViewer.VerticalOffset + VocabSelectionScrollViewer.Height) / 20.0);
                drawItemsInRange(firstItemVisible, lastItemVisible);
            };
            
            PropertyChangedCallback onScrollChanged = (s, e) =>
            {
                if (!vocabShownClicked) return;
                int firstItemVisible = (int)(VocabSelectionScrollViewer.VerticalOffset / 20.0);
                int lastItemVisible = (int)((VocabSelectionScrollViewer.VerticalOffset + VocabSelectionScrollViewer.Height) / 20.0);
                drawItemsInRange(firstItemVisible, lastItemVisible);
            };
            VocabSelectionScrollViewer.AddScrollCallback(onScrollChanged);

            /*
            foreach (string word in wordDictionary.listWords().Take(50)) // first 50 words in dictionary
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = word;
                this.AllVocabList.Items.Add(checkbox);
            }
            */

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
