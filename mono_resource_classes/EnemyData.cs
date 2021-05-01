using Godot;
using System;

public class EnemyData : Resource
{
    [Export]
    public Texture SpriteSheet;
    [Export]
    public int Health {get; set; }
    [Export]
    public int AttacksPerSec {get; set; }
    [Export]
    public float AttackTime {get; set; }
    [Export]
    public float RestTime {get; set; }

    
    public EnemyData() { }
    public EnemyData(Texture spriteSheet, int health, int attacksPerSec, float attackTime, float restTime)
    {
        SpriteSheet = spriteSheet;
        Health = health;
        AttacksPerSec = attacksPerSec;
        AttackTime = attackTime;
        RestTime = restTime;
    }
}
