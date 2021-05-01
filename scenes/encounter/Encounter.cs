using Godot;
using System;

public class Encounter : Area
{
    [Export]
    public Texture Sprite;
    
    [Export]
    private NodePath renderSprite;
    
    private float moveSpeed = 10;

    private SignalManager signalManager;

    public int Health = 8;

    private bool dead = false;

    public bool endItem = false;

    public bool caveEntrance = false;

    public float TotalAttacks; // the number of attacks done in the given time frame*

    public float AttackTimeSum; // *the given time frame to execute the total number of attacks

    public float RestTime; // wait after the total number of attacks were executed once

    private int attacksDone;

    private int hitShakes = 0;
    

    public async override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.Connect(nameof(SignalManager.FightStart),this,"OnFightStart");
        signalManager.Connect(nameof(SignalManager.PlayerAttack),this,"OnPlayerAttack");
        signalManager.Connect(nameof(SignalManager.TurnAround),this,"OnTurnAround");

        GetNode<Sprite>(renderSprite).Texture = Sprite;

        if(endItem || caveEntrance)
        {
            (GetNode<Sprite3D>("Sprite").MaterialOverride as ShaderMaterial).SetShaderParam("albedo_texture",Sprite);

            //GetNode<Sprite3D>("Sprite").Translation = new Vector3(0,3,0);
            //GetNode<Sprite3D>("Sprite").Scale = new Vector3(1,1,1);

            GD.Print(""+GetNode<Sprite3D>("Sprite").Translation+" - "+GetNode<Sprite3D>("Sprite").Scale);
        }
        else
        {
            GetNode<Timer>("AttackTimer").WaitTime = AttackTimeSum / TotalAttacks;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        //Translation += new Vector3(0, 0, moveSpeed * GetNode<Global>("/root/Global").moveSpeed);

        if(hitShakes > 0)
        {
            GetNode<Sprite3D>("Sprite").Translation = new Vector3((float)GD.RandRange(-0.25,0.25),1,(float)GD.RandRange(-0.25,0.25));
            hitShakes--;
        }
        else
        {
            GetNode<Sprite3D>("Sprite").Translation = new Vector3(0,1,0);
        }

    }

    private void Attack()
    {
        signalManager.EmitSignal(nameof(SignalManager.EnemyAttack));
    }

    private void OnAttackTimerTimeout()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("attack");
        attacksDone++;
        if(attacksDone == (int)TotalAttacks)
        {
            GetNode<Timer>("AttackTimer").WaitTime = RestTime;
            attacksDone = 0;
        }
        else if(attacksDone == 0 || attacksDone == 1)
        {
            GetNode<Timer>("AttackTimer").WaitTime = AttackTimeSum / TotalAttacks;
        }

        GetNode<Timer>("AttackTimer").Start();
    }

    private void OnFightStart()
    {
        
        if(caveEntrance)
        {
            signalManager.EmitSignal(nameof(SignalManager.CaveBeginingReached));
        }
        else if(endItem)
        {
            signalManager.EmitSignal(nameof(SignalManager.CaveEndReached));
        }
        else
        {
            GetNode<Timer>("AttackTimer").Start();
        }
        
    }

    private void OnPlayerAttack()
    {
        hitShakes += 5;
        Health--;
        if(Health <= 0 && !dead && !endItem)
        {
            dead = true;
            GetNode<Timer>("AttackTimer").Stop();
            GetNode<AnimationPlayer>("AnimationPlayer").Play("die");
        }
        if(!dead) (GetNode<Node>("HitSounds").GetChildren()[(int)GD.RandRange(0, GetNode<Node>("HitSounds").GetChildCount() - 1)] as AudioStreamPlayer).Play();
    }

    private void Dead()
    {
        signalManager.EmitSignal(nameof(SignalManager.EnemyDead));
        QueueFree();
    }

    private void OnTurnAround()
    {
        Dead();
    }
}
