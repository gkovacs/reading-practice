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

        public string loginError
        {
            get
            {
                return this.loginErrors.Content.ToString();
            }
            set
            {
                if (value == "")
                {
                    this.loginErrors.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                }
                else
                {
                    this.loginErrors.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    this.loginButton.IsEnabled = false;
                }
                this.loginErrors.Content = value;
            }
        }

        public string createAccountError
        {
            get
            {
                return this.createAccountErrors.Content.ToString();
            }
            set
            {
                if (value == "")
                {
                    this.createAccountErrors.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                }
                else
                {
                    this.createAccountErrors.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    this.createAccountButton.IsEnabled = false;
                }
                this.createAccountErrors.Content = value;
            }
        }

        public LoginScreen(ServerCommunication serverCommunication)
        {
            InitializeComponent();
            this.loginButton.IsEnabled = false;
            this.createAccountButton.IsEnabled = false;
            this.loginErrors.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            this.createAccountErrors.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
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
                    loginError = "user " + username + " does not exist";
                }
                else if (!isPasswordCorrect)
                {
                    loginError = "password for " + username + " is not correct";
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

        private void createAccountButton_Click(object sender, RoutedEventArgs e)
        {
            string username = createAccountUsername.Text;
            string password = createAccountPassword.Password;
            serverCommunication.createAccount(username, password, () =>
            {
                serverCommunication.username = username;
                userLoggedIn();
            });
            this.IsEnabled = false;
            createAccountError = "Account being created, please wait";
        }

        bool createValidUserNameEntered = false;
        string createUserNameProposed = "";

        private void createAccountUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (createAccountUsername.Text == "" || createAccountPassword.Password == "" || createAccountPassword2.Password == "")
            {
                createAccountButton.IsEnabled = false;
            }
            else
            {
                createAccountButton.IsEnabled = true;
            }
            string username = createAccountUsername.Text;
            if (username != "")
            {
                serverCommunication.doesUserExist(username, (doesUserExist) =>
                {
                    createValidUserNameEntered = !doesUserExist;
                    createUserNameProposed = username;
                    passwordMatchCheck();
                });
            }
        }

        private void createValidUsernameEnteredCheck()
        {
            if (!createValidUserNameEntered)
            {
                createAccountError = "user " + createUserNameProposed + " already exists";
            }
            else
            {
                createAccountError = "";
            }
        }

        private void passwordMatchCheck()
        {
            if (createAccountPassword.Password != "" && createAccountPassword2.Password != "")
            {
                if (createAccountPassword.Password != createAccountPassword2.Password)
                {
                    createAccountError = "entered passwords don't match";
                    createAccountButton.IsEnabled = false;
                    return;
                }
                else
                {
                    createAccountError = "";
                }
            }
            createValidUsernameEnteredCheck();
        }

        private void createAccountPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (createAccountUsername.Text == "" || createAccountPassword.Password == "" || createAccountPassword2.Password == "")
            {
                createAccountButton.IsEnabled = false;
            }
            else
            {
                createAccountButton.IsEnabled = true;
            }
            passwordMatchCheck();
        }

        private void createAccountPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (createAccountUsername.Text == "" || createAccountPassword.Password == "" || createAccountPassword2.Password == "")
            {
                createAccountButton.IsEnabled = false;
            }
            else
            {
                createAccountButton.IsEnabled = true;
            }
            passwordMatchCheck();
        }
    }
}
