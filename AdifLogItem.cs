using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shvFT991A
{
    public class AdifLogItem
    {
        public int no { get; set; }
        //--------------------------------------------------------------------His Info
        public string call { get; set; }
        public string gridsquare { get; set; }
        public string distancekm { get; set; }
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
        public string submode { get; set; }//FT4 (mode=MFSK) 
        //--------------------------------------------------------------------My Info
        public string station_callsign { get; set; }//
        public string operator_name { get; set; }
        public string my_gridsquare { get; set; }//

        //--------------------------------------------------------------------Additional for QRZ.com
        public string app_qrzlog_logid { get; set; }		    //
        public string app_qrzlog_status { get; set; }           //N

        //--------------------------------------------------------------------His Info
        public string band_rx { get; set; }			            //20M
        public string cont { get; set; }                        //OC
        public string country { get; set; }                     //
        public string cqz { get; set; }						    //27
        public string dxcc { get; set; }                        //375
        public string freq_rx { get; set; }                     //14.074
        public string ituz { get; set; }                        //50
        public string lat { get; set; }                         //S000 00.000
        public string lon { get; set; }                         //W000 00.000
        public string lotw_qsl_sent { get; set; }               //Y    

        //--------------------------------------------------------------------My Info
        public string my_city { get; set; }                     //
        public string my_country { get; set; }                  //Japan
        public string my_cq_zone { get; set; }                  //25
        public string my_iota { get; set; }                     //AS-007
        public string my_itu_zone { get; set; }                 //45
        public string my_lat { get; set; }                      //N
        public string my_lon { get; set; }                      //E
        public string my_name { get; set; }                     //

        public string qrzcom_qso_upload_date { get; set; }      //20190102
        public string qrzcom_qso_upload_status { get; set; }    //Y

        public string qsl_rcvd { get; set; }                    //N
        public string qsl_sent { get; set; }                    //N

        public string comment { get; set; }
        public string distance { get; set; }
        public string email { get; set; }
        public string eqsl_qsl_rcvd { get; set; }
        public string eqsl_qsl_sent { get; set; }
        public string iota { get; set; }
        public string lotw_qsl_rcvd { get; set; }
        public string name { get; set; }
        public string qsl_via { get; set; }
        public string qth { get; set; }
        public string srx_string { get; set; }
        public string stx_string { get; set; }
        public string app_qrzlog_qsldate { get; set; }
        public string lotw_qslrdate { get; set; }

        public string qslrdate { get; set; }
        public string qslsdate { get; set; }

        public string cnty { get; set; }
        public string state { get; set; }

        public string rx_pwr { get; set; }
        public string tx_pwr { get; set; }
    }
}
 