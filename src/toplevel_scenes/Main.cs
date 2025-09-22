using Godot;

public partial class Main : Node
{
    public override void _Ready()
    {
        AudioManager.PlayMusic(BGM.GameMusic);
    }
};
