using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;

namespace shvFT991A
{
    class ContestLogReader
    {
        private List<ContestLogItem> _contestlog;
        public MainWindow mainWindow = (MainWindow)App.Current.MainWindow;

        public List<ContestLogItem> read(/*BindingSource bindingSource*/)
        {
            _contestlog = new List<ContestLogItem>();
            string file_all_ja_contest_log = mainWindow.file_all_ja_contest_log;

            if (File.Exists(file_all_ja_contest_log))
            {
                Console.WriteLine("File Exists: " + file_all_ja_contest_log);
            }
            else
            {
                MessageBox.Show("File not found in ContestLogReader: " + file_all_ja_contest_log);
                return _contestlog;
            }

            using (StreamReader sr = new StreamReader(file_all_ja_contest_log))
            {
                //No.,DATE,TIME,DATE - TIME,BAND,MODE,CALLSIGN,SENT,No,RCVD,No,Mlt,Pts,OP,TX,memo,,,,,
                //1,2019 / 4 / 27,21:02,2019 / 4 / 27 21:02,3.5,SSB,JA1AAA,59,15H,59,13L,13,1,JK1AAA,TX2,,,,,,

                string line;
                
                while ((line = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(line);

                    string[] fields = line.Split(',');
                    
                    ContestLogItem logItem = new ContestLogItem()
                    {
                        no          = fields[ 0],
                        date        = fields[ 1],
                        time        = fields[ 2],
                        date_time   = fields[ 3],
                        band        = fields[ 4],
                        mode        = fields[ 5],
                        call_sign   = fields[ 6],
                        sent_rst    = fields[ 7],
                        sent_no     = fields[ 8],
                        rcvd_rst    = fields[ 9],
                        rcvd_no     = fields[10],
                        multi       = fields[11],
                        point       = fields[12],
                        operator_name = fields[13],
                        tx          = fields[14],
                        memo        = fields[15]
                    };
                    _contestlog.Add(logItem);
                }
                mainWindow.DataGridMyContestLog.ItemsSource = _contestlog;
                mainWindow.TextBlockMyContestStatusBar.Text = _contestlog.Count.ToString();
            }
            return _contestlog;
        }
    }
}
