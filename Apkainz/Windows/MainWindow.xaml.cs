using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Threading;

namespace Apkainz
{
    /// <summary>q
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : MetroWindow
    {

        public int idUser;
        DataTable dtType = new DataTable("Type");
        DataTable dtPlace = new DataTable("Place");
        DataTable dtGuest = new DataTable("Guest");
        DataTable dtReservation = new DataTable("Reservation");
        DataTable dtPayment = new DataTable("Payment");
        DataTable dtTypeToShowInTable;
        DataTable dtPlaceToShowInTable;
        DataTable dtGuestToShowInTable;
        DataTable dtReservationToShowInTable;
        DataTable dtPaymentToShowInTable;
        string[,] tempValueComboBoxtypeOfPlaceDes;
        string[,] tempValueComboBoxreservationGuest;
        string[,] tempValueComboBoxreservationPlace;
        string[,] tempValueComboBoxpaymentWhichReservation;
        string[,] tempValueComboBoxpayment;
        
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ReadFromDataBase();
        }

        private void ReadFromDataBase()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            MySqlDataAdapter daType;
            MySqlDataAdapter daPlace;
            MySqlDataAdapter daGuest;
            MySqlDataAdapter daReservation;
            MySqlDataAdapter daPayment;

             string connStr = "server=localhost;user=root;database=test;port=3306;password=mysql123user!";
            
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                //MessageBox.Show("Connecting to MySQL...");
                conn.Open();

                string sqlType = "SELECT * FROM placetype";
                string sqlPlace = "SELECT pla.idPlace,pla.descriptionPlace,pla.idPlaceType,plat.descriptionPlaceType, pla.numberOfBeds FROM placetype plat JOIN place pla ON plat.idPlaceType = pla.idPlaceType WHERE pla.idUser =" + idUser + " ORDER BY pla.idPlace DESC";
                string sqlGuest = "Select gu.idGuest,gu.firstName,gu.lastName,gu.phoneNumber,gu.address,gu.email,usr.idUser FROM guest gu JOIN user usr ON gu.idUser = usr.idUser WHERE gu.idUser=" + idUser + " ORDER BY gu.idGuest DESC";
                string sqlReservation = "Select res.idReservation,res.dateFrom,res.dateTo,res.idGuest,res.idPlace,res.priceForDay,usr.idUser, gu.firstName,gu.lastName,gu.phoneNumber,plat.descriptionPlaceType,pla.descriptionPlace,pla.idPlace, res.numberOfAdults, res.numberOfChildren, res.descriptionReservation " +
                    "FROM reservation res JOIN place pla ON res.idPlace = pla.idPlace JOIN placetype plat ON pla.idPlaceType=plat.idPlaceType JOIN user usr ON pla.idUser = usr.idUser JOIN guest gu ON gu.idGuest = res.idGuest WHERE usr.idUser =" + idUser + " ORDER BY res.idReservation DESC";
                string sqlPayment = "Select pay.idPayment, pay.idReservation, pay.datePayment, pay.value, res.dateFrom,res.dateTo, plat.descriptionPlaceType, pla.descriptionPlace, gu.firstName,gu.lastName,gu.phoneNumber  FROM payment pay " +
                    "JOIN reservation res ON pay.idReservation=res.idReservation JOIN place pla ON pla.idPlace=res.idPlace JOIN guest gu ON gu.idGuest=res.idGuest JOIN placetype plat ON plat.idPlaceType=pla.idPlaceType " +
                    "WHERE pla.idUser =" + idUser + " ORDER BY pay.idPayment DESC";
                daType = new MySqlDataAdapter(sqlType, conn);
                daPlace = new MySqlDataAdapter(sqlPlace, conn);
                daGuest = new MySqlDataAdapter(sqlGuest, conn);
                daReservation = new MySqlDataAdapter(sqlReservation, conn);
                daPayment = new MySqlDataAdapter(sqlPayment, conn);
                MySqlCommandBuilder cbT = new MySqlCommandBuilder(daType);
                MySqlCommandBuilder cbPl = new MySqlCommandBuilder(daPlace);
                MySqlCommandBuilder cbG = new MySqlCommandBuilder(daGuest);
                MySqlCommandBuilder cbR = new MySqlCommandBuilder(daReservation);
                MySqlCommandBuilder cbPa = new MySqlCommandBuilder(daPayment);
                
                
                dtType.Clear();
                dtPlace.Clear();
                dtGuest.Clear();
                dtReservation.Clear();
                dtPayment.Clear();

                daType.Fill(dtType);
                daPlace.Fill(dtPlace);
                daGuest.Fill(dtGuest);
                daReservation.Fill(dtReservation);
                daPayment.Fill(dtPayment);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            conn.Close();
            //MessageBox.Show("Done.");
            ValueOfComboBox();
            DateToShow(reservationDateFromValue, 0);
            DateToShow(reservationDateToValue, 1);
            DisplayReservation displayReservation = new DisplayReservation(reservation_ReoGridcontrol,dtReservation,dtPayment,dtPlace);
            displayReservation.ShowReservation2();
            //displayReservation.ShowReservation();
            SelectDataToShowInTable();
        }

        private void AddToDataBase(string sql, Operation operation)
        {
            string connStr = "server=localhost;user=root;database=test;port=3306;password=mysql123user!";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            conn.Close();

            if ((bool)changeDisplayReservation_ToggleSwitchButton.IsChecked)
            {
                whichTableToDisplay_ComboBox.SelectedIndex = whichDataSet_ComboBox.SelectedIndex;
            }

            ReadFromDataBase();
            whichOperation_ComboBox.SelectedIndex = -1;
            executeModify_button.IsEnabled = false;
           
            
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            EnableModifyButton();
        }

        private void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs args)
        {
            EnableModifyButton();
        }

        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableModifyButton();
        }
        private void NumericValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            EnableModifyButton();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (paymentWholePaymentValue == checkBox)
            {
                paymentPrepaymentValue.IsChecked = false;
            }
            else if (paymentPrepaymentValue == checkBox)
            { 
                paymentWholePaymentValue.IsChecked = false;
            }
            EnableModifyButton();
        }

        private void PaymentWholePaymentValue_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true && paymentHowMuchMoneyIsMissingValue.Content != null)
            {
                paymentValueValue.Text = paymentHowMuchMoneyIsMissingValue.Content.ToString();
            }
            CheckBox_Checked(sender, e);
        }

        private void PaymentPrepaymentValue_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true && paymentHowMuchMoneyIsMissingValue.Content != null && paymentValueValue.Text == paymentHowMuchMoneyIsMissingValue.Content.ToString())
            {
                paymentValueValue.Text = "";
            }
            CheckBox_Checked(sender, e);
        }

        private void PaymentValueValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GetValueOfValidation(paymentValueValue) == false && paymentValueValue.Text !="" && paymentHowMuchMoneyIsMissingValue.Content != null)
            {
                if (Convert.ToDecimal((sender as TextBox).Text) >= Convert.ToDecimal(paymentHowMuchMoneyIsMissingValue.Content.ToString()))
                {
                    paymentWholePaymentValue.IsChecked = true;
                    paymentPrepaymentValue.IsChecked = false;
                    (sender as TextBox).Text = paymentHowMuchMoneyIsMissingValue.Content.ToString();
                }
                else
                {
                    paymentWholePaymentValue.IsChecked = false;
                    paymentPrepaymentValue.IsChecked = true;
                }
            }

            TextChangedEventHandler(sender, e);
        }

        private void PaymentWhichReservationValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int keyReservation = 0;
            string whichOperation_ComboBox_Name = (whichOperation_ComboBox.SelectedItem as ComboBoxItem).Content as string;

            if (paymentWhichReservationValue.SelectedIndex != -1)
            {
                while (keyReservation == 0)
                {
                    for (int i = 0; i < tempValueComboBoxpaymentWhichReservation.Length / 2; i++)
                    {
                        if (paymentWhichReservationValue.SelectedValue.ToString() == tempValueComboBoxpaymentWhichReservation[i, 1])
                        {
                            keyReservation = Convert.ToInt32(tempValueComboBoxpaymentWhichReservation[i, 0]);
                        }
                    }
                }
                if (whichOperation_ComboBox_Name == "Add")
                {
                    MissingMoney m = new MissingMoney(dtReservation, dtPayment, keyReservation, null);
                    paymentHowMuchMoneyIsMissingValue.Content = m.HowMuchIsMissing().ToString().Replace('.', ',');
                }
                else if (whichOperation_ComboBox_Name == "Modify")
                {
                    int keyPayment = 0;
                    if (whichRecord_comboBox.SelectedIndex != -1)
                    {
                        string whichRecord_comboBox_Value = whichRecord_comboBox.SelectedValue.ToString();
                        while (keyPayment == 0)
                        {
                            for (int i = 0; i < tempValueComboBoxpayment.Length / 2; i++)
                            {
                                if (whichRecord_comboBox_Value == tempValueComboBoxpayment[i, 1])
                                {
                                    keyPayment = Convert.ToInt32(tempValueComboBoxpayment[i, 0]);
                                }
                            }
                        }
                        MissingMoney m = new MissingMoney(dtReservation, dtPayment, keyReservation, keyPayment);
                        paymentHowMuchMoneyIsMissingValue.Content = m.HowMuchIsMissing().ToString().Replace('.', ',');
                    }
                }
            }
            
            EnableModifyButton();
        }

        private void WhichRecord_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex != -1)
            {
                string whichRecord_comboBox_Name = ((sender as ComboBox).SelectedValue.ToString());
                string whichDataSet_ComboBox_Name = (whichDataSet_ComboBox.SelectedItem as ComboBoxItem).Content as string;
                int key = -1;
                int index1 = 0;
                int index2 = 0;
                switch (whichDataSet_ComboBox_Name)
                {
                    case "Place":

                        for (int i = 0; i < tempValueComboBoxreservationPlace.Length / 2; i++)
                        {
                            if (tempValueComboBoxreservationPlace[i, 1] == whichRecord_comboBox_Name)
                            {
                                key = Convert.ToInt32(tempValueComboBoxreservationPlace[i, 0]);
                            }
                        }

                        

                        for (int i = 0; i < dtPlace.Rows.Count; i++)
                        {
                            if (dtPlace.Rows[i].Field<int>(0) == key)
                            {
                                placeDescriptionValue.Text = dtPlace.Rows[i].Field<string>(1);
                                index1 = typeOfPlaceDescriptionValue.Items.IndexOf(dtPlace.Rows[i].Field<string>(3));
                                typeOfPlaceDescriptionValue.SelectedIndex = index1;
                                numberOfBedsNumericUoDown.Value = dtPlace.Rows[i].Field<int?>(4);
                            }
                        }

                        break;
                    case "Guest":

                        for (int i = 0; i < tempValueComboBoxreservationGuest.Length / 2; i++)
                        {
                            if (tempValueComboBoxreservationGuest[i, 1] == whichRecord_comboBox_Name)
                            {
                                key = Convert.ToInt32(tempValueComboBoxreservationGuest[i, 0]);
                            }
                        }
                       
                        for (int i = 0; i < dtGuest.Rows.Count; i++)
                        {
                            if (dtGuest.Rows[i].Field<int>(0) == key)
                            {
                                guestFirstNameValue.Text = dtGuest.Rows[i].Field<string>(1);
                                guestLastNameValue.Text = dtGuest.Rows[i].Field<string>(2);
                                guestPhoneNumberValue.Text = dtGuest.Rows[i].Field<int>(3).ToString();
                                guestAdressValue.Text = dtGuest.Rows[i].Field<string>(4);
                                guestEmailValue.Text = dtGuest.Rows[i].Field<string>(5);
                            }
                        }

                        break;
                    case "Reservation":

                        for (int i = 0; i < tempValueComboBoxpaymentWhichReservation.Length / 2; i++)
                        {
                            if (tempValueComboBoxpaymentWhichReservation[i, 1] == whichRecord_comboBox_Name)
                            {
                                key = Convert.ToInt32(tempValueComboBoxpaymentWhichReservation[i, 0]);
                            }
                        }
                   
                        for (int i = 0; i < dtReservation.Rows.Count; i++)
                        {
                            if (dtReservation.Rows[i].Field<int>(0) == key)
                            {
                                reservationDateFromValue.SelectedDate = dtReservation.Rows[i].Field<DateTime>(1);
                                reservationDateToValue.SelectedDate = dtReservation.Rows[i].Field<DateTime>(2);
                                index1 = reservationGuestValue.Items.IndexOf(dtReservation.Rows[i].Field<string>(7) + " " + dtReservation.Rows[i].Field<string>(8) + " " +  dtReservation.Rows[i].Field<int>(9).ToString());
                                reservationGuestValue.SelectedIndex = index1;
                                index2 = reservationPlaceValue.Items.IndexOf(dtReservation.Rows[i].Field<string>(10) + " " + dtReservation.Rows[i].Field<string>(11));
                                reservationPlaceValue.SelectedIndex = index2;
                                reservationPriceForDayValue.Text = dtReservation.Rows[i].Field<decimal>(5).ToString();
                                numberOfAdultsNumericUpDown.Value = dtReservation.Rows[i].Field<int?>(13);
                                numberOfChildrenNumericUpDown.Value = dtReservation.Rows[i].Field<int?>(14);
                                reservationDescriptionValue.Text = dtReservation.Rows[i].Field<string>(15);
                            }
                        }
                        break;
                    case "Payment":
                        for (int i = 0; i < tempValueComboBoxpayment.Length / 2; i++)
                        {
                            if (tempValueComboBoxpayment[i, 1] == whichRecord_comboBox_Name)
                            {
                                key = Convert.ToInt32(tempValueComboBoxpayment[i, 0]);
                            }
                        }
                        // string sqlPayment = "Select pay.idPayment 0 , pay.idReservation1, pay.datePayment2, pay.value3, pay.wholePayment4, 
                        //pay.prepayment5, res.dateFrom6,res.dateTo7, plat.description8, pla.description9, gu.firstName10,gu.lastName11,gu.phoneNumber12  FROM payment pay " +

                        for (int i = 0; i < dtPayment.Rows.Count; i++)
                        {
                            if (dtPayment.Rows[i].Field<int>(0) == key)
                            {
                                index1 = paymentWhichReservationValue.Items.IndexOf(dtPayment.Rows[i].Field<string>(8) + " " + dtPayment.Rows[i].Field<string>(9) + " " + dtPayment.Rows[i].Field<int>(10).ToString() + " " + dtPayment.Rows[i].Field<DateTime>(4).ToShortDateString() + " " + dtPayment.Rows[i].Field<DateTime>(5).ToShortDateString() + " " + dtPayment.Rows[i].Field<string>(6) + " " + dtPayment.Rows[i].Field<string>(7));
                                paymentWhichReservationValue.SelectedIndex = index1;
                                paymentDateValue.SelectedDate = dtPayment.Rows[i].Field<DateTime>(2);
                                paymentValueValue.Text = dtPayment.Rows[i].Field<decimal>(3).ToString();
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void EnableModifyButton()
        {
            if (whichOperation_ComboBox.SelectedIndex != -1)
            {
                if ((whichOperation_ComboBox.SelectedItem as ComboBoxItem).Content.ToString() != "Add")
                {
                    if (CheckIfValueToAddIsNotEmptyOrValidationHasError() == true && whichRecord_comboBox.SelectedIndex != -1)
                    {
                        executeModify_button.IsEnabled = true;
                    }
                    else
                    {
                        executeModify_button.IsEnabled = false;
                    }
                }
                else
                {
                    if (CheckIfValueToAddIsNotEmptyOrValidationHasError() == true)
                    {
                        executeModify_button.IsEnabled = true;
                    }
                    else
                    {
                        executeModify_button.IsEnabled = false;
                    }
                }
            }
        }

        private void DisableField()
        {
            typeOfPlaceDescriptionValue.IsEnabled = false;
            placeDescriptionValue.IsEnabled = false;
            guestAdressValue.IsEnabled = false;
            guestEmailValue.IsEnabled = false;
            guestFirstNameValue.IsEnabled = false;
            guestLastNameValue.IsEnabled = false;
            guestPhoneNumberValue.IsEnabled = false;
            reservationDateFromValue.IsEnabled = false;
            reservationDateToValue.IsEnabled = false;
            reservationGuestValue.IsEnabled = false;
            reservationPlaceValue.IsEnabled = false;
            reservationPriceForDayValue.IsEnabled = false;
            paymentDateValue.IsEnabled = false;
            paymentPrepaymentValue.IsEnabled = false;
            paymentWholePaymentValue.IsEnabled = false;
            paymentValueValue.IsEnabled = false;
        }

        private void EnableField()
        {
            typeOfPlaceDescriptionValue.IsEnabled = true;
            placeDescriptionValue.IsEnabled = true;
            guestAdressValue.IsEnabled = true;
            guestEmailValue.IsEnabled = true;
            guestFirstNameValue.IsEnabled = true;
            guestLastNameValue.IsEnabled = true;
            guestPhoneNumberValue.IsEnabled = true;
            reservationDateFromValue.IsEnabled = true;
            reservationDateToValue.IsEnabled = true;
            reservationGuestValue.IsEnabled = true;
            reservationPlaceValue.IsEnabled = true;
            reservationPriceForDayValue.IsEnabled = true;
            paymentDateValue.IsEnabled = true;
            paymentPrepaymentValue.IsEnabled = true;
            paymentWholePaymentValue.IsEnabled = true;
            paymentValueValue.IsEnabled = true;
        }
        private void CleanField()
        {
            typeOfPlaceDescriptionValue.SelectedIndex = -1;
            placeDescriptionValue.Text = "";
            guestAdressValue.Text = "";
            guestEmailValue.Text = "";
            guestFirstNameValue.Text = "";
            guestLastNameValue.Text = "";
            guestPhoneNumberValue.Text = "";
            numberOfBedsNumericUoDown.Value = 1;
            numberOfChildrenNumericUpDown.Value = 0;
            numberOfAdultsNumericUpDown.Value = 1;
            reservationDateFromValue.SelectedDate = null;
            reservationDateToValue.SelectedDate = null;
            reservationGuestValue.SelectedIndex = -1;
            reservationPlaceValue.SelectedIndex = -1;
            reservationPriceForDayValue.Text = "";
            paymentDateValue.SelectedDate = null;
            paymentPrepaymentValue.IsChecked = false;
            paymentWholePaymentValue.IsChecked = false;
            paymentValueValue.Text = "";
            paymentWhichReservationValue.SelectedIndex = -1;
            addDataPlace.Visibility = Visibility.Hidden;
            addDataReservation.Visibility = Visibility.Hidden;
            addDataGuest.Visibility = Visibility.Hidden;
            addDataPayment.Visibility = Visibility.Hidden;
        }
        private void WhichDataRecord_ImportData()
        {
            string myCase = (whichDataSet_ComboBox.SelectedItem as ComboBoxItem).Content as string;

            switch (myCase)
            {
                case "Place":
                    whichRecord_comboBox.Items.Clear();
                    WriteToComboBox(whichRecord_comboBox, tempValueComboBoxreservationPlace, dtPlace.Rows.Count);
                    break;
                case "Guest":
                    whichRecord_comboBox.Items.Clear();
                    WriteToComboBox(whichRecord_comboBox, tempValueComboBoxreservationGuest, dtGuest.Rows.Count);
                    break;
                case "Reservation":
                    whichRecord_comboBox.Items.Clear();
                    WriteToComboBox(whichRecord_comboBox, tempValueComboBoxpaymentWhichReservation, dtReservation.Rows.Count);
                    break;
                case "Payment":
                    whichRecord_comboBox.Items.Clear();
                    WriteToComboBox(whichRecord_comboBox, tempValueComboBoxpayment, dtPayment.Rows.Count);
                    break;

                default:
                    break;
            }

        }

        private void WhichDataSet_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex != -1)
            {
                string whichDataSet_ComboBox_Name = ((sender as ComboBox).SelectedValue as ComboBoxItem).Content as string;
                string whichOperation_ComboBox_Name = (whichOperation_ComboBox.SelectedItem as ComboBoxItem).Content as string;

                switch (whichDataSet_ComboBox_Name)
                {
                    case "Place":
                        EnableModifyButton();
                        addDataPlace.Visibility = Visibility.Visible;
                        addDataGuest.Visibility = Visibility.Hidden;
                        addDataReservation.Visibility = Visibility.Hidden;
                        addDataPayment.Visibility = Visibility.Hidden;
                        if (whichOperation_ComboBox_Name == "Modify" || whichOperation_ComboBox_Name == "Delete")
                        {
                            whichRecord_comboBox.IsEnabled = true;
                            WhichDataRecord_ImportData();
                        }
                        break;
                    case "Guest":
                        EnableModifyButton();
                        addDataPlace.Visibility = Visibility.Hidden;
                        addDataGuest.Visibility = Visibility.Visible;
                        addDataReservation.Visibility = Visibility.Hidden;
                        addDataPayment.Visibility = Visibility.Hidden;
                        if (whichOperation_ComboBox_Name == "Modify" || whichOperation_ComboBox_Name == "Delete")
                        {
                            whichRecord_comboBox.IsEnabled = true;
                            WhichDataRecord_ImportData();
                        }
                        break;
                    case "Reservation":
                        EnableModifyButton();
                        addDataPlace.Visibility = Visibility.Hidden;
                        addDataGuest.Visibility = Visibility.Hidden;
                        addDataReservation.Visibility = Visibility.Visible;
                        addDataPayment.Visibility = Visibility.Hidden;
                        if (whichOperation_ComboBox_Name == "Modify" || whichOperation_ComboBox_Name == "Delete")
                        {
                            whichRecord_comboBox.IsEnabled = true;
                            WhichDataRecord_ImportData();
                        }
                        break;
                    case "Payment":
                        EnableModifyButton();
                        addDataPlace.Visibility = Visibility.Hidden;
                        addDataGuest.Visibility = Visibility.Hidden;
                        addDataReservation.Visibility = Visibility.Hidden;
                        addDataPayment.Visibility = Visibility.Visible;
                        if (whichOperation_ComboBox_Name == "Modify" || whichOperation_ComboBox_Name == "Delete")
                        {
                            WhichDataRecord_ImportData();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private List<string> ForEachLoop(DataTable datatable, string nameOfRow)
        {
            List<string> strings = new List<string>();
            foreach (DataRow drCurrent in datatable.Rows)
            {
                strings.Add(drCurrent[nameOfRow].ToString());
            }
            return strings;  // do usuniecia
        }

        private void WriteToComboBox(ComboBox comboBox ,string [,] tab , int count)
        {
            comboBox.Items.Clear();
            for (int i = 0; i < count; i++)
            {
                comboBox.Items.Add(tab[i, 1]);
            }
        }

        private void ValueOfComboBox()
        {
            ComboBox comboBox;

            int count = 0;
            comboBox = typeOfPlaceDescriptionValue;
            count = dtType.Rows.Count;
                    tempValueComboBoxtypeOfPlaceDes = new string[count, 2];
                    
                    for (int i = 0; i < count; i++)
                    {
                        tempValueComboBoxtypeOfPlaceDes[i, 0] = dtType.Rows[i].Field<int>("idPlaceType").ToString();
                        tempValueComboBoxtypeOfPlaceDes[i, 1] = dtType.Rows[i].Field<string>("descriptionPlaceType");
                    }

                    WriteToComboBox(comboBox, tempValueComboBoxtypeOfPlaceDes, count);
                  
                    comboBox = reservationGuestValue;
                    count = dtGuest.Rows.Count;
                    tempValueComboBoxreservationGuest = new string[count, 2];

                    for (int i = 0; i < count; i++)
                    {
                            tempValueComboBoxreservationGuest[i, 0] = dtGuest.Rows[i].Field<int>(0).ToString();
                            tempValueComboBoxreservationGuest[i, 1] = dtGuest.Rows[i].Field<string>("firstName") + " " + dtGuest.Rows[i].Field<string>("lastName") + " " + dtGuest.Rows[i].Field<int>("phoneNumber").ToString();
                    }

                    WriteToComboBox(comboBox, tempValueComboBoxreservationGuest, count);
                 
                    comboBox = reservationPlaceValue;
                    count = dtPlace.Rows.Count;
                    tempValueComboBoxreservationPlace = new string[count, 2];
                    
                    for (int i = 0; i < count; i++)
                    {
                        tempValueComboBoxreservationPlace[i, 0] = dtPlace.Rows[i].Field<int>(0).ToString();
                        tempValueComboBoxreservationPlace[i, 1] = dtPlace.Rows[i].Field<string>(3) + " " + dtPlace.Rows[i].Field<string>(1);
                    }

                    WriteToComboBox(comboBox, tempValueComboBoxreservationPlace, count);
                  
                    comboBox = paymentWhichReservationValue;
                    count = dtReservation.Rows.Count;
                    tempValueComboBoxpaymentWhichReservation = new string[count, 2];

                    for (int i = 0; i < count; i++)
                    {
                         tempValueComboBoxpaymentWhichReservation[i, 0] = dtReservation.Rows[i].Field<int>(0).ToString();
                         tempValueComboBoxpaymentWhichReservation[i, 1] = dtReservation.Rows[i].Field<string>(7) + " " + dtReservation.Rows[i].Field<string>(8) + " " + dtReservation.Rows[i].Field<int>(9).ToString() + " " +
                         dtReservation.Rows[i].Field<DateTime>(1).ToShortDateString() + " " + dtReservation.Rows[i].Field<DateTime>(2).ToShortDateString() + " " + dtReservation.Rows[i].Field<string>(10) + " " + dtReservation.Rows[i].Field<string>(11);
                    }
                    WriteToComboBox(comboBox, tempValueComboBoxpaymentWhichReservation, count);
                   
                   
                    count = dtPayment.Rows.Count;
                    tempValueComboBoxpayment = new string[count, 2];
            //string sqlPayment = "Select pay.idPayment, pay.idReservation, pay.datePayment, pay.value, pay.wholePayment, pay.prepayment, res.dateFrom,res.dateTo, plat.description, pla.description, gu.firstName,gu.lastName,gu.phoneNumber  FROM payment pay " +

            for (int i = 0; i < count; i++)
                    {
                        tempValueComboBoxpayment[i, 0] = dtPayment.Rows[i].Field<int>(0).ToString();
                        tempValueComboBoxpayment[i, 1] = dtPayment.Rows[i].Field<string>(6) + " " + dtPayment.Rows[i].Field<string>(7) + " " + dtPayment.Rows[i].Field<string>(8) +
                            " " + dtPayment.Rows[i].Field<string>(9) + " " + dtPayment.Rows[i].Field<int>(10).ToString() + " " + dtPayment.Rows[i].Field<DateTime>(2).ToShortDateString() + " " + dtPayment.Rows[i].Field<decimal>(3).ToString();
                    }
                
        }

        private void DateToShow(object sender, int howManyDays)
        {
            (sender as DatePicker).DisplayDateStart = DateTime.Today.AddDays(howManyDays);
        }

        private void WhichOperation_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex != -1)
            {
                string whichOperation_ComboBox_Name = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
                string whichDataSet_ComboBox_Name = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;

                switch (whichOperation_ComboBox_Name)
                {
                    case "Add":
                        whichDataSet_ComboBox.IsEnabled = true;
                        whichRecord_comboBox.IsEnabled = false;
                        whichDataSet_ComboBox.SelectedIndex = -1;
                        whichRecord_comboBox.SelectedIndex = -1;
                        EnableField();
                        CleanField();
                        break;
                    case "Modify":
                        whichDataSet_ComboBox.IsEnabled = true;
                        executeModify_button.IsEnabled = false;
                        whichDataSet_ComboBox.SelectedIndex = -1;
                        whichRecord_comboBox.SelectedIndex = -1;
                        whichRecord_comboBox.IsEnabled = false;
                        EnableField();
                        CleanField();
                        break;
                    case "Delete":
                        whichDataSet_ComboBox.IsEnabled = true;
                        executeModify_button.IsEnabled = false;
                        whichDataSet_ComboBox.SelectedIndex = -1;
                        whichRecord_comboBox.SelectedIndex = -1;
                        whichRecord_comboBox.IsEnabled = false;
                        DisableField();
                        CleanField();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                whichDataSet_ComboBox.IsEnabled = false;
                whichRecord_comboBox.IsEnabled = false;
                whichDataSet_ComboBox.SelectedIndex = -1;
                whichRecord_comboBox.SelectedIndex = -1;
                EnableField();
                CleanField();
            }
        }

        private void ExecuteModify_button_Click(object sender, RoutedEventArgs e)
        {
            string whichOperation = (whichOperation_ComboBox.SelectedItem as ComboBoxItem).Content as string;
            
            switch (whichOperation)
            {
                case "Add":
                    ChargeDataToAddToDataBase();
                    break;
                   
                case "Modify":
                    ChargeDataToModifyToDataBase();
                    break;

                case "Delete":
                    DeleteDataFromDataBase();
                    break;

                default:
                    break;
            }
        }

        private bool CheckIfRecordIsNotDataBase()
        {
            string whichDataSet = (whichDataSet_ComboBox.SelectedItem as ComboBoxItem).Content as string;
            bool isSame = true;
            switch (whichDataSet)
            {
                case "Place":

                    for (int i = 0; i < dtPlace.Rows.Count; i++)
                    {
                        if (placeDescriptionValue.Text.Trim() == dtPlace.Rows[i].Field<string>(1).Trim() && typeOfPlaceDescriptionValue.SelectedItem.ToString() == dtPlace.Rows[i].Field<string>(3) &&
                            numberOfBedsNumericUoDown.Value == dtPlace.Rows[i].Field<int?>(4))
                        {
                            isSame = false;
                        }
                    }
                    return isSame;

                case "Guest":

                    for (int i = 0; i < dtGuest.Rows.Count; i++)
                    {
                        if (guestFirstNameValue.Text.Trim() == dtGuest.Rows[i].Field<string>(1).Trim() && guestLastNameValue.Text.Trim() == dtGuest.Rows[i].Field<string>(2).Trim() && guestPhoneNumberValue.Text.Trim() == dtGuest.Rows[i].Field<int>(3).ToString().Trim())
                        {
                            isSame = false;
                        }
                    }
                    return isSame;

                case "Reservation":

                    for (int i = 0; i < dtReservation.Rows.Count; i++)
                    {
                        if (!ReservationDateValidation())
                        {
                            isSame = false;
                        }
                    }
                    return isSame;

                default:
                    return isSame;
            }
        }

        private void ChargeDataToAddToDataBase()
        {
            string whichDataSet = (whichDataSet_ComboBox.SelectedItem as ComboBoxItem).Content as string;
            Operation operation = Operation.Add;
            string sqlCommand;
            
            string value1;
            string value2;
            string value3;
            string value4;
            string value5;
            string value6;
            string key1 = "";
            string key2 = "";
            string dateToSql1;
            string dateToSql2;
            DateTime dateTime1;
            DateTime dateTime2;

            switch (whichDataSet)
            {
                case "Place":

                    value1 = placeDescriptionValue.Text.Trim();
                    value2 = typeOfPlaceDescriptionValue.SelectedValue.ToString();
                    value3 = numberOfBedsNumericUoDown.Value.ToString().Trim() ;
                    for (int i = 0; i < tempValueComboBoxtypeOfPlaceDes.Length / 2; i++)
                    {
                        if (value2 == tempValueComboBoxtypeOfPlaceDes[i, 1].Trim())
                        {
                            key1 = tempValueComboBoxtypeOfPlaceDes[i, 0];
                            break;
                        }
                    }
                    sqlCommand = ($"INSERT INTO place (idPlaceType, descriptionPlace, idUser, numberOfBeds) VALUES" +
                        $" ({key1},'{value1}',{idUser}, {value3})");
                    if (CheckIfRecordIsNotDataBase())
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    else
                    {
                        MessageBox.Show("This record exists in the database.");
                    }
                    break;

                case "Guest":

                    value1 = guestFirstNameValue.Text.Trim();
                    value2 = guestLastNameValue.Text.Trim();
                    value3 = guestPhoneNumberValue.Text.Trim();
                    value4 = guestAdressValue.Text.Trim();
                    value5 = guestEmailValue.Text.Trim();

                    sqlCommand = ($"INSERT INTO guest (firstName, lastName, phoneNumber, address, email, idUser) VALUES" +
                        $" ('{value1}', '{value2}', '{value3}', '{value4}', '{value5}', {idUser})");

                    if (CheckIfRecordIsNotDataBase())
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    else
                    {
                        MessageBox.Show("This record exists in the database.");
                    }

                    break;

                case "Reservation":
                    
                    value1 = reservationPriceForDayValue.Text.Trim();
                    value2 = reservationGuestValue.SelectedValue.ToString().Trim();
                    value3 = reservationPlaceValue.SelectedValue.ToString().Trim();
                    dateTime1 = (DateTime)reservationDateFromValue.SelectedDate;
                    dateTime2 = (DateTime)reservationDateToValue.SelectedDate;
                    dateToSql1 = dateTime1.Year.ToString() + "-" + dateTime1.Month.ToString("00") + "-" + dateTime1.Day.ToString("00");
                    dateToSql2 = dateTime2.Year.ToString() + "-" + dateTime2.Month.ToString("00") + "-" + dateTime2.Day.ToString("00");
                    value4 = numberOfAdultsNumericUpDown.Value.ToString();
                    value5 = numberOfChildrenNumericUpDown.Value.ToString();
                    value6 = reservationDescriptionValue.Text.Trim();

                    for (int i = 0; i < tempValueComboBoxreservationGuest.Length / 2; i++)
                    {
                        if (value2 == tempValueComboBoxreservationGuest[i, 1].Trim())
                        {
                            key1 = tempValueComboBoxreservationGuest[i, 0];
                            break;
                        }
                    }

                    for (int i = 0; i < tempValueComboBoxreservationPlace.Length / 2; i++)
                    {
                        if (value3 == tempValueComboBoxreservationPlace[i, 1].Trim())
                        {
                             key2 = tempValueComboBoxreservationPlace[i, 0];
                            break;
                        }
                    }

                    sqlCommand = ($"INSERT INTO reservation (dateFrom,dateTo,idGuest,idPlace,priceForDay,numberOfAdults,numberOfChildren,descriptionReservation) VALUES" +
                        $" ('{dateToSql1}', '{dateToSql2}', {key1}, {key2}, {value1.Replace(',','.')}, {value4}, {value5}, '{value6}')");
                    if (CheckIfRecordIsNotDataBase())
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    else
                    {
                        MessageBox.Show("The date is already taken or an incorrect date has been entered.");
                    }

                    break;

                case "Payment":

                    value1 = paymentWhichReservationValue.SelectedItem.ToString();
                    for (int i = 0; i < tempValueComboBoxpaymentWhichReservation.Length/2 ; i++)
                    {
                        if (tempValueComboBoxpaymentWhichReservation[i,1].Trim() == value1.Trim())
                        {
                            key1 = tempValueComboBoxpaymentWhichReservation[i, 0];
                        }
                    }
                    dateTime1 = (DateTime)paymentDateValue.SelectedDate;
                    dateToSql1 = dateTime1.Year.ToString() + "-" + dateTime1.Month.ToString("00") + "-" + dateTime1.Day.ToString("00");
                    value2 = paymentValueValue.Text;
                   

                    sqlCommand = ($"INSERT INTO payment (idReservation,datePayment,value) VALUES" +
                         $" ({key1},'{dateToSql1}',{value2.Replace(',', '.')})");
                    if (IsCorrectAmountOfMoney())
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    else
                    {
                        MessageBox.Show("Bad ammount of money.");
                    }

                    break;

                default:
                    break;
            }
        }

        private void ChargeDataToModifyToDataBase()
        {
            string whichDataSet = (whichDataSet_ComboBox.SelectedItem as ComboBoxItem).Content as string;
            string whichRecord_comboBox_Value = whichRecord_comboBox.SelectedValue.ToString();
            Operation operation = Operation.Modify;
            string sqlCommand;

            string value1;
            string value2;
            string value3;
            string value4;
            string value5;
            string value6;
            string dateToSql1;
            string dateToSql2;
            string key1 = "";
            string key2 = "";
            string keyOfRecord = "";
            DateTime dateTime1;
            DateTime dateTime2;

            switch (whichDataSet)
            {
                case "Place":

                    value1 = placeDescriptionValue.Text.Trim();
                    value2 = typeOfPlaceDescriptionValue.SelectedValue.ToString();
                    value3 = numberOfBedsNumericUoDown.Value.ToString();

                    while (key1 == "")
                    {
                        for (int i = 0; i < tempValueComboBoxtypeOfPlaceDes.Length / 2; i++)
                        {
                            if (value2 == tempValueComboBoxtypeOfPlaceDes[i, 1].Trim())
                            {
                                key1 = tempValueComboBoxtypeOfPlaceDes[i, 0];
                            }
                        }
                    }
                    while (keyOfRecord == "")
                    {
                        for (int i = 0; i < tempValueComboBoxreservationPlace.Length / 2; i++)
                        {
                            if (whichRecord_comboBox_Value == tempValueComboBoxreservationPlace[i, 1].Trim())
                            {
                                keyOfRecord = tempValueComboBoxreservationPlace[i, 0];
                            }
                        }
                    }
                    //  sqlCommand = ($"UPDATE place (idPlaceType, description, idUser) VALUES" +
                    //    $" ({key1},'{value1}',{idUser})");
                    sqlCommand = ($"UPDATE place SET idPlaceType={key1}, descriptionPlace='{value1}', idUser={key1}, numberOfBeds={value3} WHERE idPlace={keyOfRecord}");
                    if (CheckIfRecordIsNotDataBase())
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    else
                    {
                        MessageBox.Show("This record exists in the database.");
                    }
                    break;

                case "Guest":

                    value1 = guestFirstNameValue.Text;
                    value2 = guestLastNameValue.Text;
                    value3 = guestPhoneNumberValue.Text;
                    value4 = guestAdressValue.Text;
                    value5 = guestEmailValue.Text;

                    while (keyOfRecord == "")
                    {
                        for (int i = 0; i < tempValueComboBoxreservationGuest.Length / 2; i++)
                        {
                            if (tempValueComboBoxreservationGuest[i,1] == whichRecord_comboBox_Value.Trim())
                            {
                                keyOfRecord = tempValueComboBoxreservationGuest[i, 0];
                            }
                        }
                    }
                    // sqlCommand = ($"INSERT INTO guest (firstName, lastName, phoneNumber, address, email, idUser) VALUES" +
                    //     $" ('{value1}', '{value2}', '{value3}', '{value4}', '{value5}', {idUser})");

                    sqlCommand = ($"UPDATE guest SET firstName='{value1}', lastName='{value2}', phoneNumber={value3}, address='{value4}', email='{value5}', idUser={idUser} WHERE idGuest={keyOfRecord}"); 
                    if (CheckIfRecordIsNotDataBase())
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    else
                    {
                        MessageBox.Show("This record exists in the database.");
                    }

                    break;

                case "Reservation":

                    value1 = reservationPriceForDayValue.Text.Trim(); ;
                    value2 = reservationGuestValue.SelectedValue.ToString();
                    value3 = reservationPlaceValue.SelectedValue.ToString();
                    dateTime1 = (DateTime)reservationDateFromValue.SelectedDate;
                    dateTime2 = (DateTime)reservationDateToValue.SelectedDate;
                    dateToSql1 = dateTime1.Year.ToString() + "-" + dateTime1.Month.ToString("00") + "-" + dateTime1.Day.ToString("00");
                    dateToSql2 = dateTime2.Year.ToString() + "-" + dateTime2.Month.ToString("00") + "-" + dateTime2.Day.ToString("00");
                    value4 = numberOfAdultsNumericUpDown.Value.ToString();
                    value5 = numberOfChildrenNumericUpDown.Value.ToString();
                    value6 = reservationDescriptionValue.Text;

                    while (key1 == "")
                    {
                        for (int i = 0; i < tempValueComboBoxreservationGuest.Length / 2; i++)
                        {
                            if (value2 == tempValueComboBoxreservationGuest[i, 1].Trim())
                            {
                                key1 = tempValueComboBoxreservationGuest[i, 0];
                            }
                        }
                    }
                    while (key2 == "")
                    {
                        for (int i = 0; i < tempValueComboBoxreservationPlace.Length / 2; i++)
                        {
                            if (value3 == tempValueComboBoxreservationPlace[i, 1].Trim())
                            {
                                key2 = tempValueComboBoxreservationPlace[i, 0];
                                
                            }
                        }
                    }
                    while (keyOfRecord == "")
                    {
                        for (int i = 0; i < tempValueComboBoxpaymentWhichReservation.Length / 2; i++)
                        {
                            if (whichRecord_comboBox_Value == tempValueComboBoxpaymentWhichReservation[i, 1].Trim())
                            {
                                keyOfRecord = tempValueComboBoxpaymentWhichReservation[i, 0];
                            }
                        }
                    }
                    sqlCommand = ($"UPDATE reservation SET dateFrom='{dateToSql1}', dateTo='{dateToSql2}', " +
                        $" idGuest={key1}, idPlace={key2}, priceForDay={value1.Replace(',', '.')}, numberOfAdults={value4}, numberOfChildren={value5}, descriptionReservation='{value6}'" +
                        $" WHERE idReservation={keyOfRecord}");
                    if (CheckIfRecordIsNotDataBase())
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    else
                    {
                        MessageBox.Show("The date is already taken or an incorrect date has been entered.");
                    }

                    break;

                case "Payment":

                    value1 = paymentWhichReservationValue.SelectedItem.ToString();
                    while (key1 == "")
                    {
                        for (int i = 0; i < tempValueComboBoxpaymentWhichReservation.Length / 2; i++)
                        {
                            if (tempValueComboBoxpaymentWhichReservation[i, 1] == value1.Trim())
                            {
                                key1 = tempValueComboBoxpaymentWhichReservation[i, 0];
                            }
                        }
                    }
                    dateTime1 = (DateTime)paymentDateValue.SelectedDate;
                    dateToSql1 = dateTime1.Year.ToString() + "-" + dateTime1.Month.ToString("00") + "-" + dateTime1.Day.ToString("00");
                    value2 = paymentValueValue.Text;
                    while (keyOfRecord == "")
                    {
                        for (int i = 0; i < tempValueComboBoxpayment.Length / 2; i++)
                        {
                            if (whichRecord_comboBox_Value == tempValueComboBoxpayment[i,1].Trim())
                            {
                                keyOfRecord = tempValueComboBoxpayment[i, 0];
                            }
                        }
                    }
                    sqlCommand = ($"UPDATE payment SET idReservation='{key1}', datePayment='{dateToSql1}', value='{value2.Replace(',', '.')}'" +
                      $" WHERE idPayment={keyOfRecord}");
                    if (IsCorrectAmountOfMoney())
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    else
                    {
                        MessageBox.Show("Bad ammount of money.");
                    }

                    break;

                default:
                    break;
            }
        }

        private void DeleteDataFromDataBase()
        {
            string whichDataSet = (whichDataSet_ComboBox.SelectedItem as ComboBoxItem).Content as string;
            string whichRecord_comboBox_Value = whichRecord_comboBox.SelectedValue.ToString();
            Operation operation = Operation.Delete;
            string sqlCommand;

            string keyOfRecord = "";
            ConfirmOperation confirmOperation = new ConfirmOperation();
            switch (whichDataSet)
            {
                case "Place":

                    while (keyOfRecord == "")
                    {
                        for (int i = 0; i < tempValueComboBoxreservationPlace.Length / 2; i++)
                        {
                            if (whichRecord_comboBox_Value == tempValueComboBoxreservationPlace[i, 1])
                            {
                                keyOfRecord = tempValueComboBoxreservationPlace[i, 0];
                            }
                        }
                    }
                    
                    sqlCommand = ($"DELETE FROM place WHERE idPlace={keyOfRecord}");

                    
                    confirmOperation.information_TextBlock.Text = ($"Do you want to delete this record: {whichRecord_comboBox_Value} and all connected reservations and payments with this record?");
                    confirmOperation.ShowDialog();
                    if (confirmOperation.wantToDelete == true)
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    
                    break;

                case "Guest":

                    while (keyOfRecord == "")
                    {
                        for (int i = 0; i < tempValueComboBoxreservationGuest.Length / 2; i++)
                        {
                            if (whichRecord_comboBox_Value == tempValueComboBoxreservationGuest[i, 1])
                            {
                                keyOfRecord = tempValueComboBoxreservationGuest[i, 0];
                            }
                        }
                    }
                    sqlCommand = ($"DELETE FROM guest WHERE idGuest={keyOfRecord}");

                    confirmOperation.information_TextBlock.Text = ($"Do you want to delete this record: {whichRecord_comboBox_Value} and all connected reservations and payments with this record?");
                    confirmOperation.ShowDialog();
                    if (confirmOperation.wantToDelete == true)
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    break;

                case "Reservation":

                    while (keyOfRecord == "")
                    {
                        for (int i = 0; i < tempValueComboBoxpaymentWhichReservation.Length / 2; i++)
                        {
                            if (whichRecord_comboBox_Value == tempValueComboBoxpaymentWhichReservation[i, 1])
                            {
                                keyOfRecord = tempValueComboBoxpaymentWhichReservation[i, 0];
                            }
                        }
                    }
                    sqlCommand = ($"DELETE FROM reservation WHERE idReservation={keyOfRecord}");
                    confirmOperation.information_TextBlock.Text = ($"Do you want to delete this record: {whichRecord_comboBox_Value} and all connected payments with this record?");
                    confirmOperation.ShowDialog();
                    if (confirmOperation.wantToDelete == true)
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    break;

                case "Payment":

                    while (keyOfRecord == "")
                    {
                        for (int i = 0; i < tempValueComboBoxpayment.Length / 2; i++)
                        {
                            if (whichRecord_comboBox_Value == tempValueComboBoxpayment[i, 1])
                            {
                                keyOfRecord = tempValueComboBoxpayment[i, 0];
                            }
                        }
                    }
                    sqlCommand = ($"DELETE FROM payment WHERE idPayment={keyOfRecord}");
                    confirmOperation.information_TextBlock.Text = ($"Do you want to delete this record: {whichRecord_comboBox_Value}?");
                    confirmOperation.ShowDialog();
                    if (confirmOperation.wantToDelete == true)
                    {
                        AddToDataBase(sqlCommand, operation);
                    }
                    break;
                default:
                    break;
            }
        }

        private bool ReservationDateValidation()
        {
            DateTime d3 = (DateTime)reservationDateFromValue.SelectedDate;
            DateTime d4 = (DateTime)reservationDateToValue.SelectedDate;

            DateValidation dateValidation = new DateValidation(tempValueComboBoxreservationPlace, d3, d4, dtReservation, reservationPlaceValue.SelectedItem.ToString());

            string whichOperation = (whichOperation_ComboBox.SelectedItem as ComboBoxItem).Content as string;

            switch (whichOperation)
            {
                case "Add":
                    return dateValidation.ReservationDateValidation();
                case "Modify":
                    string whichRecord_comboBox_Name = ((whichRecord_comboBox as ComboBox).SelectedValue.ToString());
                    int key = 0;
                    for (int i = 0; i < tempValueComboBoxpaymentWhichReservation.Length/2; i++)
                    {
                        if (tempValueComboBoxpaymentWhichReservation[i,1] == whichRecord_comboBox_Name)
                        {
                            key = Convert.ToInt32(tempValueComboBoxpaymentWhichReservation[i, 0]);
                        }
                    }

                    return dateValidation.ReservationDateValidation(key);
                   
                default:

                    return true;
            }

            
        }

        private bool IsCorrectAmountOfMoney()
        {
            bool isCorrect = true;

            if (Convert.ToDecimal(paymentValueValue.Text) > Convert.ToDecimal(paymentHowMuchMoneyIsMissingValue.Content))
            {
                isCorrect = false;
            }

            return isCorrect;
        }

        public bool GetValueOfValidation(TextBox textbox)
        {
            var myBindingExpression = textbox.GetBindingExpression(TextBox.TextProperty);
            bool validation = myBindingExpression.HasValidationError;
            return validation;
        }

        private bool CheckIfValueToAddIsNotEmptyOrValidationHasError()
        {
            bool result = false;
            bool validation1;
            bool validation2;
            bool validation3;

            if (whichDataSet_ComboBox.SelectedIndex != -1)
            {
                switch ((whichDataSet_ComboBox.SelectedItem as ComboBoxItem).Content as string)
                {
                    case "Place":

                        validation1 = GetValueOfValidation(placeDescriptionValue);

                        if (placeDescriptionValue.Text != "" && typeOfPlaceDescriptionValue.SelectedIndex != -1 && validation1 == false && numberOfBedsNumericUoDown.Value.HasValue)
                        {
                            result = true;
                        }

                        return result;

                    case "Guest":

                        validation1 = GetValueOfValidation(guestPhoneNumberValue);
                        validation2 = GetValueOfValidation(guestFirstNameValue);
                        validation3 = GetValueOfValidation(guestLastNameValue);

                        if (guestPhoneNumberValue.Text != "" && guestFirstNameValue.Text != "" && guestLastNameValue.Text != "" &&
                            validation1 == false && validation2 == false && validation3 == false)
                        {
                            result = true;
                        }

                        return result;

                    case "Reservation":

                        validation1 = GetValueOfValidation(reservationPriceForDayValue);

                        if (reservationGuestValue.SelectedIndex != -1 && reservationPlaceValue.SelectedIndex != -1 && reservationPriceForDayValue.Text != "" &&
                            validation1 == false && reservationDateFromValue.SelectedDate != null && reservationDateToValue.SelectedDate != null && 
                            ((numberOfAdultsNumericUpDown.Value + numberOfChildrenNumericUpDown.Value) > 0))
                        {
                            result = true;
                        }

                        return result;

                    case "Payment":

                        validation1 = GetValueOfValidation(paymentValueValue);

                        if (paymentValueValue.Text != "" && paymentWhichReservationValue.SelectedIndex != -1 && paymentDateValue.SelectedDate != null && validation1 == false && (paymentPrepaymentValue.IsChecked == true || paymentWholePaymentValue.IsChecked == true))
                        {
                            result = true;
                        }
                        return result;

                    default:
                        return result;
                }
            }
            else
            {
                return result;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /* int key = 0;
             for (int i = 0; i < tempValueComboBoxpaymentWhichReservation.Length / 2; i++)
             {
                 if (paymentWhichReservationValue.SelectedItem.ToString() == tempValueComboBoxpaymentWhichReservation[i, 1])
                 {
                     key = Convert.ToInt32(tempValueComboBoxpaymentWhichReservation[i, 0]);
                 }
             }
             MissingMoney m = new MissingMoney(dtR, dtPa, key, null); ;*/

            //McDataGrid.ItemsSource = dt.DefaultView;

            MessageBox.Show( CheckIfRecordIsNotDataBase().ToString());
        }

      

        private void changeDisplayReservation_ToggleSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (whichTableToDisplay_ComboBox.SelectedIndex == -1)
            {
                List<string> itemsList = whichTableToDisplay_ComboBox.Items
                    .Cast<ComboBoxItem>()
                    .Select(item => item.Content.ToString())
                    .ToList();

                whichTableToDisplay_ComboBox.SelectedIndex = itemsList.FindIndex(s => s.Equals("Reservations"));
            }

            if ((bool)changeDisplayReservation_ToggleSwitchButton.IsChecked)
            {
                reservation_ReoGridcontrol.Visibility = Visibility.Hidden;
                displayTable_DataGrid.Visibility = Visibility.Visible;
                whichTableToDisplay_ComboBox.IsEnabled = true;
                
            }
            else
            {
                reservation_ReoGridcontrol.Visibility = Visibility.Visible;
                displayTable_DataGrid.Visibility = Visibility.Hidden;
                whichTableToDisplay_ComboBox.IsEnabled = false;
            }
        }

        private void whichTableToDisplay_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tableName = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();

            switch (tableName)
            {
                case "Places":
                    displayTable_DataGrid.ItemsSource = dtPlaceToShowInTable.DefaultView;
                    break;
                case "Guests":
                    displayTable_DataGrid.ItemsSource = dtGuestToShowInTable.DefaultView;
                    break;
                case "Reservations":
                    displayTable_DataGrid.ItemsSource = dtReservationToShowInTable.DefaultView;
                    break;
                case "Payments":
                    displayTable_DataGrid.ItemsSource = dtPaymentToShowInTable.DefaultView;
                    break;

                default:
                    break;
            }
        }

        private void SelectDataToShowInTable()
        {
            try
            {
                dtPlaceToShowInTable = dtPlace.Select().CopyToDataTable()
                 .DefaultView.ToTable(false, "descriptionPlaceType", "descriptionPlace", "numberOfBeds");
                dtGuestToShowInTable = dtGuest.Select().CopyToDataTable()
                    .DefaultView.ToTable(false, "firstName", "lastName", "phoneNumber", "address", "email");
                dtReservationToShowInTable = dtReservation.Select().CopyToDataTable()
                    .DefaultView.ToTable(false, "lastName", "firstName", "phoneNumber", "descriptionPlaceType", "descriptionPlace", "dateFrom", "dateTo", "priceForDay", "numberOfAdults", "numberOfChildren", "descriptionReservation");
                dtPaymentToShowInTable = dtPayment.Select().CopyToDataTable()
                    .DefaultView.ToTable(false, "firstName", "lastName", "datePayment", "value", "descriptionPlaceType", "descriptionPlace", "dateFrom", "dateTo");
            }
            catch (Exception)
            {

            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Statistic window = new Statistic(dtPlace, dtReservation);
            window.Show();
            window.CalculateStatistic();
        }
    }
}
