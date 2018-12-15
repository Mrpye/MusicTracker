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
            Note_Lookup.Add("C",16.35);
            Note_Lookup.Add("C#", 17.32);
            Note_Lookup.Add("Db", 17.32);
            Note_Lookup.Add("D",18.35);
            Note_Lookup.Add("D#", 19.45);
            Note_Lookup.Add("Eb", 19.45);
            Note_Lookup.Add("E", 20.06);
            Note_Lookup.Add("F",21.83);
            Note_Lookup.Add("F#", 23.12);
            Note_Lookup.Add("Gb", 23.12);
            Note_Lookup.Add("G", 24.5);
            Note_Lookup.Add("G#", 25.96);
            Note_Lookup.Add("Ab", 25.96);
            Note_Lookup.Add("A", 27.5);
            Note_Lookup.Add("A#", 29.14);
            Note_Lookup.Add("Bb", 29.14);
            Note_Lookup.Add("B", 30.87);

        }
        public double GetFeq(string note)
        {
            if (note.Length == 2) {
                double feq = Note_Lookup[note.Substring(0, 1)];
                int oct = int.Parse(note.Substring(1, 1));
                double res= Math.Pow(feq, (oct + 1));
                return res;
            } else if (note.Length == 3) {
                double feq = Note_Lookup[note.Substring(0, 2)];
                int oct = int.Parse(note.Substring(2, 1));
                double res = Math.Pow(feq, (oct + 1));
                return res;
            } else {
                return 0;
            }
        }
    }
}
