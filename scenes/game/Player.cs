using Godot;
using System;

public class Player : Spatial
{
    [Export]
    private NodePath staminaBarPath;
    [Export]
    private NodePath healthBarPath;

    private TextureProgress staminaBar;

    private TextureProgress healthBar;
    
    private Camera camera;

    private float timeSum;

    private Vector3 camPos;

    private Global global;

    private SignalManager signalManager;

    private bool blocking = false;

    private bool playedBlock = false;

    private int health = 5;

    private bool canAct = false;

    private bool swung = false;

    private bool died = false;

    private AnimationPlayer swordAnim;
    private AnimationPlayer shieldAnim;

    private MeshInstance shield;

    private int shieldBlockShakes = 0;

    private float stamina = 25;

    private float maxStamina = 25;

    private bool tired = false;

    // Called when the node enters the scene tree for the first time.
    public async override void _Ready()
    {
        global = GetNode<Global>("/root/Global");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        camera = GetNode<Camera>("Camera");
        swordAnim = GetNode<AnimationPlayer>("SwordAnim");
        shieldAnim = GetNode<AnimationPlayer>("ShieldAnim");
        shield = GetNode<MeshInstance>("Hands/ShieldHandle/Shield");
        staminaBar = GetNode<TextureProgress>(staminaBarPath);
        healthBar = GetNode<TextureProgress>(healthBarPath);
        camPos = camera.Translation;

        signalManager.Connect(nameof(SignalManager.EnemyAppeared),this,"OnEnemyAppeard");
        signalManager.Connect(nameof(SignalManager.EnemyDead),this,"OnEnemyDead");
        signalManager.Connect(nameof(SignalManager.EnemyAttack),this,"OnEnemyAttack");
        signalManager.Connect(nameof(SignalManager.BlockInput),this,"OnBlockInput");
        signalManager.Connect(nameof(SignalManager.AttackInput),this,"OnAttackInput");
        
        await ToSignal(GetTree(),"idle_frame");

        signalManager.EmitSignal(nameof(SignalManager.UpdatePlayerHP),health);

        swordAnim.Play("reset");
        shieldAnim.Play("reset");

    }

    public override void _PhysicsProcess(float delta)
    {
        global.distance += global.moveSpeed * 2;

        staminaBar.Value = stamina;

        if(canAct)
        {
            if(blocking && !playedBlock)
            {
                shieldAnim.Play("Raise");
                playedBlock = true;
            }
            else if(!blocking && playedBlock)
            {
                shieldAnim.Play("Lower");
                playedBlock = false;
            }
        }
        else
        {
            playedBlock = false;
            stamina = stamina < maxStamina ? stamina + 1 * delta : maxStamina;
        }

        if(shieldBlockShakes > 0)
        {
            shield.Translation = new Vector3((float)GD.RandRange(-0.025f, 0.025f), (float)GD.RandRange(-0.025f,0.025f), 0);
            shieldBlockShakes--;
        }
        else if(shieldBlockShakes <= 0)
        {
            shield.Translation = new Vector3();
        }
    }



    private void StopMoving()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Stop();
        GetNode<Tween>("Tween").StopAll();
        GetNode<Tween>("Tween").InterpolateProperty(global,nameof(global.moveSpeed),global.moveSpeed,global.minMoveSpeed,0.5f);
        GetNode<Tween>("Tween").InterpolateProperty(camera,"translation",camera.Translation,new Vector3(0,0,0),0.5f);
        GetNode<Tween>("Tween").Start();
    }
    private void StartMoving()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("walk");
        GetNode<Tween>("Tween").StopAll();
        GetNode<Tween>("Tween").InterpolateProperty(global,nameof(global.moveSpeed),global.moveSpeed,global.maxMoveSpeed,0.5f);
        GetNode<Tween>("Tween").Start();
    }

    private void PlayerDead()
    {
        signalManager.EmitSignal(nameof(SignalManager.GameOver));
    }

    private void OnEnemyAppeard()
    {
        StopMoving();
    }

    private void OnEnemyDead()
    {
        canAct = false;
        StartMoving();
        swordAnim.Play("reset");
        if(playedBlock)
        {
            shieldAnim.Play("Lower");
        }
    }

    private void OnTweenCompleted(object obj, NodePath key)
    {
        if((obj as Node).Name == global.Name && global.moveSpeed <= 0)
        {
            signalManager.EmitSignal(nameof(SignalManager.FightStart));
            canAct = true;
        }
        else
        {
            //GD.Print((obj as Node).Name+" - ",key);
        }
    }

    private void OnBlockInput(bool isBlocking)
    {
        if(tired)
        {
            blocking = false;
            return;
        }
        blocking = isBlocking;
    }

    private void OnAttackInput()
    {
        if(!blocking && canAct)
        {
            signalManager.EmitSignal(nameof(SignalManager.PlayerAttack));
            if(!swung)
            {
                swordAnim.Play("SwingLeft");
                swung = true;
            }
            else if(swung)
            {
                swordAnim.Play("SwingRight");
                swung = false;
            }
        }
    }

    private void OnEnemyAttack()
    {
        if(blocking && stamina > 0)
        {
            GD.Print("Player blocked");
            shieldBlockShakes += 5;
            stamina--;
            if(stamina <= 0)
            {
                stamina = 0;
                if(!tired)
                {
                    tired = true;
                }
            }
        }
        else
        {
            health--;
            signalManager.EmitSignal(nameof(SignalManager.UpdatePlayerHP),health);
            healthBar.Value = health;
            if(health <= 0 && !died)
            {
                died = true;
                GetNode<AnimationPlayer>("AnimationPlayer").Play("die");
            }
        }
        
    }
}
