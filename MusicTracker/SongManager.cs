using System.Collections.Generic;

namespace MusicTracker
{
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
        public SongNote track1;
        public SongNote track2;
        public SongNote track3;
        public SongNote track4;
        public SongNote track5;
        public SongNote track6;
        public SongNote track7;
        public SongNote track8;

        public ref SongNote GetTrack(int trackid)
        {
            switch (trackid) {
                case 1:
                    return ref track1;

                case 2:
                    return ref track2;

                case 3:
                    return ref track3;

                case 4:
                    return ref track4;

                case 5:
                    return ref track5;

                case 6:
                    return ref track6;

                case 7:
                    return ref track7;

                case 8:
                    return ref track8;

                default:
                    return ref track1;
            }
        }
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
                this.current_track_line =0;
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
                    switch (i) {
                        case 0:
                            new_song_group.track1.note = note_data[0].Trim();
                            new_song_group.track1.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track1.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track1.sound = int.Parse(note_data[2].Trim());
                            break;

                        case 1:
                            new_song_group.track2.note = note_data[0].Trim();
                            new_song_group.track2.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track2.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track2.sound = int.Parse(note_data[2].Trim());
                            break;

                        case 2:
                            new_song_group.track3.note = note_data[0].Trim();
                            new_song_group.track3.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track3.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track3.sound = int.Parse(note_data[2].Trim());
                            break;

                        case 3:
                            new_song_group.track4.note = note_data[0].Trim();
                            new_song_group.track4.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track4.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track4.sound = int.Parse(note_data[2].Trim());
                            break;

                        case 4:
                            new_song_group.track5.note = note_data[0].Trim();
                            new_song_group.track5.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track5.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track5.sound = int.Parse(note_data[2].Trim());
                            break;

                        case 5:
                            new_song_group.track6.note = note_data[0].Trim();
                            new_song_group.track6.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track6.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track6.sound = int.Parse(note_data[2].Trim());
                            break;

                        case 6:
                            new_song_group.track7.note = note_data[0].Trim();
                            new_song_group.track7.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track7.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track7.sound = int.Parse(note_data[2].Trim());
                            break;

                        case 7:
                            new_song_group.track8.note = note_data[0].Trim();
                            new_song_group.track8.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track8.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track8.sound = int.Parse(note_data[2].Trim());
                            break;
                    }
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
        public int tempo=0;
        public int channels=0;
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
}