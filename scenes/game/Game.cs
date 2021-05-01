using Godot;
using System;

public class Game : Control
{
    
    [Export]
    private Texture heart;
    private int health = 0;

    private SignalManager signalManager;
    private Global global;


    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");

        global = GetNode<Global>("/root/Global");

        signalManager.Connect(nameof(SignalManager.UpdatePlayerHP),this,"OnUpdatePlayerHP");
        signalManager.Connect(nameof(SignalManager.CaveEndReached),this,"OnCaveEndReached");
        signalManager.Connect(nameof(SignalManager.CaveBeginingReached),this,"OnCaveBeginingReached");
        signalManager.Connect(nameof(SignalManager.EncounterCountUpdate),this,"OnEncounterCountUpdate");
        signalManager.Connect(nameof(SignalManager.EnemyDead),this,"OnFightEnd");

        OnFightEnd();
    }


    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("left_click"))
        {
            signalManager.EmitSignal(nameof(SignalManager.AttackInput));
        }
        else 
        {
            if (@event.IsActionPressed("right_click"))
            {
                signalManager.EmitSignal(nameof(SignalManager.BlockInput), true);
            }
            else if(@event.IsActionReleased("right_click"))
            {
                signalManager.EmitSignal(nameof(SignalManager.BlockInput),false);
            }
        }

        if(@event.IsActionPressed("screenshot"))
        {
            global.Screenshot();
        }
    }

    public override void _PhysicsProcess(float delta)
    {

    }

    
    public override void _Draw()
    {
        Vector2 margin = new Vector2(10, RectSize.y / 2 - 28);

        for(int i = 0; i < health; i++)
        {
            DrawTextureRect(heart, new Rect2((margin + new Vector2(20,0) * i) * 2, new Vector2(36,36)), false);
        }
        
    }

    private void OnFightEnd()
    {
        GetNode<Tween>("Tween").InterpolateProperty(GetNode<Slider>("Progress"),"value", GetNode<Slider>("Progress").Value, GetNode<Slider>("Progress").Value+20, 3,Tween.TransitionType.Sine);
        GetNode<Tween>("Tween").Start();
    }

    private void OnEncounterCountUpdate(int encounters)
    {
        GetNode<Slider>("Progress").MaxValue = 20 * encounters;
    }

    private void TransitionTurnBack()
    {
        signalManager.EmitSignal(nameof(SignalManager.TurnAround));
    }

    private void OnUpdatePlayerHP(int HPLeft)
    {
        //health = HPLeft;
        //Update();
        GD.Print("Game.cs - OnUpdatePlayerHP is depricated.");
    }

    

    private void OnCaveEndReached()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("transition");
    }

    private void OnCaveBeginingReached()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("end");
    }

    #region ambient code

    private void OnCaveSoundTimerTimeout()
    {
        (GetNode<Node>("CaveSounds").GetChildren()[(int)GD.RandRange(0, GetNode<Node>("CaveSounds").GetChildCount() - 1)] as AudioStreamPlayer).Play();
        GetNode<Timer>("CaveSoundTimer").WaitTime = (float)GD.RandRange(1,10);
        GetNode<Timer>("CaveSoundTimer").Start();
    }


    #endregion
    
}
