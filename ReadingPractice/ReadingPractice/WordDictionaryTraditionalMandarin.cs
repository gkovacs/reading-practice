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
    public class WordDictionaryTraditionalMandarin : WordDictionary
    {
        List<string> allWords = new List<string>();
        List<string> wordsByFrequency = new List<string>();
        Dictionary<string, string> readings = new Dictionary<string, string>();
        Dictionary<string, string> foreignToEnglish = new Dictionary<string, string>();
        public override Languages language
        {
            get
            {
                return Languages.TraditionalMandarin;
            }
        }

        public WordDictionaryTraditionalMandarin()
        {
            using (StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri("ReadingPractice;component/cmn-trad-word-def.txt", UriKind.Relative)).Stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\t');
                    if (parts.Length != 2)
                        throw new Exception();
                    foreignToEnglish[parts[0]] = parts[1];
                }
                reader.Close();
            }
            using (StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri("ReadingPractice;component/cmn-trad-word-reading.txt", UriKind.Relative)).Stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\t');
                    if (parts.Length != 2)
                        throw new Exception();
                    readings[parts[0]] = parts[1];
                }
                reader.Close();
            }
            foreach (string word in readings.Keys)
            {
                if (this.foreignToEnglish.ContainsKey(word))
                {
                    this.allWords.Add(word);
                }
                else
                {
                    this.foreignToEnglish.Remove(word);
                }
            }
            this.allWords.Sort((s1, s2) => getReading(s1).CompareTo(getReading(s2)));
        }

        public override string translateToEnglish(string foreignWord)
        {
            if (!foreignToEnglish.ContainsKey(foreignWord))
                return "";
            return foreignToEnglish[foreignWord];
        }

        public override string getReading(string foreignWord)
        {
            if (!readings.ContainsKey(foreignWord))
                return "";
            return readings[foreignWord];
        }

        public override int getWordcountInSentences(string word)
        {
            return 1; // TODO
        }

        public override IList<string> listWordsByFrequency()
        {
            return new List<string>(); // TODO
        }

        public override IList<string> listWords()
        {
            return this.allWords.AsReadOnly();
        }
    }
}
