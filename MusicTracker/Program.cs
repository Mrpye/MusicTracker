using PixelEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTracker
{
    public struct Note
    {
        public float note;
        public float length;

        public Note(float note, float length)
        {
            this.note = note;
            this.length = length;
        }
    }

    public class Beeping : Game
    {
        private const int OSC_SINE = 0;
        private const int OSC_SQUARE = 1;
        private const int OSC_TRIANGLE = 2;
        private const int OSC_SAW_ANA = 3;
        private const int OSC_SAW_DIG = 4;
        private const int OSC_NOISE = 5;


        private EnvelopeADSR envelope = new EnvelopeADSR();
        private TuningLookup tuning = new TuningLookup();
        //private double dOctaveBaseFrequency = 110.0; // A2		// frequency of octave represented by keyboard
        private double d12thRootOf2 = Math.Pow(2.0, 1.0 / 12.0);        // assuming western 12 notes per ocatve

        private string[,] Notes = {  { "E1", "", "", "E1", "",  "", "E1", ""},{ "C1", "E2", "C1", "E2", "C1", "E2", "C1", "E2" }, { "C0", "C1", "C2", "C3", "C0", "C1", "C2", "C3" } };
        private double[,] Feq = { {0} };

        private int[] channelvoice = { OSC_SQUARE, OSC_SQUARE , OSC_SQUARE };

        void ResizeArray<T>(ref T[,] original, int newCoNum, int newRoNum)
        {
            var newArray = new T[newCoNum, newRoNum];
            int columnCount = original.GetLength(1);
            int columnCount2 = newRoNum;
            int columns = original.GetUpperBound(0);
            for (int co = 0; co <= columns; co++)
                Array.Copy(original, co * columnCount, newArray, co * columnCount2, columnCount);
            original = newArray;
        }

        private static void Main(string[] args)
        {
            Beeping t = new Beeping(); // Create an instance
         


                t.Construct(); // Construct the app
            t.Start(); // Start the app
        }

        // Enable the sound system
        public override void OnCreate()
        {
            this.Enable(Subsystem.Audio);

            ResizeArray(ref this.Feq, this.Notes.GetLength(0), this.Notes.GetLength(1));
            

            //Convert the notes into freq help speed stuff up
            for (int c = 0; c < this.Notes.GetLength(0); c++) {
                for (int i = 0; i < this.Notes.GetLength(1); i++) {
                    Feq[c, i] = tuning.GetFeq(this.Notes[c, i]);
                }
            }


                tempo_time_elapse = (15000 / 200);

            last_time = this.GetMillisecond();
        }

        public override void OnUpdate(float elapsed)
        {


            Clear(Pixel.Presets.Black);
            for (int i = 0; i < this.Notes.GetLength(0); i++) {
                DrawText(new Point(ScreenWidth / 5, (10 * i) + 1), this.Notes[i, current_index].ToString(), Pixel.Presets.White);
            }

            base.OnUpdate(elapsed);
        }

        public int current_index = 0;

        // public float fAccumulate = 0;
        public int last_index = -1;

        private long last_time = 0;
        private long tempo_time_elapse = 0;

        private long GetMillisecond()
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds;
        }

        // Override to generate sound in real time
        // This is generating a sin wave
        public override float OnSoundCreate(int channels, float globalTime, float timeStep)
        {
           
            double val = 0;

            long a = this.GetMillisecond() - this.last_time;
            if (a > this.tempo_time_elapse) {
                current_index += 1;
                if (current_index > this.Notes.GetLength(1) - 1) {
                    current_index = 0;
                }
                //Let reset the tempo timer
                this.last_time = this.GetMillisecond();
            }

            for (int i = 0; i < this.Notes.GetLength(0); i++) {
                if (current_index != last_index) {
                    last_index = current_index;
                    if (this.Notes[i, current_index] != "") {
                        this.envelope.NoteOn(globalTime);
                    } else {
                        this.envelope.NoteOff(globalTime);
                    }
                }

               val += 0.1 * this.envelope.PlayNote(globalTime,this.Feq[i, current_index], channelvoice[i]);
               // val += 0.1 * this.envelope.PlayNote(globalTime, 200.14, channelvoice[i]);
            }

            return (float)val;
        }
    }
}
