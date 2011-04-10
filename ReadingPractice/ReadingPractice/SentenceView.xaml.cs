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
using System.Windows.Controls.Primitives;

namespace ReadingPractice
{
    public partial class SentenceView : UserControl
    {
        MainPage mainPage;
        public string nativeSentence;
        public Languages language
        {
            get
            {
                return mainPage.language;
            }
        }
        public SentenceDictionary sentenceDictionary
        {
            get
            {
                return mainPage.sentenceDictionary;
            }
        }
        public WordDictionary wordDictionary
        {
            get
            {
                return mainPage.wordDictionary;
            }
        }
        public string StudyFocus
        {
            get
            {
                return mainPage.LeftSidebar.StudyFocus;
            }
        }
        public Popup popup
        {
            get
            {
                return mainPage.popup;
            }
            set
            {
                mainPage.popup = value;
            }
        }
        public Textbooks textbooks
        {
            get
            {
                return mainPage.textbooks;
            }
        }

        public SentenceView(string nativeSentence, string translated, MainPage mainPage)
        {
            InitializeComponent();
            this.mainPage = mainPage;
            this.nativeSentence = nativeSentence;
            string[] segmented = sentenceDictionary.getWords(nativeSentence);
            if (segmented.Length == 0)
            {
                segmented = nativeSentence.ToCharArray().Select(x => x.ToString()).ToArray();
            }
            foreach (string word in segmented)
            {
                string currentWord = word;
                string reading = wordDictionary.getReading(word);
                string definition = wordDictionary.translateToEnglish(word);
                if (reading != "" && definition != "")
                {
                    Button button = new Button();
                    button.Content = word;
                    if (StudyFocus == currentWord)
                    {
                        button.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    }
                    button.FontSize = 20.0;
                    button.Click += (s, e) =>
                    {
                        popup = new Popup();
                        WordHelpPopup wordHelp = new WordHelpPopup(currentWord, mainPage);
                        popup.Child = wordHelp;
                        Point buttoncoords = button.TransformToVisual(mainPage).Transform(new Point(0, 0));
                        Point rightsidebarcoords = mainPage.RightSidebar.TransformToVisual(mainPage).Transform(new Point(0, 0));
                        if (buttoncoords.X + wordHelp.Width < mainPage.RightSidebar.ActualWidth + rightsidebarcoords.X)
                        {
                            popup.HorizontalOffset = buttoncoords.X;
                        }
                        else
                        {
                            popup.HorizontalOffset = mainPage.RightSidebar.ActualWidth + rightsidebarcoords.X - wordHelp.Width;
                        }
                        if (buttoncoords.Y - wordHelp.Height >= 0)
                        {
                            popup.VerticalOffset = buttoncoords.Y - wordHelp.Height;
                        }
                        else
                        {
                            popup.VerticalOffset = buttoncoords.Y + button.ActualHeight;
                        }
                        wordHelp.closeButton.Click += (s2, e2) =>
                        {
                            mainPage.popup = null;
                        };
                        popup.LostMouseCapture += (s3, e3) =>
                        {
                            mainPage.popup = null;
                        };
                        popup.LostFocus += (s4, e4) =>
                        {
                            mainPage.popup = null;
                        };
                        popup.IsOpen = true;
                    };
                    this.NativeLanguageSentenceDisplay.Children.Add(button);
                }
                else
                {
                    Label label = new Label();
                    label.Content = word;
                    if (StudyFocus == currentWord)
                    {
                        label.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    }
                    label.FontSize = 20.0;
                    this.NativeLanguageSentenceDisplay.Children.Add(label);
                }
            }
            this.TranslatedSentence.Text = translated;
        }
    }
}
