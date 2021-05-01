using Godot;
using System;

public class SignalManager : Node
{
    [Signal]
    public delegate void EnemyAttack();

    [Signal]
    public delegate void PlayerAttack();

    [Signal]
    public delegate void EnemyAppeared(float stopMovingTime);

    [Signal]
    public delegate void EnemyDead();

    [Signal]
    public delegate void SpawnEnemy();

    [Signal]
    public delegate void FightStart();

    [Signal]
    public delegate void PlayerBlocking(bool blocking);

    [Signal]
    public delegate void AttackInput();

    [Signal]
    public delegate void BlockInput(bool blocking);

    [Signal]
    public delegate void UpdatePlayerHP(int HPLeft);

    [Signal]
    public delegate void GameOver();

    [Signal]
    public delegate void Victory();

    [Signal]
    public delegate void TurnAround();

    [Signal]
    public delegate void CaveEndReached();

    [Signal]
    public delegate void CaveBeginingReached();

    [Signal]
    public delegate void EncounterCountUpdate(int encounterCount);
}
