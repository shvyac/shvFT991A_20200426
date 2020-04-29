using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace shvFT991A
{
    class PSKDataReader
    {
        List<PSKDataItem.activeReceiver> itemsActiveReceiver = new List<PSKDataItem.activeReceiver>();
        List<PSKDataItem.receptionReport> itemsReceptionReport = new List<PSKDataItem.receptionReport>();
        List<PSKDataItem.activeCallsign> itemsActiveCallsign = new List<PSKDataItem.activeCallsign>();

        public MainWindow mainWindow = (MainWindow)App.Current.MainWindow;

        private DataSet PSKData = new DataSet("NewDataSet");
        private WebClient wcPSK = new WebClient();

        public string xmlSessionPSK = "";
        public string xmlErrorPSK = "";
        public string xmlMessagePSK = "";
        public bool isOnlinePSK = false;
        public string xmlUrlPSK = "https://retrieve.pskreporter.info/query?";

        static readonly HttpClient client = new HttpClient();
        List<string> listCallsign = new List<string>();

        public async void GetPSKStream()
        {
            /*
            https://retrieve.pskreporter.info/query?senderCallsign=requestedcall
            senderCallsign	    Specifies the sending callsign of interest.
            receiverCallsign	Specifies the receiving callsign of interest.
            callsign	        Specifies the callsign of interest. 
                                Specify only one of these three parameters.
            flowStartSeconds    A negative number of seconds to indicate how much data to retreive. 
                                This cannot be more than 24 hours.
            mode	            The mode of the signal that was detected.
            rptlimit	        Limit the number of records returned.
            rronly	            Only return the reception report records if non zero
            noactive	        Do not return the active monitors if non zero
            frange	            A lower and upper limit of frequency. E.g. 14000000-14100000
            nolocator	        If non zero, then include reception reports that do not include a locator.
            callback	        Causes the returned document to be javascript, and it will invoke the function named in the callback.
            statistics	        Includes some statistical information
            modify	            If this has the value 'grid' then the callsign are interpreted as grid squares
            lastseqno	        Limits search to records with a sequence number greater than or equal to this parameter. 
                                The last sequence number in the database is returned on each response.
            */

            mainWindow.DataGridActiveReceiver.ItemsSource = itemsActiveReceiver;
            mainWindow.DataGridReceptionReport.ItemsSource = itemsReceptionReport;
            mainWindow.DataGridActiveCallsign.ItemsSource = itemsActiveCallsign;

            //GetRandomCallSign();
            string searchCall = null;

            if (3 < mainWindow.ComboBoxSenderCallsign.SelectedItem.ToString().Length)//--------------------SenderCallsign
            {
                searchCall = mainWindow.ComboBoxSenderCallsign.SelectedItem.ToString();
                xmlUrlPSK = xmlUrlPSK + "senderCallsign=" + searchCall;
                mainWindow.ComboBoxCallSign.SelectedIndex = 0;
                mainWindow.ComboBoxReceiverCallsign.SelectedIndex = 0;
            }
            if (3 < mainWindow.ComboBoxReceiverCallsign.SelectedItem.ToString().Length)//--------------------ReceiverCallsign
            {
                searchCall = mainWindow.ComboBoxReceiverCallsign.SelectedItem.ToString();
                xmlUrlPSK = xmlUrlPSK + "receiverCallsign=" + searchCall;
                mainWindow.ComboBoxCallSign.SelectedIndex = 0;
                mainWindow.ComboBoxSenderCallsign.SelectedIndex = 0;
            }
            if (3 < mainWindow.ComboBoxCallSign.SelectedItem.ToString().Length)//--------------------Callsign
            {
                searchCall = mainWindow.ComboBoxCallSign.SelectedItem.ToString();
                xmlUrlPSK = xmlUrlPSK + "callsign=" + searchCall;
                mainWindow.ComboBoxReceiverCallsign.SelectedIndex = 0;
                mainWindow.ComboBoxSenderCallsign.SelectedIndex = 0;
            }
            if (mainWindow.ComboBoxMode.SelectedItem.ToString() != "all")//--------------------Mode
            {
                xmlUrlPSK = xmlUrlPSK + "&mode=" + mainWindow.ComboBoxMode.SelectedItem.ToString();
            }
            if (mainWindow.ComboBoxRronly.SelectedItem.ToString() == "true")//--------------------Rronly
            {
                xmlUrlPSK = xmlUrlPSK + "&rronly=1";
            }
            if (mainWindow.ComboBoxFRange.SelectedItem.ToString() != "all")//--------------------FRange
            {
                xmlUrlPSK = xmlUrlPSK + "&frange=" + mainWindow.ComboBoxFRange.SelectedItem.ToString();
            }

            mainWindow.TextBoxPSKBaseURL.Text = xmlUrlPSK;
            mainWindow.LabelSearchedCall.Content = searchCall;
            mainWindow.TextBoxPSKMSG.AppendText(xmlUrlPSK + "\n");
            Debug.WriteLine(xmlUrlPSK);

            HttpClient hc = new HttpClient();

            Task<Stream> taskGet = hc.GetStreamAsync(xmlUrlPSK);

            Stream PSKstream = await taskGet;

            PSKData.Clear();

            PSKData.ReadXml(PSKstream, XmlReadMode.InferSchema);

            PSKstream.Close();

            Debug.WriteLine("GetPSKStream End");

            await Task.Run(() => XMLproc(searchCall));
        }

        public void XMLproc(string sCall)
        {
            foreach (DataTable table1 in PSKData.Tables)
            {
                string Key1 = table1.TableName;

                Debug.WriteLine(Key1 + " " + table1.Rows.Count);
                DispatcherPSKMsg(Key1 + " Rows= " + table1.Rows.Count.ToString() + " ");

                DataRow dr = PSKData.Tables[Key1].Rows[0];
                DataTable tableKey1 = PSKData.Tables[Key1];

                DispatcherPSKMsg("x Columns= " + tableKey1.Columns.Count.ToString() + "\n");

                switch (Key1)
                {
                    case "receptionReports":
                        foreach (DataRow row1 in tableKey1.Rows)
                        {
                            foreach (DataColumn column1 in tableKey1.Columns)
                            {
                                string str = "";
                                if (column1.ColumnName.Contains("Seconds"))
                                {
                                    str = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(row1[column1].ToString())).ToString("yyyy/MM/dd HH:mm:ss");
                                }
                                else
                                {
                                    str = row1[column1].ToString();
                                }
                                DispatcherPSKMsg("\t" + column1.ColumnName + " = " + str + "\n");
                            }
                        }
                        break;

                    case "lastSequenceNumber":
                        foreach (DataRow row1 in tableKey1.Rows)
                        {
                            foreach (DataColumn column1 in tableKey1.Columns)
                            {
                                DispatcherPSKMsg("\t" + column1.ColumnName + " = " + row1[column1].ToString() + "\n");
                            }
                        }
                        break;

                    case "maxFlowStartSeconds":
                        foreach (DataRow row1 in tableKey1.Rows)
                        {
                            foreach (DataColumn column1 in tableKey1.Columns)
                            {
                                string str = "";
                                if (column1.ColumnName.Contains("value"))
                                {
                                    str = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(row1[column1].ToString())).ToString("yyyy/MM/dd HH:mm:ss");
                                }
                                else
                                {
                                    str = row1[column1].ToString();
                                }
                                DispatcherPSKMsg("\t" + column1.ColumnName + " = " + str + "\n");
                            }
                        }
                        break;

                    case "senderSearch":
                        foreach (DataRow row1 in tableKey1.Rows)
                        {
                            foreach (DataColumn column1 in tableKey1.Columns)
                            {
                                string str = "";
                                if (column1.ColumnName.Contains("Seconds"))
                                {
                                    str = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(row1[column1].ToString())).ToString("yyyy/MM/dd HH:mm:ss");
                                }
                                else
                                {
                                    str = row1[column1].ToString();
                                }
                                DispatcherPSKMsg("\t" + column1.ColumnName + " = " + str + "\n");
                            }
                        }
                        break;

                    case "lotw":
                        foreach (DataRow row1 in tableKey1.Rows)
                        {
                            foreach (DataColumn column1 in tableKey1.Columns)
                            {
                                string str = row1[column1].ToString();
                                DispatcherPSKMsg("\t" + column1.ColumnName + " = " + str + "\n");
                            }
                        }
                        break;

                    case "activeReceiver":
                        mainWindow.TextBoxPSKLeftDownMSG.Dispatcher.Invoke(() =>
                        {
                            mainWindow.TextBoxPSKLeftDownMSG.Text = "activeReceiver= " + tableKey1.Rows.Count.ToString();
                        });

                        foreach (DataRow row1 in tableKey1.Rows)
                        {
                            string callsign = "";
                            string locator = "";
                            string frequency = "";
                            string diff_frequency = "";
                            string region = "";
                            string DXCC = "";
                            string decoderSoftware = "";
                            string antennaInformation = "";
                            string mode = "";

                            foreach (DataColumn column1 in tableKey1.Columns)
                            {
                                string strValue = row1[column1].ToString();

                                if (column1.ColumnName.Equals("callsign")) callsign = strValue;
                                else if (column1.ColumnName.Equals("locator")) locator = strValue;
                                else if (column1.ColumnName.Equals("frequency")) frequency = strValue;
                                else if (column1.ColumnName.Equals("region")) region = strValue;
                                else if (column1.ColumnName.Equals("DXCC")) DXCC = strValue;
                                else if (column1.ColumnName.Equals("decoderSoftware")) decoderSoftware = strValue;
                                else if (column1.ColumnName.Equals("antennaInformation")) antennaInformation = strValue;
                                else if (column1.ColumnName.Equals("mode")) mode = strValue;
                            }

                            string[] str = ConvertToBandDiff(frequency, mode);
                            frequency = str[0];
                            diff_frequency = str[1];

                            PSKDataItem.activeReceiver ar = new PSKDataItem.activeReceiver()
                            {
                                callsign = callsign,
                                locator = locator,
                                frequency = frequency,
                                diff_frequency = diff_frequency,
                                region = region,
                                DXCC = DXCC,
                                decoderSoftware = decoderSoftware,
                                antennaInformation = antennaInformation,
                                mode = mode
                            };

                            itemsActiveReceiver.Add(ar);
                        }

                        mainWindow.DataGridActiveReceiver.Dispatcher.Invoke(() =>
                        {
                            //Console.WriteLine(itemsActiveReceiver.Count);

                            mainWindow.DataGridActiveReceiver.ItemsSource = itemsActiveReceiver;
                            mainWindow.DataGridActiveReceiver.Items.Refresh();

                            if (mainWindow.DataGridActiveReceiver.Items.Count > 0)
                            {
                                var border = VisualTreeHelper.GetChild(mainWindow.DataGridActiveReceiver, 0) as Decorator;
                                if (border != null)
                                {
                                    var scroll = border.Child as ScrollViewer;
                                    if (scroll != null) scroll.ScrollToEnd();
                                }
                            }
                        });

                        break;

                    case "receptionReport":
                        mainWindow.TextBoxPSKLeftUpperMSG.Dispatcher.Invoke(() =>
                        {
                            mainWindow.TextBoxPSKLeftUpperMSG.Text = sCall + "  receptionReport= " + tableKey1.Rows.Count.ToString();
                        });

                        foreach (DataRow row1 in tableKey1.Rows)
                        {
                            //Debug.Write(Key1);
                            string msg = "";
                            string receiverCallsign = "";
                            string receiverLocator = "";
                            string senderCallsign = "";
                            string senderLocator = "";
                            string frequency = "";
                            string diff_frequency = "";
                            string flowStartSeconds = "";
                            string mode = "";
                            string isSender = "";
                            string isReceiver = "";
                            string senderRegion = "";  //Aichi
                            string senderDXCC = "";    //Japan
                            string senderDXCCCode = "";    //JA
                            string senderDXCCLocator = ""; //
                            string senderLotwUpload = "";  //2020-03-09
                            string senderEqslAuthGuar = "";    //A
                            string sNR = "";

                            foreach (DataColumn column1 in tableKey1.Columns)
                            {
                                string strValue = row1[column1].ToString();

                                if (column1.ColumnName.Equals("receiverCallsign")) receiverCallsign = strValue;
                                else if (column1.ColumnName.Equals("receiverLocator")) receiverLocator = strValue;
                                else if (column1.ColumnName.Equals("senderCallsign")) senderCallsign = strValue;
                                else if (column1.ColumnName.Equals("senderLocator")) senderLocator = strValue;
                                else if (column1.ColumnName.Equals("frequency")) frequency = strValue;
                                else if (column1.ColumnName.Equals("flowStartSeconds"))
                                    flowStartSeconds = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(strValue)).ToString("yyyy/MM/dd HH:mm:ss");
                                else if (column1.ColumnName.Equals("mode")) mode = strValue;
                                else if (column1.ColumnName.Equals("isSender")) isSender = strValue;
                                else if (column1.ColumnName.Equals("isReceiver")) isReceiver = strValue;
                                else if (column1.ColumnName.Equals("senderRegion")) senderRegion = strValue;
                                else if (column1.ColumnName.Equals("senderDXCC")) senderDXCC = strValue;
                                else if (column1.ColumnName.Equals("senderDXCCCode")) senderDXCCCode = strValue;
                                else if (column1.ColumnName.Equals("senderDXCCLocator")) senderDXCCLocator = strValue;
                                else if (column1.ColumnName.Equals("senderLotwUpload")) senderLotwUpload = strValue;
                                else if (column1.ColumnName.Equals("senderEqslAuthGuar")) senderEqslAuthGuar = strValue;
                                else if (column1.ColumnName.Equals("sNR")) sNR = strValue;
                            }

                            string[] str = ConvertToBandDiff(frequency, mode);
                            frequency = str[0];
                            diff_frequency = str[1];

                            PSKDataItem.receptionReport rr = new PSKDataItem.receptionReport()
                            {
                                receiverCallsign = receiverCallsign,
                                receiverLocator = receiverLocator,
                                senderCallsign = senderCallsign,
                                senderLocator = senderLocator,
                                frequency = frequency,
                                diff_frequency = diff_frequency,
                                flowStartSeconds = flowStartSeconds,
                                mode = mode,
                                isSender = isSender,
                                isReceiver = isReceiver,
                                senderRegion = senderRegion,
                                senderDXCC = senderDXCC,
                                senderDXCCCode = senderDXCCCode,
                                senderDXCCLocator = senderDXCCLocator,
                                senderLotwUpload = senderLotwUpload,
                                senderEqslAuthGuar = senderEqslAuthGuar,
                                sNR = sNR
                            };

                            itemsReceptionReport.Add(rr);
                        }

                        mainWindow.DataGridReceptionReport.Dispatcher.Invoke(() =>
                        {
                            //Console.WriteLine(itemsActiveReceiver.Count);

                            mainWindow.DataGridReceptionReport.ItemsSource = itemsReceptionReport;
                            mainWindow.DataGridReceptionReport.Items.Refresh();

                            if (mainWindow.DataGridReceptionReport.Items.Count > 0)
                            {
                                var border = VisualTreeHelper.GetChild(mainWindow.DataGridReceptionReport, 0) as Decorator;
                                if (border != null)
                                {
                                    var scroll = border.Child as ScrollViewer;
                                    if (scroll != null) scroll.ScrollToEnd();
                                }
                            }
                        });

                        break;
                    case "activeCallsign":
                        mainWindow.TextBoxPSKCenterDownMSG.Dispatcher.Invoke(() =>
                        {
                            mainWindow.TextBoxPSKCenterDownMSG.Text = "activeCallsign= " + tableKey1.Rows.Count.ToString();
                        });

                        foreach (DataRow row1 in tableKey1.Rows)
                        {
                            //Debug.Write(Key1);
                            string msg = "";

                            string callsign = "";
                            string reports = "";
                            string DXCC = "";
                            string DXCCcode = "";
                            string frequency = "";
                            string diff_frequency = "";

                            foreach (DataColumn column1 in tableKey1.Columns)
                            {
                                string strValue = row1[column1].ToString();

                                if (column1.ColumnName.Equals("callsign")) callsign = strValue;
                                else if (column1.ColumnName.Equals("reports")) reports = strValue;
                                else if (column1.ColumnName.Equals("DXCC")) DXCC = strValue;
                                else if (column1.ColumnName.Equals("DXCCcode")) DXCCcode = strValue;
                                else if (column1.ColumnName.Equals("frequency")) frequency = strValue;
                            }

                            string[] str = ConvertToBandDiff(frequency, "");
                            frequency = str[0];
                            diff_frequency = str[1];

                            PSKDataItem.activeCallsign rr = new PSKDataItem.activeCallsign()
                            {
                                callsign = callsign,
                                reports = reports,
                                DXCC = DXCC,
                                DXCCcode = DXCCcode,
                                frequency = frequency,
                                diff_frequency = diff_frequency
                            };

                            itemsActiveCallsign.Add(rr);
                        }

                        mainWindow.DataGridActiveCallsign.Dispatcher.Invoke(() =>
                        {
                            //Console.WriteLine(itemsActiveReceiver.Count);

                            mainWindow.DataGridActiveCallsign.ItemsSource = itemsActiveCallsign;
                            mainWindow.DataGridActiveCallsign.Items.Refresh();

                            if (mainWindow.DataGridActiveCallsign.Items.Count > 0)
                            {
                                var border = VisualTreeHelper.GetChild(mainWindow.DataGridActiveCallsign, 0) as Decorator;
                                if (border != null)
                                {
                                    var scroll = border.Child as ScrollViewer;
                                    if (scroll != null) scroll.ScrollToEnd();
                                }
                            }
                            listCallsign.Add("-");
                            foreach (var actCall in itemsActiveCallsign)
                            {
                                listCallsign.Add(actCall.callsign);
                            }
                            listCallsign.Sort();//sort call sign for combobox

                            mainWindow.ComboBoxCallSign.Items.Clear();
                            mainWindow.ComboBoxSenderCallsign.Items.Clear();
                            mainWindow.ComboBoxReceiverCallsign.Items.Clear();

                            foreach (var call1 in listCallsign)
                            {
                                mainWindow.ComboBoxCallSign.Items.Add(call1);
                                mainWindow.ComboBoxSenderCallsign.Items.Add(call1);
                                mainWindow.ComboBoxReceiverCallsign.Items.Add(call1);
                            }
                            mainWindow.ComboBoxCallSign.SelectedIndex = 1;
                            mainWindow.ComboBoxSenderCallsign.SelectedIndex = 0;
                            mainWindow.ComboBoxReceiverCallsign.SelectedIndex = 0;

                            mainWindow.ButtonGetPSKxml.Background = Brushes.LightBlue;// change cbutton color //
                        });
                        break;

                    default:
                        break;
                }
            }
            Debug.WriteLine("XMLproc2 End");

            mainWindow.DataGridActiveCallsign.Dispatcher.Invoke(() =>
            {
                mainWindow.ButtonGetPSKxml.Background = Brushes.LightBlue;// change cbutton color //

                SortColDataGrid(mainWindow.DataGridReceptionReport, 6, ListSortDirection.Descending);
            });
        }

        public static void SortColDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var column2 = dataGrid.Columns[columnIndex];
            dataGrid.Items.SortDescriptions.Clear();
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column2.SortMemberPath, sortDirection));
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column2.SortDirection = sortDirection;
            dataGrid.Items.Refresh();
        }

        public string GetRandomCallSign()
        {
            string[] letters = new string[15] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" };
            Random r1 = new System.Random();
            int i1 = r1.Next(0, 15);
            int i2 = r1.Next(0, 15);
            int i3 = r1.Next(0, 15);
            string call = @"JA1" + letters[i1] + letters[i2] + letters[i3];
            return call;
        }

        private void DispatcherPSKMsg(string strMSG)
        {
            mainWindow.TextBoxPSKInfo.Dispatcher.Invoke(() =>
            {
                mainWindow.TextBoxPSKMSG.AppendText(strMSG);
                mainWindow.TextBoxPSKMSG.ScrollToEnd();
            });
        }

        private void DispatcherPSKInfo(string strMSG)
        {
            mainWindow.TextBoxPSKInfo.Dispatcher.Invoke(() =>
            {
                mainWindow.TextBoxPSKInfo.AppendText(strMSG);
                mainWindow.TextBoxPSKInfo.ScrollToEnd();
            });
        }

        double[] BandBase = new double[21] {
                1840000.0, 3573000.0 , 3578000.0, 5357000.0 , 7041000.0, 7047000.0, 7074000.0, 7078000.0,
                10136000.0, 10140000.0, 14074000.0, 14078000.0, 14080000.0, 18100000.0,
                21074000.0, 24915000.0, 28074000.0, 50313000.0, 144174000.0, 144460000.0, 430510000.0 };

        public string[] ConvertToBandDiff(string freq, string mode)
        {
            string[] retString = new string[2];

            if (freq.Length < 1) // if Null freq
            {
                retString[0] = freq.Length.ToString();
                DispatcherPSKInfo(mode + " " + freq + "\n");
                return retString;
            }

            double freqDouble = Convert.ToDouble(freq);

            if (mode.Contains("FT"))    // FT4 FT8
            {
                double freqDiff = 0;
                foreach (double base1 in BandBase)
                {
                    if (base1 < freqDouble && freqDouble < (base1 + 3000))// ---3KHz
                    {
                        freqDiff = freqDouble - base1;
                        retString[0] = (base1 / 1000000.0).ToString("F3");
                        retString[1] = freqDiff.ToString();
                        return retString;
                    }
                }
                retString[0] = (freqDouble / 1000000.0).ToString("F3"); // Out of 3KHz
                DispatcherPSKInfo(mode + " " + freq + "\n");
                return retString;
            }
            else
            {
                retString[0] = (freqDouble / 1000000.0).ToString("F3"); // JS8 ...
                DispatcherPSKInfo(mode + " " + freq + "\n");
                return retString;
            }
        }

        public string PSKField(DataRow row, string f, string label = "")
        {
            if (row.Table.Columns.Contains(f))
            {
                if (label != "")
                {
                    return label + "= " + row[f].ToString();
                }
                else
                {
                    return f + "= " + row[f].ToString();
                }
            }
            else return "";
        }
    }
}
