using System;
using System.Diagnostics;
using System.IO;

namespace MusicTracker
{
    public class Channel
    {
        private double current_wav_pos = 0;
        public float[][] sound_data = null;
        private double volume = 0;
        private double feq = 0;
        public bool is_playing = false;
        public int last_index = -1;
        public double last_feq = 0;
        public int current_sound=0;
        public void Set_Sound_Data(ref float[][] sound_data)
        {
            this.sound_data = sound_data;
        }

        public void PlayNote(double feq, double volume,int sound)
        {
            if (is_playing == true) { return ; }
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
                val = this.sound_data[this.current_sound][(int)this.current_wav_pos]* (float)this.volume;
            }
            return val;
        }
        



    }
}