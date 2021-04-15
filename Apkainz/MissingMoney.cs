using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace Apkainz
{
    class MissingMoney
    {
        public DateTime FirstDayOfReservation { get; set; }
        public DateTime LastDayOfReservation { get; set; }
        public decimal WholePrice { get; set; }
        public decimal PriceForDay { get; set; }
        public decimal PaidSoFar { get; set; }
        public DataTable PaymentDataTable { get; set; }

        public MissingMoney(DataTable reservationDataTable, DataTable paymentDataTable, int idReservation, int? idPayment)
        {
            var res =
                from reservation in reservationDataTable.AsEnumerable()
                where reservation.Field<int>("idReservation") == idReservation
                
                select reservation;

            DataTable dtReservation = res.CopyToDataTable();

            FirstDayOfReservation = dtReservation.Rows[0].Field<DateTime>(1);
            LastDayOfReservation = dtReservation.Rows[0].Field<DateTime>(2);
            PriceForDay = dtReservation.Rows[0].Field<decimal>(5);

            try
            {
                var paym =
                from payment in paymentDataTable.AsEnumerable()
                where payment.Field<int>("idReservation") == idReservation
                select payment;
                DataTable dtPayment = paym.CopyToDataTable();
                this.PaymentDataTable = dtPayment;
                if (idPayment == null)
                {
                    PaidSoFar = HowMuchIsPaidSoFar();
                }
                else
                {
                    PaidSoFar = HowMuchIsPaidSoFar((int)idPayment);
                }       
            }
            catch (Exception)
            {
                PaidSoFar = 0;
            }
            WholePrice = ValueOfWholePrice();
        }

        

        public decimal ValueOfWholePrice()
        {
            int days = (LastDayOfReservation - FirstDayOfReservation).Days;
            decimal value = days * PriceForDay;
            return value;
        }

        public decimal HowMuchIsPaidSoFar()
        {
            decimal value = 0;
            for (int i = 0; i < PaymentDataTable.Rows.Count; i++)
            {
                value = +PaymentDataTable.Rows[i].Field<decimal>(3);
            }
            return value;
        }

        public decimal HowMuchIsPaidSoFar(int idPayment)
        {
            decimal value = 0;
            for (int i = 0; i < PaymentDataTable.Rows.Count; i++)
            {
                if (PaymentDataTable.Rows[i].Field<int>(0) != idPayment)
                {
                    value = +PaymentDataTable.Rows[i].Field<decimal>(3);
                }
            }
            return value;
        }

        public decimal HowMuchIsMissing()
        {
            return WholePrice - PaidSoFar;
        }
    }
}
