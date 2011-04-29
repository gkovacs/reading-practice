using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ReadingPractice
{
    public partial class LoginScreen : UserControl
    {
        private ServerCommunication serverCommunication;
        public event Action userLoggedIn;

        public LoginScreen(ServerCommunication serverCommunication)
        {
            InitializeComponent();
            this.serverCommunication = serverCommunication;
            this.loginButton.Click += (o, e) =>
            {
                serverCommunication.username = "gkovacs";
                userLoggedIn();
            };
        }
    }
}
