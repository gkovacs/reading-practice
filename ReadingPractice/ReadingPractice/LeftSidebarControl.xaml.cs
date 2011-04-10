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
using System.Windows.Threading;
using System.Threading;

namespace ReadingPractice
{
    public partial class LeftSidebarControl : UserControl
    {
        public MainPage mainPage;
        public event Action<string> focusWordChanged;
        public event Action displayedListChanged;

        private string _studyFocus;

        private HashSet<string> kSetAllowedWords;

        public IList<string> kMatches = null;
        public string kPrevSearchTerm = "";

        double dLineHeight = 40.0;

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
        public Textbooks textbooks
        {
            get
            {
                return mainPage.textbooks;
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

            /*
            TreeView tree = new TreeView();

            TreeViewItem t = new TreeViewItem();

            tree.Items.Add(t);

            for (int i = 0; i < 50; ++i)
            {
                //                CheckBox c = new CheckBox();
                //                t.Items.Add(c);
                //            }
                Polygon p = new Polygon();
                p.Points.Add(new Point(0, 0));
                p.Points.Add(new Point(4, 0));
                p.Points.Add(new Point(0, 4));
                p.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                t.Items.Add(p);
            }

//            StackPanel s = ;
 //           tree.Items.Add(obj);

            VocabSelectionScrollViewer.Content = tree;
            */
            StudyFocus = "地震";
            //StudyFocus = "";

            // load books
            

            //this.kSetAllowedWords = new HashSet<string>();



            //Label showVocab = new Label();
            //showVocab.Height = 20.0;
            //showVocab.Content = "show all vocab";
            //kMatches = wordDictionary.listWords();
            /*
            IList<UIElement> elementList = new List<UIElement>();
            showVocab.SetValue(Canvas.LeftProperty, 0.0);
            showVocab.SetValue(Canvas.TopProperty, 0.0);
            elementList.Add(showVocab);
            */
            // going ahead to create an Element List of every possible UI element that could ever be displayed on the canvas
            

            /*
            IList<string> allWords = wordDictionary.listWords();
            IList<UIElement> elementList = new List<UIElement>();
            TreeView tree = new TreeView();

            VocabSelectionScrollViewer.Content = tree;

            //tree
            
            

            */
            /*
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
            */


            //int iOffset = 0;
            
            //Search_TextChanged(null,null);
            findMatchingTextSynchronous("");
            VocabSelectionCanvas.Height = dLineHeight * this.kMatches.Count;

            PropertyChangedCallback onScrollChanged = (s, e) =>
            {
                //if (!vocabShownClicked) return;
                DrawSearchMatches();
            };
            // attach onScrolledChanged, so it is called everytime scrolling happens
            VocabSelectionScrollViewer.AddScrollCallback(onScrollChanged);


/*            double dLineHeight = 20.0;

            int iFirstVisibleItem = (int)(VocabSelectionScrollViewer.VerticalOffset/20.0);
            int iLastItemVisible = (int)((VocabSelectionScrollViewer.VerticalOffset+VocabSelectionScrollViewer.Height)/20.0);

            for (int i = iFirstVisibleItem; i <= iLastItemVisible; ++i)
            {
                CheckBox kCheckBoxBook = new CheckBox();
                kCheckBoxBook.Height = dLineHeight;
                kCheckBoxBook.Content = this.kMatches[i];
                kCheckBoxBook.SetValue(Canvas.LeftProperty,10.0);
                kCheckBoxBook.SetValue(Canvas.TopProperty,dLineHeight*i+dLineHeight);
                VocabSelectionCanvas.Children.Add(kCheckBoxBook);
            }

 */

            /*
            // for each book
            for (int i = 0; i < Textbooks.textbooks.Length; ++i)
            {

                CheckBox kCheckBoxBook = new CheckBox();
                kCheckBoxBook.Height = dLineHeight;
                kCheckBoxBook.Content = textbooks.textbooks[i].textbookName;
                kCheckBoxBook.SetValue(Canvas.LeftProperty, 10.0);
                kCheckBoxBook.SetValue(Canvas.TopProperty, 20.0 * iOffset + 20.0);
                VocabSelectionCanvas.Children.Add(kCheckBoxBook);
                iOffset += 1;
                // for each chapter
                for (int j = 0; j < textbooks[i].Length; ++j)
                {
                    CheckBox kCheckBoxChapter = new CheckBox();
                    elementList.Add(kCheckBoxChapter);

                    iOffset += 1;
                    // for each word
                    for (int k = 0; k < textbooks[i][j].Length; ++k)
                    {
                        CheckBox kCheckBoxWord = new CheckBox();
                        elementList.Add(newVocab);
                        // read in the word
                        string kWord = "word";
                        textbooks[i][j][k] = kWord;
                        iOffset += 1;
                    }
                }
            }
            */


/*
            VocabSelectionCanvas.Children.Add(showVocab);
            // given two input integers, draws the ui elements whos indices are in the list designated by the range indices
            Action<int, int> drawItemsInRange = (int firstItemVisible, int lastItemVisible) =>
            {
                VocabSelectionCanvas.Children.Clear();
                int startpos = Math.Max(firstItemVisible - 2, 0);
                int endpos = Math.Min(lastItemVisible + 2, allWords.Count - 1);
                for (int i = startpos; i < endpos; ++i)
                {
                    //elementList[i].SetValue(Canvas.LeftProperty, 10.0);
                    //elementList[i].SetValue(Canvas.TopProperty, 20.0 * i + 20.0);
                    CheckBox newVocab = new CheckBox();
                    newVocab.Height = 20.0;
                    newVocab.Content = allWords[i];
                    newVocab.SetValue(Canvas.LeftProperty, 10.0);
                    newVocab.SetValue(Canvas.TopProperty, 20.0 * i + 20.0);
                    VocabSelectionCanvas.Children.Add(newVocab);
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
*/
            /*
            foreach (string word in wordDictionary.listWords().Take(50)) // first 50 words in dictionary
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = word;
                this.AllVocabList.Items.Add(checkbox);
            }
            */
//#endif
        }


        public void DrawSearchMatches ()
        {
            //Search.Dispatcher.BeginInvoke(() => {
            VocabSelectionCanvas.Children.Clear();
            
            int iFirstVisibleItem = (int)(VocabSelectionScrollViewer.VerticalOffset/dLineHeight);
            int iLastItemVisible = (int)((VocabSelectionScrollViewer.VerticalOffset + VocabSelectionScrollViewer.Height)/dLineHeight);

            iLastItemVisible = Math.Min(iLastItemVisible,this.kMatches.Count);
            for (int i = iFirstVisibleItem; i < iLastItemVisible; ++i)
            {
                TextBlock kWord = new TextBlock();
                kWord.Height = dLineHeight;
                kWord.Width = 70.0;
                kWord.Text = this.kMatches[i];
                kWord.TextWrapping = TextWrapping.Wrap;
                kWord.SetValue(Canvas.LeftProperty, 0.0);
                kWord.SetValue(Canvas.TopProperty, dLineHeight * i);
                TextBlock kRomanization = new TextBlock();
                kRomanization.Height = dLineHeight;
                kRomanization.Width = 100.0;
                kRomanization.Text = wordDictionary.getReading(this.kMatches[i]);
                kRomanization.TextWrapping = TextWrapping.Wrap;
                kRomanization.SetValue(Canvas.LeftProperty, kWord.Width);
                kRomanization.SetValue(Canvas.TopProperty, dLineHeight * i);
                TextBlock kTranslation = new TextBlock();
                kTranslation.Height = dLineHeight;
                kTranslation.Width = 200.0;
                kTranslation.Text = wordDictionary.translateToEnglish(this.kMatches[i]);
                kTranslation.TextWrapping = TextWrapping.Wrap;
                kTranslation.SetValue(Canvas.LeftProperty, kWord.Width + kRomanization.Width);
                kTranslation.SetValue(Canvas.TopProperty, dLineHeight * i);
                CheckBox kDisplayWord = new CheckBox();
                kDisplayWord.Height = dLineHeight / 2.0;
                kDisplayWord.Content = "Display word in sentences?";
                kDisplayWord.SetValue(Canvas.LeftProperty, kWord.Width + kRomanization.Width + kTranslation.Width);
                kDisplayWord.SetValue(Canvas.TopProperty, dLineHeight * i);
                Button kMakeStudyFocus = new Button();
                kMakeStudyFocus.Height = dLineHeight / 2.0;
                kMakeStudyFocus.Content = "Make study focus";
                kMakeStudyFocus.SetValue(Canvas.LeftProperty, kWord.Width + kRomanization.Width + kTranslation.Width);
                kMakeStudyFocus.SetValue(Canvas.TopProperty, dLineHeight * i + dLineHeight / 2.0);
                VocabSelectionCanvas.Children.Add(kWord);
                VocabSelectionCanvas.Children.Add(kRomanization);
                VocabSelectionCanvas.Children.Add(kTranslation);
                VocabSelectionCanvas.Children.Add(kDisplayWord);
                VocabSelectionCanvas.Children.Add(kMakeStudyFocus);
            }
            //});
        }

        /// <summary>
        /// TODO
        /// returns true if this word (given in the foreign language) is allowed to be displayed in sentences
        /// </summary>
        public bool isDisplayed(string foreignWord)
        {
            return true;
            //return this.kSetAllowedWords.Contains(foreignWord);
        }

        private void reviewButton_Click(object sender, RoutedEventArgs e)
        {
            this.StudyFocus = "";
        }

        private void findMatchingTextSynchronous(string searchText)
        {
            lock(Search) {
            IList<string> kSearchBase;
            // if search text is empty, or current search text is not a substring of the previous search
            if (searchText == "")
            {
                this.kMatches = wordDictionary.listWords();
                this.kPrevSearchTerm = searchText;
                VocabSelectionCanvas.Dispatcher.BeginInvoke(() =>
                {
                    VocabSelectionCanvas.Height = dLineHeight * kMatches.Count;
                    this.DrawSearchMatches();
                });
                return;
            }
            else if (searchText.Contains(kPrevSearchTerm))
            {
                kSearchBase = this.kMatches;
            }
            else
            {
                kSearchBase = wordDictionary.listWords();
            }

            IList<string> kNewMatches = new List<string>();
            foreach (string match in kSearchBase)
            {
                if (match.Contains(searchText)
                || wordDictionary.getReading(match).Contains(searchText)
                || wordDictionary.translateToEnglish(match).Contains(searchText))
                {
                    kNewMatches.Add(match);
                }
            }

            // update the state: kMatches, kPrevSearchTerm
            this.kMatches = kNewMatches;
            this.kPrevSearchTerm = searchText;
            VocabSelectionCanvas.Dispatcher.BeginInvoke(() =>
            {
                VocabSelectionCanvas.Height = dLineHeight * kMatches.Count;
                this.DrawSearchMatches();
            });
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = Search.Text;
            Thread t = new Thread(() =>
            {
                findMatchingTextSynchronous(searchText);
            });
            t.Start();
        }
    }
}
