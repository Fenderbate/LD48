using Godot;
using System;

public class Transition : Control
{
    private string newScenePath;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void ChangeScene(string newScene)
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeIn");
        newScenePath = newScene;
    }

    private void Pause(bool pause)
    {
        GetTree().Paused = pause;
    }

    private void Change()
    {
        GetNode<Global>("/root/Global").Reset();
        PackedScene scene = GD.Load<PackedScene>(newScenePath);
        GD.Print("Result: "+ GetTree().ChangeSceneTo(scene));
        GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeOut");
    }
}
