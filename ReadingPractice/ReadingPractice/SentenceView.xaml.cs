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

        public SentenceView(string nativeSentence, MainPage mainPage)
        {
            InitializeComponent();
            this.mainPage = mainPage;
            this.NativeLanguageSentence.Text = nativeSentence;
            this.TranslatedSentence.Text = mainPage.sentenceDictionary.translateToEnglish(nativeSentence, mainPage.language);

        }
    }
}
