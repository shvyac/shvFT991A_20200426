using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shvFT991A
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    using System.Media;
    //using shvMorse;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Collections.Specialized;

    public class MorseModem
    {
        private int TimeUnitInMilliSeconds { get; set; } = 30;
        private int frequency { get; set; } = 650;
        public char DotUnicode { get; set; } = '●'; // U+25CF
        public char DashUnicode { get; set; } = '▬'; // U+25AC
        public char Dot { get; set; } = '.';
        public char Dash { get; set; } = '-';

        public MorseCodes codeStore;
        public MainWindow mainWindow = (MainWindow)App.Current.MainWindow;

        public void Main()
        {
            //codeStore = new Codes();
        }

        public string ConvertToMorseCode(string sentence, bool addStartAndEndSignal = false)
        {
            codeStore = new MorseCodes();

            ((INotifyCollectionChanged)mainWindow.ListViewMorseDebug.Items).CollectionChanged += this.ListBoxCollectionChanged;

            var generatedCodeList = new List<string>();
            var wordsInSentence = sentence.Split(' ');

            foreach (var word in wordsInSentence)
            {
                foreach (var letter in word.ToUpperInvariant().ToCharArray())
                {
                    //Console.WriteLine(codeStore[letter]);

                    generatedCodeList.Add(codeStore[letter]);
                    generatedCodeList.Add(" ");
                }
                generatedCodeList.Add("  ");
            }

            if (addStartAndEndSignal)
            {
                //generatedCodeList.Add(codeStore.GetSignalCode(SignalCodes.EndOfWork));
            }
            else
            {
                //generatedCodeList.RemoveAt(generatedCodeList.Count - 1);
            }

            string strRet = "   " + string.Join("", generatedCodeList);  //.Replace("_", "   ") ;
            Console.WriteLine("strRet = {0} ", strRet);
            return strRet;
        }

        static async void DelaySample()
        {
            await Task.Delay(3000);
        }

        public async Task PlayMorseTone(string morseStringOrSentence, int cwPitch, int wpmStart, int wpmEnd, int wpmDecrement, bool answerBefore, string messageABC)
        {
            var wavArray = new string[30];
            var tasks = new List<Task>();

            mainWindow.ButtonSpeedDown.Background = new SolidColorBrush(Colors.Red);  //BackColor = Colors.Red;  

            //---------------------------------------------------------------------------------------
            ThreadPool.GetMaxThreads(out int workerThreads, out int portThreads);
            Console.WriteLine("------------------------------ Worker threads={0}, Completion port threads={1}", workerThreads, portThreads);
            //---------------------------------------------------------------------------------------

            if (IsValidMorse(morseStringOrSentence))
            {
                var pauseBetweenLetters = "_"; // One Time Unit
                var pauseBetweenWords = "___"; // Seven Time Unit

                morseStringOrSentence = morseStringOrSentence.Replace("   ", pauseBetweenWords);
                morseStringOrSentence = morseStringOrSentence.Replace(" ", pauseBetweenLetters);

                int[] speeddata = { 120, 100, 80, 67, 57, 50, 44, 40, 36, 33, 30, 28, 25, 23, 20, 18, 15, 12, 9 };
                //int[] wpmdata = { 10, 13, 16, 20, 23, 26, 30, 33, 36, 40, 43, 46, 50, 53, 56, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180 };
                int[] wpmdata = { };
                int wav_count = -1;

                for (int wpm = wpmStart; wpm >= wpmEnd; wpm = wpm - wpmDecrement)
                {
                    Console.WriteLine("WPM = " + wpm);

                    //mainWindow.textBoxSystemAnswer.Text = morseStringOrSentence.ToString();

                    mainWindow.labelCurrentSpeed.Content = wpm.ToString() + @"WPM";
                    //mainWindow.Refresh();
                    //mainWindow.labelCurrentSpeed.Refresh();
                    mainWindow.textBoxUserAnswer.SelectAll();
                    mainWindow.ListViewMorseDebug.Items.Add(messageABC + @" ---" + wpm.ToString() + $"WPM ");

                    //mainWindow.ListViewDebug.ScrollIntoView()

                    //}
                    //foreach (int w in wpmdata)
                    //for (int speed = 60; speed > 30; speed--)
                    //{
                    //int wpm = (int) 60 * 1000 / 50 / speed;

                    int speed = (int)60 * 1000 / 50 / wpm;

                    Console.WriteLine();
                    Console.WriteLine("------------------ " + wpm.ToString() + "WPM  " + speed.ToString() + "mSec/elem ");

                    for (int i = 0; i < 1; i++)
                    {
                        //Task<string> task = PlayBeep(morseStringOrSentence, 750, speed, wpm);

                        wav_count++;
                        //---------------------------------------------------------------------------------------

                        wavArray[wav_count] = await Task.Run(() => PlayBeep(morseStringOrSentence, cwPitch, speed, wpm, messageABC));

                        //---------------------------------------------------------------------------------------

                        //wavArray[wav_count] = this.PlayBeep(morseStringOrSentence, cwPitch, speed, wpm, messageABC));

                        Console.WriteLine("wavArray[wav_count]= " + wavArray[wav_count].ToString());
                        Console.WriteLine(string.Join("\n", wavArray));

                        //task.Wait();
                        //PlayBeep( "-.-._--.-_", 750, speed, w );
                        //PlayBeepORG(750, speed);
                    }

                    //Thread.Sleep(1500);
                    await Task.Delay(1500);
                }
            }
            else
            {
                await PlayMorseTone(ConvertToMorseCode(morseStringOrSentence), cwPitch, wpmStart, wpmEnd, wpmDecrement, answerBefore, messageABC);
            }

            mainWindow.ButtonSpeedDown.Background = new SolidColorBrush(Colors.Blue);
            mainWindow.TextBoxRepeatCycles.Text = (Convert.ToInt16(mainWindow.TextBoxRepeatCycles.Text) - 1).ToString();

            if (0 < Convert.ToInt16(mainWindow.TextBoxRepeatCycles.Text))
            {
                mainWindow.ButtonSpeedDown_Click(null, null);
            }
        }

        private void ListBoxCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    mainWindow.ListViewMorseDebug.ScrollIntoView(mainWindow.ListViewMorseDebug.Items[e.NewStartingIndex]);
                    break;
            }
        }

        private bool IsValidMorse(string sentence)
        {
            var countDot = sentence.Count(x => x == '.');
            var countDash = sentence.Count(x => x == '-');
            var countSpace = sentence.Count(x => x == ' ');

            return
                sentence.Length > (countDot + countDash + countSpace)
                ? false : true;
        }

        public static string PlayBeep(string playchar, int frequency, int msDuration, int wpmspeed, string messageABC, UInt16 volume = 16383)
        {
            //static async Task<string> StartAsync()

            string path = @"./shvMorse_wav/";
            string wavfilename =
            path +
            messageABC.ToString() + @"_" +
            frequency.ToString() + @"Hz_" +
            wpmspeed.ToString() + @"WPM_" +
            msDuration.ToString("F") + @"mSec.wav";

            Console.WriteLine(wavfilename);

            //-----------------------------------------------------------------------------------------------------------

            int numChar = playchar.Count();

            Console.WriteLine("playchar = " + playchar);
            Console.WriteLine("numChar = " + numChar.ToString());
            Console.WriteLine("frequency = " + frequency.ToString());
            Console.WriteLine("msDuration = " + msDuration.ToString());

            int totalDuration = 0;
            foreach (char c in playchar)
            {
                if (Convert.ToString(c) == ".") totalDuration += msDuration * 2; // *_
                if (Convert.ToString(c) == "-") totalDuration += msDuration * 4; // ***_
                if (Convert.ToString(c) == "_") totalDuration += msDuration * 2; // _ _
            }
            totalDuration += msDuration * 6; // finally  
            Console.WriteLine(playchar.ToString() + " totalDuration= " + totalDuration.ToString() + "mSec");

            var mStrm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mStrm);

            const double TAU = 2 * Math.PI;
            int formatChunkSize = 16;
            int headerSize = 8;
            short formatType = 1;
            short tracks = 1;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            int bytesPerSecond = samplesPerSecond * frameSize;
            int waveSize = 4;
            int samples = (int)((decimal)samplesPerSecond * totalDuration / 1000);
            int dataChunkSize = samples * frameSize;
            int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;

            // var encoding = new System.Text.UTF8Encoding();
            writer.Write(0x46464952);           // = encoding.GetBytes("RIFF")
            writer.Write(fileSize);             // waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            Console.WriteLine(" fileSize= " + fileSize.ToString());

            writer.Write(0x45564157);           // = encoding.GetBytes("WAVE")
            writer.Write(0x20746D66);           // = encoding.GetBytes("fmt ")
            writer.Write(formatChunkSize);      // 16
            writer.Write(formatType);           // 1
            writer.Write(tracks);               // 1
            writer.Write(samplesPerSecond);     // 44100
            writer.Write(bytesPerSecond);       // samplesPerSecond * frameSize;
            Console.WriteLine(" bytesPerSecond= " + bytesPerSecond.ToString());

            writer.Write(frameSize);            // (short)(tracks * ((bitsPerSample + 7) / 8));
            Console.WriteLine(" frameSize= " + frameSize.ToString());

            writer.Write(bitsPerSample);        // 16

            writer.Write(0x61746164);           // = encoding.GetBytes("data")
            writer.Write(dataChunkSize);        // samples * frameSize;
            Console.WriteLine(" dataChunkSize= " + dataChunkSize.ToString());


            foreach (char c in playchar)
            {
                //ushort frequency2 = 750;
                if (Convert.ToString(c) == ".")
                {
                    //frequency2 = frequency;
                    samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
                    //Console.WriteLine(". start");
                    //Console.WriteLine("frequency2 = " + frequency2.ToString());
                    //Console.WriteLine("samples = " + samples.ToString());

                    double theta = frequency * TAU / (double)samplesPerSecond;
                    //Console.WriteLine("theta = " + theta.ToString());

                    // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                    // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
                    double amp = volume >> 2; // so we simply set amp = volume / 2

                    double rad1 = 0.0 + 6 * Math.PI;
                    double rad2 = theta * samples - 6 * Math.PI;
                    //Console.WriteLine("rad1 = " + rad1.ToString());
                    //Console.WriteLine("rad2 = " + rad2.ToString());

                    for (int step = 0; step < samples; step++)
                    {
                        double radx = theta * (double)step;

                        double ratioy = 1.0;
                        if (radx < rad1) ratioy = 1.0 / (6.0 * Math.PI) * radx;
                        if (rad2 < radx) ratioy = -1 * (radx - rad2) / (6 * Math.PI) + 1;

                        short s = (short)(ratioy * amp * Math.Sin(radx));
                        writer.Write(s);
                        /*
                        Console.WriteLine( //"s, amp, theta, step, radx, ratioy = " + 
                            s.ToString() + ", " + amp.ToString() + ", " + theta.ToString() + ", " + step.ToString() + ", " + radx.ToString()
                            + ", " + ratioy.ToString() );
                            */
                    }

                    // blank tone--------------------------------------------------------------
                    theta = frequency * TAU / (double)samplesPerSecond;

                    // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                    // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)

                    amp = volume >> 2; // so we simply set amp = volume / 2
                    amp = 0;
                    for (int step = 0; step < samples; step++)
                    {
                        short s = (short)(amp * Math.Sin(theta * (double)step));
                        writer.Write(s);
                        //Console.WriteLine("s, amp, theta, step = " + s.ToString() + ", " + amp.ToString() + ", " + theta.ToString() + ", " + step.ToString());
                    }

                }
                if (Convert.ToString(c) == "-")
                {
                    //frequency = frequency;

                    samples = (int)((decimal)samplesPerSecond * msDuration * 3 / 1000);
                    //Console.WriteLine("-");

                    double theta = frequency * TAU / (double)samplesPerSecond;
                    // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                    // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
                    double amp = volume >> 2; // so we simply set amp = volume / 2

                    double rad1 = 0.0 + 6 * Math.PI;
                    double rad2 = theta * samples - 6 * Math.PI;
                    //Console.WriteLine("rad1 = " + rad1.ToString());
                    //Console.WriteLine("rad2 = " + rad2.ToString());

                    for (int step = 0; step < samples; step++)
                    {
                        double radx = theta * (double)step;

                        double ratioy = 1.0;
                        if (radx < rad1) ratioy = 1.0 / (6.0 * Math.PI) * radx;
                        if (rad2 < radx) ratioy = -1 * (radx - rad2) / (6 * Math.PI) + 1;

                        short s = (short)(ratioy * amp * Math.Sin(radx));
                        writer.Write(s);
                        /*
                        Console.WriteLine( //"s, amp, theta, step, radx, ratioy = " + 
                            s.ToString() + ", " + amp.ToString() + ", " + theta.ToString() + ", " + step.ToString() + ", " + radx.ToString()
                            + ", " + ratioy.ToString());
                            */
                    }
                    // blank one tone--------------------------------------------------------------
                    samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
                    theta = frequency * TAU / (double)samplesPerSecond;
                    // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                    // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
                    amp = volume >> 2; // so we simply set amp = volume / 2
                    amp = 0.0;
                    for (int step = 0; step < samples; step++)
                    {
                        short s = (short)(amp * Math.Sin(theta * (double)step));
                        writer.Write(s);
                        //Console.Write(s);
                    }
                }
                if (Convert.ToString(c) == "_")
                {
                    // blank two tone  two + above One = 3 tone
                    //frequency = frequency;
                    samples = (int)((decimal)samplesPerSecond * msDuration * 2 / 1000);
                    //Console.WriteLine("_");

                    double theta = frequency * TAU / (double)samplesPerSecond;
                    // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                    // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
                    double amp = volume >> 2; // so we simply set amp = volume / 2
                    amp = 0.0;
                    for (int step = 0; step < samples; step++)
                    {
                        short s = 0; // (short)(amp * Math.Sin(theta * (double)step));
                        writer.Write(s);
                        //Console.Write(s);
                    }
                }
            }
            //----------------------------------------------------------------------------------------------
            samples = (int)((decimal)samplesPerSecond * msDuration * 6 / 1000);
            for (int step = 0; step < samples; step++)
            {
                short s = 0;
                writer.Write(s);
            }
            //----------------------------------------------------------------------------------------------

            mStrm.Seek(0, SeekOrigin.Begin);
            new System.Media.SoundPlayer(mStrm).PlaySync();

            /* stop 
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine("PlayBeep path = " + path);
            }

            using (var fileStream = new FileStream(wavfilename, FileMode.CreateNew, FileAccess.Write))
            {
                mStrm.Position = 0;
                mStrm.CopyTo(fileStream);
            }
            */

            writer.Dispose();
            mStrm.Dispose();

            writer.Close();
            mStrm.Close();

            return wavfilename;
        }

        public static void PlayBeepORG(UInt16 frequency, int msDuration, UInt16 volume = 16383)
        {
            var mStrm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mStrm);

            const double TAU = 2 * Math.PI;
            int formatChunkSize = 16;
            int headerSize = 8;
            short formatType = 1;
            short tracks = 1;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            int bytesPerSecond = samplesPerSecond * frameSize;
            int waveSize = 4;
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
            Console.WriteLine("samples = " + samples);

            int dataChunkSize = samples * frameSize;
            int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            Console.WriteLine("fileSize = " + fileSize);

            // var encoding = new System.Text.UTF8Encoding();
            writer.Write(0x46464952); // = encoding.GetBytes("RIFF")
            writer.Write(fileSize);
            writer.Write(0x45564157); // = encoding.GetBytes("WAVE")
            writer.Write(0x20746D66); // = encoding.GetBytes("fmt ")
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write(samplesPerSecond);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(0x61746164); // = encoding.GetBytes("data")
            writer.Write(dataChunkSize);
            {
                double theta = frequency * TAU / (double)samplesPerSecond;
                // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
                double amp = volume >> 2; // so we simply set amp = volume / 2
                for (int step = 0; step < samples; step++)
                {
                    short s = (short)(amp * Math.Sin(theta * (double)step));
                    writer.Write(s);
                    Console.WriteLine(s);
                }
            }

            mStrm.Seek(0, SeekOrigin.Begin);
            new System.Media.SoundPlayer(mStrm).PlaySync();
            writer.Close();
            mStrm.Close();
        }
    }
}