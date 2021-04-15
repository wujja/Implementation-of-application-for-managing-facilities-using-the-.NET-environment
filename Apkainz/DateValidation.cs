using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Apkainz
{
    class DateValidation
    {
        private DateTime d3;
        private DateTime d4;
        private string[,] tab; //tempValueComboBoxreservationPlace
        private string reservationPlaceValue_string; // reservationPlaceValue.SelectedItem.ToString()
        private DataTable dtReservation;

        public string[,] Tab { get => tab; set => tab = value; }
        public DateTime D4 { get => d4; set => d4 = value; }
        public DataTable DtReservation { get => dtReservation; set => dtReservation = value; }
        public string ReservationPlaceValue_string { get => reservationPlaceValue_string; set => reservationPlaceValue_string = value; }
        public DateTime D3 { get => d3; set => d3 = value; }

        public DateValidation(string[,] tab, DateTime d3, DateTime d4, DataTable dtReservation, string reservationPlaceValue_string)
        {
            Tab = tab;
            ReservationPlaceValue_string = reservationPlaceValue_string;
            D3 = d3;
            D4 = d4;
            DtReservation = dtReservation;
        }

        public bool ReservationDateValidation()
        {
            bool flag = true;
            string idPlace = "";

            for (int j = 0; j < Tab.Length / 2; j++)
            {
                if (ReservationPlaceValue_string.Equals(Tab[j, 1]))
                {
                    idPlace = Tab[j, 0];
                    break;
                }
            }

            for (int i = 0; i < DtReservation.Rows.Count; i++)
            {
                string ItemFromBase = DtReservation.Rows[i].Field<int>(12).ToString();

                if (idPlace.Equals(ItemFromBase))
                {
                    if (flag)
                    {
                        if (DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D3) < 0 && DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D4) < 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D4) < 0)
                        {
                            flag = false;
                        }
                        else if (DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D4) < 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D4) > 0)
                        {
                            flag = false;
                        }
                        else if (DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D3) >= 0 && DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D4) < 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D4) <= 0)
                        {
                            flag = false;
                        }
                        else if (DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D3) <= 0 && DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D4) < 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D4) >= 0)
                        {
                            flag = false;
                        }
                    }
                }
            }

            if (D3.AddDays(1) >= D4)
            {
                flag = false;
            }
            return flag;
        }

        public bool ReservationDateValidation(int idReservation)
        {
            bool flag = true;
            string idPlace = "";


            for (int j = 0; j < Tab.Length / 2; j++)
            {
                if (ReservationPlaceValue_string.Equals(Tab[j, 1]))
                {
                    idPlace = Tab[j, 0];
                    break;
                }
            }

            for (int i = 0; i < DtReservation.Rows.Count; i++)
            {
                string ItemFromBase = DtReservation.Rows[i].Field<int>(12).ToString();

                if (idPlace.Equals(ItemFromBase) && !(idReservation.Equals(DtReservation.Rows[i].Field<int>(0))))
                {
                    if (flag)
                    {
                        if (DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D3) < 0 && DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D4) < 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D4) < 0)
                        {
                            flag = false;
                        }
                        else if (DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D4) < 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D4) > 0)
                        {
                            flag = false;
                        }
                        else if (DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D3) >= 0 && DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D4) < 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D4) <= 0)
                        {
                            flag = false;
                        }
                        else if (DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D3) <= 0 && DtReservation.Rows[i].Field<DateTime>(1).CompareTo(D4) < 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D3) > 0 && DtReservation.Rows[i].Field<DateTime>(2).CompareTo(D4) >= 0)
                        {
                            flag = false;
                        }
                    }
                }
            }

            if (D3.AddDays(1) >= D4)
            {
                flag = false;
            }
            return flag;
        }
    }
}