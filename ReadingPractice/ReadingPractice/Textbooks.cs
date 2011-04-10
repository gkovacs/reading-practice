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
using System.IO;
using System.Linq;

namespace ReadingPractice
{
    public class Textbooks
    {
        Textbook[] textbooks;
        Dictionary<string, Textbook> textbookDictionary = new Dictionary<string, Textbook>();

        public class Chapter
        {
            public readonly string chapterName;
            public readonly string[] words;
            public readonly HashSet<string> wordSet = new HashSet<string>();
            public Chapter(string chapterName, string[] words)
            {
                this.chapterName = chapterName;
                this.words = words;
                foreach (string word in words)
                    wordSet.Add(word);
            }
        }

        public class Textbook
        {
            public readonly string textbookName;
            public readonly Chapter[] chapters;
            public readonly Dictionary<string, Chapter> chapterDictionary = new Dictionary<string, Chapter>();
            public Textbook(string textbookName, Chapter[] chapters)
            {
                this.textbookName = textbookName;
                this.chapters = chapters;
                foreach (Chapter chapter in chapters)
                    chapterDictionary[chapter.chapterName] = chapter;
            }
        }

        public Textbooks()
        {
            using (StreamReader reader = new StreamReader(
                Application.GetResourceStream(
                new Uri(
                    "ReadingPractice;component/textbooks.txt",
                    UriKind.Relative)).Stream))
            {
                LinkedList<string> words = new LinkedList<string>();
                string currentChapterName = "";
                LinkedList<Chapter> chapters = new LinkedList<Chapter>();
                string currentTextbookName = "";
                LinkedList<Textbook> textbooks = new LinkedList<Textbook>();

                Action endChapter = () =>
                {
                    if (words.Count > 0)
                    {
                        chapters.AddLast(new Chapter(currentChapterName, words.ToArray()));
                        currentChapterName = "";
                        words.Clear();
                    }
                };
                Action endTextbook = () =>
                {
                    if (chapters.Count > 0)
                    {
                        textbooks.AddLast(new Textbook(currentTextbookName, chapters.ToArray()));
                        currentTextbookName = "";
                        chapters.Clear();
                    }
                };
                while (!reader.EndOfStream)
                {
                    string curline = reader.ReadLine();
                    if (!curline.StartsWith("\t")) // new textbook
                    {
                        endChapter();
                        endTextbook();
                        currentTextbookName = curline.Trim();
                    }
                    else if (!curline.StartsWith("\t\t")) // new chapter
                    {
                        endChapter();
                        currentChapterName = curline.Trim();
                    }
                    else // word
                    {
                        if (curline.Trim() != "")
                            words.AddLast(curline.Trim());
                    }
                }
                endChapter();
                endTextbook();
                reader.Close();
                this.textbooks = textbooks.ToArray();
                foreach (Textbook text in textbooks)
                {
                    this.textbookDictionary[text.textbookName] = text;
                }
            }

        }

    }
    
}
