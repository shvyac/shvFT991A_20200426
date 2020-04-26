using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shvFT991A
{
    class QRZCallbookItem
    {
        /*
        ----------------------------------------------------------------------------------------------------------------------
                名称	都道府県	無線局の目的	免許の年月日
                ＊＊＊＊＊（XXXXXX）	
                アマチュア業務用	平26.6.18
                ＊＊＊＊＊（XXXXXX）	
                アマチュア業務用	平29.12.21
        ----------------------------------------------------------------------------------------------------------------------
                無線局の種別	アマチュア局	無線局の目的	アマチュア業務用	運用許容時間	常　時
                免許の年月日	平29.12.21	免許の有効期間	令4.12.20まで
                通信事項	アマチュア業務に関する事項
                通信の相手方	アマチュア局
                移動範囲	移動しない 

                無線設備の設置場所／常置場所
                
                電波の型式、周波数及び空中線電力
                3MA                     		
                        1910 kHz		
                          1  kW
                3HA                     		
                      3537.5 kHz		
                          1  kW
                3HD                     		
                        3798 kHz		
                          1  kW
                3HA                     		
                        7100 kHz		
                          1  kW
                2HC                     		
                       10125 kHz		
                          1  kW
                2HA                     		
                       14175 kHz		
                          1  kW
                3HA                     		
                       18118 kHz		
                          1  kW
        ----------------------------------------------------------------------------------------------------------------------
        */
        public string call_sign { get; set; }
        public string station_type { get; set; } //移動範囲	移動しない 
        public string station_address { get; set; } //
        public string address_code { get; set; } //
        public string bool_14mhz { get; set; }
        public string max_power_watt { get; set; } // 1kW
        public string checked_date { get; set; } // download date
    }
}

