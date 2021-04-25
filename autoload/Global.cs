using Godot;
using System;

public class Global : Node
{
    
    public float minMoveSpeed = 0f;
    public float maxMoveSpeed = 0.014f;
    public float caveLength = 25;

    public float moveSpeed = 0.014f;

    public float distance = 0;

    public int health = 3;

    public bool leavingCave = false;

    public void Reset()
    {
        moveSpeed = maxMoveSpeed;
        distance = 0;
        health = 3;
        leavingCave = false;
    }
}
