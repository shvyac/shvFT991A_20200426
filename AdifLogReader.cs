using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
//using System.Windows.Forms;

namespace shvFT991A
{
    class AdifLogReader
    {
        private List<AdifLogItem> _myqsolog;
        public MainWindow mainWindow = (MainWindow)App.Current.MainWindow;

        public string[] getvalues(string aLine)
        {
            string[] xx = Regex.Split(aLine.Replace("\n", "").Replace("\r", ""), @"<(.*?):.*?>([^<\t\n\r\f\v]+)").Where(S => !string.IsNullOrEmpty(S)).ToArray();

            return xx;
        }

        public List<AdifLogItem> ReadAdi(bool boMerge = false)
        {
            if (boMerge)
            {
                _myqsolog = mainWindow._qsolog;  //Merge = true = Keep List
            }
            else
            {
                _myqsolog = new List<AdifLogItem>(); //Merge = false = Clear List
            }


            string FileNameImportFrom = mainWindow.ComboBoxMyLogFileName.Text;
            Console.WriteLine("AdifLogReader read FileNameImportFrom" + FileNameImportFrom);

            mainWindow.ListViewMyLogMSG.Items.Add(FileNameImportFrom + "\n");
            //mainWindow.TextBlockMyLogMSG.Text = FileNameImportFrom + "\n";

            using (StreamReader sr = new StreamReader(FileNameImportFrom))
            {
                string line;
                string valueofkey = "";
                string debugline = "";
                int keywordid = 0;

                string[] keyword = {

                    "call","gridsquare","mode","rst_sent","rst_rcvd",//4
                    "qso_date", "time_on", "qso_date_off", "time_off","band",//9

                    "freq", "station_callsign", "my_gridsquare", "operator","app_qrzlog_logid",//14
                    "app_qrzlog_status","band_rx","cont","country","cqz",//19

                    "dxcc","freq_rx","ituz","lat","lon",//24
                    "lotw_qsl_sent","my_city","my_country","my_cq_zone","my_iota",//29

                    "my_itu_zone","my_lat","my_lon","my_name","qrzcom_qso_upload_date",//34
                    "qrzcom_qso_upload_status","qsl_rcvd","qsl_sent","comment","distance",//39

                    "email","eqsl_qsl_rcvd","eqsl_qsl_sent","iota","lotw_qsl_rcvd",//44
                    "name","qsl_via","qth","srx_string","stx_string" ,//49

                    "app_qrzlog_qsldate","lotw_qslrdate"  ,"qslrdate"  ,"qslsdate", "cnty",//54
                    "state" ,"rx_pwr", "tx_pwr","submode" //58
                };

                string[] keywordh =
                {
                    "ADIF_VER","CREATED_TIMESTAMP","PROGRAMID","PROGRAMVERSION","USERDEFn"
                };

                int nodata = 0;
                int iStart = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] valueofkeys = new string[keyword.Count()];

                    if (iStart < 1) // 0 ------------------------- Header Part
                    {
                        if (-1 < line.IndexOf("<eoh>"))
                        {
                            iStart = 1;
                        }
                        else
                        {
                            string[] x0 = getvalues(line);
                            if (1 < x0.Length)
                            {
                                for (int i = 0; i < keywordh.Length; i++)
                                {
                                    int callid = Array.IndexOf(x0, keywordh[i]);
                                    if (callid != -1)
                                    {
                                        valueofkey = x0[callid + 1];

                                        mainWindow.ListViewMyLogMSG.Items.Add(keywordh[i] + ":" + valueofkey);
                                        //mainWindow.TextBlockMyLogMSG.Text += " " + valueofkey;
                                    }
                                }
                            }
                        }
                    }
                    else // 1 ------------------------- Body Part
                    {
                        string[] x = getvalues(line);

                        //Regex.Split(line.Replace("\n", "").Replace("\r", ""), @"<(.*?):.*?>([^<\t\n\r\f\v]+)").Where(S => !string.IsNullOrEmpty(S)).ToArray();
                        //Console.WriteLine(line);

                        string[] xkeyword = new string[x.Count() / 2];

                        for (int ix = 0; ix < x.Length - 2; ix++) //without <eor>
                        {
                            int mod = ix % 2;
                            if (mod == 0)
                            {
                                xkeyword[ix / 2] = x[ix];
                            }
                        }

                        for (int i = 0; i < keyword.Length; i++)
                        {
                            keywordid = i;
                            int callid = Array.IndexOf(x, keyword[keywordid]);
                            if (callid != -1)
                            {
                                valueofkey = x[callid + 1];
                                debugline += keyword[keywordid] + " " + valueofkey + " ";
                                valueofkeys[keywordid] = valueofkey.Trim();
                            }
                        }

                        string[] resultArray = xkeyword.Except(keyword).ToArray();  //check new keyword that shoud be defined

                        if (0 < resultArray.Length)
                        {
                            string strMSG = "*** keyword not defined:" + string.Join(" ", resultArray);
                            Console.Write(strMSG);
                            Console.WriteLine();

                            mainWindow.ListViewMyLogMSG.Items.Add(strMSG);
                        }

                        if (valueofkeys[3] == null || valueofkeys[4] == null)
                        {
                            string strRSTerror = string.Format("***error RST report is null, on {0}", valueofkeys[0]);
                            mainWindow.ListViewMyLogMSG.Items.Add(strRSTerror);
                        }

                        nodata++;

                        AdifLogItem logItem = new AdifLogItem()
                        {
                            no = nodata,
                            call = valueofkeys[0],
                            gridsquare = valueofkeys[1],
                            mode = valueofkeys[2],
                            rst_sent = valueofkeys[3],
                            rst_rcvd = valueofkeys[4],

                            qso_date = valueofkeys[5],
                            time_on = valueofkeys[6],
                            qso_date_off = valueofkeys[7],
                            time_off = valueofkeys[8],
                            band = valueofkeys[9],

                            freq = valueofkeys[10],
                            station_callsign = valueofkeys[11],
                            my_gridsquare = valueofkeys[12],
                            operator_name = valueofkeys[13],
                            app_qrzlog_logid = valueofkeys[14],

                            app_qrzlog_status = valueofkeys[15],
                            band_rx = valueofkeys[16],
                            cont = valueofkeys[17],
                            country = valueofkeys[18],
                            cqz = valueofkeys[19],

                            dxcc = valueofkeys[20],
                            freq_rx = valueofkeys[21],
                            ituz = valueofkeys[22],
                            lat = valueofkeys[23],
                            lon = valueofkeys[24],

                            lotw_qsl_sent = valueofkeys[25],
                            my_city = valueofkeys[26],
                            my_country = valueofkeys[27],
                            my_cq_zone = valueofkeys[28],
                            my_iota = valueofkeys[29],

                            my_itu_zone = valueofkeys[30],
                            my_lat = valueofkeys[31],
                            my_lon = valueofkeys[32],
                            my_name = valueofkeys[33],
                            qrzcom_qso_upload_date = valueofkeys[34],

                            qrzcom_qso_upload_status = valueofkeys[35],
                            qsl_rcvd = valueofkeys[36],
                            qsl_sent = valueofkeys[37],
                            comment = valueofkeys[38],
                            distance = valueofkeys[39],

                            email = valueofkeys[40],
                            eqsl_qsl_rcvd = valueofkeys[41],
                            eqsl_qsl_sent = valueofkeys[42],
                            iota = valueofkeys[43],
                            lotw_qsl_rcvd = valueofkeys[44],

                            name = valueofkeys[45],
                            qsl_via = valueofkeys[46],
                            qth = valueofkeys[47],
                            srx_string = valueofkeys[48],
                            stx_string = valueofkeys[49],

                            app_qrzlog_qsldate = valueofkeys[50],
                            lotw_qslrdate = valueofkeys[51],
                            qslrdate = valueofkeys[52],
                            qslsdate = valueofkeys[53],
                            cnty = valueofkeys[54],

                            state = valueofkeys[55],
                            rx_pwr = valueofkeys[56],
                            tx_pwr = valueofkeys[57],
                            submode = valueofkeys[58]
                        };

                        if (boMerge)
                        {
                            foreach (AdifLogItem it in _myqsolog)
                            {
                                string st =
                                    it.call + "-" + valueofkeys[0] + "-" +
                                    it.qso_date + "-" + valueofkeys[5] + "-" +
                                    it.time_on + "-" + valueofkeys[6].Substring(0, 4) + "-" +
                                    it.rst_sent + "-" + valueofkeys[3];

                                //Console.WriteLine(st1);
                                //string callwsjtx = valueofkeys[0].Trim();

                                if (it.call == valueofkeys[0] && it.qso_date == valueofkeys[5] && it.time_on == valueofkeys[6].Substring(0, 4) && it.rst_sent != valueofkeys[3])
                                {
                                    //string st = valueofkeys[0] + " " + valueofkeys[5] + " " + valueofkeys[6] + " " + valueofkeys[3];
                                    Console.WriteLine("== " + st);
                                    //mainWindow.ListViewMyLogMSG.Items.Add(st);

                                    _myqsolog[it.no - 1].rst_sent = valueofkeys[3];
                                    _myqsolog[it.no - 1].rst_rcvd = valueofkeys[4];
                                    Console.WriteLine("== " + _myqsolog[it.no - 1].rst_sent + "|" + _myqsolog[it.no - 1].rst_rcvd);
                                }
                                else
                                {
                                    //Console.WriteLine("xx " + st + it.call.Length.ToString() + " " + valueofkeys[0].Length.ToString());
                                }
                            }
                        }
                        else
                        {
                            _myqsolog.Add(logItem);
                        }

                    }
                    //_qsolog.Add(logItem);

                    //Console.WriteLine(debugline);
                    debugline = "";
                }

                mainWindow.DataGridMyLog.ItemsSource = _myqsolog;
                //mainWindow.TextBlockMyLogStatusBar.Text = _qsolog.Count.ToString();

                mainWindow.TextBlockMyLogStatusBar.Dispatcher.Invoke(() =>
                {
                    mainWindow.TextBlockMyLogStatusBar.Text = _myqsolog.Count.ToString();
                });
            }
            //------------------------------------------------------------------------------------Check Merged Data
            if (boMerge)
            {
                foreach (AdifLogItem chk in _myqsolog)
                {
                    if (chk.rst_sent == null || chk.rst_rcvd == null)
                    {
                        string strRSTerror = string.Format("***error RST report is null, on {0} {1} |{2}|{3}|", chk.call, chk.qso_date, chk.rst_sent, chk.rst_rcvd);
                        Console.WriteLine(strRSTerror);
                    }
                }
            }
            //------------------------------------------------------------------------------------
            return _myqsolog;

        }
    }
}