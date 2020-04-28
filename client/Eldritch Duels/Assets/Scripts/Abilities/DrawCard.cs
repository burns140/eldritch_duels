using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;
using eldritch.cards;
public class DrawCard : Effect
{
    
    public override void execute(ref Card target)
    {
        throw new System.NotImplementedException();
    }

    public override void execute(ref PlayerState ps)
    {
        //DuelFunctions.DrawCard(ref ps);
    }


    public override string GetName()
    {
        return "Draw card";
    }

    public override EffectTarget GetTargetType()
    {
        return EffectTarget.DRAW;
    }

    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
    }

}
