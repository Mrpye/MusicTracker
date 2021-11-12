using PixelEngine;
using System;

namespace MusicTracker
{
    public class Beeping : Game
    {

        private MusicModLib mm = null;

        private static void Main(string[] args)
        {
            Beeping t = new Beeping(); // Create an instance

          


            t.Construct(400, 400, 2, 2); // Construct the app
            t.Start(); // Start the app
        }

        // Enable the sound system
        public override void OnCreate()
        {
            //*******************************
            //Let read in the song data first
            //*******************************
            this.mm = new MusicModLib();
            this.mm.LoadSong(@"D:\Projects\Net\MusicTracker\Music\song.txt");


            this.Enable(Subsystem.Audio );

        }

        private string GetNoteDispay(int sb, int tl, int t)
        {
           // string note_data = this.mm.song.song_blocks[sb].track_lines[tl].GetTrack(t).note.ToString();
            SongNote s = this.mm.song.song_blocks[sb].track_lines[tl].track[t];
            string note_data=s.note.ToString();
            string sound = s.sound.ToString();
            //string volume=  this.mm.song.song_blocks[sb].track_lines[tl].GetTrack(t).volume.ToString();

            if (note_data.Trim() == "") {
                return ".";
            } else {
                return note_data.PadRight(3) + " " + "1.0 " + sound;
            }
        }

        public override void OnUpdate(float elapsed)
        {
            // this.song.song_blocks[0].track_line[current_index].GetTrack();

            Clear(Pixel.Presets.Black);

            int max_right = 0;
            int tracks = this.mm.song.Get_Current_Song_Block().track_lines.Count;
            int curr_trackline = this.mm.song.Get_Current_Song_Block().current_track_line;
            for (int i = 0; i < tracks ; i++) {
               
                for (int c = 0; c < this.mm.CHANNEL_COUNT; c++) {
                    DrawText(new Point((c *100)+10,( i * 10)+10), GetNoteDispay(0, i, c), Pixel.Presets.White);
                    max_right = c * 100 +40;
                    
                }
                if (curr_trackline == i) {
                    this.DrawRect(new Point(0, (curr_trackline * 10)+10), new Point(max_right + 60, (curr_trackline * 10) + 20), Pixel.Presets.White);
                }
            }

            for (int c = 0; c < this.mm.CHANNEL_COUNT; c++) {
                this.DrawRect(new Point((c * 100), 0), new Point((c * 100)+100, 300), Pixel.Presets.Blue);
            }


                base.OnUpdate(elapsed);
        }


        // Override to generate sound in real time
        // This is generating a sin wave
        public override float OnSoundCreate(int channels, float globalTime, float timeStep)
        {
            return (float)this.mm.Update();
        }
    }
}