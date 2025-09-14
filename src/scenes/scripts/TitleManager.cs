using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class TitleManager : Node
{
    AnimationPlayer animPlayer;
   
    

    public override void _Ready()
    {
       
        animPlayer = GetNode<AnimationPlayer>("OpeningAnimation");
        animPlayer.Play("TitleAnimation");
        animPlayer.AnimationFinished += AnimPlayer_AnimationFinished;
       
    }

    private void AnimPlayer_AnimationFinished(StringName animName)
    {
        GD.Print($"Animation '{animName}' finished!");
        if (animName == "TitleAnimation")
        {
           
        }
    }

   

  
   
}
