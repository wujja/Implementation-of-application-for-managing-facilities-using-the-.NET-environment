using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace Apkainz
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Statistic : Window
    {
        DataTable dtPlace;
        DataTable dtReservation;
        List<DateTime> dateTimes = new List<DateTime>();
        double percentOfReservations;
        decimal income = 0;
        public Statistic(DataTable dtPlace, DataTable dtReservation)
        {
            InitializeComponent();
            this.dtPlace = dtPlace;
            this.dtReservation = dtReservation;
        }

        public void CalculateStatistic()
        {
            int howManyDaysTotal = 0;
            for (int j = 0; j < dtReservation.Rows.Count; j++)
            {
                DateTime dateFrom = dtReservation.Rows[j].Field<DateTime>(1);
                DateTime dateTo = dtReservation.Rows[j].Field<DateTime>(2);
                int howManyDaysOfReservation = dateTo.DayOfYear - dateFrom.DayOfYear;
                decimal pricePerDay = dtReservation.Rows[j].Field<decimal>(5);
                income += howManyDaysOfReservation * pricePerDay;


                percentOfReservations += howManyDaysOfReservation;
                List<DateTime> dateTimes = new List<DateTime>();
                CheckWhichMonth(dateTimes);
                
                foreach (var item in dateTimes)
                {
                    howManyDaysTotal += System.DateTime.DaysInMonth(item.Year, item.Month) * dateTimes.Count;
                }
                
            }
            percentOfReservations = percentOfReservations / howManyDaysTotal;
            percentageReservedPlace.Content = percentOfReservations.ToString("P", CultureInfo.InvariantCulture);
            incomeValue.Content = income;
        }

        private void CheckWhichMonth(List<DateTime> list)
        {
            list.Clear();
            for (int i = 0; i < dtReservation.Rows.Count; i++)
            {
                CheckMonthOfOneReservation(i, list);
            }
            list.Sort();
        }

        private void CheckMonthOfOneReservation(int idReservation, List<DateTime> list)
        {
            DateTime dateFrom = new DateTime(dtReservation.Rows[idReservation].Field<DateTime>(1).Year, dtReservation.Rows[idReservation].Field<DateTime>(1).Month, 1);
            DateTime dateTo = new DateTime(dtReservation.Rows[idReservation].Field<DateTime>(2).Year, dtReservation.Rows[idReservation].Field<DateTime>(2).Month, 1);

            for (DateTime j = dateFrom; j <= dateTo; j = j.AddMonths(1))
            {
                if (!list.Exists(x => x == j))
                {
                    list.Add(j);
                }
            }
        }
    }
}
