using Godot;
using System;

public class Transition : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    private async void OnChangeScene(String newScene)
    {
        PackedScene scene = GD.Load<PackedScene>(newScene);
        GetTree().ChangeSceneTo(scene);
        await ToSignal(GetTree(),"idle_frame");
        GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeOut");
    }

    private void Pause(bool pause)
    {
        GetTree().Paused = pause;
    }
}
