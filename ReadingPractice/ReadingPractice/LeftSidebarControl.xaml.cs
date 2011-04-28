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

using System.Globalization;

using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using System.IO;
using System.IO.IsolatedStorage;

namespace ReadingPractice
{
    public partial class LeftSidebarControl : UserControl
    {
        public MainPage mainPage;
        public event Action<string> focusWordChanged;
        public event Action displayedListChanged;

        private string _studyFocus = "";

        public HashSet<string> kSetAllowedWords = new HashSet<string>();

        public void updateDisplayedWords()
        {
            displayedListChanged();
        }

        public bool batchChanges = false;
        public void allowWord(string word, bool sendupdate=true, bool displayListChange=true)
        {
            if (sendupdate && !batchChanges)
                mainPage.sendAllowWord(word);
            bool alreadyAllowed = kSetAllowedWords.Contains(word);
            if (!alreadyAllowed)
                this.kSetAllowedWords.Add(word);
            if (this.wordAllowedCheckboxes.ContainsKey(word))
                this.wordAllowedCheckboxes[word].IsChecked = true;
            if (alreadyAllowed || batchChanges)
                return;
            if (displayListChange && displayedListChanged != null)
                displayedListChanged();
        }

        public void banWord(string word, bool sendupdate = true, bool displayListChange = true)
        {
            if (sendupdate && !batchChanges)
                mainPage.sendBanWord(word);
            bool alreadyBanned = !kSetAllowedWords.Contains(word);
            if (!alreadyBanned)
                this.kSetAllowedWords.Remove(word);
            if (word == StudyFocus)
                StudyFocus = "";
            if (this.wordAllowedCheckboxes.ContainsKey(word))
                this.wordAllowedCheckboxes[word].IsChecked = false;
            if (alreadyBanned || batchChanges)
                return;
            if (displayListChange && displayedListChanged != null)
                displayedListChanged();
        }

        public IList<string> kMatches = null;
        public string kPrevSearchTerm = "";

        public string selectedTextbook = "";
        public string selectedChapter = "";


        double dLineHeight = 40.0;

        private Brush defaultBrush = null;

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
                string prevStudyFocus = _studyFocus;
                if (this.wordMakeStudyFocusButtons.ContainsKey(prevStudyFocus))
                {
                    this.wordMakeStudyFocusButtons[prevStudyFocus].IsEnabled = true;
                }
                _studyFocus = value;
                if (_studyFocus != "")
                {
                    allowWord(_studyFocus, true, false);
                    int existingIdx = StudyFocusForeignWord.Items.IndexOf(_studyFocus);
                    if (existingIdx >= 0)
                    {
                        //StudyFocusForeignWord.Items.RemoveAt(existingIdx);
                        StudyFocusForeignWord.SelectedIndex = existingIdx;
                    }
                    else
                    {
                        StudyFocusForeignWord.Items.Insert(1, _studyFocus);
                        StudyFocusForeignWord.SelectedIndex = 1;
                    }
                    StudyFocusReading.Content = wordDictionary.getReading(_studyFocus);
                    StudyFocusTranslation.Content = wordDictionary.translateToEnglish(_studyFocus);
                    //reviewButton.IsEnabled = true;
                    StudyFocusForeignWord.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    if (focusWordChanged != null)
                        focusWordChanged(_studyFocus);
                }
                else // general review
                {
                    StudyFocusForeignWord.SelectedIndex = 0;
                    StudyFocusReading.Content = "Reviewing all words";
                    StudyFocusTranslation.Content = " ";
                    //reviewButton.IsEnabled = false;
                    StudyFocusForeignWord.Background = defaultBrush;
                    if (!isDisplayed(StudyFocus))
                        allowWord(StudyFocus); // does changing already
                    else if (focusWordChanged != null)
                        focusWordChanged(_studyFocus);
                }
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
            Opacity = 0.0;
            InitializeComponent();
        }

        //Dictionary<string, double> translationStringHeights = new Dictionary<string, double>();
        //Dictionary<string, double> wordStringHeights = new Dictionary<string, double>();
        //Dictionary<string, double> readingStringHeights = new Dictionary<string, double>();
        double wordColumnWidth = 70.0;
        double readingColumnWidth = 100.0;
        double translationColumnWidth = 200.0;
        double[] positions;

        string allTextbooks = "Show All Vocabulary (don't filter by textbook)";
        string allChapters = "Show Vocab from All Chapters";

        public void performOnStartup()
        {
            defaultBrush = StudyFocusForeignWord.Background;
            StudyFocusForeignWord.Items.Add("General Review");
            StudyFocusForeignWord.SelectedIndex = 0;
            /*
            foreach (var x in stringHeights.Take(100))
            {
                Debug.WriteLine(x.Key + x.Value);
            }
            */
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
            StudyFocus = "";

            // load books
            /*
            Dictionary<string, int> wordFreqs = new Dictionary<string,int>();
            foreach (var y in mainPage.sentenceDictionary.getSentences("", q => true))
            {
                foreach (var x in mainPage.sentenceDictionary.getWords(y))
                {
                    if (!wordFreqs.ContainsKey(x))
                    {
                        wordFreqs[x] = 0;
                    }
                    ++wordFreqs[x];
                }
            }
            List<Tuple<int, string>> wordFreqsL = new List<Tuple<int, string>>();
            foreach (var x in wordFreqs)
            {
                wordFreqsL.Add(Tuple.Create(x.Value, x.Key));
            }
            wordFreqsL.Sort();
            wordFreqsL.Reverse();
            FileStream f = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\gezaSpecialFile2.txt", FileMode.Create);
            //IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("gezaFileSilverlight.txt", FileMode.Create, isoFile);
            StreamWriter sw = new StreamWriter(f);
            foreach (var x in wordFreqsL)
            {
                sw.WriteLine(x.Item2 + "\t" + x.Item1);
            }
            sw.Close();
            f.Close();
            */

            /*
            foreach (string word in wordDictionary.listWordsByFrequency().Take(2000))
            {
                this.kSetAllowedWords.Add(word);
            }
            */

            selectedTextbook = allTextbooks;
            selectedChapter = allChapters;
            this.textbookSelect.Items.Add(allTextbooks);

            for (int i = 0; i < textbooks.textbooks.Length; ++i)
            {
                ComboBoxItem kItem = new ComboBoxItem();
                kItem.Content = textbooks.textbooks[i].textbookName;
                this.textbookSelect.Items.Add(textbooks.textbooks[i].textbookName);
            }
            this.textbookSelect.SelectedIndex = 0;
            textbookSelect.SelectionChanged += (s, e) =>
            {
                this.kPrevSearchTerm = "";
                if (this.textbookSelect.SelectedItem.ToString() != this.selectedTextbook)
                {
                    this.chapterSelect.Items.Clear();
                    this.selectedTextbook = this.textbookSelect.SelectedItem.ToString();

                    if (textbooks.textbookDictionary.ContainsKey(textbookSelect.SelectedItem.ToString()))
                    {
                        Textbooks.Textbook t = textbooks.textbookDictionary[textbookSelect.SelectedItem.ToString()];
                        this.chapterSelect.Items.Add(allChapters);
                        for (int i = 0; i < t.chapters.Length; ++i)
                        {
                            this.chapterSelect.Items.Add(t.chapters[i].chapterName);
                        }
                    }
                    if (this.chapterSelect.Items.Count > 0)
                        this.chapterSelect.SelectedIndex = 0;
                    Search_TextChanged(null, null);
                }
            };

            chapterSelect.SelectionChanged += (s, e) =>
            {
                this.kPrevSearchTerm = "";
                if (this.chapterSelect.SelectedItem != null
                && this.chapterSelect.SelectedItem.ToString() != this.selectedChapter)
                {
                    this.selectedChapter = this.chapterSelect.SelectedItem.ToString();
                    Search_TextChanged(null, null);
                }
            };


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
            sortPinYin(null,null);
            //VocabSelectionCanvas.Height = dLineHeight * this.kMatches.Count;
            VocabSelectionCanvas.Height = canvasHeight;

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

        private double measureStringHeight(string str, double width)
        {
            TextBlock txt = new TextBlock();
            txt.Width = width;
            txt.MaxWidth = width;
            txt.MinWidth = width;
            txt.TextWrapping = TextWrapping.Wrap;
            txt.Text = str;
            txt.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            txt.LineHeight = this.dLineHeight / 2.0;
            return txt.ActualHeight;
        }

        private Paragraph toParagraphHighlighting(string text, string highlight)
        {
            Paragraph par = new Paragraph();
            if (highlight != "")
            {
                int idx = 0;
                while (idx < text.Length)
                {
                    int nextMatch = text.IndexOf(highlight, idx, StringComparison.OrdinalIgnoreCase);
                    if (nextMatch < 0)
                    {
                        par.Inlines.Add(new Run { Text = text.Substring(idx) });
                        break;
                    }
                    par.Inlines.Add(new Run { Text = text.Substring(idx, nextMatch - idx) });
                    par.Inlines.Add(new Run { Text = text.Substring(nextMatch, highlight.Length), FontWeight = FontWeights.Bold });
                    idx = nextMatch + highlight.Length;
                }
            }
            else
            {
                par.Inlines.Add(new Run { Text = text });
            }
            return par;
        }

        public Dictionary<string, CheckBox> wordAllowedCheckboxes = new Dictionary<string, CheckBox>();
        public Dictionary<string, Button> wordMakeStudyFocusButtons = new Dictionary<string, Button>();

        public void DrawSearchMatches ()
        {
            lock (VocabSelectionCanvas)
            {
                string searchText = Search.Text;
                wordAllowedCheckboxes.Clear();
                wordMakeStudyFocusButtons.Clear();
                //Search.Dispatcher.BeginInvoke(() => {
                VocabSelectionCanvas.Children.Clear();

                double verticalOffsetStart = VocabSelectionScrollViewer.VerticalOffset;
                double scrollViewerHeight = VocabSelectionScrollViewer.Height;
                double scrollViewerWidth = VocabSelectionScrollViewer.ActualWidth;
                double verticalOffsetEnd = verticalOffsetStart + scrollViewerHeight;

                if (positions.Length == 0)
                    return;
                int first = 0;
                int last = positions.Length - 1;
                
                while (first < last)
                {
                    int mid = (first + last) / 2;
                    double curval = positions[mid];
                    if (curval >= verticalOffsetStart && curval <= verticalOffsetEnd)
                    {
                        break;
                    }
                    else if (curval <= verticalOffsetStart)
                    {
                        first = mid + 1;
                    }
                    else
                    {
                        last = mid - 1;
                    }
                }
                int iFirstVisibleItem = (first + last) / 2;
                int iLastItemVisible = (first + last) / 2;
                while (iFirstVisibleItem > 0 && positions[iFirstVisibleItem] >= verticalOffsetStart)
                {
                    --iFirstVisibleItem;
                }
                while (iLastItemVisible < positions.Length - 1 && positions[iLastItemVisible] <= verticalOffsetEnd)
                {
                    ++iLastItemVisible;
                }
                iFirstVisibleItem = Math.Max(iFirstVisibleItem, 0);
                iLastItemVisible = Math.Min(iLastItemVisible, kMatches.Count - 1);
                //int iFirstVisibleItem = (int)(VocabSelectionScrollViewer.VerticalOffset/dLineHeight);
                //int iLastItemVisible = (int)((VocabSelectionScrollViewer.VerticalOffset + VocabSelectionScrollViewer.Height)/dLineHeight);

                //iLastItemVisible = Math.Min(iLastItemVisible,this.kMatches.Count);
                SolidColorBrush highlightColor = new SolidColorBrush(Color.FromArgb(255, 224, 251, 255));;
                for (int i = iFirstVisibleItem; i <= iLastItemVisible; ++i)
                {
                    string word = this.kMatches[i];
                    double height = wordDictionary.getWordHeight(word);
                    double position = positions[i];

                    if (i % 2 == 1)
                    {
                        Rectangle highlightColumn = new Rectangle();
                        highlightColumn.Fill = highlightColor;
                        highlightColumn.Height = height;
                        highlightColumn.Width = scrollViewerWidth;
                        highlightColumn.SetValue(Canvas.LeftProperty, 0.0);
                        highlightColumn.SetValue(Canvas.TopProperty, position);
                        VocabSelectionCanvas.Children.Add(highlightColumn);
                    }

                    RichTextBox kWord = new RichTextBox();
                    kWord.Height = height;
                    kWord.MinHeight = height;
                    kWord.Width = wordColumnWidth;
                    kWord.Blocks.Add(toParagraphHighlighting(word, searchText));
                    kWord.Background = new SolidColorBrush(Colors.Transparent);
                    kWord.BorderThickness = new Thickness(0.0);
                    kWord.IsReadOnly = true;
                    kWord.TextWrapping = TextWrapping.Wrap;
                    kWord.SetValue(Canvas.LeftProperty, 0.0);
                    kWord.SetValue(Canvas.TopProperty, position);
                    RichTextBox kRomanization = new RichTextBox();
                    kRomanization.Height = height;
                    kRomanization.MinHeight = height;
                    kRomanization.Width = readingColumnWidth;
                    kRomanization.Blocks.Add(toParagraphHighlighting(wordDictionary.getReading(word), searchText));
                    kRomanization.Background = new SolidColorBrush(Colors.Transparent);
                    kRomanization.BorderThickness = new Thickness(0.0);
                    kRomanization.IsReadOnly = true;
                    kRomanization.TextWrapping = TextWrapping.Wrap;
                    kRomanization.SetValue(Canvas.LeftProperty, kWord.Width + 5.0);
                    kRomanization.SetValue(Canvas.TopProperty, position);
                    RichTextBox kTranslation = new RichTextBox();
                    kTranslation.Height = height;
                    kTranslation.MinHeight = height;
                    kTranslation.Width = translationColumnWidth;
                    kTranslation.Blocks.Add(toParagraphHighlighting(wordDictionary.translateToEnglish(word), searchText));
                    kTranslation.TextWrapping = TextWrapping.Wrap;
                    //kTranslation.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                    //kTranslation.LineHeight = dLineHeight / 2.0;
                    kTranslation.Background = new SolidColorBrush(Colors.Transparent);
                    kTranslation.BorderThickness = new Thickness(0.0);
                    kTranslation.IsReadOnly = true;
                    kTranslation.SetValue(Canvas.LeftProperty, kWord.Width + kRomanization.Width + 10.0);
                    kTranslation.SetValue(Canvas.TopProperty, position);
                    CheckBox kDisplayWord = new CheckBox();
                    kDisplayWord.Height = dLineHeight / 2.0;
                    kDisplayWord.Content = "May appear in sentences";
                    kDisplayWord.SetValue(Canvas.LeftProperty, kWord.Width + kRomanization.Width + kTranslation.Width + 15.0);
                    kDisplayWord.SetValue(Canvas.TopProperty, position);
                    kDisplayWord.IsChecked = this.isDisplayed(word);
                    kDisplayWord.Checked += (s, e) =>
                    {
                        allowWord(word);
                        /*
                        Debug.WriteLine("checked" + word);
                        if (!kSetAllowedWords.Contains(word))
                            this.kSetAllowedWords.Add(word);
                        if (!batchChanges)
                            displayedListChanged();
                        */
                    };
                    kDisplayWord.Unchecked += (s, e) =>
                    {
                        banWord(word);
                        /*
                        Debug.WriteLine("unchecked" + word);
                        if (kSetAllowedWords.Contains(word))
                            this.kSetAllowedWords.Remove(word);
                        if (!batchChanges)
                            displayedListChanged();
                       */
                    };
                    Button kMakeStudyFocus = new Button();
                    if (word == StudyFocus)
                    {
                        kMakeStudyFocus.IsEnabled = false;
                        kDisplayWord.IsEnabled = false;
                    }
                    kMakeStudyFocus.Height = dLineHeight / 2.0;
                    kMakeStudyFocus.Content = "Make study focus";
                    kMakeStudyFocus.SetValue(Canvas.LeftProperty, kWord.Width + kRomanization.Width + kTranslation.Width + 15.0);
                    kMakeStudyFocus.SetValue(Canvas.TopProperty, dLineHeight / 2.0 + position);
                    kMakeStudyFocus.Click += (s, e) =>
                    {
                        this.StudyFocus = word;
                        kDisplayWord.IsChecked = true;
                        this.allowWord(word);
                        kDisplayWord.IsEnabled = false;
                        kMakeStudyFocus.IsEnabled = false;
                    };

                    wordAllowedCheckboxes.Add(word, kDisplayWord);
                    wordMakeStudyFocusButtons.Add(word, kMakeStudyFocus);
                    VocabSelectionCanvas.Children.Add(kWord);
                    VocabSelectionCanvas.Children.Add(kRomanization);
                    VocabSelectionCanvas.Children.Add(kTranslation);
                    VocabSelectionCanvas.Children.Add(kDisplayWord);
                    VocabSelectionCanvas.Children.Add(kMakeStudyFocus);
                }
            }
            //});
        }

        /// <summary>
        /// TODO
        /// returns true if this word (given in the foreign language) is allowed to be displayed in sentences
        /// </summary>
        public bool isDisplayed(string foreignWord)
        {
            //return true;
            //Debug.WriteLine(foreignWord + ":" + this.kSetAllowedWords.Contains(foreignWord));
            return this.kSetAllowedWords.Contains(foreignWord);
        }

        private void reviewButton_Click(object sender, RoutedEventArgs e)
        {
            this.StudyFocus = "";
        }

        private double canvasHeight;

        private void computeOffsets()
        {
            positions = new double[kMatches.Count];
            double total = 0.0;
            for (int i = 0; i < positions.Length; ++i)
            {
                double currentWordHeight = wordDictionary.getWordHeight(kMatches[i]);
                positions[i] = total;
                total += currentWordHeight;
            }
            canvasHeight = total;
        }

        private IList<string> filterToTextbookAndChapter(IList<string> wordList)
        {
            if (this.selectedTextbook == allTextbooks || this.selectedTextbook == "")
                return wordList;
            Textbooks.Textbook currentTextbook = textbooks.textbookDictionary[selectedTextbook];
            List<string> list = new List<string>();
            Func<string, Textbooks.Chapter[], bool> wordAllowed = (word, chapters) =>
            {
                foreach (var chapter in chapters)
                {
                    if (chapter.wordSet.Contains(word))
                        return true;
                }
                return false;
            };
            if (this.selectedChapter == allChapters || this.selectedChapter == "") // everything in the textbook
            {
                foreach (string word in wordList)
                {
                    if (wordAllowed(word, currentTextbook.chapters))
                        list.Add(word);
                }
                return list;
            }
            Textbooks.Chapter currentChapter = currentTextbook.chapterDictionary[selectedChapter];
            Textbooks.Chapter[] allowedChapters = new Textbooks.Chapter[] { currentChapter };
            foreach (string word in wordList)
            {
                if (wordAllowed(word, allowedChapters))
                    list.Add(word);
            }
            return list;
        }

        private void findMatchingTextSynchronous(string searchText)
        {
            lock (Search)
            {
                IList<string> kSearchBase;
                // if search text is empty, or current search text is not a substring of the previous search
                if (searchText == "")
                {
                    Debug.WriteLine("findMatchingTextSynchronous");
                    this.kMatches = filterToTextbookAndChapter(wordDictionary.listWords());
                    this.kPrevSearchTerm = searchText;
                    computeOffsets();
                    VocabSelectionCanvas.Dispatcher.BeginInvoke(() =>
                    {
                        VocabSelectionCanvas.Height = canvasHeight; //dLineHeight * kMatches.Count;
                        VocabSelectionScrollViewer.ScrollToTop();
                        this.DrawSearchMatches();
                    });
                    return;
                }
                else if (searchText.Contains(kPrevSearchTerm) && kPrevSearchTerm != "")
                {
                    kSearchBase = filterToTextbookAndChapter(this.kMatches);
                }
                else
                {
                    kSearchBase = filterToTextbookAndChapter(wordDictionary.listWords());
                }

                IList<string> kNewMatches = new List<string>();
                foreach (string match in kSearchBase)
                {
                    if (match.IndexOf(searchText, 0, StringComparison.OrdinalIgnoreCase) >= 0
                    || wordDictionary.getReading(match).IndexOf(searchText, 0, StringComparison.OrdinalIgnoreCase) >= 0
                    || wordDictionary.translateToEnglish(match).IndexOf(searchText, 0, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        kNewMatches.Add(match);
                    }
                }

                // update the state: kMatches, kPrevSearchTerm
                this.kMatches = kNewMatches;
                this.kPrevSearchTerm = searchText;
                computeOffsets();
                VocabSelectionCanvas.Dispatcher.BeginInvoke(() =>
                {
                    VocabSelectionCanvas.Height = canvasHeight;
                    VocabSelectionScrollViewer.ScrollToTop();
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

        private void banAll_Click(object sender, RoutedEventArgs e)
        {
            string[] tmpw = kMatches.ToArray();
            batchChanges = true;
            foreach (string word in tmpw)
            {
                banWord(word, false, false);
            }
            mainPage.sendBanWordGroup(tmpw);
            displayedListChanged();
            batchChanges = false;
        }

        private void allowAll_Click(object sender, RoutedEventArgs e)
        {
            string[] tmpw = kMatches.ToArray();
            batchChanges = true;
            foreach (string word in tmpw)
            {
                allowWord(word, false, false);
            }
            mainPage.sendAllowWordGroup(tmpw);
            displayedListChanged();
            batchChanges = false;
        }

        int StrokeCountCompare(string x, string y)
        {
            return CompareInfo.GetCompareInfo("zh-CN_stroke").Compare(x, y);
        }

        int PinyinCompare(string x, string y)
        {
            return wordDictionary.getReading(x).CompareTo(wordDictionary.getReading(y));
        }

        int EnglishCompare(string x, string y)
        {
            return wordDictionary.translateToEnglish(x).CompareTo(wordDictionary.translateToEnglish(y));
        }

        int DisplayedCompare(string x, string y)
        {
            bool containsX = this.kSetAllowedWords.Contains(x);
            bool containsY = this.kSetAllowedWords.Contains(y);

            if ( (containsX && containsY) || (!containsX && !containsY))
            {
                return EnglishCompare(x,y);
            }


            return containsX && !containsY ? -1 : 1;
        }

        void resetSortButtonColors ()
        {
            sortByPinyin.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            sortByEnglish.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            sortByDisplayed.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        private void sortPinYin(object sender, RoutedEventArgs e)
        {
            if ( kMatches == null )
                return;

            List<string> k = (List<string>)(kMatches);//.sort
            k.Sort(PinyinCompare);
            computeOffsets();

            resetSortButtonColors();
            sortByPinyin.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));

            // then issue re-display
            DrawSearchMatches();
        }

        private void sortEnglish(object sender, RoutedEventArgs e)
        {
            if ( kMatches == null )
                return;
            
            computeOffsets();

            List<string> k = (List<string>)(kMatches);//.sort
            k.Sort(EnglishCompare);
            computeOffsets();

            resetSortButtonColors();
            sortByEnglish.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));

            // then issue re-display
            DrawSearchMatches();
        }

        private void sortDisplayed(object sender, RoutedEventArgs e)
        {
            if (kMatches == null)
                return;

            List<string> k = (List<string>)(kMatches);//.sort
            k.Sort(DisplayedCompare);
            computeOffsets();

            resetSortButtonColors();
            sortByDisplayed.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));

            // then issue re-display
            DrawSearchMatches();
        }

        private void StudyFocusForeignWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudyFocusForeignWord.SelectedIndex == 0)
                StudyFocus = "";
            else
                StudyFocus = (string)StudyFocusForeignWord.SelectedValue;
        }

    }
}
