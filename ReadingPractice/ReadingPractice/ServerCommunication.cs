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
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace ReadingPractice
{
    public class ServerCommunication
    {
        private Queue<string> sendMessageQueue = new Queue<string>();
        private Queue<IEnumerable<string>> dataQueue = new Queue<IEnumerable<string>>();
        private string server = "gkovacs.xvm.mit.edu";
        private string folder = "reading-practice/secret-backend-path";
        public string baseurl
        {
            get
            {
                return "http://" + server + "/" + folder + "/";
            }
        }

        private string _username = null;
        public string username
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username != null)
                    throw new Exception("username already set");
                _username = value;
            }
        }

        public ServerCommunication()
        {
            new Thread(sendMessageLoop).Start();
        }

        private void sendMessageLoop()
        {
            while (true)
            {
                if (sendMessageQueue.Count > 0)
                {
                    string curval = null;
                    IEnumerable<string> data = null;
                    lock (sendMessageQueue)
                    {
                        curval = sendMessageQueue.Dequeue();
                        if (curval[0] == '@')
                            data = dataQueue.Dequeue();
                    }
                    if (data == null)
                    {
                        sendMessageActual(curval);
                    }
                    else
                    {
                        sendMessageWithDataActual(curval.Substring(1), data);
                    }

                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        public void sendMessageActual(string message)
        {
            Debug.WriteLine("sending message " + message);
            bool lockVar = false;
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (o, e) => { lockVar = true; };
            wc.OpenReadAsync(new Uri(baseurl + message));
            while (!lockVar)
            {
                Thread.Sleep(50);
            }
            Debug.WriteLine("done sending message " + message);
        }

        public void sendMessageWithDataActual(string message, IEnumerable<string> data)
        {
            Debug.WriteLine("sending message with data " + message);
            bool lockVar = false;
            WebClient wc = new WebClient();
            wc.OpenWriteCompleted += (o, e) =>
            {
                using (StreamWriter st = new StreamWriter(e.Result))
                {
                    foreach (string x in data)
                    {
                        st.WriteLine(x);
                    }
                }
                lockVar = true;
            };
            wc.OpenWriteAsync(new Uri(baseurl + message));
            while (!lockVar)
            {
                Thread.Sleep(200);
            }
            Debug.WriteLine("done sending message with data " + message);
        }

        public void sendMessage(string message)
        {
            lock (sendMessageQueue)
            {
                sendMessageQueue.Enqueue(message);
            }
        }

        public void sendMessageWithData(string message, IEnumerable<string> data)
        {
            lock (sendMessageQueue)
            {
                sendMessageQueue.Enqueue("@" + message);
                dataQueue.Enqueue(data);
            }
        }

        public void sendNewStudyFocus(string text)
        {
            sendMessage("setStudyFocus.cgi.py?userName=" + username + "&studyFocus=" + text);
        }

        public void sendAllowWord(string text)
        {
            sendMessage("addDisplayedWord.cgi.py?userName=" + username + "&displayedWord=" + text);
        }

        public void sendAllowAllWord()
        {
            sendMessage("addAllDisplayedWord.aspx?userName=" + username);
        }

        public void sendBanWord(string text)
        {
            sendMessage("rmDisplayedWord.cgi.py?userName=" + username + "&displayedWord=" + text);
        }

        public void sendBanAllWord()
        {
            sendMessage("rmAllDisplayedWord.aspx?userName=" + username);
        }

        public void sendAllowWordGroup(IEnumerable<string> words)
        {
            sendMessageWithData("addDisplayedWordMany.cgi.py?userName=" + username, words);
            //sendMessage("postpage.cgi.py?userName=" + username + "&page=addDisplayedWordMany.cgi.py&postdata=" + String.Join("%0A", words));
        }

        public void sendBanWordGroup(IEnumerable<string> words)
        {
            sendMessageWithData("rmDisplayedWordMany.cgi.py?userName=" + username, words);
            //sendMessage("postpage.cgi.py?userName=" + username + "&page=rmDisplayedWordMany.cgi.py&postdata=" + String.Join("%0A", words));
        }
    }
}
