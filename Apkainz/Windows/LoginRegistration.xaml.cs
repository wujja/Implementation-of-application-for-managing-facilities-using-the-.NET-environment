using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;

namespace Apkainz
{
    /// <summary>
    /// Logika interakcji dla klasy LoginRegistration.xaml
    /// </summary>
    public partial class LoginRegistration : MetroWindow
    {
        MySqlConnection mySqlConnection = new MySqlConnection("server=localhost;user=root;database=test;port=3306;password=mysql123user!");
        MySqlCommand mySqlCommand;
        string query = "";

        public LoginRegistration()
        {
            InitializeComponent();
        }

        private void GoToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.idUser = 1;
            mainWindow.Show();
            Close();
        }

        private void Login(string login, string password)
        {
            try
            {
               if (mySqlConnection.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConnection.Open();

                    query = string.Format("SELECT COUNT(idUser) FROM user WHERE login= '{0}'",login);
                    mySqlCommand = new MySqlCommand(query, mySqlConnection);
                    int value = Convert.ToInt32(mySqlCommand.ExecuteScalar());
                    if (value == 0)
                    {
                        MessageBox.Show("There is no such user", "Information",MessageBoxButton.OK);
                    }
                    else
                    {
                        query = string.Format("SELECT idUser, password FROM user WHERE login = '{0}'",login);
                        mySqlCommand = new MySqlCommand(query, mySqlConnection);
                        var reader =  mySqlCommand.ExecuteReader();
                        string passwordDataBase = "";
                        int idUser = 0;
                        while (reader.Read())
                        {
                            passwordDataBase = reader["password"].ToString();
                            idUser = Convert.ToInt32(reader["idUser"]);
                        }
                        reader.Close();

                        if (PasswordHash.ValidatePassword(password, passwordDataBase)) 
                        {
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.idUser = idUser;
                            mainWindow.Show();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Password is not correct", "Information", MessageBoxButton.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = string.Format("Error when login: \n{0}", ex.Message);
                MessageBox.Show(error, "Connection error", MessageBoxButton.OK);
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        private void Registration(string login, string password)
        {
            try
            {
                if (mySqlConnection.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConnection.Open();

                    query = string.Format("SELECT COUNT(idUser) FROM user WHERE login= '{0}'",login);
                    mySqlCommand = new MySqlCommand(query, mySqlConnection);
                    int value = Convert.ToInt32(mySqlCommand.ExecuteScalar());

                    if (value == 1)
                    {
                        MessageBox.Show(string.Format("User with login '{0}' already exists",login),"Information",MessageBoxButton.OK);
                        login_Login_TB.Focus();
                    }
                    else
                    {
                        string hashedPassword = PasswordHash.HashPassword(password);
                        query = string.Format("INSERT INTO user (login, password) VALUES ('{0}','{1}')",login, hashedPassword);
                        mySqlCommand.CommandText = query;
                        mySqlCommand.ExecuteNonQuery();
                        MessageBox.Show("User added", "Information",MessageBoxButton.OK);
                        ClearField();
                        chooseLoginOrRegistration_GD.Visibility = Visibility.Visible;
                        registration_GD.Visibility = Visibility.Hidden;
                        login_GD.Visibility = Visibility.Hidden;
                        goBackChoose_BT.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (Exception ex)
            {
                string error = string.Format("Error when adding user to the database: \n{0}", ex.Message);
                MessageBox.Show(error, "Connection error",MessageBoxButton.OK);
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        private void ClearField()
        {
            login_Login_TB.Text = "";
            login_Password_PB.Password = "";
            registration_login_TB.Text = "";
            registration_Password_PB.Password = "";
            registration_ConfirmPassword_PB.Password = "";
        }

       /* private void Registration_BT_Click(object sender, RoutedEventArgs e)
        {
           string password2 =  PasswordHash.HashPassword(registration_Password_PB.Password.ToString());
         //  MessageBox.Show(   PasswordHash.ValidatePassword(password2_PB_Copy.Password.ToString(), password2).ToString());
            
    //        MessageBox.Show((SecurePassword.ReferenceEquals(password, password2)).ToString());
        }*/

        private void Choose_Login_BT_Click(object sender, RoutedEventArgs e)
        {
            chooseLoginOrRegistration_GD.Visibility = Visibility.Hidden;
            registration_GD.Visibility = Visibility.Hidden;
            login_GD.Visibility = Visibility.Visible;
            goBackChoose_BT.Visibility = Visibility.Visible;
        }

        private void Choose_registration_BT_Click(object sender, RoutedEventArgs e)
        {
            chooseLoginOrRegistration_GD.Visibility = Visibility.Hidden;
            registration_GD.Visibility = Visibility.Visible;
            login_GD.Visibility = Visibility.Hidden;
            goBackChoose_BT.Visibility = Visibility.Visible;
        }

        private void GoBackChoose_BT_Click(object sender, RoutedEventArgs e)
        {
            chooseLoginOrRegistration_GD.Visibility = Visibility.Visible;
            registration_GD.Visibility = Visibility.Hidden;
            login_GD.Visibility = Visibility.Hidden;
            goBackChoose_BT.Visibility = Visibility.Hidden;
        }

        private void Registration_BT_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(registration_login_TB.Text) || string.IsNullOrWhiteSpace(registration_Password_PB.Password) || string.IsNullOrWhiteSpace(registration_ConfirmPassword_PB.Password))
            {
                MessageBox.Show("You have to enter your login and password", "Information", MessageBoxButton.OK);
                return;
            }
            if (registration_login_TB.Text.Length < 5 || registration_Password_PB.Password.Length < 5 || registration_ConfirmPassword_PB.Password.Length <5)
            {
                MessageBox.Show("Password and login must have at least 5 characters", "Information", MessageBoxButton.OK);
                return;
            }
            if (!string.Equals(registration_Password_PB.Password,registration_ConfirmPassword_PB.Password))
            {
                MessageBox.Show("Passwords must have be the same", "Information", MessageBoxButton.OK);
                return;
            }
            Registration(registration_login_TB.Text, registration_Password_PB.Password);
        }

        private void Login_BT_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(login_Login_TB.Text) || string.IsNullOrWhiteSpace(login_Password_PB.Password))
            {
                MessageBox.Show("You have to enter your login and password", "Information", MessageBoxButton.OK);
                return;
            }
            Login(login_Login_TB.Text, login_Password_PB.Password);
        }

        private void Testpass_Click(object sender, RoutedEventArgs e)
        {
            string pass =  PasswordHash.HashPassword(login_Password_PB.Password);
            
        }
    }
}
