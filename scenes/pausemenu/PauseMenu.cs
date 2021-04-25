using Godot;
using System;

public class PauseMenu : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public async override void _Ready()
    {
        
    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("pause"))
        {
            GetTree().Paused = !GetTree().Paused;
            if(GetTree().Paused)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    private void OnMusicToggled(bool pressed)
    {
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), !pressed);
    }

    private void OnSoundToggled(bool pressed)
    {
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Sound"), !pressed);
    }

    private void OnContinueButtonDown()
    {
        GetTree().Paused = !GetTree().Paused;
            if(GetTree().Paused)
            {
                GetNode<Button>("CenterContainer/Panel/CenterContainer/VBoxContainer/Music").Pressed = !AudioServer.IsBusMute(AudioServer.GetBusIndex("Music"));
                GetNode<Button>("CenterContainer/Panel/CenterContainer/VBoxContainer/Sound").Pressed = !AudioServer.IsBusMute(AudioServer.GetBusIndex("Sound"));
                Show();
            }
            else
            {
                Hide();
            }
    }
}
