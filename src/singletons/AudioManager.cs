using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class SFX
{
    //should either be a hashmap or lazy-loaded
    public static AudioStream Footsteps = GD.Load<AudioStream>(
        "res://assets/audio/sfx/footsteps3.wav"
    );
    public static AudioStream GunEat = GD.Load<AudioStream>("res://assets/audio/sfx/eating.wav");
    public static AudioStream Sniffing = GD.Load<AudioStream>(
        "res://assets/audio/sfx/sniffing.wav"
    );
    public static AudioStream Gun = GD.Load<AudioStream>("res://assets/audio/sfx/gunshot.wav");
    public static AudioStream GhostAttack = GD.Load<AudioStream>(
        "res://assets/audio/sfx/ghostattack.wav"
    );
    public static AudioStream MetalPipe = GD.Load<AudioStream>(
        "res://assets/audio/sfx/metalpipe.mp3"
    );
    public static AudioStream Punch = GD.Load<AudioStream>("res://assets/audio/sfx/punch.wav");
}

public partial class AudioManager : Node
{
    const int SFX_PLAYER_COUNT = 5;
    int bus = AudioServer.GetBusIndex("Master");
    AudioStreamPlayer BgmPlayer = new();
    List<AudioStreamPlayer> SfxPlayers = [];

    public static AudioManager Ref = null!; // static singleton instance, global nodes are loaded before the scene tree.

    public override void _Ready()
    {
        Ref = this;
        AddChild(BgmPlayer);
        for (int i = 0; i < SFX_PLAYER_COUNT; i++)
        {
            var sfxPlayer = new AudioStreamPlayer();
            AddChild(sfxPlayer);
            SfxPlayers.Add(sfxPlayer);
        }
        BgmPlayer.ProcessMode = ProcessModeEnum.Always;
    }

    public static void PlayMusic(AudioStream music)
    {
        if (Ref.BgmPlayer.Stream == music)
        {
            return;
        }
        Ref.BgmPlayer.Stream = music;
        Ref.BgmPlayer.Play();
    }

    public static void PlaySfx(AudioStream sfx, bool singleStreamOnly = false)
    {
        // only one stream playing at a time
        if (singleStreamOnly)
        {
            var alreadyPlayingStream = Ref.SfxPlayers.FirstOrDefault(sfxPlayer =>
                sfxPlayer.Stream == sfx && sfxPlayer.Playing
            );
            if (alreadyPlayingStream != null)
            {
                return;
            }
        }

        var sfxPlayer = Ref.SfxPlayers.FirstOrDefault(sfxPlayer => !sfxPlayer.IsPlaying());

        if (sfxPlayer == null)
        {
            return;
        }

        sfxPlayer.Stream = sfx;
        sfxPlayer.Play();
    }

    public static void StopSfx(AudioStream sfx)
    {
        var sfxStream = Ref.SfxPlayers.FirstOrDefault(sfxPlayer =>
            sfxPlayer.Stream == sfx && sfxPlayer.Playing
        );
        sfxStream?.Stop();
    }

    public static void SetVolume(float value)
    {
        AudioServer.SetBusVolumeDb(Ref.bus, value);
    }

    public static void StopAll()
    {
        Ref.SfxPlayers.ForEach(SfxPlayer => SfxPlayer.Stop());
    }

    public static void PauseMusic()
    {
        Ref.BgmPlayer.Stop();
    }
}
