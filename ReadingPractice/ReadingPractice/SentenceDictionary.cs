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

namespace ReadingPractice
{
    public interface SentenceDictionary
    {
        IList<string> getSentences(string focusWord, Func<string, bool> isWordAllowedFunc);
        string translateToEnglish(string foreignSentence);
        string translateToForeign(string englishSentence);
        string[] getWords(string foreignSentence);
        WordDictionary wordDictionary { get; }
        Languages language { get; }
    }
}
