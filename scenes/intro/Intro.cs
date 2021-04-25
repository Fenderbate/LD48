using Godot;
using System;

public class Intro : Control
{
    [Export]
    private String nextScene;
    
    private int index = -1;

    private Vector2 [] positions = new Vector2 [] {new Vector2(0,0), new Vector2(512,0), new Vector2(0,512), new Vector2(512,512)};

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        NextSlide();

    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("left_click") || @event.IsActionPressed("right_click"))
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("flip");
        }
    }

    private void NextSlide()
    {
        index++;
        foreach(object child in GetNode<Control>("Labels").GetChildren())
        {
            (child as Control).Hide();
        }
        
        if(index >= 5)
        {
            GetNode<Transition>("/root/Transition").ChangeScene(nextScene);
            return;
        }

        if(index < 4) GetNode<Sprite>("Sprite").Frame = index;
        if(index < 5) GetNode<Label>("Labels/Label"+index).Show();
        
    }

    private void NextPos()
    {
        index++;
        
        foreach(object child in GetNode<Control>("Camera/Labels").GetChildren())
        {
            (child as Control).Hide();
        }
        
        if(index >= 5)
        {
            GetNode<Transition>("/root/Transition").ChangeScene(nextScene);
            return;
        }

        if(index < 4) GetNode<Camera2D>("Camera").Position = positions[index];
        if(index < 5) GetNode<Label>("Camera/Labels/Label"+index).Show();
        

    }
}
