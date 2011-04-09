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
    [ScriptableType]
    public class SentenceDictionarySimplifiedMandarin : SentenceDictionary
    {
        List<string> sentences = new List<string>();
        Dictionary<string, string[]> segmentation = new Dictionary<string, string[]>();
        Dictionary<string, string> foreignToEnglish = new Dictionary<string, string>();
        Dictionary<string, string> englishToForeign = new Dictionary<string, string>();
        readonly WordDictionary _wordDictionary;
        public override WordDictionary wordDictionary
        {
            get
            {
                return _wordDictionary;
            }
        }
        public override Languages language
        {
            get
            {
                return Languages.SimplifiedMandarin;
            }
        }

        public SentenceDictionarySimplifiedMandarin()
        {
            _wordDictionary = new WordDictionarySimplifiedMandarin();
            using (StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri("ReadingPractice;component/cmn-simp-eng.txt", UriKind.Relative)).Stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\t');
                    if (parts.Length != 2)
                        throw new Exception();
                    foreignToEnglish[parts[0]] = parts[1];
                    englishToForeign[parts[1]] = parts[0];
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
                    segmentation[parts[0]] = parts[1].Split(' ');
                }
                reader.Close();
            }
            foreach (string sent in foreignToEnglish.Keys)
            {
                sentences.Add(sent);
            }
        }

        public override IList<string> getSentences(string focusWord, Func<string, bool> isWordAllowedFunc)
        {
            return sentences.Where((sent) =>
            {
                string[] words = segmentation[sent];
                if (focusWord != "" && !words.Contains(focusWord))
                    return false;
                foreach (string word in words)
                {
                    if (!isWordAllowedFunc(word))
                        return false;
                }
                return true;
            }).ToArray();
        }

        public override string translateToEnglish(string foreignSentence)
        {
            if (!foreignToEnglish.ContainsKey(foreignSentence))
                return "";
            return foreignToEnglish[foreignSentence];
        }

        public override string translateToForeign(string englishSentence)
        {
            if (!englishToForeign.ContainsKey(englishSentence))
                return "";
            return englishToForeign[englishSentence];
        }

        public override string[] getWords(string foreignSentence)
        {
            Func<string, string> longestStartWord = null;
            longestStartWord = (remaining) =>
            {
                if (wordDictionary.translateToEnglish(remaining) != "")
                {
                    return remaining;
                }
                string asSimplified = WordMapSimplifiedToTraditional.toSimplified(remaining);
                if (remaining.Length == 1)
                {
                    if (asSimplified != "")
                        return asSimplified;
                    return remaining;
                }
                if (asSimplified != "")
                {
                    return asSimplified;
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
