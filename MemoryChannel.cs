using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static shvFT991A.MainWindow;

namespace shvFT991A
{
    class MemoryChannel
    {
        public int No { get; set; } // 1-117
        public int Freq { get; set; } //Hz
        public int ClarifierFreq { get; set; }
        public bool ClarifierSwitchRX { get; set; }
        public bool ClarifierSwitchTX { get; set; }
        public ModeKind ModeFreq { get; set; }
        public bool VfoOrMemory { get; set; }
        // false=VFO true=Memory
        public int CtcssDcsMode { get; set; }
        //別名CTCSS (Continuous Tone-Coded Squelch System)。別名DCS (Digital-Coded Squelch)。
        // 0=CTCSS OFF
        // 1=CTCSS ENC/DEC
        // 2=CTCSS ENC
        // 3=DCS ENC/DEC
        // 4=DCS ENC
        public int SimplexMode { get; set; }
        // 0=simplex 1=plus shift 2=minus shift
        public string MemoryTag { get; set; }
        /*
        public override string ToString()
        {
            //_ = (string)TypeDescriptor.GetConverter(ModeEnum).ConvertTo(ModeEnum, typeof(string));            
            string name = Enum.GetName(typeof(ModeEnum), 1 );
            return $"{No} - {(float)Freq/1000000.0:F3}, {ModeEnum}, {MemoryTag}";
        }
        */
        //Enum myServer = Servers.Exchange;
        //string myServerString = "BizTalk";
        //Console.WriteLine(TypeDescriptor.GetConverter(myServer).ConvertTo(myServer, typeof(string))); 
        //Console.WriteLine(TypeDescriptor.GetConverter(myServer).ConvertFrom(myServerString)); 
    }
    /*
    public enum ModeEnumBack
    {
        LSB         = 1,
        USB         = 2,
        CW          = 3,
        FM          = 4,
        AM          = 5,
        RTTY_LSB    = 6,
        CW_R        = 7,
        DATA_LSB    = 8,
        RTTY_USB    = 9,
        DATA_FM     = 10,
        FM_N        = 11,
        DATA_USB    = 12,
        AM_N        = 13,
        C4FM        = 14
    }
    */
}

