﻿using System;

namespace MusicTracker
{
    public class EnvelopeADSR
    {
        private const int OSC_SINE = 0;
        private const int OSC_SQUARE = 1;
        private const int OSC_TRIANGLE = 2;
        private const int OSC_SAW_ANA = 3;
        private const int OSC_SAW_DIG = 4;
        private const int OSC_NOISE = 5;

        #region Fields

        private double StartAmplitude;
        private double TriggerOffTime;
        private double TriggerOnTime;
        public bool isNoteON;

        #endregion Fields

        #region Properties

        public double AttackTime { get; set; }
        public double DecayTime { get; set; }
        public double SustainAmplitude { get; set; }
        public double ReleaseTime { get; set; }

        #endregion Properties

        private double dOctaveBaseFrequency = 32.70; // A2		// frequency of octave represented by keyboard
        private double d12thRootOf2 = Math.Pow(2.0, 1.0 / 12.0);        // assuming western 12 notes per ocatve

        public EnvelopeADSR()
        {
            this.AttackTime = 0.1;
            this.DecayTime = 0.1;
            this.StartAmplitude = 1.0;
            this.SustainAmplitude = 0.5;
            this.ReleaseTime = 0.20;
            this.isNoteON = false;
            this.TriggerOffTime = 0.0;
            this.TriggerOnTime = 0.0;
        }

        // Call when key is pressed
        public void NoteOn(double dTimeOn)
        {
            if (this.isNoteON == true) { return; }
            this.TriggerOnTime = dTimeOn;
            this.isNoteON = true;
        }

        // Call when key is released
        public void NoteOff(double dTimeOff)
        {
            if (this.isNoteON == false) { return; }
            this.TriggerOffTime = dTimeOff;
            this.isNoteON = false;
        }

        public double PlayNote(float globalTime, double feq, int sound)
        {
            //This handles the note
           // double dFrequencyOutput = dOctaveBaseFrequency * Math.Pow(d12thRootOf2, note);
            double dFrequencyOutput = dOctaveBaseFrequency * Math.Pow(d12thRootOf2, 1);

            double dOutput = this.GetAmplitude(globalTime) * 1.0 * osc(feq * 0.5, globalTime, sound);
            /*     (
                     +
                     + 1.0 * osc(dFrequencyOutput * 0.5, globalTime, OSC_SAW_DIG)
                 );*/
            return dOutput;
        }

        internal double w(double dHertz)
        {
            return dHertz * 2.0 * Math.PI;
        }

        internal double osc(double dHertz, double dTime, int nType = OSC_SINE)
        {
            switch (nType) {
                case OSC_SINE: // Sine wave bewteen -1 and +1
                    return Math.Sin(w(dHertz) * dTime);

                case OSC_SQUARE: // Square wave between -1 and +1
                    return Math.Sin(w(dHertz) * dTime) > 0 ? 1.0 : -1.0;

                case OSC_TRIANGLE: // Triangle wave between -1 and +1
                    return Math.Sin(Math.Sin(w(dHertz) * dTime)) * (2.0 / Math.PI);

                case OSC_SAW_ANA: // Saw wave (analogue / warm / slow)
                {
                        double dOutput = 0.0;

                        for (double n = 1.0; n < 40.0; n++)
                            dOutput += (Math.Sin(n * w(dHertz) * dTime)) / n;
                        return dOutput * (2.0 / Math.PI);
                    }

                case OSC_SAW_DIG: // Saw Wave (optimised / harsh / fast)
                    return (2.0 / Math.PI) * (dHertz * Math.PI * Math.IEEERemainder(dTime, 1.0 / dHertz) - (Math.PI / 2.0));

                //   case OSC_NOISE: // Pseudorandom noise
                //         return 2.0 * ((double)rand() / (double)RAND_MAX) - 1.0;

                default:
                    return 0.0;
            }
        }

        // Get the correct amplitude at the requested point in time
        public double GetAmplitude(double dTime)
        {
            double dAmplitude = 0.0;
            double dLifeTime = dTime - TriggerOnTime;

            if (this.isNoteON) {
                if (dLifeTime <= this.AttackTime) {
                    // In attack Phase - approach max amplitude
                    dAmplitude = (dLifeTime / this.AttackTime) * this.StartAmplitude;
                }

                if (dLifeTime > this.AttackTime && dLifeTime <= (this.AttackTime + this.DecayTime)) {
                    // In decay phase - reduce to sustained amplitude
                    dAmplitude = ((dLifeTime - this.AttackTime) / this.DecayTime) * (this.SustainAmplitude - this.StartAmplitude) + this.StartAmplitude;
                }

                if (dLifeTime > (this.AttackTime + this.DecayTime)) {
                    // In sustain phase - dont change until note released
                    dAmplitude = this.SustainAmplitude;
                }
            } else {
                // Note has been released, so in release phase
                dAmplitude = ((dTime - this.TriggerOffTime) / this.ReleaseTime) * (0.0 - this.SustainAmplitude) + this.SustainAmplitude;
            }

            // Amplitude should not be negative
            if (dAmplitude <= 0.0001)
                dAmplitude = 0.0;

            return dAmplitude;
        }
    }
}