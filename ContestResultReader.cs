using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace shvFT991A
{
    class ContestResultReader
    {
        private List<ContestResultItem> _alljaresult;
        public MainWindow mainWindow = (MainWindow)App.Current.MainWindow;

        public List<ContestResultItem> read()
        {
            _alljaresult = new List<ContestResultItem>();

            string _bumon;
            string _bumon_code;
            string _rank;
            string _call_sign;
            string _total_point;
            string _tx_power;
            string file_all_ja_result = mainWindow.file_all_ja_result;

            using (StreamReader sr = new StreamReader(file_all_ja_result))
            {
                _bumon = @"";
                _bumon_code = @"";
                _rank = @"";
                _call_sign = @"";
                _total_point = @"";
                _tx_power = @"";

                /*
                2018 ALL JA コンテスト
                書類提出局全リスト
                電話部門
                電話部門シングルオペオールバンド PA
                  1   JF2AAA/2                 575 ×  97 ＝      55,775
                  2   JJ1AAA                   564 ×  87 ＝      49,068
                */

                string line;
                int nodata = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);

                    string[] x = Regex.Split(line.Trim().Replace("\n", "").Replace("\r", ""), @"\s+"); //.Where(S => !string.IsNullOrEmpty(S)).ToArray();


                    if (x.Length == 1)
                    {
                        _bumon = x[0];
                    }
                    else if (x.Length == 2)
                    {
                        _bumon_code = line;
                    }
                    else if (x.Length == 7)
                    {
                        _rank = x[0];
                        _call_sign = x[1];
                        _total_point = x[6];
                    }
                    else if (x.Length == 8)
                    {
                        _rank = x[0];
                        _call_sign = x[1];
                        _total_point = x[6];
                        _tx_power = x[7];
                    }

                    if (_call_sign != "")
                    {
                        nodata++;

                        ContestResultItem logItem = new ContestResultItem()
                        {
                            no = nodata,
                            bumon = _bumon,
                            bumon_code = _bumon_code,
                            rank = _rank,
                            call_sign = _call_sign,
                            total_point = _total_point,
                            tx_power = _tx_power                            
                        };

                        _alljaresult.Add(logItem);
                        _call_sign = @"";
                    }                    
                };
                mainWindow.DataGridContestResult.ItemsSource = _alljaresult;
            }
            return _alljaresult;
        }
    }
}

