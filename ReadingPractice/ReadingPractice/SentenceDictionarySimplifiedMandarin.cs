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
    public class SentenceDictionarySimplifiedMandarin : SentenceDictionary
    {
        Dictionary<Languages, List<string>> sentences = new Dictionary<Languages, List<string>>();
        Dictionary<Languages, Dictionary<string, string[]>> segmentation = new Dictionary<Languages, Dictionary<string, string[]>>();
        Dictionary<Languages, Dictionary<string, string>> foreignToEnglish = new Dictionary<Languages, Dictionary<string, string>>();
        Dictionary<Languages, Dictionary<string, string>> englishToForeign = new Dictionary<Languages, Dictionary<string, string>>();
        readonly WordDictionary _wordDictionary;
        readonly WordDictionary _traditionalWordDictionary;
        public WordDictionary wordDictionary
        {
            get
            {
                return _wordDictionary;
            }
        }
        public Languages language
        {
            get
            {
                return Languages.SimplifiedMandarin;
            }
        }

        public SentenceDictionarySimplifiedMandarin()
        {
            _wordDictionary = new WordDictionarySimplifiedMandarin();
            _traditionalWordDictionary = new WordDictionaryTraditionalMandarin();
            foreach (Languages lang in EnumHelper.GetValues<Languages>())
            {
                segmentation[lang] = new Dictionary<string, string[]>();
                englishToForeign[lang] = new Dictionary<string, string>();
                foreignToEnglish[lang] = new Dictionary<string, string>();
                sentences[lang] = new List<string>();
            }
            using (StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri("ReadingPractice;component/cmn-simp-eng.txt", UriKind.Relative)).Stream))
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
            using (StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri("ReadingPractice;component/cmn-simp-seg.txt", UriKind.Relative)).Stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\t');
                    if (parts.Length != 2)
                        throw new Exception();
                    segmentation[Languages.SimplifiedMandarin][parts[0]] = parts[1].Split(' ');
                }
                reader.Close();
            }
            foreach (string sent in foreignToEnglish[Languages.SimplifiedMandarin].Keys)
            {
                sentences[Languages.SimplifiedMandarin].Add(sent);
            }
        }

        public IList<string> getSentences(string focusWord, Func<string, bool> isWordAllowedFunc)
        {
            return sentences[language].Where((sent) =>
            {
                string[] words = segmentation[language][sent];
                if (!words.Contains(focusWord))
                    return false;
                foreach (string word in words)
                {
                    if (!isWordAllowedFunc(word))
                        return false;
                }
                return true;
            }).ToArray();
        }

        public string translateToEnglish(string foreignSentence)
        {
            if (!foreignToEnglish[language].ContainsKey(foreignSentence))
                return "";
            return foreignToEnglish[language][foreignSentence];
        }

        public string translateToForeign(string englishSentence)
        {
            if (!englishToForeign[language].ContainsKey(englishSentence))
                return "";
            return englishToForeign[language][englishSentence];
        }

        public string[] getWords(string foreignSentence)
        {
            Func<string, string> longestStartWord = null;
            longestStartWord = (remaining) =>
            {
                if (remaining.Length == 1)
                {
                    return remaining;
                }
                if (wordDictionary.translateToEnglish(remaining) != "")
                {
                    return remaining;
                }
                if (_traditionalWordDictionary.translateToEnglish(remaining) != "")
                {
                    return WordMapSimplifiedToTraditional.toSimplified(remaining);
                }
                return longestStartWord(remaining.Substring(0, remaining.Length - 1));
            };
            LinkedList<string> words = new LinkedList<string>();
            for (int i = 0; i < foreignSentence.Length; )
            {
                string nextWord = longestStartWord(foreignSentence.Substring(i));
                words.AddLast(nextWord);
                i += nextWord.Length;
            }
            return words.ToArray();
        }
    }
}
