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
using System.Linq;
using System.IO;
using System.Windows.Browser;

namespace ReadingPractice
{
    //[ScriptableType]
    public abstract class SentenceDictionary
    {
        public abstract IList<string> getSentences(string focusWord, Func<string, bool> isWordAllowedFunc);
        public abstract string translateToEnglish(string foreignSentence);
        public abstract string[] getWords(string foreignSentence);
        public abstract WordDictionary wordDictionary { get; }
        public abstract Languages language { get; }
        public abstract void addSentence(string sentence, string translation);
    }
}
