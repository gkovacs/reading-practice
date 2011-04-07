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

        public SentenceView(string nativeSentence, MainPage mainPage)
        {
            InitializeComponent();
            this.mainPage = mainPage;
            this.nativeSentence = nativeSentence;
            foreach (string word in sentenceDictionary.getWords(nativeSentence, language))
            {
                string currentWord = word;
                string reading = wordDictionary.getReading(word, language);
                string definition = wordDictionary.translateToEnglish(word, language);
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
                        System.Windows.Controls.Primitives.Popup popup = new System.Windows.Controls.Primitives.Popup();
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
                            popup.IsOpen = false;
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
            this.TranslatedSentence.Text = sentenceDictionary.translateToEnglish(nativeSentence, language);
        }
    }
}
