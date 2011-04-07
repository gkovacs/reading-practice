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

        public SentenceView(string nativeSentence, MainPage mainPage)
        {
            InitializeComponent();
            this.mainPage = mainPage;
            this.nativeSentence = nativeSentence;
            foreach (string word in sentenceDictionary.getWords(nativeSentence, language))
            {
                string reading = wordDictionary.getReading(word, language);
                string definition = wordDictionary.translateToEnglish(word, language);
                if (reading != "" && definition != "")
                {
                    Button button = new Button();
                    button.Content = word;
                    string currentWord = word;
                    button.FontSize = 20.0;
                    button.Click += (s, e) =>
                    {
                        System.Windows.Controls.Primitives.Popup popup = new System.Windows.Controls.Primitives.Popup();
                        WordHelpPopup wordHelp = new WordHelpPopup(currentWord, mainPage);
                        popup.Child = wordHelp;
                        Point buttoncoords = button.TransformToVisual(mainPage).Transform(new Point(0, 0));
                        popup.HorizontalOffset = buttoncoords.X;
                        if (buttoncoords.Y - wordHelp.Height >= 0)
                        {
                            popup.VerticalOffset = buttoncoords.Y - wordHelp.Height;
                        }
                        else
                        {
                            popup.VerticalOffset = buttoncoords.Y + button.ActualHeight;
                        }
                        popup.IsOpen = true;
                        wordHelp.closeButton.Click += (s2, e2) =>
                        {
                            popup.IsOpen = false;
                        };
                    };
                    this.NativeLanguageSentenceDisplay.Children.Add(button);
                }
                else
                {
                    Label label = new Label();
                    label.Content = word;
                    label.FontSize = 20.0;
                    this.NativeLanguageSentenceDisplay.Children.Add(label);
                }
            }
            this.TranslatedSentence.Text = sentenceDictionary.translateToEnglish(nativeSentence, language);
        }
    }
}
