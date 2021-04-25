using Godot;
using System;

public class LostMenu : Control
{
    [Export]
    private string gameScenePath;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.GameOver),this,"OnGameOver");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void OnRetryButtonDown()
    {
        GetNode<Transition>("/root/Transition").ChangeScene(gameScenePath);
    }

    private void OnGameOver()
    {
        GetTree().Paused = true;
        Show();
    }
}
