using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shvFT991A
{
    class ContestResultItem
    {
        /*
                電話部門
                電話部門シングルオペオールバンド PA
                  1   JF2AAA/2                 575 ×  97 ＝      55,775    
                  2   JJ1AAA                   564 ×  87 ＝      49,068    
                  3   JR1AAA/1                 458 ×  74 ＝      33,892  
        */

        public int no { get; set; }
        public string bumon { get; set; }
        public string bumon_code { get; set; }
        public string rank { get; set; }
        public string call_sign { get; set; }
        public string total_point { get; set; }
        public string tx_power { get; set; }

    }
}
