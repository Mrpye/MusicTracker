# MusicTracker
This project is a simple music tracker written in c++. In my youth i owned a Amiga 1200 and love writing tunes using music trackers it was my first venture into writing music. So i thought i would have a play and see if i could write a basic music tracker.

# How it work 
It quite simple just by playing back the samples at diffrent rates produces diffrent pitches
- It has 8 channels
- set the note
- set volume


new_song_group.track6.note = note_data[0].Trim();
                            new_song_group.track6.feq = tuning.GetFeq(note_data[0].Trim());
                            new_song_group.track6.volume = double.Parse(note_data[1].Trim());
                            new_song_group.track6.sound = int.Parse(note_data[2].Trim());

# Create a song
you create a file that 
- sets the temo 
- number of channes
- list the sample (have a folder with same name as file)
- create the channel music

so each line represents a step and contains the note infomation for each of the defined channels
in this example we have 3 channels  **C1:1:0,C1:0.5:2,C0:1:4** this breaks down to
**Chanel 1**
  - note=C1
  - volume=1
  - sound=0 (Kick.wav)
**Chanel 2**
  - note=C1
  - volume=0.5
  - sound=0 (snare.wav)
**Chanel 3**
  - note=C0
  - volume=1
  - sound=0 (crash.wav)

If you dont want the nmote to play set the note to **-**

```
TEMPO 200
CHANNELS 3
BLOCK_SAMPES
	Kick.wav
	snare.wav
	cymb.wav
	crash.wav
	bass.wav
BLOCK_END
BLOCK_START
C1:1:0,C1:0.5:2,C0:1:4
- :1:0,C1:0.5:3,:1:4
C1:1:1,C1:0.5:2,D1:1:4
- :1:0,C1:0.5:3,D0:1:4
C1:1:0,C1:0.5:2,F1:1:4
- :1:0,C1:0.5:3,-:1:4
C1:1:1,C1:0.5:2,-:1:4
- :1:0,C1:0.5:3,-:1:4
C1:1:0,C1:0.5:2,:1:4
- :1:0,C1:0.5:3,:1:4
C1:1:1,C1:0.5:2,C0:1:4
- :1:0,C1:0.5:3,:1:4
C1:1:0,C1:0.5:2,:1:4
- :1:0,C1:0.5:3,F1:1:4
C1:1:1,C1:0.5:2,-:1:4
- :1:0,C1:0.5:3,-:1:4
BLOCK_END
```


# IDE
Visual studio 2019
C++ project

## Libraries
MusicTracker utilizes the following third-party libraries:
- [https://github.com/OneLoneCoder/olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine) (olcPixelGameEngine)

