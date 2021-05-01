using Godot;
using System;

public class EncounterData : Resource
{
    public enum ENCOUNTERTYPE
    {
        None = -1,
        Pickup,
        Exit,
        Dialogue,
        Enemy
    }

    [Export]
    public ENCOUNTERTYPE EncounterType {get; set;}
    [Export]
    public Texture Sprite {get;set;}
    
    public EncounterData() { }
    public EncounterData(ENCOUNTERTYPE encounterType)
    {
        EncounterType = encounterType;
    }
    
}
