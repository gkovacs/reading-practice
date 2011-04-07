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

        public SentenceView(string nativeSentence, MainPage mainPage)
        {
            InitializeComponent();
            this.mainPage = mainPage;
            this.nativeSentence = nativeSentence;
            foreach (string word in mainPage.sentenceDictionary.getWords(nativeSentence, language))
            {
                Button button = new Button();
                button.Content = word;
                button.FontSize = 20.0;
                this.NativeLanguageSentenceDisplay.Children.Add(button);
            }
            this.TranslatedSentence.Text = mainPage.sentenceDictionary.translateToEnglish(nativeSentence, mainPage.language);
        }
    }
}
