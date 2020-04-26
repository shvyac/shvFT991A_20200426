using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace shvFT991A
{
    class QRZCallbookReader
    {
        private List<RadioStationItem> _radiostation_serial;
        private List<string> _radiostation_not_found;

        public MainWindow mainWindow = (MainWindow)App.Current.MainWindow;

        string file_radio_station = @"radio_station_info_from_soumugojp.csv";
        string file_not_found = @"not_found_station_on_soumugojp.csv";

        public List<RadioStationItem> Read_SoumuGoJpSerial(List<string> requested_call_signs, /*BindingSource bindingSource,*/ int parseNumber)
        {
            //------------------------------------------------------------------------------------------
            _radiostation_serial = read_prev_data();
            _radiostation_not_found = read_notfound_data();
            //------------------------------------------------------------------------------------------

            int loop_counter = 0;
            int loop_end = parseNumber;

            foreach (string call1 in requested_call_signs)
            {
                //-------------------------------------------------------------------------------------Delete Portable Info
                string call;
                if (call1.Contains("/"))
                {
                    call = call1.Substring(0, call1.IndexOf("/"));
                    string m = "Read_SoumuGoJpSerial: remove /portable = " + call1 + " " + call;
                    Console.WriteLine(m);
                    mainWindow.LabelStatusBarSoumu.Content = m;
                }
                else
                {
                    call = call1;
                }

                //-------------------------------------------------------------------------------------Already ?
                int indexcall = _radiostation_serial.FindIndex(m => m.call_sign == call);
                if (-1 < indexcall)
                {
                    string m = "Read_SoumuGoJpSerial: call ALREADY in index  =  " + call + " file = " + file_radio_station + " index = " + indexcall;
                    //Console.WriteLine(m);
                    mainWindow.LabelStatusBarSoumu.Content = m;

                    continue;
                }

                int indexcall2 = _radiostation_not_found.FindIndex(m => m == call);
                if (-1 < indexcall2)
                {
                    string m = "Read_SoumuGoJpSerial: call ALREADY in _radiostation_not_found index  =  " + call + " index = " + indexcall2;
                    Console.WriteLine(m);
                    mainWindow.LabelStatusBarSoumu.Content = m;

                    continue;
                }
                //-------------------------------------------------------------------------------------Download URL
                // https://www.qrz.com/db/RZ1O/

                string uricall = @"https://www.tele.soumu.go.jp/musen/SearchServlet?SC=1&pageID=3&SelectID=1&CONFIRM=0&SelectOW=01&IT=&HC=&HV=&FF=&TF=&HZ=3&NA=&DFY=&DFM=&DFD=&DTY=&DTM=&DTD=&SK=2&DC=100&MA=" + call;

                //Console.WriteLine("Read_SoumuGoJpSerial: uricall = " + uricall);

                WebClient wc = new WebClient();
                wc.Encoding = Encoding.Default;
                string source = wc.DownloadString(uricall);
                wc.Dispose();

                //-------------------------------------------------------------------------------------Parse NEXT Page
                CompleteDownloadProcSerial(source, call/*,  bindingSource*/);

                /*
                try
                {
                    wc.DownloadStringCompleted += CompleteDownloadProc;
                    wc.DownloadStringAsync(new Uri(uricall), call);
                }
                catch (WebException exc)
                {
                    //textBox1.Text += exc.Message;
                    Console.WriteLine("Read_SoumuGoJp: exc.Message = " + exc.Message + " " + call);
                }
                */

                //-------------------------------------------------------------------------------------Check Finish
                loop_counter++;


                if (loop_end < loop_counter)
                {
                    break;
                }

                //-------------------------------------------------------------------------------------Next Call Sign
                System.Threading.Thread.Sleep(3000);

                DateTime dt = DateTime.Now;
                Console.WriteLine("Now-------------------------   " + dt + " --- " + loop_counter.ToString() + " / " + loop_end.ToString());


            }

            Console.WriteLine("Read_SoumuGoJpSerial: _radiostation_serial.Count() " + _radiostation_serial.Count());
            Console.WriteLine("Read_SoumuGoJpSerial: _radiostation_not_found.Count() " + _radiostation_not_found.Count());

            return _radiostation_serial;
        }

        public void CompleteDownloadProcSerial(string result, string call/*, BindingSource bindingSource*/)
        {
            //Console.WriteLine("CompleteDownloadProcSerial: e.Result = " + result);

            MatchCollection zeromatch = Regex.Matches(result, "検索結果が0件です。");

            if (0 < zeromatch.Count)
            {
                //-------------------------------------------------------------------------------------検索結果が0件です。
                string m = "CompleteDownloadProcSerial: zeromatch.Count = " + zeromatch.Count + " " + call;
                mainWindow.LabelStatusBarSoumu.Content = m;

                //-----------------------------------------------------------item data .csv write

                DateTime dt_now = DateTime.Now;
                string[] aStrings = { call, dt_now.ToString("yyyy-MM-dd") };
                string strSeparator = ",";
                string strLine = string.Join(strSeparator, aStrings);

                using (StreamWriter sw = new StreamWriter(file_not_found, true))
                {
                    sw.WriteLine(strLine);
                    sw.Close();
                }
            }
            else
            {

                MatchCollection matches = Regex.Matches(result, ".*[都道府県].*<br>");

                foreach (Match m in matches)
                {
                    //Console.WriteLine(m.Value);
                    string address = m.Value.Replace("<br>", "").Trim();
                    Console.WriteLine("CompleteDownloadProcSerial: address = " + address + " " + call);
                }

                //-----------------------------------------------------------
                //<a href="./SearchServlet?pageID=4&IT=A&DFCD=0000009844&DD=1&styleNumber=50" target="_blank">ハムクラブ（XXXXXX）</a>
                //-----------------------------------------------------------

                string HRefPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
                MatchCollection mat2 = Regex.Matches(result, HRefPattern);

                foreach (Match m in mat2)
                {
                    if (m.Value.Contains("pageID=4"))
                    {
                        //-------------------------------------------------------------------------------------parse subpage 
                        string href = m.Value;

                        Console.WriteLine("CompleteDownloadProcSerial: address = " + href + " " + call);

                        string url = href.Remove(0, 7);
                        string url2 = "https://www.tele.soumu.go.jp/musen" + url.Remove(url.Length - 1, 1);
                        System.Net.WebClient wc = new System.Net.WebClient();
                        wc.Encoding = System.Text.Encoding.Default;

                        string source = wc.DownloadString(url2);
                        wc.Dispose();

                        //Console.WriteLine(source);

                        //----
                        string[] deli = { "\n" };
                        string[] lines = source.Split(deli, StringSplitOptions.None);

                        //----------------------------------------------------------識別信号
                        int index0 = Array.FindIndex(lines, ContainsCallsign);
                        Console.WriteLine("CompleteDownloadProcSerial: lines[index + 4] = " + lines[index0 + 4].Trim() + " " + call);
                        string callsign = lines[index0 + 4].Replace("<BR>", "").Trim();

                        //----------------------------------------------------------氏名又は名称
                        int index1 = Array.FindIndex(lines, ContainsKeyword);
                        Console.WriteLine("CompleteDownloadProcSerial: lines[index + 4] = " + lines[index1 + 4].Trim() + " " + call);
                        string op_name = lines[index1 + 4].Trim();

                        //----------------------------------------------------------移動範囲

                        string type_station = "";

                        if (source.Contains("陸上、海上及び上空"))
                        {
                            type_station = "陸上、海上及び上空";
                        }
                        else if (source.Contains("移動しない"))
                        {
                            type_station = "移動しない";
                        }

                        //----------------------------------------------------------無線設備の設置場所／常置場所
                        string sta_place = "";
                        int index2 = Array.FindIndex(lines, ContainsPlace);
                        for (int i = 0; i < 40; i++)
                        {
                            //Console.WriteLine("CompleteDownloadProc: lines[index + 4] = " + lines[index2 + i].Trim() + " " + call);

                            if (lines[index2 + i].Contains("<br>"))
                            {
                                sta_place = lines[index2 + i].Trim().Replace("<br>", "");
                                Console.WriteLine("CompleteDownloadProcSerial: Contains <br> = " + sta_place + " " + call);

                            }
                        }

                        //----------------------------------------------------------14175 MHz
                        int index3 = Array.FindIndex(lines, Contains14175);
                        string power = "";
                        string bool14mhz = "NO";
                        if (0 < index3)
                        {
                            power = lines[index3 + 22].Replace("&nbsp;", "").Replace(@"<td><pre class=""defpre"">", "").Replace(@"</pre></td>", "").Trim();

                            Console.WriteLine("CompleteDownloadProcSerial: power = 14175MHz " + power.Trim() + " " + call);
                            bool14mhz = "YES14MHz";
                        }
                        else
                        {
                            bool14mhz = "NO";
                        }

                        //-----------------------------------------------------------item data Add
                        DateTime dt_now = DateTime.Now;
                        string date_today = dt_now.ToString("yyyy-MM-dd");

                        RadioStationItem newItem = new RadioStationItem()
                        {
                            call_sign = callsign,
                            station_type = type_station,
                            station_address = sta_place,
                            bool_14mhz = bool14mhz,
                            max_power_watt = power,
                            checked_date = date_today
                        };

                        _radiostation_serial.Add(newItem);

                        mainWindow.DataGridSoumuGoJp.ItemsSource = _radiostation_serial;

                        //bindingSource.DataSource = _radiostation_serial;

                        Console.WriteLine("CompleteDownloadProcSerial: _radiostation_serial.Count() " + _radiostation_serial.Count() + " " + call);

                        //-----------------------------------------------------------item data .csv write

                        string[] aStrings = { callsign, type_station, sta_place, bool14mhz, power, date_today };
                        string strSeparator = ",";
                        string strLine = string.Join(strSeparator, aStrings);

                        using (StreamWriter sw = new StreamWriter(file_radio_station, true))
                        {
                            sw.WriteLine(strLine);
                            sw.Close();
                        }
                    }
                }
            }
        }

        private static bool ContainsCallsign(String s)
        {
            if (s.Contains("識別信号"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ContainsKeyword(String s)
        {
            if (s.Contains("氏名又は名称"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ContainsPlace(String s)
        {
            if (s.Contains("無線設備の設置場所／常置場所"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool Contains14175(String s)
        {
            if (s.Contains("14175"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public List<RadioStationItem> read_prev_data()
        {
            List<RadioStationItem> _radiostation_prev = new List<RadioStationItem>();

            using (StreamReader sr = new StreamReader(file_radio_station))
            {
                Console.WriteLine(@"read_prev_data file_radio_station = " + file_radio_station);

                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);

                    string[] fields = line.Split(',');

                    RadioStationItem logItem = new RadioStationItem()
                    {
                        call_sign = fields[0],
                        station_type = fields[1],
                        station_address = fields[2],
                        bool_14mhz = fields[3],
                        max_power_watt = fields[4],
                        checked_date = fields[5]
                    };

                    _radiostation_prev.Add(logItem);

                    //bindingSource.DataSource = _radiostation_prev;
                }
            }
            Console.WriteLine(@"read_prev_data _radiostation_prev.Count() = " + _radiostation_prev.Count());

            return _radiostation_prev;
        }

        public List<string> read_notfound_data()
        {
            List<string> _radiostation_not_found = new List<string>();

            using (StreamReader sr = new StreamReader(file_not_found))
            {
                Console.WriteLine(@"read_notfound_data file_not_found = " + file_not_found);

                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);

                    string[] fields = line.Split(',');

                    _radiostation_not_found.Add(fields[0]);
                }
            }
            Console.WriteLine(@"read_notfound_data _radiostation_not_found.Count() = " + _radiostation_not_found.Count());

            return _radiostation_not_found;
        }
    }
}

