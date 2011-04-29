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
            this.loginButton.IsEnabled = false;
            this.createAccountButton.IsEnabled = false;
            this.serverCommunication = serverCommunication;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = loginUserNameTextBox.Text;
            string password = loginPasswordTextBox.Password;
            serverCommunication.doesUserExistAndIsPasswordCorrect(username, password,
            (doesUserExist, isPasswordCorrect) =>
            {
                if (!doesUserExist)
                {
                    loginErrors.Content = "user " + username + " does not exist";
                    loginErrors.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                }
                else if (!isPasswordCorrect)
                {
                    loginErrors.Content = "password for " + username + " is not correct";
                    loginErrors.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                }
                else
                {
                    serverCommunication.username = username;
                    userLoggedIn();
                }
            });
        }

        private void loginUserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loginUserNameTextBox.Text == "" || loginPasswordTextBox.Password == "")
            {
                loginButton.IsEnabled = false;
            }
            else
            {
                loginButton.IsEnabled = true;
            }
        }

        private void loginPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (loginUserNameTextBox.Text == "" || loginPasswordTextBox.Password == "")
            {
                loginButton.IsEnabled = false;
            }
            else
            {
                loginButton.IsEnabled = true;
            }
        }
    }
}
