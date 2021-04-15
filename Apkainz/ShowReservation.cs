using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Globalization;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;
using MahApps.Metro.Controls;
using System.Windows;

namespace Apkainz
{

    public class DisplayReservation
    {
        public List<DateTime> listDate = new List<DateTime>();
        public List<int> listIdPlace = new List<int>();
        private ReoGridControl reoGridControl;
        private DataTable dataTableReservation;
        private readonly DataTable dataTablePayment;
        private DataTable dataTablePlace;
        public bool update = false;


        public DisplayReservation(ReoGridControl reoGridControl, DataTable dataTableReservation, DataTable dataTablePayment, DataTable dataTablePlace)
        {

            this.reoGridControl = reoGridControl;
            this.dataTableReservation = dataTableReservation;
            this.dataTablePayment = dataTablePayment;
            this.dataTablePlace = dataTablePlace;


        }

        private void BorderCells(RangePosition range, int k)
        {
            var currnetSheed = reoGridControl.Worksheets[k];
            reoGridControl.DoAction(currnetSheed,
                new SetRangeBorderAction(range,
                BorderPositions.Outside,
                new RangeBorderStyle
                {
                    Color = SolidColor.Black,
                    Style = BorderLineStyle.BoldSolid
                })
                );
        }

        private void MergeCells()
        {
            for (int k = 0; k < listDate.Count; k++)
            {
                DateTime first = listDate[k];
                DateTime end = first.AddMonths(1);
                var currnetSheed = reoGridControl.Worksheets[k];
                int[] firstCellToMerge = new int[2];
                int[] lastCellToMerge = new int[2];
                string valueOfCell = "";
                int firstCell = 0;
                int lastCell = end.AddDays(-1).Day * 2;
                //DateTime i = first; i < end.AddDays(-1); i = i.AddDays(1)
                for (int j = 0; j < listIdPlace.Count; j++)
                {
                    int columnNumber = 0;
                    for (int i = firstCell; i < lastCell; i++)
                    {
                        int lastDay = lastCell;
                        if (currnetSheed.GetCellText(j, columnNumber).ToString() != "" && valueOfCell == "")
                        {
                            valueOfCell = currnetSheed.GetCellText(j, columnNumber).ToString();
                            firstCellToMerge = new int[] { j, columnNumber };
                            if (currnetSheed.GetCellText(j, columnNumber).ToString() != currnetSheed.GetCellText(j, columnNumber + 1).ToString())
                            {
                                lastCellToMerge = new int[] { j, columnNumber };
                                var range = new RangePosition(firstCellToMerge[0], firstCellToMerge[1], 1, lastCellToMerge[1] - firstCellToMerge[1] + 1);
                                currnetSheed.MergeRange(range);
                                BorderCells(range, k);
                                valueOfCell = "";
                            }
                        }
                        if ((currnetSheed.GetCellText(j, columnNumber).ToString() != currnetSheed.GetCellText(j, columnNumber + 1).ToString() &&
                            valueOfCell != ""))
                        {
                            lastCellToMerge = new int[] { j, columnNumber };
                            var range = new RangePosition(firstCellToMerge[0], firstCellToMerge[1], 1, lastCellToMerge[1] - firstCellToMerge[1] + 1);

                            currnetSheed.MergeRange(range);
                            BorderCells(range, k);
                            valueOfCell = "";
                        }
                        if (columnNumber + 2 == lastDay && valueOfCell != "")
                        {
                            lastCellToMerge = new int[] { j, columnNumber + 1 };
                            var range = new RangePosition(firstCellToMerge[0], firstCellToMerge[1], 1, lastCellToMerge[1] - firstCellToMerge[1] + 1);

                            currnetSheed.MergeRange(range);
                            BorderCells(range, k);
                            valueOfCell = "";
                        }
                        columnNumber++;
                    }
                }
            }
        }

        private void BorderCellsBackground(int k, RangePosition range)
        {
            var currnetSheed = reoGridControl.Worksheets[k];
            reoGridControl.DoAction(currnetSheed,
                new SetRangeBorderAction(range,
                BorderPositions.Right,
                new RangeBorderStyle
                {
                    Color = SolidColor.White
                    // Style = BorderLineStyle.None
                })
                );
        }

        private void SetBackgroundCells()
        {
            for (int k = 0; k < listDate.Count; k++)
            {
                var currnetSheed = reoGridControl.Worksheets[k];

                for (int j = 0; j < currnetSheed.ColumnCount; j += 2)
                {
                    BorderCellsBackground(k, new RangePosition(0, j, currnetSheed.RowCount, 1));
                }
            }
        }

        private SolidColor SetColorReservation(int idReservation)
        {
            MissingMoney missingMoney = new MissingMoney(dataTableReservation, dataTablePayment, idReservation, null);
            decimal howMuchisMissing = missingMoney.HowMuchIsMissing();

            if (howMuchisMissing <= 0)
            {
                return SolidColor.LightSkyBlue;
            }
            if (howMuchisMissing > 0 && howMuchisMissing < missingMoney.WholePrice)
            {
                return SolidColor.Yellow;
            }
            if (howMuchisMissing >= missingMoney.WholePrice)
            {
                return SolidColor.IndianRed;
            }
            return SolidColor.Purple;
        }

        private void CheckWhichMonth(List<DateTime> list)
        {
            list.Clear();
            for (int i = 0; i < dataTableReservation.Rows.Count; i++)
            {
                CheckMonthOfOneReservation(i, list);
            }
            list.Sort();
        }

        private void CheckMonthOfOneReservation(int idReservation, List<DateTime> list)
        {
            DateTime dateFrom = new DateTime(dataTableReservation.Rows[idReservation].Field<DateTime>(1).Year, dataTableReservation.Rows[idReservation].Field<DateTime>(1).Month, 1);
            DateTime dateTo = new DateTime(dataTableReservation.Rows[idReservation].Field<DateTime>(2).Year, dataTableReservation.Rows[idReservation].Field<DateTime>(2).Month, 1);

            for (DateTime j = dateFrom; j <= dateTo; j = j.AddMonths(1))
            {
                if (!list.Exists(x => x == j))
                {
                    list.Add(j);
                }
            }
        }


        public void SortPlace()
        {
            DataView dataView = dataTablePlace.DefaultView;
            dataView.Sort = "idPlace asc";
            dataTablePlace = dataView.ToTable();
        }

        public void WorkSheedName(bool Update)
        {
            if (Update == false)
            {
                for (int i = reoGridControl.Worksheets.Count - 1; i >= 0; i--)
                {
                    reoGridControl.RemoveWorksheet(i);
                }

                var sheet = reoGridControl.CurrentWorksheet;
                var sheeds = new List();
                for (int i = 0; i < listDate.Count; i++)
                {
                    string nameWorkSheet = ((listDate[i].Month) + "." + (listDate[i].Year)).ToString();
                    /* if (i == 0)
                     {
                         sheet.Name = nameWorkSheet;
                     }*/
                    //else
                    //{
                    reoGridControl.AddWorksheet(reoGridControl.CreateWorksheet(nameWorkSheet));

                    //}
                }
            }
            else
            {
                int countOfWorkSheed = reoGridControl.Worksheets.Count;
                List<string> sheetsNames = new List<string>();
                sheetsNames.Clear();
                for (int i = 0; i < countOfWorkSheed; i++)
                {
                    sheetsNames.Add(reoGridControl.Worksheets[i].Name);
                }
                for (int i = 0; i < listDate.Count; i++)
                {
                    string nameWorkSheet = ((listDate[i].Month) + "." + (listDate[i].Year)).ToString();
                    if (!sheetsNames.Contains(nameWorkSheet))
                    {
                        reoGridControl.AddWorksheet(reoGridControl.CreateWorksheet(nameWorkSheet));
                    }
                }
            }
        }


        public void ShowReservation2()
        {
            CheckWhichMonth(listDate);
            WorkSheedName(update);
            SortPlace();

            listIdPlace.Clear();

            for (int i = 0; i < dataTablePlace.Rows.Count; i++)
            {
                listIdPlace.Add(dataTablePlace.Rows[i].Field<int>(0));
             
            }
            SetBackgroundCells();

            List<DateTime> reservationMonth = new List<DateTime>();

            for (int i = 0; i < dataTableReservation.Rows.Count; i++)
            {
                SolidColor colorOfReservation = SetColorReservation(dataTableReservation.Rows[i].Field<int>(0));
                reservationMonth.Clear();
                CheckMonthOfOneReservation(i ,reservationMonth);
               
                int rowIndex = listIdPlace.IndexOf(dataTableReservation.Rows[i].Field<int>(4));

                for (int j = 0; j < reservationMonth.Count; j++)
                {
                    DateTime dateFrom = dataTableReservation.Rows[i].Field<DateTime>(1);
                    DateTime dateTo = dataTableReservation.Rows[i].Field<DateTime>(2);
                    int howManyDaysOfReservation = dateTo.DayOfYear - dateFrom.DayOfYear;
                    var currnetSheed = reoGridControl.Worksheets[listDate.IndexOf(reservationMonth.ElementAt(j))];

                    if (dateTo > reservationMonth.ElementAt(j).AddMonths(1).AddDays(-1))
                    {
                        currnetSheed[rowIndex, reservationMonth.ElementAt(j).AddMonths(1).AddDays(-1).Day * 2 - 1] = dataTableReservation.Rows[i].Field<string>(7) + " " + dataTableReservation.Rows[i].Field<string>(8) + " " + dataTableReservation.Rows[i].Field<decimal>(5).ToString();
                        var cell1 = currnetSheed.Cells[rowIndex, reservationMonth.ElementAt(j).AddMonths(1).AddDays(-1).Day * 2 - 1];
                        cell1.Style.BackColor = colorOfReservation;
                    }

                    if (dateFrom < reservationMonth.ElementAt(j))
                    {
                        dateFrom = reservationMonth.ElementAt(j);
                    }
                    if (dateTo > reservationMonth.ElementAt(j).AddMonths(1).AddDays(-1))
                    {
                        dateTo = reservationMonth.ElementAt(j).AddMonths(1).AddDays(-1);
                    }

                    if (dateTo.Day == 1 || dateFrom.Day ==1)
                    {
                        currnetSheed[rowIndex, 0] = dataTableReservation.Rows[i].Field<string>(7) + " " + dataTableReservation.Rows[i].Field<string>(8) + " " + dataTableReservation.Rows[i].Field<decimal>(5).ToString();
                        var cell1 = currnetSheed.Cells[rowIndex, 0];
                        cell1.Style.BackColor = colorOfReservation;
                    }
                   
                    for (DateTime k = dateFrom; k < dateTo; k = k.AddDays(1))
                    {           
                            currnetSheed[rowIndex, k.Day * 2 - 1] = dataTableReservation.Rows[i].Field<string>(7) + " " + dataTableReservation.Rows[i].Field<string>(8) + " " + dataTableReservation.Rows[i].Field<decimal>(5).ToString();
                            var cell1 = currnetSheed.Cells[rowIndex, k.Day * 2 - 1];
                            cell1.Style.BackColor = colorOfReservation;
                            currnetSheed[rowIndex, k.Day * 2 ] = dataTableReservation.Rows[i].Field<string>(7) + " " + dataTableReservation.Rows[i].Field<string>(8) + " " + dataTableReservation.Rows[i].Field<decimal>(5).ToString();
                            var cell2 = currnetSheed.Cells[rowIndex, k.Day * 2 ];
                            cell2.Style.BackColor = colorOfReservation;
                    }
                }
            }

            for (int k = 0; k < listDate.Count; k++)
            {
                int columnNumber = 0;
                DateTime first = listDate[k];
                DateTime end = first.AddMonths(1);
                var currnetSheed = reoGridControl.Worksheets[k];

                for (DateTime i = first; i < end; i = i.AddDays(1))
                {
                    currnetSheed.ColumnHeaders[columnNumber].Text = i.Day.ToString("00") + "." + i.Month.ToString("00");
                    currnetSheed.ColumnHeaders[columnNumber + 1].Text = "";
                    columnNumber += 2;
                }
                currnetSheed.Resize(listIdPlace.Count, columnNumber);
                currnetSheed.SetColumnsWidth(0, columnNumber, 30);
            }

            for (int i = 0; i < dataTablePlace.Rows.Count; i++)
            {
                for (int j = 0; j < listDate.Count; j++)
                {
                    var currnetSheed = reoGridControl.Worksheets[j];
                    currnetSheed.RowHeaders[i].Text = dataTablePlace.Rows[i].Field<string>(1) + " " + dataTablePlace.Rows[i].Field<string>(3);
                    currnetSheed.RowHeaderWidth = 100;
                }
            }
            MergeCells();
        }
    }
}