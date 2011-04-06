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
    public class WordDictionary
    {
        List<string> allWords = new List<string>();
        Dictionary<Languages, Dictionary<string, string>> readings = new Dictionary<Languages,Dictionary<string,string>>();
        Dictionary<Languages, Dictionary<string, string>> englishToForeign = new Dictionary<Languages,Dictionary<string,string>>();
        Dictionary<Languages, Dictionary<string, string>> foreignToEnglish = new Dictionary<Languages,Dictionary<string,string>>();

        public WordDictionary()
        {
            foreach (Languages lang in EnumHelper.GetValues<Languages>())
            {
                readings[lang] = new Dictionary<string, string>();
                englishToForeign[lang] = new Dictionary<string, string>();
                foreignToEnglish[lang] = new Dictionary<string, string>();
            }
            using (StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri("ReadingPractice;component/cmn-simp-word-def.txt", UriKind.Relative)).Stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\t');
                    if (parts.Length != 2)
                        throw new Exception();
                    foreignToEnglish[Languages.SimplifiedMandarin][parts[0]] = parts[1];
                    englishToForeign[Languages.SimplifiedMandarin][parts[1]] = parts[0];
                }
                reader.Close();
            }
            using (StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri("ReadingPractice;component/cmn-simp-word-reading.txt", UriKind.Relative)).Stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\t');
                    if (parts.Length != 2)
                        throw new Exception();
                    readings[Languages.SimplifiedMandarin][parts[0]] = parts[1];
                }
                reader.Close();
            }
            foreach (string word in readings[Languages.SimplifiedMandarin].Keys)
            {
                if (this.foreignToEnglish[Languages.SimplifiedMandarin].ContainsKey(word))
                {
                    this.allWords.Add(word);
                }
                else
                {
                    this.foreignToEnglish[Languages.SimplifiedMandarin].Remove(word);
                }
            }
        }

        public string translateToEnglish(string foreignWord, Languages language)
        {
            return foreignToEnglish[language][foreignWord];
        }

        public string translateToForeign(string englishWord, Languages language)
        {
            return englishToForeign[language][englishWord];
        }

        public string getReading(string foreignWord, Languages language)
        {
            return readings[language][foreignWord];
        }

        public IList<string> listWords()
        {
            return this.allWords.AsReadOnly();
        }
    }
}
