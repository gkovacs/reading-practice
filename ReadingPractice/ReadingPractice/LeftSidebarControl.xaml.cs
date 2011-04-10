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

using System.Diagnostics;

namespace ReadingPractice
{
    public partial class LeftSidebarControl : UserControl
    {
        public MainPage mainPage;
        public event Action<string> focusWordChanged;
        public event Action displayedListChanged;

        private string _studyFocus;

        private HashSet<string> kSetAllowedWords;


        /// <summary>
        /// getter returns empty string if no study focus is set (reviewing mode), returns study focus in
        /// the foreign language being studied otherwise.
        /// setter sets the study focus and updates GUI appropriately
        /// </summary>
        ///
        public string StudyFocus
        {
            get
            {
                return _studyFocus;
            }
            set
            {
                _studyFocus = value;
                if (_studyFocus != "")
                {
                    StudyFocusForeignWord.Content = _studyFocus;
                    StudyFocusReading.Content = wordDictionary.getReading(_studyFocus);
                    StudyFocusTranslation.Content = wordDictionary.translateToEnglish(_studyFocus);
                    reviewButton.IsEnabled = true;
                }
                else // general review
                {
                    StudyFocusForeignWord.Content = "General Review";
                    StudyFocusReading.Content = "Reviewing all words";
                    StudyFocusTranslation.Content = "";
                    reviewButton.IsEnabled = false;
                }
                if (focusWordChanged != null)
                    focusWordChanged(_studyFocus);
            }
        }
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
            //StudyFocus = "";

            // load books


            this.kSetAllowedWords = new HashSet<string>();


            Label showVocab = new Label();
            showVocab.Height = 20.0;
            showVocab.Content = "show all vocab";
            IList<string> allWords = wordDictionary.listWords();
            IList<UIElement> elementList = new List<UIElement>();
            showVocab.SetValue(Canvas.LeftProperty, 0.0);
            showVocab.SetValue(Canvas.TopProperty, 0.0);
            elementList.Add(showVocab);
            // going ahead to create an Element List of every possible UI element that could ever be displayed on the canvas
            for (int i = 0; i < allWords.Count; ++i)
            {
                string word = allWords[i];
                CheckBox newVocab = new CheckBox();
                newVocab.Height = 20.0;
                newVocab.Content = word;
                newVocab.Checked += (s,e) =>
                {
                    this.kSetAllowedWords.Add(word);
                };
                newVocab.Unchecked += (s,e) =>
                {
                    this.kSetAllowedWords.Remove(word);
                };

                //newVocab.SetValue(Canvas.LeftProperty, 10.0);
                //newVocab.SetValue(Canvas.TopProperty, 20.0 * i + 20.0);
                elementList.Add(newVocab);
            }
            VocabSelectionCanvas.Children.Add(showVocab);
            // given two input integers, draws the ui elements whos indices are in the list designated by the range indices
            Action<int, int> drawItemsInRange = (int firstItemVisible, int lastItemVisible) =>
            {
                VocabSelectionCanvas.Children.Clear();
                int startpos = Math.Max(firstItemVisible - 2, 0);
                int endpos = Math.Min(lastItemVisible + 2, elementList.Count - 1);
                for (int i = startpos; i < endpos; ++i)
                {
                    elementList[i].SetValue(Canvas.LeftProperty, 10.0);
                    elementList[i].SetValue(Canvas.TopProperty, 20.0 * i + 20.0);
                    VocabSelectionCanvas.Children.Add(elementList[i]);
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

            // function that determines what ui elements are visible, and then calls drawItemsInRange to draw them
            PropertyChangedCallback onScrollChanged = (s, e) =>
            {
                if (!vocabShownClicked) return;
                int firstItemVisible = (int)(VocabSelectionScrollViewer.VerticalOffset / 20.0);
                int lastItemVisible = (int)((VocabSelectionScrollViewer.VerticalOffset + VocabSelectionScrollViewer.Height) / 20.0);
                drawItemsInRange(firstItemVisible, lastItemVisible);
            };
            // attach onScrolledChanged, so it is called everytime scrolling happens
            VocabSelectionScrollViewer.AddScrollCallback(onScrollChanged);

            /*
            foreach (string word in wordDictionary.listWords().Take(50)) // first 50 words in dictionary
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = word;
                this.AllVocabList.Items.Add(checkbox);
            }
            */

            Debug.WriteLine("hello");
        }



        /// <summary>
        /// TODO
        /// returns true if this word (given in the foreign language) is allowed to be displayed in sentences
        /// </summary>
        public bool isDisplayed(string foreignWord)
        {
            return this.kSetAllowedWords.Contains(foreignWord);
        }

        private void reviewButton_Click(object sender, RoutedEventArgs e)
        {
            this.StudyFocus = "";
        }
    }
}
