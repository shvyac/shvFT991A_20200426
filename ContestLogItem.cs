using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shvFT991A
{
    public class ContestLogItem
    {
        //No.,DATE,TIME,DATE-TIME,BAND,MODE,CALLSIGN,SENT,No,RCVD,No,Mlt,Pts,OP,TX,memo,,,,,
        public string no { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string date_time { get; set; }
        public string band { get; set; }
        public string mode { get; set; }
        public string call_sign { get; set; }
        public string sent_rst { get; set; }
        public string sent_no { get; set; }
        public string rcvd_rst { get; set; }
        public string rcvd_no { get; set; }
        public string multi { get; set; }
        public string point { get; set; }
        public string operator_name { get; set; }
        public string tx { get; set; }
        public string memo { get; set; }

        //---------------------------------------------

        public string jarl_2018_power { get; set; }
        public string jarl_2018_result { get; set; }    //https://www.jarl.org/Japanese/1_Tanoshimo/1-1_Contest/all_ja/2018/entry.html

        public string soumu_pref { get; set; }    //https://www.tele.soumu.go.jp/

        public string qrz_com { get; set; }    //https://www.qrz.com/lookup

        public string eqsl_cc { get; set; }    //https://www.eqsl.cc/Member.cfm?JA1AAA

        public string ham_qth { get; set; }    //https://www.hamqth.com/ja1aaa

        //---------------------------------------------
    }
}