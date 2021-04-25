using Godot;
using System;

public class Enemy : Area
{
    [Export]
    public Texture sprite;
    
    [Export]
    private NodePath renderSprite;
    
    private float moveSpeed = 10;

    private SignalManager signalManager;

    private int health = 8;

    private bool dead = false;

    public bool endItem = false;

    public bool caveEntrance = false;

    public float TotalAttacks; // the number of attacks done in the given time frame*

    public float AttackTimeSum; // *the given time frame to execute the total number of attacks

    public float RestTime; // wait after the total number of attacks were executed once

    private int attacksDone;
    

    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.Connect(nameof(SignalManager.FightStart),this,"OnFightStart");
        signalManager.Connect(nameof(SignalManager.PlayerAttack),this,"OnPlayerAttack");
        signalManager.Connect(nameof(SignalManager.TurnAround),this,"OnTurnAround");

        GetNode<Sprite>(renderSprite).Texture = sprite;

        if(endItem || caveEntrance)
        {
            (GetNode<Sprite3D>("Sprite").MaterialOverride as ShaderMaterial).SetShaderParam("albedo_texture",sprite);
            GetNode<Sprite3D>("Sprite").Translation = new Vector3();
            GetNode<Sprite3D>("Sprite").Scale = new Vector3(1,1,1);
        }
        else
        {
            GetNode<Timer>("AttackTimer").WaitTime = AttackTimeSum / TotalAttacks;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        Translation += new Vector3(0, 0, moveSpeed * GetNode<Global>("/root/Global").moveSpeed);
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
        
        health--;
        if(health <= 0 && !dead && !endItem)
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
