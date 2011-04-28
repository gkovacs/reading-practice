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
using System.IO;
using System.Collections.Generic;

namespace ReadingPractice
{
    public static class WordMapSimplifiedToTraditional
    {
        static Dictionary<string, string> simpToTrad = new Dictionary<string, string>();
        static Dictionary<string, string> tradToSimp = new Dictionary<string, string>();

        static WordMapSimplifiedToTraditional()
        {
            /*
            using (StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri("ReadingPractice;component/cmn-word-simptotrad.txt", UriKind.Relative)).Stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\t');
                    if (parts.Length != 2)
                        throw new Exception();
                    simpToTrad[parts[0]] = parts[1];
                    tradToSimp[parts[1]] = parts[0];
                }
                reader.Close();
            }
            */
        }

        public static string toSimplified(string word)
        {
            if (!tradToSimp.ContainsKey(word))
                return "";
            return tradToSimp[word];
        }

        public static string toTraditional(string word)
        {
            if (!simpToTrad.ContainsKey(word))
                return "";
            return simpToTrad[word];
        }
    }
}
