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
                    button.FontSize = 20.0;
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
