using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using MonoCustomResourceRegistry;

public class EnemySpawner : Spatial
{
    [Export]
    PackedScene enemy;

    [Export]
    List<Resource> encounters = new List<Resource>();

    [Export]
    private float fightWaitTime = 2f;

    [Export(PropertyHint.Range,"0,1")]
    private float WaitTimeToEnemySpawnRatio = 0.5f;

    

    private Global global;

    private SignalManager signalManager;

    private int currentIndex = 0;


    // Called when the node enters the scene tree for the first time.
    public async override void _Ready()
    {
        global = GetNode<Global>("/root/Global");
        signalManager = GetNode<SignalManager>("/root/SignalManager");

        signalManager.Connect(nameof(SignalManager.EnemyDead),this,"OnEnemyDead");
        signalManager.Connect(nameof(SignalManager.TurnAround),this,"OnTurnAround");
        
        


        GetNode<Timer>("SpawnTimer").WaitTime = fightWaitTime * WaitTimeToEnemySpawnRatio;
        GetNode<Timer>("SpawnTimer").Start();
        
        await ToSignal(GetTree(),"idle_frame");

        signalManager.EmitSignal(nameof(SignalManager.EncounterCountUpdate),encounters.Count);

    }

    public override void _PhysicsProcess(float delta)
    {
        

    }

    private void SpawnEnemy(Resource data)
    {
        var e = enemy.Instance();

        if(data is EnemyData)
        {
            (e as Encounter).endItem = false;
            (e as Encounter).caveEntrance = false;
            (e as Encounter).Health = (data as EnemyData).Health;
            (e as Encounter).Sprite = (data as EnemyData).SpriteSheet;
            (e as Encounter).TotalAttacks = (data as EnemyData).AttacksPerSec;
            (e as Encounter).AttackTimeSum = (data as EnemyData).AttackTime;
            (e as Encounter).RestTime = (data as EnemyData).RestTime;
        }
        
        else if(data is EncounterData)
        {
            (e as Encounter).Sprite = (data as EncounterData).Sprite;
            
            if((data as EncounterData).EncounterType == EncounterData.ENCOUNTERTYPE.Dialogue)
            {
                (e as Encounter).endItem = true;
            }
            else if((data as EncounterData).EncounterType == EncounterData.ENCOUNTERTYPE.Exit)
            {
                (e as Encounter).endItem = false;
                (e as Encounter).caveEntrance = true;
            }
            
            GD.Print("Implement encounter data!");
        }
        else
        {
            GD.Print("unrecognited data.");
        }

        AddChild(e);

        GetNode<Tween>("Tween").InterpolateProperty(e,"translation",(e as Spatial).Translation,GetNode<Position3D>("EndPos").Translation,fightWaitTime - (fightWaitTime * WaitTimeToEnemySpawnRatio),Tween.TransitionType.Quad,Tween.EaseType.Out);
        GetNode<Tween>("Tween").Start();
        GetNode<Timer>("EnemyTimer").Start();
    }

    private void OnTweenCompleted(Godot.Object obj, string key)
    {
        signalManager.EmitSignal(nameof(SignalManager.FightStart));
    }

    private void OnSpawnTimerTimeout()
    {
        SpawnEnemy(encounters[currentIndex]);
    }

    private void OnEnemyTimerTimeout()
    {
        signalManager.EmitSignal(nameof(SignalManager.EnemyAppeared), fightWaitTime - (fightWaitTime * WaitTimeToEnemySpawnRatio) - GetNode<Timer>("EnemyTimer").WaitTime);
    }

    private void OnEnemyDead()
    {
        currentIndex++;
        GetNode<Timer>("SpawnTimer").Start();
    }

    private void OnTurnAround()
    {
        GD.Print("EnemySpawner - CHANGE TO SECOND ENEMY SET!");
    }


}
