using Godot;

public partial class Controls : Node
{
    public static PackedScene packedScene = GD.Load<PackedScene>("res://src/scenes/controls.tscn");

    public override void _Ready()
    {
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetNode<TextureButton>("Paused/Panel/Resume").Pressed += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetTree().Paused = false;
            QueueFree();
        };
        GetNode<TextureButton>("Paused/Panel/Home").Pressed += () =>
            GetTree().ChangeSceneToFile("res://src/scenes/title.tscn");
    }
}
