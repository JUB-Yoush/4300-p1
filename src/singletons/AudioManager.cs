using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class AudioManager : Node
{
    const int SFX_PLAYER_COUNT = 5;
    int bus = AudioServer.GetBusIndex("Master");
    AudioStreamPlayer BgmPlayer = new();
    List<AudioStreamPlayer> SfxPlayers = [];

    public override void _Ready()
    {
        AddChild(BgmPlayer);
        for (int i = 0; i < SFX_PLAYER_COUNT; i++)
        {
            var sfxPlayer = new AudioStreamPlayer();
            AddChild(sfxPlayer);
            SfxPlayers.Add(sfxPlayer);
        }
        BgmPlayer.ProcessMode = ProcessModeEnum.Always;
    }

    public void PlayMusic(AudioStream music)
    {
        if (BgmPlayer.Stream == music)
        {
            return;
        }
        BgmPlayer.Stream = music;
        BgmPlayer.Play();
    }

    public void PlaySfx(AudioStream sfx)
    {
        var sfxPlayer = SfxPlayers.FirstOrDefault(sfxPlayer => !sfxPlayer.IsPlaying());

        if (sfxPlayer == null)
        {
            return;
        }

        sfxPlayer.Stream = sfx;
        sfxPlayer.Play();
    }

    public void SetVolume(float value)
    {
        AudioServer.SetBusVolumeDb(bus, value);
    }

    public void StopAll()
    {
        SfxPlayers.ForEach(SfxPlayer => SfxPlayer.Stop());
    }

    public void PauseMusic()
    {
        BgmPlayer.Stop();
    }
}
