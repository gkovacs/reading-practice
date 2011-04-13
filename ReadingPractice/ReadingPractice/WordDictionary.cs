using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Resources;
using System.IO;
using System.Windows.Browser;

namespace ReadingPractice
{
    [ScriptableType]
    public abstract class WordDictionary
    {
        public abstract string translateToEnglish(string foreignWord);
        public abstract string translateToForeign(string englishWord);
        public abstract string getReading(string foreignWord);
        public abstract IList<string> listWords();
        public abstract Languages language { get; }
        public abstract int getWordcountInSentences(string foreignWord);
        public abstract IList<string> listWordsByFrequency();
    }
}
