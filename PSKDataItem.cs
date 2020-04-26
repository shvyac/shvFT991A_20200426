using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shvFT991A
{
    class PSKDataItem
    {
        /* 
<activeReceiver callsign="NAXXX" locator="DM03tu58" frequency="7074804" region="California" DXCC="United States" decoderSoftware="WSJT-X v2.1.2 0068f9" antennaInformation="FLEX RADUI 660, EFV 31', GEMINI HF-1K AMP" mode="FT8"/>
        */

        public class activeReceiver
        {
            public string   callsign { get; set; } 
            public string   locator { get; set; } 
            public string frequency { get; set; } 
            public string diff_frequency { get; set; } 
            public string   region { get; set; }              //Texas
            public string   DXCC { get; set; }                //United States
            public string   decoderSoftware { get; set; }     //WSJT-X v2.1.2 0068f9
            public string   antennaInformation { get; set; }  //Dipole
            public string   mode { get; set; }                //FT8
        }

        /*
<receptionReport receiverCallsign="XXXXXX" receiverLocator="QM07ei" senderCallsign="XXXXXX" senderLocator="PM84QR" frequency="7041413" flowStartSeconds="1585372979" mode="FT8" isSender="1" isReceiver="0" senderRegion="Aichi" senderDXCC="Japan" senderDXCCCode="JA" senderDXCCLocator="PM" senderLotwUpload="2020-03-09" senderEqslAuthGuar="A" sNR="5"/>

Technical IPFIX Information
The attributes used for this application are:

Name	                Attribute Id	Type	            Meaning
senderCallsign	        30351.1	        string	            The callsign of the sender of the transmission.
receiverCallsign	    30351.2	        string	            The callsign of the receiver of the transmission.
senderLocator	        30351.3	        string	            The locator of the sender of the transmission.
receiverLocator	        30351.4	        string	            The locator of the receiver of the transmission.
frequency	            30351.5	        unsignedInteger	    The frequency of the transmission in Hertz.
sNR	                    30351.6	        integer	    T       he signal to noise ration of the transmission. Normally 1 byte.
iMD	                    30351.7	        integer	            The intermodulation distortion of the transmission. Normally 1 byte.
decoderSoftware	        30351.8	        string	            The name and version of the decoding software.
antennaInformation	    30351.9	        string	            A freeform description of the receiving antenna.
mode	                30351.10	    string	            The mode of the communication. One of the ADIF values for MODE or SUBMODE.
informationSource	    30351.11	    integer	            Identifies the source of the record. The bottom 2 bits have the following meaning: 
                                                            1 = Automatically Extracted. 
                                                            2 = From a Call Log (QSO). 
                                                            3 = Other Manual Entry. 
                                                            The 0x80 bit indicates that this record is a test transmission. Normally 1 byte.
persistentIdentifier	30351.12	    string	            Random string that identifies the sender. 
                                                            This may be used in the future as a primitive form of security.
flowStartSeconds	    150	            dateTimeSeconds (Integer)	
                                                            The time of the transmission (absolute seconds since 1/1/1970).

        */

        public class receptionReport
        {
            public string   receiverCallsign { get; set; }  
            public string   receiverLocator { get; set; }  
            public string   senderCallsign { get; set; }   
            public string   senderLocator { get; set; }  
            public string   frequency { get; set; }  
            public string   diff_frequency { get; set; }   
            public string   flowStartSeconds { get; set; } 
            public string   mode { get; set; } 
            public string   isSender { get; set; } 
            public string   isReceiver { get; set; }  
            public string   senderRegion { get; set; }  //Aichi
            public string   senderDXCC { get; set; }    //Japan
            public string   senderDXCCCode { get; set; }    //JA
            public string   senderDXCCLocator { get; set; } //
            public string   senderLotwUpload { get; set; }  //2020-03-09
            public string   senderEqslAuthGuar { get; set; }    //A
            public string   sNR { get; set; } 
        }

        /*
activeCallsigncallsign= XXXXXX, reports= 1, DXCC= Japan, DXCCcode= JA, frequency= 14075922, receptionReports_Id= 0,  
         */

        public class activeCallsign
        {
            public string   callsign { get; set; }
            public string   reports { get; set; }
            public string   DXCC { get; set; }
            public string   DXCCcode { get; set; }
            public string frequency { get; set; }
            public string diff_frequency { get; set; }
        }
    }
}
