using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MusicTracker
{
    public class MusicModLib
    {
        #region Fields
        private TuningLookup tuning = new TuningLookup();
        public  int CHANNEL_COUNT= 1;
        private Channel[] channel = null;
        public Song song = null;
        private long last_time = 0;
        private long tempo_time_elapse = 0;
        public int TEMPO=120;
        private float[][] sound_bank = null;
        #endregion


        public void LoadSong(string filename)
        { 
            this.song = new Song();
           
            if (!System.IO.File.Exists(filename)) { throw new Exception("Missing file"); }

            this.song = new Song();
            this.song.LoadSong(filename);

            CHANNEL_COUNT = this.song.channels;
            this.TEMPO = this.song.tempo;

            this.channel = new Channel[CHANNEL_COUNT];
            //SET UP THE CHANNELS
            for (int i = 0; i < CHANNEL_COUNT; i++) {
                this.channel[i] = new Channel();
            }


            //now we load in the sample data
            this.sound_bank = new float[this.song.sample_list.Count()][];

            string data_path = System.IO.Path.GetDirectoryName(filename);
            string song_name = System.IO.Path.GetFileNameWithoutExtension(filename);
            string sample_path = System.IO.Path.Combine(data_path, song_name);
            for (int i = 0; i < this.song.sample_list.Count(); i++) {
                string full_sample_path = System.IO.Path.Combine(sample_path, this.song.sample_list[i]);
                if (!System.IO.File.Exists(filename)) { throw new Exception("Missing file"); }
                this.sound_bank[i] =this.LoadSample(full_sample_path);
            }

            for (int i = 0; i < CHANNEL_COUNT; i++) {
                this.channel[i].Set_Sound_Data(ref this.sound_bank);
            }

            tempo_time_elapse = (30000 / this.TEMPO);

            last_time = this.GetMillisecond();
        }

        public long GetMillisecond()
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds;
        }


        public double Update()
        {
            double val = 0;
            //This check to see if we need to progress the song along
            long time = this.GetMillisecond() - this.last_time;
            if (time > this.tempo_time_elapse) {
                this.song.Next_Step();
                //Let reset the tempo timer
                this.last_time = this.GetMillisecond();
            }

            for (int i = 0; i < channel.Length; i++) {
                SongNote sn = this.song.Get_Current_TrackLine().track[i ];
                if (this.song.Get_Current_Song_Block().current_track_line != this.channel[i].last_index) {
                    this.channel[i].last_index = this.song.Get_Current_Song_Block().current_track_line;
                    // sn.feq = 300/(this.channel[i].ldata.Length*0.1);
                    if (sn.feq == -1) {
                        sn.feq = this.channel[i].last_feq;
                    } else {
                        this.channel[i].StopNote();
                        if (sn.note != "") {
                            this.channel[i].last_feq = sn.feq;
                            this.channel[i].PlayNote(sn.feq, sn.volume,sn.sound);
                        }
                    }
                }
                val += this.channel[i].Update();
            }

      
            return (float)val;
        }

        public float[] LoadSample(string filename)
        {
             float[] sound_data = null;

            try {
                using (FileStream fs = File.Open(filename, FileMode.Open)) {
                    BinaryReader reader = new BinaryReader(fs);

                    // chunk 0
                    int chunkID = reader.ReadInt32();
                    int fileSize = reader.ReadInt32();
                    int riffType = reader.ReadInt32();

                    // chunk 1
                    int fmtID = reader.ReadInt32();
                    int fmtSize = reader.ReadInt32(); // bytes for this chunk
                    int fmtCode = reader.ReadInt16();
                    int channels = reader.ReadInt16();
                    int sampleRate = reader.ReadInt32();
                    int byteRate = reader.ReadInt32();
                    int fmtBlockAlign = reader.ReadInt16();
                    int bitDepth = reader.ReadInt16();

                    if (fmtSize == 18) {
                        // Read any extra values
                        int fmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(fmtExtraSize);
                    }

                    // chunk 2
                    int dataID = reader.ReadInt32();
                    int bytes = reader.ReadInt32();

                    // DATA!
                    byte[] byteArray = reader.ReadBytes(bytes);

                    int bytesForSamp = bitDepth / 8;
                    int samps = bytes / bytesForSamp;

                    float[] asFloat = null;
                    switch (bitDepth) {
                        case 64:
                            double[]
                            asDouble = new double[samps];
                            Buffer.BlockCopy(byteArray, 0, asDouble, 0, bytes);
                            asFloat = Array.ConvertAll(asDouble, e => (float)e);
                            break;

                        case 32:
                            asFloat = new float[samps];
                            Buffer.BlockCopy(byteArray, 0, asFloat, 0, bytes);
                            break;

                        case 24:
                            asFloat = new float[samps];
                            Buffer.BlockCopy(byteArray, 0, asFloat, 0, bytes);
                            break;

                        case 16:
                            Int16[]
                            asInt16 = new Int16[samps];
                            Buffer.BlockCopy(byteArray, 0, asInt16, 0, bytes);
                            asFloat = Array.ConvertAll(asInt16, e => e / (float)Int16.MaxValue);
                            break;
                        
                      
                           
                    }

                    switch (channels) {
                        case 1:
                            sound_data = asFloat;
                            //R = null;
                            return sound_data;

                        case 2:
                            sound_data = new float[samps];
                            // R = new float[samps];
                            for (int i = 0, s = 0; i < samps; i++) {
                                sound_data[i] = asFloat[s++];
                                //   R[i] = asFloat[s++];
                            }
                           return sound_data;
                        default:
                            return null;
                    }
                }
            } catch {
                Debug.WriteLine("...Failed to load note: " + filename);
                return null;
                //left = new float[ 1 ]{ 0f };
            }

        }


    }

    public class TuningLookup
    {
        public IDictionary<string, double> Note_Lookup { get; set; } = new Dictionary<string, double>();
        public TuningLookup()
        {
            double step = 0.5 / 16;
            Note_Lookup.Add("C", 0.5);
            Note_Lookup.Add("C#", 0.5 + (step * 1));
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

    public struct SongNote
    {
        public string note;
        public double feq;
        public double volume;
        public int sound;

        public SongNote(string note, double feq, double volume)
        {
            this.note = note;
            this.feq = 0;
            this.volume = 0;
            this.sound = 6;
        }
    }

    public class SongNoteGroup
    {
        public SongNote[] track = {new SongNote(), new SongNote(), new SongNote(), new SongNote(), new SongNote(), new SongNote(), new SongNote() , new SongNote() };
    

    }

    public class SongBlock
    {
        public List<SongNoteGroup> track_lines = new List<SongNoteGroup>();
        private SongNoteGroup track_line = null;

        public SongBlock()
        {
        }

        public int current_track_line = -1;

        public bool Next_Track_Line()
        {
            current_track_line += 1;
            if (this.current_track_line > track_lines.Count - 1) {
                this.current_track_line = 0;
                this.track_line = this.track_lines[this.current_track_line];
                return true;
            } else {
                this.track_line = this.track_lines[this.current_track_line];
            }
            return false;
        }

        public SongNoteGroup Get_Current_TrackLine()
        {
            if (this.current_track_line == -1) {
                this.current_track_line = 0;
            }
            if (this.track_line == null) { this.track_line = this.track_lines[this.current_track_line]; }
            return this.track_line;
        }

        internal void ReadSongBlock(System.IO.StreamReader file)
        {
            string line;
            TuningLookup tuning = new TuningLookup();
            while ((line = file.ReadLine()) != null) {
                if (line.ToUpper() == "BLOCK_END") {
                    return;
                }

                //READ THE DATA
                //Split the track data
                SongNoteGroup new_song_group = new SongNoteGroup();
                string[] TrackData = line.Split(',');
                for (int i = 0; i < TrackData.Length; i++) {
                    //we need to split the data and store in out song note
                    string[] note_data = TrackData[i].Split(':');
                    new_song_group.track[i].note = note_data[0].Trim();
                    new_song_group.track[i].feq = tuning.GetFeq(note_data[0].Trim());
                    new_song_group.track[i].volume = double.Parse(note_data[1].Trim());
                    new_song_group.track[i].sound = int.Parse(note_data[2].Trim());
                   
                }
                this.track_lines.Add(new_song_group);
            }

            tuning = null;
        }
    }

    public class Arrangment
    {
        public List<SongNoteGroup> track_line = new List<SongNoteGroup>();

        public Arrangment()
        {
        }
    }

    /// <summary>
    /// The song hold all the elements together
    /// </summary>
    public class Song
    {
        public List<SongBlock> song_blocks = new List<SongBlock>();
        private SongBlock current_song_block = null;
        public List<string> sample_list { get; set; } = new List<string>();
        public int tempo = 0;
        public int channels = 0;
        public Song()
        {
        }

        public SongBlock Get_Current_Song_Block()
        {
            return this.current_song_block;
        }

        public SongNoteGroup Get_Current_TrackLine()
        {
            if (current_song_block == null) { current_song_block = song_blocks[0]; }
            return this.current_song_block.Get_Current_TrackLine();
        }

        public void Next_Step()
        {
            current_song_block = song_blocks[0];
            current_song_block.Next_Track_Line();
        }

        public void LoadSong(string file_name)
        {
            int counter = 0;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(file_name);
            while ((line = file.ReadLine()) != null) {
                if (line.ToUpper() == "BLOCK_START") {
                    SongBlock song_block = new SongBlock();
                    song_block.ReadSongBlock(file);
                    this.song_blocks.Add(song_block);
                } else if (line.ToUpper() == "BLOCK_SAMPES") {
                    Read_Sample_Block(file);
                } else if (line.ToUpper().StartsWith("TEMPO")) {
                    this.tempo = int.Parse(line.Split(' ')[1]);
                } else if (line.ToUpper().StartsWith("CHANNELS")) {
                    this.channels = int.Parse(line.Split(' ')[1]);
                }
                counter++;
            }

            file.Close();
        }

        internal void Read_Sample_Block(System.IO.StreamReader file)
        {
            string line;
            TuningLookup tuning = new TuningLookup();
            while ((line = file.ReadLine()) != null) {
                if (line.ToUpper() == "BLOCK_END") {
                    return;
                }
                this.sample_list.Add(line.Trim());
            }
        }
    }


    public class Channel
    {
        private double current_wav_pos = 0;
        public float[][] sound_data = null;
        private double volume = 0;
        private double feq = 0;
        public bool is_playing = false;
        public int last_index = -1;
        public double last_feq = 0;
        public int current_sound = 0;
        public void Set_Sound_Data(ref float[][] sound_data)
        {
            this.sound_data = sound_data;
        }

        public void PlayNote(double feq, double volume, int sound)
        {
            if (is_playing == true) { return; }
            is_playing = true;
            this.volume = volume;
            this.feq = feq;
            this.current_sound = sound;
            this.current_wav_pos = 0; //-= feq;

        }

        public void StopNote()
        {
            this.current_wav_pos = 0;
            this.is_playing = false;
        }


        public double Update()
        {
            float val = 0;
            if (is_playing == true) {
                this.current_wav_pos += this.feq;
                if (this.current_wav_pos > this.sound_data[this.current_sound].Length - 1) {
                    this.current_wav_pos = 0;
                    is_playing = false;
                }
                val = this.sound_data[this.current_sound][(int)this.current_wav_pos] * (float)this.volume;
            }
            return val;
        }




    }


}