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

namespace ReadingPractice
{
    public interface WordDictionary
    {
        string translateToEnglish(string foreignWord);
        string translateToForeign(string englishWord);
        string getReading(string foreignWord);
        IList<string> listWords();
        Languages language { get; }
    }
}
