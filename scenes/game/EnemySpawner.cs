using Godot;
using Godot.Collections;
using System;

public class EnemySpawner : Spatial
{
    [Export]
    PackedScene enemy;

    [Export]
    Texture [] aliveSprites = new Texture [0];

    [Export]
    string [] attackTimings = new string [0];

    private Global global;

    private SignalManager signalManager;

    private float [] enemyPoints = new float [] {1f, 6f, 11f, 16f, 21f, 26f, 31f, 36f};

    private int currentIndex = 0;

    private bool spawned = false;
    private bool endReached = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");
        signalManager = GetNode<SignalManager>("/root/SignalManager");

        signalManager.Connect(nameof(SignalManager.EnemyDead),this,"OnEnemyDead");
        signalManager.Connect(nameof(SignalManager.TurnAround),this,"OnTurnAround");
    }

    public override void _PhysicsProcess(float delta)
    {
        if(currentIndex < enemyPoints.Length)
        {
            if(global.distance >= enemyPoints[currentIndex] && !spawned)
            {
                if(currentIndex == 3 || currentIndex == 7)
                {
                    if(!endReached)
                    {
                        if(!global.leavingCave)
                        {
                            SpawnEnemy(aliveSprites[3],true);
                        }
                        else
                        {
                            SpawnEnemy(aliveSprites[7],false,true);
                        }
                        endReached = true;
                        spawned = true;
                        return;
                    }
                    else
                    {
                        GD.Print("endReached "+ endReached+ " - Spawn "+ currentIndex);
                    }
                }
                SpawnEnemy(aliveSprites[currentIndex]);
                spawned = true;
            }
        }

    }

    private void SpawnEnemy(Texture aliveSprite, bool isEndItem = false, bool isCaveEntrance = false)
    {
        var e = enemy.Instance();

        (e as Enemy).endItem = isEndItem;
        (e as Enemy).caveEntrance = isCaveEntrance;
        (e as Enemy).sprite = aliveSprite;
        (e as Enemy).TotalAttacks = int.Parse((attackTimings[currentIndex][0]).ToString());
        (e as Enemy).AttackTimeSum = int.Parse((attackTimings[currentIndex][1]).ToString());
        (e as Enemy).RestTime = (float)Convert.ToDouble((attackTimings[currentIndex][2]).ToString()) / (float)Convert.ToDouble((attackTimings[currentIndex][3]).ToString());
        

        if(isCaveEntrance)
        {
            (e as Enemy).GetNode<Sprite3D>("Sprite").Texture = aliveSprite;
        }
        AddChild(e);

        GetNode<Tween>("Tween").InterpolateProperty(e,"translation",(e as Spatial).Translation,GetNode<Position3D>("EndPos").Translation,global.enemySpeed,Tween.TransitionType.Quad,Tween.EaseType.Out);
        GetNode<Tween>("Tween").Start();
        GetNode<Timer>("EnemyTimer").Start();
        
    }

    private void OnTweenCompleted(Godot.Object obj, string key)
    {
        signalManager.EmitSignal(nameof(SignalManager.FightStart));
    }

    private void OnEnemyTimerTimeout()
    {
        signalManager.EmitSignal(nameof(SignalManager.EnemyAppeared), global.enemySpeed - GetNode<Timer>("EnemyTimer").WaitTime);
    }

    private void OnEnemyDead()
    {
        currentIndex++;
        spawned = false;
    }

    private void OnTurnAround()
    {
        endReached = false;

        GD.Print("EnemySpawner - CHANGE TO SECOND ENEMY SET!");
    }


}
