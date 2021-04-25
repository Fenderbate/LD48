using Godot;
using System;

public class CaveWalls : Spatial
{
    [Export]
    ShaderMaterial wallMaterial;

    Vector2 matUVOffset;

    

    Global global;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");
    }


  public override void _Process(float delta)
  {
      wallMaterial.SetShaderParam("uv_offset", new Vector2(0, -global.distance));
  }
}
