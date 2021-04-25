using Godot;
using System;

public class Camera : Godot.Camera
{
    int shakes = -1;
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.UpdatePlayerHP),this,"OnPlayerHurt");
    }

    public override void _PhysicsProcess(float delta)
    {
        if(shakes > 0)
        {
            Translation = new Vector3((float)GD.RandRange(-0.05f, 0.05f)*(shakes/2), (float)GD.RandRange(-0.05f, 0.05f)*(shakes/2), 0);
            shakes--;
        }
        else if(shakes == 0)
        {
            Translation = new Vector3();
            shakes--;
        }
    }

    private void OnPlayerHurt(int health)
    {
        shakes += 20;
    }
}
