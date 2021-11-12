using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTracker
{
   public class TuningLookup
    {
        public IDictionary<string, double> Note_Lookup { get; set; } = new Dictionary<string, double>();
        public TuningLookup()
        {
            double step = 0.5 / 16;
            Note_Lookup.Add("C", 0.5);
            Note_Lookup.Add("C#", 0.5 + (step*1));
            Note_Lookup.Add("Db", 0.5 + (step * 2));
            Note_Lookup.Add("D", 0.51 + (step * 3));
            Note_Lookup.Add("D#", 0.5 + (step * 4));
            Note_Lookup.Add("Eb", 0.5 + (step * 5));
            Note_Lookup.Add("E", 0.5 + (step * 6));
            Note_Lookup.Add("F", 0.5 + (step * 7));
            Note_Lookup.Add("F#", 0.51 + (step * 8));
            Note_Lookup.Add("Gb", 0.51 + (step * 9));
            Note_Lookup.Add("G", 0.5 + (step * 10));
            Note_Lookup.Add("G#", 0.5 + (step * 11));
            Note_Lookup.Add("Ab", 0.5 + (step * 12));
            Note_Lookup.Add("A", 0.5 + (step * 13));
            Note_Lookup.Add("A#", 0.5 + (step * 14));
            Note_Lookup.Add("Bb", 0.5 + (step * 15));
            Note_Lookup.Add("B", 0.5 + (step * 16));
    

        }
        public double GetFeq(string note)
        {
            if (note.Trim().Length == 2) {
                double feq = Note_Lookup[note.Substring(0, 1)];
                int oct = int.Parse(note.Substring(1, 1));
                double res = feq + oct;// Math.Pow(feq, (oct + 1));
                return res;
            } else if (note.Trim().Length == 3) {
                double feq = Note_Lookup[note.Substring(0, 2)];
                int oct = int.Parse(note.Substring(2, 1));
                double res = feq + oct;
                return res;
            } else {
                if (note == "-") {
                    return -1;
                } else {
                    return 0;
                }
               
            }
        }
    }

}
