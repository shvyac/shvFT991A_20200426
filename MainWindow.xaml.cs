using Microsoft.Win32;
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
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace shvFT991A
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    /// 

    public enum ModeKind
    {
        LSB = 1,
        USB = 2,
        CW = 3,
        FM = 4,
        AM = 5,
        RTTY_LSB = 6,
        CW_R = 7,
        DATA_LSB = 8,
        RTTY_USB = 9,
        DATA_FM = 10,
        FM_N = 11,
        DATA_USB = 12,
        AM_N = 13,
        C4FM = 14
    }

    public static class ArrayExtensionMethods
    {
        public static ArraySegment<T> GetSegment<T>(this T[] arr, int offset, int? count = null)
        {
            if (count == null) { count = arr.Length - offset; }
            return new ArraySegment<T>(arr, offset, count.Value);
        }
    }

    public partial class MainWindow : Window
    {
        public string file_radio_station = @"radio_station_info_from_soumugojp.csv";
        public string file_not_found = @"radio_station_not_found_on_soumugojp.csv";
        public string file_all_ja_result = @"2018-all-ja-result.txt";
        public string file_all_ja_contest_log = @"2019-all-ja.csv";
        public string file_mylog_from_adi = @"";

        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        string TimeZone = @"UTC";

        //private List<AdifLogItem> _MyQsoLog;

        private List<ContestResultItem> _2018AllJaResult;
        private List<RadioStationItem> _SoumuRadioStation;

        public MainWindow()
        {
            InitializeComponent();

            SetFT991ATabInitially();

            SetHAMLogTabInitially();

            SetMorseTabInitially();

            SetPSKTabInitially();

            //TabItemHAMLog.IsSelected = true;
        }

        //===========================================================================================================================TAB FT991A

        List<MemoryChannel> itemsMemoryChannel = new List<MemoryChannel>();

        private void SetFT991ATabInitially()
        {
            SetFT991A_Memory_Channel();

            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();

            Button14MHz_Click(null, null);
            ButtonUSB_Click(null, null);
            ButtonFreqSwap_Click(null, null);
            Button50MHz_Click(null, null);
            ButtonFM_Click(null, null);

            //WebBrowserQRZ.Navigate(@"http://s.com");
        }

        public static string DisplayModeName(ModeKind mode)
        {
            string[] names = { "LSB", "USB", "CW", "FM", "AM", "RTTY_LSB", "CW_R", "DATA_LSB", "RTTY_USB", "DATA_FM", "FM_N", "DATA_USB", "AM_N", "C4FM" };
            return names[(int)mode];
        }

        private void SetFT991A_Memory_Channel()
        {
            int[] Freqs = { 7074000, 10136000, 14074000, 18100000, 21074000, 24915000, 28074000, 50313000, 145000000, 432000000 };
            ModeKind[] Modes = { ModeKind.USB, ModeKind.LSB, ModeKind.CW, ModeKind.FM, ModeKind.AM };
            int nodata = 0;
            var randM = new Random();

            for (int iF = 0; iF < Freqs.Count(); iF++)
            {
                int baseFreq = Freqs[iF];

                for (int iC = 0; iC < 10; iC++)
                {
                    int appFreq = baseFreq + iC * 1000;
                    int appMode = randM.Next(5);
                    ModeKind imode = Modes[appMode];

                    nodata++;
                    MemoryChannel mc = new MemoryChannel()
                    {
                        No = nodata,
                        Freq = appFreq,
                        ClarifierFreq = 0,
                        ClarifierSwitchRX = false,
                        ClarifierSwitchTX = false,
                        ModeFreq = imode,
                        VfoOrMemory = false,
                        CtcssDcsMode = 0,
                        SimplexMode = 0,
                        MemoryTag = appFreq.ToString() + " "
                    };
                    itemsMemoryChannel.Add(mc);
                }
            }
            DataGridMemoryChannel.ItemsSource = itemsMemoryChannel;

            //itemsMemoryChannel.Add(new MemoryChannel() { No = 0, Freq = 14074000, ModeEnum = 1, MemoryTag = "7MHz FT8" });
        }

        private void Button1800KHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 1800000);
        }

        private void Button3500KHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 3543000);
        }

        private void Button07MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 7074000);
        }

        private void Button10MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 10813000);
        }

        public void Button14MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 14074000);
        }

        private void Button18MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 18100000);
        }

        private void Button21MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 21074000);
        }

        private void Button24MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 24915000);
        }

        private void Button28MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 28074000);
        }

        private void Button50MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 50313000);
        }

        private void Button144MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 145000000);
        }

        private void Button430MHz_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 433000000);
        }

        private void ButtonGENBand_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 76400000);
        }

        private void ButtonMWBand_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 954000);
        }

        private void ButtonAirBand_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = string.Format("{0:N0}", 124100000);
        }

        private void ButtonDateTime_Click(object sender, RoutedEventArgs e)
        {
            if (TimeZone == @"UTC")
            {
                TimeZone = @"JST";
            }
            else
            {
                TimeZone = @"UTC";
            }
        }

        private void Timer_Click(object sender, EventArgs e)

        {
            DateTime d;
            SolidColorBrush col = Brushes.Blue;

            if (TimeZone == @"UTC")
            {
                d = DateTime.UtcNow;
                col = Brushes.Blue;
            }
            else
            {
                d = DateTime.Now;
                col = Brushes.Black;
            }

            buttonDateTime.Content = string.Format("{3}  {0:D2} : {1:D2} : {2:D2} ", d.Hour, d.Minute, d.Second, TimeZone);
            buttonDateTime.Foreground = col;

            this.Title = string.Format("shvF991A 1.00  {0:D4}/{1:D2}/{2:D2} {3}", d.Year, d.Month, d.Day, d.DayOfWeek);
        }

        private void ButtonFreqCopyAtoB_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOB.Text = textboxFreqVFOA.Text;
        }

        private void ButtonFreqCopyBtoA_Click(object sender, RoutedEventArgs e)
        {
            textboxFreqVFOA.Text = textboxFreqVFOB.Text;
        }

        private void ButtonFreqSwap_Click(object sender, RoutedEventArgs e)
        {
            string frq = textboxFreqVFOB.Text;
            string mod = textboxVFOBMode.Text;

            textboxFreqVFOB.Text = textboxFreqVFOA.Text;
            textboxFreqVFOA.Text = frq;

            textboxVFOBMode.Text = textboxVFOAMode.Text;
            textboxVFOAMode.Text = mod;
        }

        private void ButtonLSB_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"LSB";
        }

        private void ButtonUSB_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"USB";
        }

        private void ButtonCWL_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"CW L";
        }

        private void ButtonCWU_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"CW U";
        }

        private void ButtonDATAL_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"DATA L";
        }

        private void ButtonDATAU_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"DATA U";
        }

        private void ButtonRTTYL_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"RTTY L";
        }

        private void ButtonRTTYU_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"RTTY U";
        }

        private void ButtonFM_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"FM";
        }

        private void ButtonAM_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"AM";
        }

        private void ButtonFMN_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"FM N";
        }

        private void ButtonAMN_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"AM N";
        }

        private void ButtonDATAF_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"DATA F";
        }

        private void ButtonC4FM_Click(object sender, RoutedEventArgs e)
        {
            textboxVFOAMode.Text = @"C4FM";
        }

        private void ButtonFreqCopyVFOtoM_Click(object sender, RoutedEventArgs e)
        {
            int freq = int.Parse(textboxFreqVFOA.Text, System.Globalization.NumberStyles.AllowThousands);

            MemoryChannel mc = new MemoryChannel();
            mc.Freq = freq;
            mc.ModeFreq = ModeKind.USB;

            itemsMemoryChannel.Add(mc);

            DataGridMemoryChannel.ItemsSource = itemsMemoryChannel;
            DataGridMemoryChannel.Items.Refresh();
        }

        private void ButtonFreqCopyMtoVFO_Click(object sender, RoutedEventArgs e)
        {
            int selRow = DataGridMemoryChannel.SelectedIndex;

            MemoryChannel mc = new MemoryChannel();
            mc = itemsMemoryChannel[selRow];

            textboxFreqVFOA.Text = string.Format("{0:N0}", mc.Freq);

            ///textboxVFOAMode.Text = ModeKind.USB.D
        }

        private void DataGridMemoryChannel_AutoGeneratedColumns(object sender, EventArgs e)
        {

        }

        private void DataGridMemoryChannel_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            Style h_Center = new Style();
            h_Center.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));
            Style h_Right = new Style();
            h_Right.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            Style h_Stretch = new Style();
            h_Stretch.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));

            Style v_ctr = new Style();
            v_ctr.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));

            if (headername == "MiddleName")
            {
                e.Cancel = true;
            }

            switch (headername)
            {
                case "No":
                    e.Column.Header = "No";
                    e.Column.CellStyle = h_Right;
                    break;
                case "Freq":
                    e.Column.Header = "Freq";
                    e.Column.CellStyle = h_Right;
                    break;
                case "ModeFreq":
                    e.Column.Header = "Mode";
                    e.Column.CellStyle = h_Right;
                    break;
                case "MemoryTag":
                    e.Column.Header = "Memo";
                    e.Column.CellStyle = h_Right;
                    break;
                case "ClarifierFreq":
                    e.Cancel = true;
                    break;
                case "ClarifierSwitchRX":
                    e.Cancel = true;
                    break;
                case "ClarifierSwitchTX":
                    e.Cancel = true;
                    break;
                case "CtcssDcsMode":
                    e.Cancel = true;
                    break;
                case "SimplexMode":
                    e.Cancel = true;
                    break;
                case "VfoOrMemory":
                    e.Cancel = true;
                    break;

                default:
                    e.Column.Header = "default";
                    e.Column.CellStyle = h_Right;
                    break;
            }
        }

        //===========================================================================================================================TAB HAMLOG

        private void SetHAMLogTabInitially()
        {
            //https://qiita.com/Tood/items/197c1eaa38ed159a1b06


            //----------------------------------------------------------My Log
            string fn = "wsjtx_log.adi";
            ComboBoxMyLogFileName.Items.Add(fn);
            ButtonMyLogImportFrom.IsEnabled = false;
            ButtonExportCSVfile.IsEnabled = false;

            //----------------------------------------------------------My Contest Log
            ContentRendered += (s, e) => SetMyContestLog();


            //----------------------------------------------------------JARL Contest Results
            ComboBoxFileResult.Items.Add(file_all_ja_result);

            //----------------------------------------------------------SoumuGoJp
            ContentRendered += (s, e) => SetSoumugojpTable();

            ScrollBarParseNumber.Value = 3;
            TextBoxSoumuParseNumber.Text = "3";

            //----------------------------------------------------------QRZ.com
            SetQRZTab();
        }

        private void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = TabControlMain.SelectedIndex;
            //Console.WriteLine(selectedIndex.ToString() + "番目のタブが選択されました TabControlMain");
        }

        private void TabControlHAMLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = TabControlHAMLog.SelectedIndex;

            //Console.WriteLine(selectedIndex.ToString() + "番目のタブが選択されました TabControlHAMLog ");

            switch (selectedIndex)
            {
                case 0:

                    break;

                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    break;

                default:
                    break;
            }
        }

        //=================================================================================================================MyLOG
        public List<AdifLogItem> _qsolog, _wsjtxlog;

        private void ButtonSelectMylogFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "adi file (*.adi)|*.adi|All file (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine(dialog.FileName);
                file_mylog_from_adi = dialog.FileName;

                int iPosiotion = ComboBoxMyLogFileName.Items.Add(file_mylog_from_adi);
                ComboBoxMyLogFileName.SelectedIndex = iPosiotion;
                ButtonMyLogImportFrom.IsEnabled = true;

                int posMSG = ListViewMyLogMSG.Items.Add(file_mylog_from_adi);
                ListViewMyLogMSG.SelectedIndex = posMSG;
            }
        }

        private void ButtonMyLogImportFrom_Click(object sender, RoutedEventArgs e)
        {
            MyLogImport();
        }

        private void MyLogImport()
        {
            //----------------------------------------------------QsoLogItem
            //_MyQsoLog = new List<AdifLogItem>();
            //AdifLogItem qsologItem = new AdifLogItem() { call = "", gridsquare = "", mode = "" };
            //_MyQsoLog.Add(qsologItem);
            //DataGridMyLog.ItemsSource = _MyQsoLog;
            //qsoLogItemBindingSource.DataSource = _qsolog;
            //----------------------------------------------------

            AdifLogReader af = new AdifLogReader();

            _qsolog = af.ReadAdi();  // Merge = false

            Set_MyLog_Table();

            ButtonExportCSVfile.IsEnabled = true;
        }

        public void Set_MyLog_Table()
        {
            //datatable = dataset.Tables.Add("SpotInfo");
            //dataGridViewLog.DataSource = dataset;
            //dataGridViewLog.DataMember = "SpotInfo";
            //dv = datatable.DefaultView;
            //dv.Sort = "Column1, Column2 ASC";
            //dtable.Columns.Add("Date");
            //dataGridViewSpots.Columns["Date"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //datarow = datatable.NewRow();

            //--------------------------------------------------------------------------------------------------------------------
            string[] headerlabel = {
                        "Call","Grid Square","Mode","RST Sent","RST Rcvd",
                        "QSO Date", "Time On", "QSO Date Off", "Time Off",
                        "Band","Freq", "Station Callsign", "My Grid Square", "Operator",
                        "app_qrzlog_logid","app_qrzlog_status","band_rx","cont","country","cqz",
                        "dxcc","freq_rx","ituz","lat","lon","lotw_qsl_sent",
                        "my_city","my_country","my_cq_zone","my_iota","my_itu_zone","my_lat",
                        "my_lon","my_name","qrzcom_qso_upload_date","qrzcom_qso_upload_status","qsl_rcvd","qsl_sent" };
            //--------------------------------------------------------------------------------------------------------------------

            /*
            public string app_qrzlog_logid { get; set; }		    //394228140
        public string app_qrzlog_status { get; set; }           //N
        //band										            //20M
        public string band_rx { get; set; }			            //20M
        //call									                //DU1/
        public string cont { get; set; }                        //OC
        public string country { get; set; }                     //Philippines
        public string cqz { get; set; }						    //27
        public string dxcc { get; set; }                        //375
        //freq										            //14.074
        public string freq_rx { get; set; }                     //14.074
        public string ituz { get; set; }                        //50
        public string lat { get; set; }                         //S000 00.000
        public string lon { get; set; }                         //W000 00.000
        public string lotw_qsl_sent { get; set; }               //Y
        //mode                                                  //FT8                     
        public string my_city { get; set; }                     // 
        public string my_country { get; set; }                  //Japan
        public string my_cq_zone { get; set; }                  //25
        //my_gridsquare                                         //
        public string my_iota { get; set; }                     //AS-007
        public string my_itu_zone { get; set; }                 //45
        public string my_lat { get; set; }                      // 36.200
        public string my_lon { get; set; }                      // 57.400
        public string my_name { get; set; }                     // 
        public string qrzcom_qso_upload_date { get; set; }      //20190102
        public string qrzcom_qso_upload_status { get; set; }    //Y
        public string qsl_rcvd { get; set; }                    //N
        public string qsl_sent { get; set; }                    //N
        */

            //--------------------------------------------------------------------------------------------------------------------
            Style h_ctr = new Style();
            h_ctr.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));

            Style h_right = new Style();
            h_right.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));

            for (int i = 0; i < DataGridMyLog.Columns.Count; i++)
            {
                //DataGridMyLog.Columns[i].Header = headerlabel[i];                
                DataGridMyLog.Columns[i].Width = DataGridLength.Auto;

                DataGridMyLog.Columns[i].HeaderStyle = h_ctr;
                //DataGridMyLog.Columns[i].CellStyle = h_right;
            }
            //--------------------------------------------------------------------------------------------------------------------

            //for (int j = 0; j < DataGridMyLog.Columns.Count; j++)
            //{
            //DataGridMyLog.Columns[j].Width = 200;
            //this.DataGridMyLog.Columns[j].HeaderTemplate.  .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //DataGridMyLog.Columns[j].HeaderCell.Style.WrapMode = DataGridViewTriState.False;
            //DataGridMyLog.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //}
            //this.dataGridViewLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //this.dataGridViewLog.ColumnHeadersHeight = this.dataGridViewLog.ColumnHeadersHeight * 1;
            //this.dataGridViewLog.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            //this.DataGridMyLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //this.dataGridViewLog.Scroll += new ScrollEventHandler(this.dgv_Scroll); //dgv_Scroll
            //this.dataGridViewLog.Paint += new PaintEventHandler(dataGridView1_Paint);
            //dataGridViewLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            //DataGridMyLog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void ButtonMyLogMergeRST_Click(object sender, RoutedEventArgs e)
        {
            AdifLogReader afMerge = new AdifLogReader();

            _wsjtxlog = afMerge.ReadAdi(true);  // Merge = true
        }

        private void ButtonCalcGSDistance_Click(object sender, RoutedEventArgs e)
        {
            int numData = _qsolog.Count;
            AdifLogItem adi = new AdifLogItem();

            for (int i = 0; i < numData; i++)
            {
                adi = _qsolog[i];
                string gsHis = adi.gridsquare;
                string gsMine = adi.my_gridsquare;

                if (gsHis.Length < 4 | gsMine.Length < 4) continue;

                double lat1 = GetLatitudeDeg(gsMine);
                double lat2 = GetLatitudeDeg(gsHis);

                double lon1 = GetLongitudeDeg(gsMine);
                double lon2 = GetLongitudeDeg(gsHis);

                double lat1rad = GetRadFromDeg(lat1);
                double lat2rad = GetRadFromDeg(lat2);

                double latDiff = (lat2 - lat1) / 180.0 * Math.PI;
                double lonDiff = (lon2 - lon1) / 180.0 * Math.PI;

                //var a = Math.sin(Δφ / 2) * Math.sin(Δφ / 2) +Math.cos(φ1) * Math.cos(φ2) *Math.sin(Δλ / 2) * Math.sin(Δλ / 2);
                //var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
                //var d = R * c;

                double a = Math.Sin(latDiff / 2.0) * Math.Sin(latDiff / 2.0) + Math.Cos(lat1rad) * Math.Cos(lat2rad) * Math.Sin(lonDiff / 2.0) * Math.Sin(lonDiff / 2.0);
                double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double d = 6371.0 * c;//Km

                Console.WriteLine(string.Format(" {0} {1} {2} {3} ", i, a, c, d));
                _qsolog[i].distancekm = d.ToString("F0");
            }

            DataGridMyLog.ItemsSource = _qsolog;

        }

        private double GetRadFromDeg(double Deg)
        {
            return Deg / 180.0 * Math.PI;
        }

        private double GetLongitudeDeg(string Gridsqure4)
        {
            //= (CODE(MID(B3, 1, 1)) - 65) * 20 + VALUE(MID(B3, 3, 1)) * 2 + 1 - 180

            int code = (int)Gridsqure4.Substring(0, 1)[0];
            double douLon1 = code - 65;
            double douLon2 = Convert.ToDouble(Gridsqure4.Substring(2, 1)) * 2.0 + 1.0;
            double douLon = douLon1 * 20.0 + douLon2 - 180.0;
            return douLon; // (double)(douLon / 180.0 * Math.PI);
        }

        private double GetLatitudeDeg(string Gridsqure4)
        {
            //= (CODE(MID(B3, 2, 1)) - 65) * 10 + VALUE(MID(B3, 4, 1)) + 0.5 - 90

            int code = (int)Gridsqure4.Substring(1, 1)[0];
            double douLat1 = code - 65;
            double douLat2 = Convert.ToDouble(Gridsqure4.Substring(3, 1)) + 0.5;
            double douLat = douLat1 * 10.0 + douLat2 - 90.0;
            return douLat;  // (double)(douLat / 180.0 * Math.PI); 
        }

        private void ButtonExportCSVfile_Click(object sender, RoutedEventArgs e)
        {
            AdifLogItem a = new AdifLogItem();
            int numData = _qsolog.Count;
            Encoding enc = Encoding.GetEncoding("utf-8");

            string strToday = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
            string file_mylog_to_adi = "MyLogExported_" + strToday + ".csv";

            int posMSG = ListViewMyLogMSG.Items.Add(file_mylog_to_adi);
            ListViewMyLogMSG.SelectedIndex = posMSG;

            using (StreamWriter writer = new StreamWriter(file_mylog_to_adi, false, enc))
            {
                string[] strHeaders = {
                    "No", "QSO_Year", "Month","Day","Time_on", "Call", "Grid Square","DistanceKm",
                    "Band", "Freq", "Mode","Submode","RST_Rcvd" , "RST_Sent",
                    "Info RX","Info TX","QTH","QSL Via","Name","email","Comment",
                    "QSL Rcvd","QSL Rcvd Date","QSL Sent","QSL Sent Date","MyRig","MyAnt","TXpower",
                    "P16","P26","P36","P46","P56","P66"
                };
                string strS = ",";
                string strLine1 = string.Join(strS, strHeaders);
                writer.WriteLine(strLine1);

                for (int i = 0; i < numData; i++)
                {
                    a = _qsolog[i];
                    double douFreq = Convert.ToDouble(Gs(a.freq));
                    string freq = douFreq.ToString("F3");

                    string info_tx = Gs(a.stx_string);
                    bool isKirifuri = info_tx.Contains("Kirifuri");

                    string antenna = isKirifuri ? "Whip" : "VDP";
                    string myrig = isKirifuri ? "FT991AM" : "FT991A";
                    string txPower = isKirifuri ? "10" : "50";

                    string amode = Gs(a.mode);
                    string asubmode = Gs(a.submode);
                    bool isFT8 = amode.Contains("FT") | asubmode.Contains("FT"); // show GL only FT* (FT8,FT4), show Info-RX for FM AM SSB etc,.
                    string gridSquare = isFT8 ? a.gridsquare : "";

                    string callSign = Gs(a.call);
                    var aryL6 = new string[] { "", "", "", "", "", "" };
                    if (callSign.Length <= 6)
                    {
                        for (int j = 0; j < callSign.Length; j++)
                        {
                            aryL6[j] = callSign.Substring(j, 1);
                        }
                    }

                    string[] strData = {
                        a.no.ToString(),
                        Gd(Gs(a.qso_date),"year"), Gd(Gs(a.qso_date),"month"), Gd(Gs(a.qso_date),"day"), Gd(Gs(a.time_on),"hour"),
                        callSign, gridSquare,Gs(a.distancekm),
                        Gs(a.band),freq,amode,asubmode,
                        Gs(a.rst_rcvd),Gs(a.rst_sent),
                        Gs(a.srx_string),Gs(a.stx_string),Gs(a.qth),Gs(a.qsl_via),Gs(a.name),Gs(a.email),Gs(a.comment),
                        Gs(a.qsl_rcvd),Gs(a.qslrdate),Gs(a.qsl_sent),Gs(a.qslsdate),
                        myrig,antenna,txPower,
                        aryL6[0],aryL6[1],aryL6[2],aryL6[3],aryL6[4],aryL6[5],
                    };
                    string strSep = ",";
                    string strLine = string.Join(strSep, strData);
                    writer.WriteLine(strLine);
                }
            }

            string fullPath = Path.GetFullPath(file_mylog_to_adi);
            Process.Start(fullPath);
        }

        private string Gd(string OrgString, string Param)
        {
            //20191201
            string strReturn = "";

            if (OrgString.Length == 8) // 19561201 >>> 1956 12 01
            {
                switch (Param)
                {
                    case "year":
                        strReturn = OrgString.Substring(0, 4);
                        break;
                    case "month":
                        strReturn = OrgString.Substring(4, 2);
                        break;
                    case "day":
                        strReturn = OrgString.Substring(6, 2);
                        break;
                    default:
                        break;
                }
            }
            else if (OrgString.Length == 6 || OrgString.Length == 4)  // 120100,1201 >>> 12:01
            {
                switch (Param)
                {
                    case "hour":
                        strReturn = OrgString.Substring(0, 2) + ":" + OrgString.Substring(2, 2);
                        break;
                    default:
                        break;
                }
            }
            return strReturn;
        }

        private string Gs(string OrgString)
        {
            if ((OrgString != null) && (OrgString.Trim().Length != 0))
            {
                string s = OrgString.Replace(",", ".");
                return s.Trim();
            }
            else
            {
                return "";
            }
        }

        private void ComboBoxMyLogFileName_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxMyLogFileName.SelectedIndex = 0;
        }

        private void DataGridMyLog_AutoGeneratedColumns(object sender, EventArgs e)
        {
            if (DataGridMyLog.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(DataGridMyLog, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        private void DataGridMyLog_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            /*
             * call = valueofkeys[0],
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
                        operatorname = valueofkeys[13]
                        */

            string headername = e.Column.Header.ToString();

            //Cancel the column you don't want to generate

            Style h_Center = new Style();
            h_Center.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));
            Style h_Right = new Style();
            h_Right.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            Style h_Stretch = new Style();
            h_Stretch.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));

            Style v_ctr = new Style();
            v_ctr.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));

            if (headername == "MiddleName")
            {
                e.Cancel = true;
            }

            //update column details when generating

            switch (headername)
            {
                case "no": e.Column.Header = "No"; e.Column.CellStyle = h_Right; break;
                case "call": e.Column.Header = "Call"; e.Column.CellStyle = h_Stretch; break;
                case "gridsquare": e.Column.Header = "Grid Squre"; e.Column.CellStyle = h_Stretch; break;
                case "distancekm": e.Column.Header = "Distance Km"; e.Column.CellStyle = h_Right; break;
                case "mode": e.Column.Header = "Mode"; e.Column.CellStyle = h_Right; break;
                case "submode": e.Column.Header = "Submode"; e.Column.CellStyle = h_Right; break;
                case "rst_sent": e.Column.Header = "RST Sent"; e.Column.CellStyle = h_Right; break;
                case "rst_rcvd": e.Column.Header = "RST Rcvd"; e.Column.CellStyle = h_Right; break;
                case "qso_date": e.Column.Header = "QSO Date"; e.Column.CellStyle = h_Right; break;
                case "time_on": e.Column.Header = "Time"; e.Column.CellStyle = h_Right; break;
                case "qso_date_off": e.Column.Header = "QSO Date Off"; e.Column.CellStyle = h_Right; break;
                case "time_off": e.Column.Header = "Time Off"; e.Column.CellStyle = h_Right; break;
                case "band": e.Column.Header = "Band"; e.Column.CellStyle = h_Right; break;
                case "freq": e.Column.Header = "Freq"; e.Column.CellStyle = h_Right; break;
                case "station_callsign": e.Column.Header = "Station Call"; e.Column.CellStyle = h_Right; break;
                case "my_gridsquare": e.Column.Header = "My Grid"; e.Column.CellStyle = h_Right; break;
                case "operatorname": e.Column.Header = "Oper Name"; e.Column.CellStyle = h_Right; break;

                case "app_qrzlog_logid": e.Column.Header = "QRZID"; e.Column.CellStyle = h_Right; break;//
                case "app_qrzlog_status": e.Column.Header = "Status"; e.Column.CellStyle = h_Right; break;//N
                case "band_rx": e.Column.Header = "Band RX"; e.Column.CellStyle = h_Right; break;//20M
                case "cont": e.Column.Header = "Continent"; e.Column.CellStyle = h_Right; break;//OC
                case "country": e.Column.Header = "Country"; e.Column.CellStyle = h_Right; break;//Philippines
                case "cqz": e.Column.Header = "CQ Zone"; e.Column.CellStyle = h_Right; break;//27
                case "dxcc": e.Column.Header = "DXCC"; e.Column.CellStyle = h_Right; break;//375
                case "freq_rx": e.Column.Header = "Freq RX"; e.Column.CellStyle = h_Right; break;//14.074
                case "ituz": e.Column.Header = "ITU Zone"; e.Column.CellStyle = h_Right; break;//50
                case "lat": e.Column.Header = "LAT"; e.Column.CellStyle = h_Right; break;//S000 00.000
                case "lon": e.Column.Header = "LON"; e.Column.CellStyle = h_Right; break;//W000 00.000
                case "lotw_qsl_sent": e.Column.Header = "LOTW QSL Sent"; e.Column.CellStyle = h_Right; break;//Y
                case "my_city": e.Column.Header = "My City"; e.Column.CellStyle = h_Right; break;// 
                case "my_country": e.Column.Header = "My Country"; e.Column.CellStyle = h_Right; break;//Japan
                case "my_cq_zone": e.Column.Header = "my CQ Zone"; e.Column.CellStyle = h_Right; break;//25
                case "my_iota": e.Column.Header = "My IOTA"; e.Column.CellStyle = h_Right; break;//AS-007
                case "my_itu_zone": e.Column.Header = "My ITU Zone"; e.Column.CellStyle = h_Right; break;//45
                case "my_lat": e.Column.Header = "My LAT"; e.Column.CellStyle = h_Right; break;// 36.200
                case "my_lon": e.Column.Header = "My LON"; e.Column.CellStyle = h_Right; break;// 57.400
                case "my_name": e.Column.Header = "My Name"; e.Column.CellStyle = h_Right; break;// 
                case "qrzcom_qso_upload_date": e.Column.Header = "QRZ Upload Date"; e.Column.CellStyle = h_Right; break;//20190102
                case "qrzcom_qso_upload_status": e.Column.Header = "QRZ Upload Status"; e.Column.CellStyle = h_Right; break;//Y
                case "qsl_rcvd": e.Column.Header = "QSL Rcvd"; e.Column.CellStyle = h_Right; break;//N
                case "qsl_sent": e.Column.Header = "QSL Sent"; e.Column.CellStyle = h_Right; break;//N

                case "comment": e.Column.Header = "Comment"; e.Column.CellStyle = h_Right; break;
                case "distance": e.Column.Header = "DistanceQRZ"; e.Column.CellStyle = h_Right; break;
                case "email": e.Column.Header = "eMail"; e.Column.CellStyle = h_Right; break;
                case "eqsl_qsl_rcvd": e.Column.Header = "eQSL Rcvd"; e.Column.CellStyle = h_Right; break;
                case "eqsl_qsl_sent": e.Column.Header = "eQSL Sent"; e.Column.CellStyle = h_Right; break;
                case "iota": e.Column.Header = "IOTA"; e.Column.CellStyle = h_Right; break;
                case "lotw_qsl_rcvd": e.Column.Header = "LOTW QSL Rcvd"; e.Column.CellStyle = h_Right; break;
                case "name": e.Column.Header = "Name"; e.Column.CellStyle = h_Right; break;
                case "qsl_via": e.Column.Header = "QSL Via"; e.Column.CellStyle = h_Right; break;
                case "qth": e.Column.Header = "QTH"; e.Column.CellStyle = h_Right; break;
                case "srx_string": e.Column.Header = "Info RX"; e.Column.CellStyle = h_Right; break;
                case "stx_string": e.Column.Header = "Info TX"; e.Column.CellStyle = h_Right; break;
                case "app_qrzlog_qsldate": e.Column.Header = "QRZ QSL Date"; e.Column.CellStyle = h_Right; break;
                case "lotw_qslrdate": e.Column.Header = "LOTW QSL Rcvd Date"; e.Column.CellStyle = h_Right; break;
                case "qslrdate": e.Column.Header = "QSL Rcvd Date"; e.Column.CellStyle = h_Right; break;
                case "qslsdate": e.Column.Header = "QSL Sent Date"; e.Column.CellStyle = h_Right; break;
                case "cnty": e.Column.Header = "County"; e.Column.CellStyle = h_Right; break;
                case "state": e.Column.Header = "State"; e.Column.CellStyle = h_Right; break;
                case "rx_pwr": e.Column.Header = "RX Power"; e.Column.CellStyle = h_Right; break;
                case "tx_pwr": e.Column.Header = "TX Power"; e.Column.CellStyle = h_Right; break;

                default: e.Column.Header = "Default"; e.Column.CellStyle = h_Right; break;
            }
        }

        //=================================================================================================================MyCONTEST
        private List<ContestLogItem> _MyContestLog;

        private void SetMyContestLog()
        {
            _MyContestLog = new List<ContestLogItem>();

            ContestLogItem contestlogItem = new ContestLogItem() { call_sign = "JA1AAA", band = "PM96", mode = "SSB" };
            _MyContestLog.Add(contestlogItem);

            //DataGridMyContestLog.ItemsSource = _MyContestLog;

            //contestLogItemBindingSource.DataSource = _contestlog;

            Set_MyContestlog_Table();

            ContestLogReader cr = new ContestLogReader();
            _MyContestLog = cr.read();
        }

        public void Set_MyContestlog_Table()
        {
            string[] headerlabel = {
                "No", "Date", "Time", "Date_Time", "Band", "Mode", "Call_Sign",
                "Sent_RST", "Sent_No", "Rcvd_RST", "Rcvd_No", "Multi", "Point", "Operator_Name", "TX", "Memo",
                "JARL_2018_Power","JARL_2018_Result","総務省　都道府県","QRZ.com","eQSL.cc","HAMQTH"
            };

            for (int i = 0; i < DataGridMyContestLog.Columns.Count; i++)
            {
                this.DataGridMyContestLog.Columns[i].Header = headerlabel[i];
            }

            for (int j = 0; j < this.DataGridMyContestLog.Columns.Count; j++)
            {
                //this.dataGridViewLog.Columns[j].Width = 200;
                //this.DataGridMyContestLog.Columns[j].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //this.dataGridViewContestCheck.Columns[j].HeaderCell.Style.WrapMode = DataGridViewTriState.False;
                //this.dataGridViewContestCheck.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            //this.dataGridViewContestCheck.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //this.dataGridViewContestCheck.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void ComboBoxFileResult_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxFileResult.SelectedIndex = 0;
        }

        private void ComboBoxMyContestImportFile_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxMyContestImportFile.SelectedIndex = 0;
        }

        private void ComboBoxMyContestExportFile_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxMyContestExportFile.SelectedIndex = 0;
        }

        private void DataGridMyContestLog_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }

        private void DataGridMyContestLog_AutoGeneratedColumns(object sender, EventArgs e)
        {
            if (DataGridMyContestLog.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(DataGridMyContestLog, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        private void ButtonMyContestImportFrom_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonMyContestExportTo_Click(object sender, RoutedEventArgs e)
        {

        }

        //=================================================================================================================JARL CONTEST RESULT

        private void Button2018ALLJA_Click(object sender, RoutedEventArgs e)
        {
            //----------------------------------------------------AllJAResultItem
            _2018AllJaResult = new List<ContestResultItem>();

            //ContestResultItem item = new ContestResultItem() { call_sign = "JA1AAA" };
            //_2018AllJaResult.Add(item);

            //DataGridContestResult.ItemsSource = _2018AllJaResult;            

            ContestResultReader ar = new ContestResultReader();

            _2018AllJaResult = ar.read();

            LabelContestResultMSG.Content = _2018AllJaResult.Count.ToString();

            //----------------------------------------------------
            set_alljaresult_table();
            set_jarl2018result_into_mycontest_fields();
        }

        public void set_alljaresult_table()
        {
            string[] headerlabel = { "No", "部門", "部門コード", "順位", "Call_Sign", "総合得点", "出力" };
            Style h_ctr = new Style();
            h_ctr.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));

            for (int i = 0; i < headerlabel.Count(); i++)
            {
                //DataGridContestResult.Columns[i].Header = headerlabel[i];

                DataGridContestResult.Columns[i].HeaderStyle = h_ctr;
            }

            for (int j = 0; j < this.DataGridContestResult.Columns.Count; j++)
            {
                //this.dataGridViewLog.Columns[j].Width = 200;
                //this.dataGridViewAllJaResult.Columns[j].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //this.dataGridViewAllJaResult.Columns[j].HeaderCell.Style.WrapMode = DataGridViewTriState.False;
                //this.dataGridViewAllJaResult.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            //this.dataGridViewAllJaResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //this.dataGridViewAllJaResult.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void set_jarl2018result_into_mycontest_fields()
        {
            foreach (var contest in _MyContestLog)
            {
                Console.WriteLine("contest.call_sign = " + contest.call_sign);

                //var healTargets = partyList.Where(chara => chara.Hp <= 60);

                IEnumerable<ContestResultItem> result_calls = _2018AllJaResult.Where(obj => obj.call_sign == contest.call_sign);

                if (0 < result_calls.Count())
                {
                    foreach (var result in result_calls)
                    {
                        Console.WriteLine("\t\t result.call_sign = " + result.call_sign);

                        contest.jarl_2018_power = result.tx_power;

                        contest.jarl_2018_result = result.rank + "位 " + result.total_point + " " + result.bumon_code;

                        LabelMSG.Content = result.rank + "位 " + result.total_point + " " + result.bumon_code;
                    }
                }
            }
            //this.DataGridContestResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //DataGridMyContestLog.ItemsSource = _contestlog;
            DataGridMyContestLog.Items.Refresh();
        }

        private void DataGridContestResult_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            /*
        public int no { get; set; }
        public string bumon { get; set; }
        public string bumon_code { get; set; }
        public string rank { get; set; }
        public string call_sign { get; set; }
        public string total_point { get; set; }
        public string tx_power { get; set; }
                        */

            string headername = e.Column.Header.ToString();

            //Cancel the column you don't want to generate

            Style h_Center = new Style();
            h_Center.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));
            Style h_Right = new Style();
            h_Right.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            Style h_Stretch = new Style();
            h_Stretch.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));

            Style v_ctr = new Style();
            v_ctr.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));

            if (headername == "MiddleName")
            {
                e.Cancel = true;
            }

            //update column details when generating 

            switch (headername)
            {
                case "no":
                    e.Column.Header = "No";
                    e.Column.CellStyle = h_Right;
                    break;
                case "bumon":
                    e.Column.Header = "部門";
                    e.Column.CellStyle = h_Right;
                    break;
                case "bumon_code":
                    e.Column.Header = "部門コード";
                    e.Column.CellStyle = h_Right;
                    break;
                case "rank":
                    e.Column.Header = "順位";
                    e.Column.CellStyle = h_Right;
                    break;
                case "call_sign":
                    e.Column.Header = "Call Sign";
                    e.Column.CellStyle = h_Stretch;
                    break;
                case "total_point":
                    e.Column.Header = "総合得点";
                    e.Column.CellStyle = h_Right;
                    break;
                case "tx_power":
                    e.Column.Header = "出力";
                    e.Column.CellStyle = h_Right;
                    break;

                default:
                    e.Column.Header = "default";
                    e.Column.CellStyle = h_Right;
                    break;
            }
        }

        private void DataGridContestResult_AutoGeneratedColumns(object sender, EventArgs e)
        {
            if (DataGridContestResult.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(DataGridContestResult, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        //=================================================================================================================SOUMU.GO.JP

        public void SetSoumugojpTable()
        {
            //LabelNumberOfSoumu.Content = @"Number of data";

            string[] headerlabel = { "no.", "Call Sign", "固定/移動", "アドレス", "Code", "14MHz?", "出力", "Date" };

            for (int i = 0; i < DataGridSoumuGoJp.Columns.Count(); i++)
            {
                DataGridSoumuGoJp.Columns[i].Header = headerlabel[i];
            }

            //for (int j = 0; j < DataGridSoumuGoJp.Columns.Count; j++)
            //{
            //this.dataGridViewLog.Columns[j].Width = 200;
            //this.dataGridViewSoumuGoJp.Columns[j].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //this.dataGridViewSoumuGoJp.Columns[j].HeaderCell.Style.WrapMode = DataGridViewTriState.False;
            //this.dataGridViewSoumuGoJp.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //}
            //this.dataGridViewSoumuGoJp.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //this.dataGridViewSoumuGoJp.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //DataGridSoumuGoJp.Items.Refresh();
        }

        private void ButtonReadSoumuSerial_Click(object sender, RoutedEventArgs e)
        {
            _SoumuRadioStation = new List<RadioStationItem>();

            //RadioStationItem item = new RadioStationItem() { call_sign = "JA1AAA" };
            //_radiostation.Add(item);

            //---------------------------------------------------------------------------Request Contest Log Call Sign

            List<string> callsign_req = new List<string>();

            foreach (var contest in _MyContestLog)
            {
                Console.WriteLine("ButtonSoumuGoJpSerial_Click: contest.call_sign = " + contest.call_sign);
                callsign_req.Add(contest.call_sign);
            }

            Console.WriteLine("ButtonSoumuGoJpSerial_Click: callsign_req.Count() = " + callsign_req.Count());
            LabelStatusBarSoumu.Content = "ButtonSoumuGoJpSerial_Click: callsign_req.Count() = " + callsign_req.Count();
            LabelNumberOfSoumu.Content = callsign_req.Count();

            //---------------------------------------------------------------------------Read Site Soumu

            RadioStationReaderSerial rsrs = new RadioStationReaderSerial();

            int pNumber = Convert.ToInt16(TextBoxSoumuParseNumber.Text);

            _SoumuRadioStation = rsrs.Read_SoumuGoJpSerial(callsign_req, /*radioStationItemBindingSource,*/ pNumber);

            Console.WriteLine("ButtonSoumuGoJpSerial_Click: _radiostation.Count() " + _SoumuRadioStation.Count());
            Console.WriteLine("ButtonSoumuGoJpSerial_Click: _radiostation.Count() " + _SoumuRadioStation[_SoumuRadioStation.Count() - 1].call_sign);

            DataGridSoumuGoJp.ItemsSource = _SoumuRadioStation;
            LabelNumberOfSoumu.Content = _SoumuRadioStation.Count();

            //radioStationItemBindingSource.DataSource = _radiostation;
            ////radioStationItemBindingSource.ResetBindings(true);
            //binding.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            SetSoumugojpTable();

            DataGridSoumuGoJp.Items.Refresh();
        }

        private void DataGridSoumuGoJp_AutoGeneratedColumns(object sender, EventArgs e)
        {
            if (DataGridSoumuGoJp.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(DataGridSoumuGoJp, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        private void DataGridSoumuGoJp_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }

        //=================================================================================================================QRZ
        private void ButtonQRZSearch_Click(object sender, RoutedEventArgs e)
        {
            clear_Group();
            GetCallsign(TextBoxSearchCallsign.Text.ToString());
        }

        private void ButtonSetLoginInfo_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.ShowDialog();
        }

        public SettingLogin loginWindow = (SettingLogin)Application.Current.MainWindow;
        private DataSet QRZData = new DataSet("QData");
        private WebClient wc = new WebClient();
        public string username = "";
        public string password = "";
        public string xmlSession = "";
        public string xmlError = "";
        public string xmlMessage = "";
        public bool isOnline = false;
        public string dburl = "http://www.qrz.com/dxml/xml.pl";

        private void GetCallsign(string cs)
        {
            if (cs.Length < 3) return;
            string url = dburl + "?s=" + xmlSession + ";callsign=" + cs;
            CallQRZ(url);
            if (!isOnline)
            {
                loginWindow.ShowDialog();
                if (isOnline)
                {
                    GetCallsign(cs);
                }
                return;
            }

            Write_QRZBox();
        }

        public void CallQRZ(string url)
        {
            Console.WriteLine("CallQRZ url = " + url);

            Stream qrzstrm;
            try
            {
                QRZData.Clear();
                qrzstrm = wc.OpenRead(url);
                QRZData.ReadXml(qrzstrm, XmlReadMode.InferSchema);
                qrzstrm.Close();

                if (!QRZData.Tables.Contains("QRZDatabase"))
                {
                    MessageBox.Show("Error: failed to receive QRZDatabase object", "XML Server Error");
                    return;
                }

                DataRow dr = QRZData.Tables["QRZDatabase"].Rows[0];

                LabelVersion.Content = QRZField(dr, "version", "Version");

                DataTable sess = QRZData.Tables["Session"];
                DataRow sr = sess.Rows[0];
                xmlError = QRZField(sr, "Error");
                TextBlockStatusBar.Text = xmlError;

                xmlSession = sr["Key"].ToString();    //QRZField(sr, "Key");

                xmlMessage = QRZField(sr, "Message");
                LabelGMTime.Content = QRZField(sr, "GMTime");
                LabelCount.Content = QRZField(sr, "Count");
                LabelSubExp.Content = QRZField(sr, "SubExp");
                LabelKey.Content = xmlSession;

                Console.WriteLine("CallQRZ xmlError = " + xmlError);
                Console.WriteLine("CallQRZ xmlSession = " + xmlSession);
                Console.WriteLine("CallQRZ xmlMessage = " + xmlMessage);

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "XML Error");
            }
            isOnline = (xmlSession.Length > 0) ? true : false;

        }

        public void SetQRZTab()
        {
            loginWindow = new SettingLogin(this);
            //clearGroup();

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loginWindow.ShowDialog();

            //LoginForm.ShowDialog(this);
        }

        public string QRZField(DataRow row, string f, string label = "")
        {
            if (row.Table.Columns.Contains(f))
            {
                if (label != "")
                {
                    return label + ": " + row[f].ToString();
                }
                else
                {
                    return f + ": " + row[f].ToString();
                }
            }
            else return "";
        }

        public void doLogin()
        {
            LabelLoginID.Content = username + " Logined";

            string url = dburl + "?username=" + username + ";password=" + password;
            CallQRZ(url);
            if (isOnline)
            {
                TextBlockStatusBar.Text = username + " Login OK";
            }
        }

        private void Write_QRZBox()
        {

            if (!QRZData.Tables.Contains("Callsign")) return;
            DataTable callTable = QRZData.Tables["Callsign"];
            if (callTable.Rows.Count == 0) return;
            DataRow dr = callTable.Rows[0];


            TextBoxSearchCallsign.Text = "";

            LabelCall.Content = QRZField(dr, "call", "Call");
            LabelFullname.Content = QRZField(dr, "fname", "First Name") + " " + QRZField(dr, "name", "Last Name");
            LabelAddr1.Content = QRZField(dr, "addr1", "Addr1");
            LabelAddr2.Content = QRZField(dr, "addr2", "Addr2") + " " + QRZField(dr, "state", "State") + " " + QRZField(dr, "zip", "ZIP");
            LabelCountry.Content = QRZField(dr, "country", "Country");


            LabelLat.Content = QRZField(dr, "lat", "LAT");
            LabelLon.Content = QRZField(dr, "lon", "LON");
            LabelGrid.Content = QRZField(dr, "grid", "Grid");
            LabelCounty.Content = QRZField(dr, "county", "Country");
            LabelLand.Content = QRZField(dr, "land", "Land");
            LabelEfdate.Content = QRZField(dr, "efdate", "EF Date");
            LabelExpdate.Content = QRZField(dr, "expdate", "EXP Date");
            LabelP_call.Content = QRZField(dr, "p_call", "P Call");
            LabelBorn.Content = QRZField(dr, "born", "Born");
            LabelTrustee.Content = QRZField(dr, "trustee", "xzzzzzz");
            LabelClass.Content = QRZField(dr, "class", "Class");
            LabelCodes.Content = QRZField(dr, "codes", "Codes");
            LabelQslmgr.Content = QRZField(dr, "qslmgr", "QSL Manager");
            LabelEmail.Content = QRZField(dr, "email", "E Mail");
            LabelUrl.Content = QRZField(dr, "url", "URL");
            LabelU_views.Content = QRZField(dr, "u_views", "User Views");
            LabelBio.Content = QRZField(dr, "bio", "BIO");
            LabelImage.Content = QRZField(dr, "image", "Image");
            LabelModdate.Content = QRZField(dr, "moddate", "Mod Date");
            LabelAreaCode.Content = QRZField(dr, "AreaCode", "Area Code");
            LabelTimeZone.Content = QRZField(dr, "TimeZone", "Time Zone");
            LabelGMTOffset.Content = QRZField(dr, "GMTOffset", "GMT Offset");
            LabelDST.Content = QRZField(dr, "DST", "DST");
            LabelEqsl.Content = QRZField(dr, "eqsl", "eQSL");
            LabelMqsl.Content = QRZField(dr, "mqsl", "mQSL");
            LabelEqsl.Content = QRZField(dr, "eqsl", "eQSL");
            LabelCqzone.Content = QRZField(dr, "cqzone", "CQ Zone");
            LabelItuzone.Content = QRZField(dr, "ituzone", "ITU Zone");
            LabelLocref.Content = QRZField(dr, "locref", "xzzzzzz");
            LabelIota.Content = QRZField(dr, "iota", "IOTA");
            LabelLotw.Content = QRZField(dr, "lotw", "LOTW");
            LabelUser.Content = QRZField(dr, "user", "User");
            LabelFips.Content = QRZField(dr, "fips", "FIPS");
            LabelLand.Content = QRZField(dr, "land", "Land");
            LabelMSA.Content = QRZField(dr, "MSA", "MSA");

        }

        private void clear_Group()
        {
            /*

            foreach (Control c in GroupBoxResults.get)
            {
                if ((string)c.Tag == "x") c.Text = "";
            }
            this.Refresh();
            */
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            clear_Group();
            GetCallsign(TextBoxSearchCallsign.Text.ToString());
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DataGridQRZcom_AutoGeneratedColumns(object sender, EventArgs e)
        {
            if (DataGridQRZcom.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(DataGridQRZcom, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        private void DataGridQRZcom_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        //===========================================================================================================================MORSE

        private void SetMorseTabInitially()
        {
            textBoxSystemAnswer.Text = @"Answer shows here";
            textBoxUserAnswer.Text = @"Your Answer";

            //TextBoxSpeedStart.Text = 40.ToString();
            //TextBoxSpeedEnd.Text = 20.ToString();
            //TextBoxSpeedDecrement.Text = @"10";
            //TextBoxRepeatCycles.Text = 5.ToString();

            ScrollBarSpeedStart.Value = 30;
            ScrollBarSpeedEnd.Value = 20;
            ScrollBarSpeedDecrement.Value = 1;
            ScrollBarRepeatCycles.Value = 20;

            for (int cwPitch = 300; cwPitch <= 1000; cwPitch += 50)
            {
                ComboBoxCWPitch.Items.Add(cwPitch.ToString() + @"Hz");
            }
            ComboBoxCWPitch.SelectedIndex = 8;


            radioButtonAnswerBefore.IsChecked = true;

            //--------------------------------------------------------------------------------------------------------------------
            //DataGridMemoryChannel.Columns[0].Width = 10;
            //var rows = DataGridMemoryChannel.ItemsSource
            //var columns = rows; // rows.SelectMany(d => d.Keys).Distinct(StringComparer.OrdinalIgnoreCase);
            //var column = new DataGridTextColumn();
            //column.Header = $"列{0}";
            //DataGridMemoryChannel.Columns.Append(column);
            //DataGridMemoryChannel.Columns[0].HeaderText = "教科";
            //DataGridColumn<MemoryChannel> dgc = new DataGridColumn<MemoryChannel>;
            //DataGridMemoryChannel.Columns.Clear();
            //DataGridMemoryChannel.Columns.Add()
            //dgc.Header
            //DataGridMemoryChannel.Columns[0] = DataGridTemplateColumn.HeaderProperty
            //--------------------------------------------------------------------------------------------------------------------
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("POWER SWITCH");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonPowerSwitch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        public void ButtonSpeedDown_Click(object sender, RoutedEventArgs e)
        {
            //-------------------------------------------------------------------------------------------------
            var modem = new MorseModem();

            int speedS = (int)Convert.ToInt16(TextBoxSpeedStart.Text);
            int speedE = (int)Convert.ToInt16(TextBoxSpeedEnd.Text);
            int speedD = (int)Convert.ToInt16(TextBoxSpeedDecrement.Text);

            int repeatCycles = (int)Convert.ToInt16(TextBoxRepeatCycles.Text);

            string hz = ComboBoxCWPitch.Text;
            string freq = hz.Remove(hz.Length - 2);
            int cwPitch = int.Parse(freq);

            string mem = string.Format("Tone={0}, WPM Start={1}, End={2}, Decrement={3}, Cycles={4} ", hz, speedS, speedE, speedD, repeatCycles);
            ListViewMorseDebug.Items.Add(mem);

            //bool isFinished = false;
            //for (int i = 0; i < repeatCycles; i++)
            //{

            var messageABC = generateRandomCall();

            if ((bool)radioButtonAnswerBefore.IsChecked)
            {
                Console.WriteLine("radioButtonAnswerBefore");

                textBoxSystemAnswer.Text = messageABC.ToString();

            }

            var morseCode = modem.ConvertToMorseCode(messageABC);

            Console.WriteLine($"Morse Code for Sentence : {messageABC}");
            Console.WriteLine(morseCode);

            //textBox1.Text += $"Morse Code for Sentence : {messageABC}\r\n";

            Console.WriteLine("Playing from Morse Code.");




            bool answerBefore = false;

            //-------------------------------------------------------------------------------------------------

            _ = modem.PlayMorseTone(morseCode, cwPitch, speedS, speedE, speedD, answerBefore, messageABC);

            //-------------------------------------------------------------------------------------------------

            if ((bool)radioButtonAnswerAllAfter.IsChecked)
            {
                Console.WriteLine("radioButtonAnswerAllAfter");

                textBoxSystemAnswer.Text = messageABC.ToString();

            }
            // }
        }

        private string generateRandomCall()
        {

            string[] callJPN = {
                "JA", "JB", "JC", "JD", "JE", "JF", "JG", "JH", "JI", "JJ", "JK", "JL", "JM", "JN", "JO", "JP", "JQ", "JR", "JS",
                "7J", "7K", "7L", "7M", "7N",
                "8J", "8K", "8L", "8M", "8N" }; //JA - JS 日本 7J - 7N 日本 8J - 8N 日本

            string[] callABC = {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            string[] powerClass = {
                "H", "M", "L", "P" };

            /*
            (注1)空中線電力別の表記(アルファベット)については、次の区分による。
            100Ｗ超 Ｈ
            20Ｗを超え100Ｗ以下 Ｍ
            5Ｗを超え20Ｗ以下 Ｌ
            5Ｗ以下 Ｐ
            */
            string[] domainPrefNumberStr = {
                "101","102","103","104","105","106","107","108","109","110","111","112","113","114",
                "02","03","04","05","06","07","08","09","10","11","12","13","14","15","16","17","18","19","20",
                "21","22","23","24","25","26","27","28","29","30","31","32","33","34","35","36","37","38","39","40",
                "41","42","43","44","45","46","47","48" };

            string[] domainPrefName = {
                "宗谷","留萌","上川","オホーツク","空知","石狩","根室","後志","十勝","釧路",
                "日高","胆振","桧山","渡島",
                "青森","岩手","秋田","山形","宮城","福島","新潟","長野","東京",
                "神奈川","千葉","埼玉","茨城","栃木","群馬","山梨","静岡","岐阜","愛知",
                "三重","京都","滋賀","奈良","大阪","和歌山","兵庫","富山","福井","石川",
                "岡山","島根","山口","鳥取","広島","香川","徳島","愛媛","高知","福岡",
                "佐賀","長崎","熊本","大分","宮崎","鹿児島","沖縄","小笠原" };

            /*
            都府県・地域等のナンバー・リスト
            ■北海道の地域
            宗谷 101,留 萌 102, 上 川 103, オホーツク   104, 空 知 105, 石 狩 106, 根 室 107, 後 志 108, 十 勝 109, 釧 路 110, 
            日 高 111, 胆 振 112, 桧 山 113, 渡 島 114 
            ■都府県
             青森 02, 岩 手 03, 秋 田 04, 山 形 05, 宮 城 06, 福 島 07, 新 潟 08, 長 野 09, 東 京 10, 
             神奈川   11, 千 葉 12, 埼 玉 13, 茨 城 14, 栃 木 15, 群 馬 16, 山 梨 17, 静 岡 18, 岐 阜 19, 愛 知 20, 
             三 重 21, 京 都 22, 滋 賀 23, 奈 良 24, 大 阪 25, 和歌山   26, 兵 庫 27, 富 山 28, 福 井 29, 石 川 30, 
             岡 山 31, 島 根 32, 山 口 33, 鳥 取 34, 広 島 35, 香 川 36, 徳 島 37, 愛 媛 38, 高 知 39, 福 岡 40, 
             佐 賀 41, 長 崎 42, 熊 本 43, 大 分 44, 宮 崎 45, 鹿児島 46, 沖 縄 47, 小笠原 48, 
             (注）北海道・札幌市は石狩地域の区域、沖ﾉ鳥島、南鳥島、硫黄島は小笠原の区域に含まれる。
            */

            //シード値(1000)を使用して初期化
            //シード値が変わらなければ毎回同じ乱数を返す
            //System.Random r = new System.Random(1000);
            //シード値を指定しないとシード値として Environment.TickCount が使用される

            System.Random r = new System.Random();

            int intPrefix = r.Next(callJPN.Count());
            int intAreaNumber = r.Next(10);
            int intSurfix1 = r.Next(callABC.Count());
            int intSurfix2 = r.Next(callABC.Count());
            int intSurfix3 = r.Next(callABC.Count());

            int intPref = r.Next(domainPrefNumberStr.Count());
            int intPower = r.Next(powerClass.Count());

            string genCall = callJPN[intPrefix] + intAreaNumber.ToString() + callABC[intSurfix1] + callABC[intSurfix2] + callABC[intSurfix3]
                + @" 5NN " + domainPrefNumberStr[intPref] + powerClass[intPower] + @" K";

            Console.WriteLine(genCall);

            return genCall;

        }

        private void SliderSpeedStart_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Slider scv = (Slider)sender;
            //scv.s
            //scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            //e.Handled = true;
        }

        private void SliderSpeedStart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) ScrollBarSpeedStart.Value++;
            else ScrollBarSpeedStart.Value--;
        }

        private void SliderSpeedEnd_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) ScrollBarSpeedEnd.Value++;
            else ScrollBarSpeedEnd.Value--;
        }

        private void SliderSpeedDecrement_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) ScrollBarSpeedDecrement.Value++;
            else ScrollBarSpeedDecrement.Value--;
        }

        private void ScrollBarRepeatCycles_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) ScrollBarRepeatCycles.Value++;
            else ScrollBarRepeatCycles.Value--;
        }

        private void ScrollBarSpeedDecrement_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) ScrollBarSpeedDecrement.Value++;
            else ScrollBarSpeedDecrement.Value--;
        }

        private void ScrollBarSpeedEnd_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) ScrollBarSpeedEnd.Value++;
            else ScrollBarSpeedEnd.Value--;
        }

        private void ScrollBarSpeedStart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) ScrollBarSpeedStart.Value++;
            else ScrollBarSpeedStart.Value--;
        }

        private void ComboBoxCWPitch_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) ComboBoxCWPitch.SelectedIndex++;
            else
            {
                if (0 < ComboBoxCWPitch.SelectedIndex)
                {
                    ComboBoxCWPitch.SelectedIndex--;
                }
            }

            //Console.WriteLine(e.Delta + " " + ComboBoxCWPitch.SelectedIndex);
        }

        //===========================================================================================================================ALERT

        List<UDPModel.StatusModel> itemsStatusModel = new List<UDPModel.StatusModel>();
        List<UDPModel.DecodeModel> itemsDecodeModel = new List<UDPModel.DecodeModel>();
        private System.Net.Sockets.UdpClient udpClient = null;
        private DispatcherTimer _timer;
        //List<UDPModel.HeartbeatModel> itemsHeartbeatModel = new List<UDPModel.HeartbeatModel>();

        private void ButtonAlertConnect2Mshv_Click(object sender, RoutedEventArgs e)
        {
            //ListenBroadcastMessage();
            //RecieveUDPMessage();
            //DeserializeUDPMessage();
            //Task t = await OpenUDP(2237);

            DataGridAlertResult.ItemsSource = itemsDecodeModel;
            //ListViewAlertDebug.ItemsSource = itemsHeartbeatModel;

            SetupTimer();
            Console.WriteLine("ButtonAlertConnect2Mshv_Click SetupTimer");
        }

        private void ButtonStopAlert_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Console.WriteLine("ButtonStopAlert_Click _timer.Stop()");
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 15);
            _timer.Tick += new EventHandler(MyTimerMethod);
            _timer.Start();
            Console.WriteLine("SetupTimer _timer.Start()");

            // 画面が閉じられるときに、タイマを停止
            this.Closing += new CancelEventHandler(StopTimer);
        }

        private void MyTimerMethod(object sender, EventArgs e)
        {
            Console.WriteLine("MyTimerMethod Start");
            if (udpClient != null)
            {
                return;
            }

            //((Button)sender).IsEnabled = false;
            System.Net.IPEndPoint localEP = new System.Net.IPEndPoint(System.Net.IPAddress.Any, int.Parse("2237"));
            udpClient = new System.Net.Sockets.UdpClient(localEP);


            DataGridAlertResult.Items.Refresh();

            //非同期的なデータ受信を開始する
            udpClient.BeginReceive(ReceiveUDPCallback, udpClient);
        }

        private void ReceiveUDPCallback(IAsyncResult ar)
        {
            System.Net.Sockets.UdpClient udp = (System.Net.Sockets.UdpClient)ar.AsyncState;

            //非同期受信を終了する
            System.Net.IPEndPoint remoteEP = null;
            byte[] rcvBytes;

            try
            {
                rcvBytes = udp.EndReceive(ar, ref remoteEP);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.WriteLine("受信エラー({0}/{1})", ex.Message, ex.ErrorCode);
                return;
            }
            catch (ObjectDisposedException ee)
            {
                Console.WriteLine("Socketは閉じられています。");
                Console.WriteLine("Caught: {0}", ee.Message);
                return;
            }

            string rcvMsg = System.Text.Encoding.UTF8.GetString(rcvBytes);
            string displayMsg = string.Format("[{0} ({1})] > {2}", remoteEP.Address, remoteEP.Port, rcvMsg);
            string rcvHEX = BitConverter.ToString(rcvBytes);

            Console.WriteLine(rcvHEX);
            Console.WriteLine(displayMsg);

            //-----------------------------------------------

            UDPMessageUtils nmu = new UDPMessageUtils();
            nmu.gIndex = 0;

            //UDPModel.HeartbeatModel hm;
            //UDPModel.StatusModel sm;
            UDPModel.DecodeModel dm;

            uint iMagic = nmu.Unpack4uint(rcvBytes, "magic");
            uint iSchema = nmu.Unpack4uint(rcvBytes, "schema");
            uint iMessageType = nmu.Unpack4uint(rcvBytes, "messageType");
            Console.WriteLine("iMessageType {0}", iMessageType);

            switch (iMessageType)
            {
                case 0://Heartbeat Out/In 0 quint32

                    string id0 = nmu.Unpackstring(rcvBytes, "id");//Id (unique key) utf8
                    uint maxsch = nmu.Unpack4uint(rcvBytes, "maxscheme");//Maximum schema number quint32
                    string version = nmu.Unpackstring(rcvBytes, "version");//version utf8
                    string revision = nmu.Unpackstring(rcvBytes, "revision");//revision utf8

                    string hm = string.Format("{0} {1} {2} {3}", id0, maxsch, version, revision);

                    ListViewAlertDebug.Dispatcher.Invoke(() =>
                    {
                        ListViewAlertDebug.Items.Add(hm);
                        Console.WriteLine(ListViewAlertDebug.Items.Count);
                        ListViewAlertDebug.Items.Refresh();

                        if (ListViewAlertDebug.Items.Count > 0)
                        {
                            var border = VisualTreeHelper.GetChild(ListViewAlertDebug, 0) as Decorator;
                            if (border != null)
                            {
                                var scroll = border.Child as ScrollViewer;
                                if (scroll != null) scroll.ScrollToEnd();
                            }
                        }
                    });

                    break;

                case 1:                                                                     //Status Out 1 quint32
                    string id1 = nmu.Unpackstring(rcvBytes, "id1");                         //Id (unique key) utf8

                    if (id1.Contains("MSHV"))
                    {
                        UInt64 DialFrequency = nmu.Unpack8uint(rcvBytes, "DialFrequency");      //Dial Frequency (Hz) quint64
                        string Mode = nmu.Unpackstring(rcvBytes, "Mode");                       //Mode utf8
                        string DXcall = nmu.Unpackstring(rcvBytes, "DXcall");                   //DX call utf8
                        string Report = nmu.Unpackstring(rcvBytes, "Report");                   //Report utf8
                        string TxMode = nmu.Unpackstring(rcvBytes, "TxMode");                   //Tx Mode utf8
                        bool TxEnbledBool = nmu.Unpackbool(rcvBytes, "TxEnbledBool");           //Tx Enabled bool
                        bool TransmittingBool = nmu.Unpackbool(rcvBytes, "TransmittingBool");   //Transmitting bool
                        bool DecodingBool = nmu.Unpackbool(rcvBytes, "DecodingBool");           //Decoding bool
                        int RxDF = nmu.Unpack4int(rcvBytes, "RxDF");                            //Rx DF qint32
                        int TxDF = nmu.Unpack4int(rcvBytes, "TxDF");                            //Tx DF qint32
                        string DEcall = nmu.Unpackstring(rcvBytes, "DEcall");                   //DE call utf8
                        string DEgrid = nmu.Unpackstring(rcvBytes, "DEgrid");                   //DE grid utf8
                        string DXgrid = nmu.Unpackstring(rcvBytes, "DXgrid");                   //DX grid utf8
                        bool TxWatchingBool = nmu.Unpackbool(rcvBytes, "TxWatchingBool");       //Tx Watchdog bool

                        string SubMode = nmu.Unpackstring(rcvBytes, "SubMode");                 //Sub-mode utf8
                        bool FastModeBool = nmu.Unpackbool(rcvBytes, "FastModeBool");           //Fast mode bool
                        int SpecialOpMode = nmu.Unpack1int(rcvBytes, "SpecialOpMode");          //Special operation mode quint8

                        string sm = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16} {17} ",
                            id1, DialFrequency, Mode, DXcall, Report, TxMode, TxEnbledBool, TransmittingBool, DecodingBool, RxDF, TxDF,
                            DEcall, DEgrid, DXgrid, TxWatchingBool, SubMode, FastModeBool, SpecialOpMode);

                        ListViewAlertDebug.Dispatcher.Invoke(() =>
                        {
                            ListViewAlertDebug.Items.Add(sm);
                            Console.WriteLine(ListViewAlertDebug.Items.Count);
                            ListViewAlertDebug.Items.Refresh();

                            if (ListViewAlertDebug.Items.Count > 0)
                            {
                                var border = VisualTreeHelper.GetChild(ListViewAlertDebug, 0) as Decorator;
                                if (border != null)
                                {
                                    var scroll = border.Child as ScrollViewer;
                                    if (scroll != null) scroll.ScrollToEnd();
                                }
                            }
                        });
                    }
                    else if (id1.Contains("WSJT"))
                    {
                        UInt64 DialFrequency = nmu.Unpack8uint(rcvBytes, "DialFrequency");      //Dial Frequency (Hz) quint64
                        string Mode = nmu.Unpackstring(rcvBytes, "Mode");                       //Mode utf8
                        string DXcall = nmu.Unpackstring(rcvBytes, "DXcall");                   //DX call utf8
                        string Report = nmu.Unpackstring(rcvBytes, "Report");                   //Report utf8
                        string TxMode = nmu.Unpackstring(rcvBytes, "TxMode");                   //Tx Mode utf8
                        bool TxEnbledBool = nmu.Unpackbool(rcvBytes, "TxEnbledBool");           //Tx Enabled bool
                        bool TransmittingBool = nmu.Unpackbool(rcvBytes, "TransmittingBool");   //Transmitting bool
                        bool DecodingBool = nmu.Unpackbool(rcvBytes, "DecodingBool");           //Decoding bool
                        uint RxDF = nmu.Unpack4uint(rcvBytes, "RxDF");                          //Rx DF qint32 >>> quint32
                        uint TxDF = nmu.Unpack4uint(rcvBytes, "TxDF");                          //Tx DF qint32 >>> quint32
                        string DEcall = nmu.Unpackstring(rcvBytes, "DEcall");                   //DE call utf8
                        string DEgrid = nmu.Unpackstring(rcvBytes, "DEgrid");                   //DE grid utf8
                        string DXgrid = nmu.Unpackstring(rcvBytes, "DXgrid");                   //DX grid utf8
                        bool TxWatchingBool = nmu.Unpackbool(rcvBytes, "TxWatchingBool");       //Tx Watchdog bool
                        string SubMode = nmu.Unpackstring(rcvBytes, "SubMode");                 //Sub-mode utf8
                        bool FastModeBool = nmu.Unpackbool(rcvBytes, "FastModeBool");           //Fast mode bool
                        int SpecialOpMode = nmu.Unpack1int(rcvBytes, "SpecialOpMode");          //Special operation mode quint8

                        int FrequencyTolerance = nmu.Unpack4int(rcvBytes, "FrequencyTolerance");          //Frequency Tolerance quint32
                        int TRPeriod = nmu.Unpack4int(rcvBytes, "TRPeriod");                                  //T / R Period quint32
                        string ConfigurationName = nmu.Unpackstring(rcvBytes, "ConfigurationName");          //Configuration Name utf8

                        string sm = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16} {17} {18} {19} {20} ",
                            id1, DialFrequency, Mode, DXcall, Report, TxMode, TxEnbledBool, TransmittingBool, DecodingBool, RxDF, TxDF,
                            DEcall, DEgrid, DXgrid, TxWatchingBool, SubMode, FastModeBool, SpecialOpMode, FrequencyTolerance, TRPeriod, ConfigurationName);

                        ListViewAlertDebug.Dispatcher.Invoke(() =>
                        {
                            ListViewAlertDebug.Items.Add(sm);
                            Console.WriteLine(ListViewAlertDebug.Items.Count);
                            ListViewAlertDebug.Items.Refresh();

                            if (ListViewAlertDebug.Items.Count > 0)
                            {
                                var border = VisualTreeHelper.GetChild(ListViewAlertDebug, 0) as Decorator;
                                if (border != null)
                                {
                                    var scroll = border.Child as ScrollViewer;
                                    if (scroll != null) scroll.ScrollToEnd();
                                }
                            }
                        });
                    }
                    else
                    {
                        Console.WriteLine("*** Illegal id1 {0} Found. It is not MSHV and WSJT.", id1);
                    }

                    break;

                case 2:
                    string id2 = nmu.Unpackstring(rcvBytes, "id");
                    bool boNew = nmu.Unpackbool(rcvBytes, "isNew");
                    DateTime datetm = nmu.UnpackDateTime(rcvBytes, "tm");
                    int SNR = nmu.Unpack4int(rcvBytes, "snr");
                    float DT = nmu.Unpack8float(rcvBytes, "dt");
                    uint DF = nmu.Unpack4uint(rcvBytes, "df");
                    string MODE = nmu.Unpackstring(rcvBytes, "mode");
                    string Message = nmu.Unpackstring(rcvBytes, "message");

                    dm = new UDPModel.DecodeModel()
                    {
                        //heartbeat_client_id = iMagic,
                        //heartbeat_maximum_schema_number = iSchema,
                        decode_client_id = id2,
                        decode_new = boNew,
                        decode_time = datetm,
                        decode_snr = SNR,
                        decode_delta_time = DT,
                        decode_delta_frequency = DF,
                        decode_mode = MODE,
                        decode_message = Message
                    };

                    DataGridAlertResult.Dispatcher.Invoke(() =>
                    {
                        itemsDecodeModel.Add(dm);
                        Console.WriteLine(itemsDecodeModel.Count);

                        DataGridAlertResult.ItemsSource = itemsDecodeModel;
                        DataGridAlertResult.Items.Refresh();
                        //DataGridAlertResult.UpdateLayout();

                        if (DataGridAlertResult.Items.Count > 0)
                        {
                            var border = VisualTreeHelper.GetChild(DataGridAlertResult, 0) as Decorator;
                            if (border != null)
                            {
                                var scroll = border.Child as ScrollViewer;
                                if (scroll != null) scroll.ScrollToEnd();
                            }
                        }
                    });
                    break;

                default:
                    Console.WriteLine("*** iMessageType {0} Found", iMessageType);
                    break;
            }

            //再びデータ受信を開始する
            udp.BeginReceive(ReceiveUDPCallback, udp);
            Console.WriteLine("BeginReceive Again");
        }

        private void StopTimer(object sender, CancelEventArgs e)// タイマを停止
        {
            _timer.Stop();
            Console.WriteLine("StopTimer _timer.Stop()");
        }

        public async Task OpenUDP(int port)
        {
            using (var client = new UdpClient(new IPEndPoint(IPAddress.Any, port)))
            {
                while (true)
                {
                    var response = await client.ReceiveAsync();
                }
            }
        }

        private void ListenBroadcastMessage()
        {
            // 送受信に利用するポート番号
            var port = 2237;

            // ブロードキャストを監視するエンドポイント
            var remote = new IPEndPoint(IPAddress.Any, port);

            // UdpClientを生成
            var client = new UdpClient(port);

            // データ受信を待機（同期処理なので受信完了まで処理が止まる）
            // 受信した際は、 remote にどの IPアドレス から受信したかが上書きされる
            var buffer = client.Receive(ref remote);

            // 受信データを変換
            var data = Encoding.UTF8.GetString(buffer);
            Console.WriteLine(data);
            Console.WriteLine(remote.Address.ToString());

            // 受信イベントを実行
            this.OnReceive(data);
        }

        private void OnReceive(string data)
        {
            Console.WriteLine(data);
            ListViewAlertDebug.Items.Add(data);
        }

        private void RecieveUDPMessage()
        {
            string localIpString = "127.0.0.1";
            int localPort = 2237;

            System.Net.IPAddress localAddress = System.Net.IPAddress.Parse(localIpString);
            System.Net.IPEndPoint localEP = new System.Net.IPEndPoint(localAddress, localPort);
            System.Net.Sockets.UdpClient udp = new System.Net.Sockets.UdpClient(localEP);

            for (; ; )
            {
                System.Net.IPEndPoint remoteEP = null;
                byte[] rcvBytes = udp.Receive(ref remoteEP);
                //var data = udp.Receive(ref remoteEP);

                string rcvMsg = System.Text.Encoding.UTF8.GetString(rcvBytes);
                string rcvHEX = BitConverter.ToString(rcvBytes);

                Console.WriteLine("受信したデータ:{0} {1}", rcvMsg.Length, rcvMsg);
                Console.WriteLine("受信したデータ:{0} {1}", rcvHEX.Length, rcvHEX);
                Console.WriteLine("送信元アドレス:{0}/ポート番号:{1}", remoteEP.Address, remoteEP.Port);

                //"exit"を受信したら終了
                if (rcvMsg.Equals("exit"))
                {
                    break;
                }
            }

            //UdpClientを閉じる
            udp.Close();

            Console.WriteLine("終了しました。");
            Console.ReadLine();
        }

        public string bytesToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(2 * bytes.Length);
            foreach (byte b in bytes)
            {
                sb.Append(String.Format("%02x", b & 0xff));
            }
            return sb.ToString();
        }

        /*
        private void DeserializeUDPMessage()
        {
            var formatter = new BinaryFormatter();
            FileStream v1File = null;
            try
            {
                v1File = null;
                v1File = new FileStream(@"..\..\..\Output\v1Output.bin", FileMode.Open);
                var data = formatter.Deserialize(v1File);
                var udpm = data as UDPModel;

                Console.WriteLine("Name={0} , Address={1}, BirthDate={2}", 
                    udpm.heartbeat_client_id, udpm.heartbeat_maximum_schema_number.ToString(),
                    udpm.heartbeat_revision, udpm.heartbeat_version);
            }
            catch (Exception ex)
            {
                Console.WriteLine("エラー={0}", ex.Message);
            }
            finally
            {
                if (v1File != null)
                    v1File.Close();
            }
            Console.WriteLine("Complete deseriazed!");
        }
        */

        /*
        public int no { get; set; }
        //--------------------------------------------------------------------His Info
        public string call { get; set; }
        public string gridsquare { get; set; }

        //--------------------------------------------------------------------QSO Info
        public string rst_sent { get; set; }
        public string rst_rcvd { get; set; }

        public string qso_date { get; set; }//20181228
        public string time_on { get; set; }//0426
        public string qso_date_off { get; set; }
        public string time_off { get; set; }

        public string band { get; set; }
        public string freq { get; set; }//14.074
        public string mode { get; set; }//FT8 
                                        //--------------------------------------------------------------------My Info
        public string station_callsign { get; set; }//JA1AAA
        public string operatorname { get; set; }
        public string my_gridsquare { get; set; }//
        */

        //=========================================================================================================================== PSKReporter


        private void SetPSKTabInitially()
        {
            ComboBoxCallSign.Items.Add("-");
            ComboBoxCallSign.Items.Add(GetRandomCallSign());
            ComboBoxCallSign.SelectedIndex = 1;

            ComboBoxSenderCallsign.Items.Add("-");
            ComboBoxSenderCallsign.SelectedIndex = 0;

            ComboBoxReceiverCallsign.Items.Add("-");
            ComboBoxReceiverCallsign.SelectedIndex = 0;

            string[] arrMode = { "all", "FT8", "FT4", "CW", "SSB" };
            foreach (var mode1 in arrMode)
            {
                ComboBoxMode.Items.Add(mode1);
            }
            ComboBoxMode.SelectedIndex = 0;

            string[] arrBand = { "all", "160m", "80m", "60m", "40m", "30m", "20m", "17m", "15m", "12m", "11m", "10m", "6m", "4m", "2m", "70cm" };
            foreach (var band1 in arrBand)
            {
                ComboBoxFBand.Items.Add(band1);
            }
            ComboBoxFBand.SelectedIndex = 0;

            string[] arrRange = { "all", "1810000-1912500", "3500000-3805000", "5351500-5366500", "7000000-7200000", "10100000-10150000", 
                "14000000-14350000", "18068000-18168000", "21000000-21450000", "24890000-24990000", "26000000-27900000", "28000000-29700000", 
                "50000000-54000000","70000000-70500000", "144000000-146000000", "430000000-440000000" };
            foreach (var range1 in arrRange)
            {
                ComboBoxFRange.Items.Add(range1);
            }
            ComboBoxFRange.SelectedIndex = 0;

            ComboBoxRronly.Items.Add("false");
            ComboBoxRronly.Items.Add("true");
            ComboBoxRronly.SelectedIndex = 0;
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

        private void ButtonGetPSKxml_Click(object sender, RoutedEventArgs e)
        {
            ButtonGetPSKxml.Background = Brushes.Red;

            PSKDataReader pskr = new PSKDataReader();
            pskr.GetPSKStream();

            //pskr.XMLproc();          
        }

        private void ButtonDiag1_Click(object sender, RoutedEventArgs e)
        {
            var window = new DialogWindow1();
            window.TectBlockClickButton.Text = (sender as Button).Content + "が押されました。";
            bool? res = window.ShowDialog();
            if (res == true)
            {
                TextBlockMSG.Text = "OKでダイアログが終了。";
            }
            else
            {
                TextBlockMSG.Text = "キャンセルでダイアログが終了。";
            }
        }

        private void ButtonDiag2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonNextDiag_Click(object sender, RoutedEventArgs e)
        {
            var window = new PSKWindow1();
            window.Show();
            this.Hide();
        }

        private string appText;

        private void ButtonGetPSKxml_Click_1(object sender, RoutedEventArgs e)
        {
            appText = appText + @"abc ";
            TextBoxPSKMSG.AppendText(appText + "\r\n");
            TextBoxPSKMSG.ScrollToEnd();
        }

        private void ButtonPSKinfo_Click(object sender, RoutedEventArgs e)
        {
            appText = appText + @"xyz ";
            TextBoxPSKMSG.AppendText(appText + "\n");
            TextBoxPSKMSG.ScrollToEnd();
        }

        private void DataGridReceptionReport_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            Style h_Center = new Style();
            h_Center.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));

            Style h_Right = new Style();
            h_Right.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));

            Style h_Stretch = new Style();
            h_Stretch.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));

            Style v_ctr = new Style();
            v_ctr.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));

            if (headername == "MiddleName")
            {
                e.Cancel = true;
            }
            switch (headername)
            {
                case "receiverCallsign": e.Column.Header = "recCall"; e.Column.CellStyle = h_Right; break;
                case "receiverLocator": e.Column.Header = "recLoc"; e.Column.CellStyle = h_Right; break;
                case "senderCallsign": e.Column.Header = "senCall"; e.Column.CellStyle = h_Right; break;
                case "senderLocator": e.Column.Header = "senLoc"; e.Column.CellStyle = h_Right; break;
                case "frequency": e.Column.Header = "Freq"; e.Column.CellStyle = h_Right; break;
                case "diff_frequency": e.Column.Header = "Diff"; e.Column.CellStyle = h_Right; break;
                case "flowStartSeconds": e.Column.Header = "Start"; e.Column.CellStyle = h_Right; break;
                case "mode": e.Column.Header = "Mode"; e.Column.CellStyle = h_Right; break;
                case "isSender": e.Column.Header = "isSen"; e.Column.CellStyle = h_Right; break;
                case "isReceiver": e.Column.Header = "isRec"; e.Column.CellStyle = h_Right; break;
                case "senderRegion": e.Column.Header = "senRegion"; e.Column.CellStyle = h_Right; break;
                case "senderDXCC": e.Column.Header = "senDXCC"; e.Column.CellStyle = h_Right; break;
                case "senderDXCCCode": e.Column.Header = "Code"; e.Column.CellStyle = h_Right; break;
                case "senderDXCCLocator": e.Column.Header = "Loc"; e.Column.CellStyle = h_Right; break;
                case "senderLotwUpload": e.Column.Header = "LOTW"; e.Column.CellStyle = h_Right; break;
                case "senderEqslAuthGuar": e.Column.Header = "eQSL"; e.Column.CellStyle = h_Right; break;
                case "sNR": e.Column.Header = "sNR"; e.Column.CellStyle = h_Right; break;

                default: e.Column.Header = "default"; e.Column.CellStyle = h_Right; break;
            }
        }

        private void DataGridActiveReceiver_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            Style h_Center = new Style();
            h_Center.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));

            Style h_Right = new Style();
            h_Right.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));

            Style h_Stretch = new Style();
            h_Stretch.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));

            Style v_ctr = new Style();
            v_ctr.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));

            if (headername == "MiddleName")
            {
                e.Cancel = true;
            }
            switch (headername)
            {
                case "callsign": e.Column.Header = "Call"; e.Column.CellStyle = h_Right; break;
                case "locator": e.Column.Header = "Loc"; e.Column.CellStyle = h_Right; break;
                case "frequency": e.Column.Header = "Freq"; e.Column.CellStyle = h_Right; break;
                case "diff_frequency": e.Column.Header = "Diff"; e.Column.CellStyle = h_Right; break;
                case "region": e.Column.Header = "Region"; e.Column.CellStyle = h_Right; break;
                case "DXCC": e.Column.Header = "DXCC"; e.Column.CellStyle = h_Right; break;
                case "decoderSoftware": e.Column.Header = "Software"; e.Column.CellStyle = h_Right; break;
                case "antennaInformation": e.Column.Header = "Antenna"; e.Column.CellStyle = h_Right; break;
                case "mode": e.Column.Header = "Mode"; e.Column.CellStyle = h_Right; break;

                default: e.Column.Header = "default"; e.Column.CellStyle = h_Right; break;
            }
        }

        private void DataGridActiveCallsign_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            Style h_Center = new Style();
            h_Center.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));

            Style h_Right = new Style();
            h_Right.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));

            Style h_Stretch = new Style();
            h_Stretch.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));

            Style v_ctr = new Style();
            v_ctr.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));

            if (headername == "MiddleName")
            {
                e.Cancel = true;
            }
            switch (headername)
            {
                case "callsign": e.Column.Header = "Call"; e.Column.CellStyle = h_Right; break;
                case "reports": e.Column.Header = "sNR"; e.Column.CellStyle = h_Right; break;
                case "DXCC": e.Column.Header = "DXCC"; e.Column.CellStyle = h_Right; break;
                case "DXCCcode": e.Column.Header = "Code"; e.Column.CellStyle = h_Right; break;
                case "frequency": e.Column.Header = "Freq"; e.Column.CellStyle = h_Right; break;
                case "diff_frequency": e.Column.Header = "Diff"; e.Column.CellStyle = h_Right; break;

                default: e.Column.Header = "default"; e.Column.CellStyle = h_Right; break;
            }
        }

        //static readonly HttpClient hClient = new HttpClient();

        private void DataGridReceptionReport_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var elem = e.MouseDevice.DirectlyOver as FrameworkElement;
            if (elem != null)
            {
                DataGridCell cell = elem.Parent as DataGridCell;
                if (cell == null)
                {
                    // ParentでDataGridCellが拾えなかった時はTemplatedParentを参照
                    // （Borderをダブルクリックした時）
                    cell = elem.TemplatedParent as DataGridCell;
                }
                if (cell != null)
                {
                    PSKDataItem.receptionReport rp = new PSKDataItem.receptionReport();

                    rp = (PSKDataItem.receptionReport)cell.DataContext;
                    string r = "https://www.qrz.com/db/" + rp.receiverCallsign ;

                    Process.Start(r);

                    string s = "https://www.qrz.com/db/" + rp.senderCallsign;
                    Process.Start("Chrome", s);
                }
            }
        }

        private void DataGridReceptionReport_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = false;

            var sortDir = e.Column.SortDirection;

            if (ListSortDirection.Ascending != sortDir)sortDir = ListSortDirection.Ascending;
            else sortDir = ListSortDirection.Descending;
            /*
            if (ListSortDirection.Ascending == sortDir)
            {
                if ("Column1" == e.Column.SortMemberPath) DataGridReceptionReport.ItemsSource = TestList.OrderBy(c => c.Column1).Take(13);
                if ("Column2" == e.Column.SortMemberPath) DataGridReceptionReport.ItemsSource = TestList.OrderBy(c => c.Column2).Take(13);
                if ("Column3" == e.Column.SortMemberPath) DataGridReceptionReport.ItemsSource = TestList.OrderBy(c => c.Column3).Take(13);
                if ("Column4" == e.Column.SortMemberPath) DataGridReceptionReport.ItemsSource = TestList.OrderBy(c => c.Column4).Take(13);
            }
            else
            {
                if ("Column1" == e.Column.SortMemberPath) DataGridReceptionReport.ItemsSource = TestList.OrderByDescending(c => c.Column1).Take(13);
                if ("Column2" == e.Column.SortMemberPath) DataGridReceptionReport.ItemsSource = TestList.OrderByDescending(c => c.Column2).Take(13);
                if ("Column3" == e.Column.SortMemberPath) DataGridReceptionReport.ItemsSource = TestList.OrderByDescending(c => c.Column3).Take(13);
                if ("Column4" == e.Column.SortMemberPath) DataGridReceptionReport.ItemsSource = TestList.OrderByDescending(c => c.Column4).Take(13);
            }
            */
            foreach (var column in DataGridReceptionReport.Columns)
            {
                if (column.SortMemberPath == e.Column.SortMemberPath)
                {
                    //column.SortDirection = sortDir;
                }
            }
        }

        private void ComboBoxCallSign_DropDownClosed(object sender, EventArgs e)// CallSign
        {
            if (ComboBoxCallSign.SelectedIndex != 0)// 0="-"
            {
                ComboBoxReceiverCallsign.SelectedIndex = 0;
                ComboBoxSenderCallsign.SelectedIndex = 0;
            }
        }

        private void ComboBoxSenderCallsign_DropDownClosed(object sender, EventArgs e)// SenderCallsign
        {
            if (ComboBoxSenderCallsign.SelectedIndex != 0)// 0="-"
            {
                ComboBoxCallSign.SelectedIndex = 0;
                ComboBoxReceiverCallsign.SelectedIndex = 0;
            }            
        }      

        private void ComboBoxReceiverCallsign_DropDownClosed(object sender, EventArgs e)// ReceiverCallsign
        {
            if (ComboBoxReceiverCallsign.SelectedIndex != 0)// 0="-"
            {
                ComboBoxCallSign.SelectedIndex = 0;
                ComboBoxSenderCallsign.SelectedIndex = 0;
            }
        }

        private void ComboBoxFBand_DropDownClosed(object sender, EventArgs e)// FBand
        {
            //if (ComboBoxFBand.SelectedIndex != 0)
            {
                ComboBoxFRange.SelectedIndex = ComboBoxFBand.SelectedIndex;
            }
        }

        private void ComboBoxFRange_DropDownClosed(object sender, EventArgs e)// FRange
        {
            //if (ComboBoxFRange.SelectedIndex != 0)
            {
                ComboBoxFBand.SelectedIndex = ComboBoxFRange.SelectedIndex;
            }
        }

        private void ButtonSpeedDownStop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonNewWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window2();
            window.Show();
        }
    }
}
