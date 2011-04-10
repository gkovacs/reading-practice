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

namespace ReadingPractice
{
    public static class Textbooks
    {
        // open the textbooks file, and parse it

        parseTextbookFile();

        static string[][][] textbooks;


        static Textbooks()
        {
            // read in how many textbooks there are
            int iNumBooks = 1;
            textbooks = new string[iNumBooks][][];

            // for each book
            for (int i = 0; i < iNumBooks; ++i)
            {
                //read in number of chapters
                int iNumChapters = 1;

                textbooks[i] = new string[iNumChapters][];
                
                //for each chapter
                for (int j = 0; j < iNumChapters; ++j)
                {
                    //read in the number of words
                    int iNumWords = 1;

                    textbooks[i][j] = new string[iNumWords];

                    for (int k = 0; k < iNumWords; ++k)
                    {
                        // read in the word
                        string kWord = "word";
                        textbooks[i][j][k] = kWord;
                    }
                }
            }

        }

        private enum ParseStateType
        {
            PS_CHAPTER,
            PS_END,
            PS_TITLE,
            PS_WORD,
            PS_QUANTITY
        };
        private enum ParseInputType
        {
            PI_CHAPTER,
            PI_TITLE,
            PI_WORD,
            PI_QUANTITY
        }
        private void ParseTextbookFile (string filename)
        {
            ParseStateType eState = ParseStateType.PS_TITLE;

            StreamReader kReader = new StreamReader(
                Application.GetResourceStream(
                new Uri(
                    "ReadingPractice;component/cmn-simp-word-def.txt",
                    UriKind.Relative)).Stream);

            LinkedList<LinkedList<LinkedList<string>>> textbooks = new LinkedList<LinkedList<LinkedList<string>>>();

            while (eState != ParseStateType.PS_END)
            {
                string kLine = kReader.ReadLine();

                string kTrimmed = TrimTabs(kLine);
                ParseInputType kInputType = GetParseInputType(kLine);

                switch ( eState )
                {
                    case ParseStateType.PS_CHAPTER:
                        eState = chapterStateParse(type,kName,);
                        break;
                    case ParseStateType.PS_TITLE:
                        eState = titleStateParse(type,kName,);
                        break;
                    case ParseStateType.PS_WORD:
                        eState = wordStateParse(type,kName,);
                        break;
                }
            }
        }
        private string TrimTabs (string kString)
        {
            int i = kString.IndexOf('\t');
            if ( i == -1 )
                return kString;
            return kString.Substring(i+1);
        }
        private parseInputType GetParseInputType(kLine)

        private ParseState parseChapter (StreamReader kReader, )
        {
            
        }

        private ParseState parseWord (StreamReader kReader, )
        {

        }

    }


    
}
