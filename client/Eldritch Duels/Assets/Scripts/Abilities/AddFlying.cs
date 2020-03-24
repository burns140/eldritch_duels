using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch.cards;
using eldritch;
public class AddFlying : Effect
{
    
    public override void execute(ref Card target)
    {
        target.HasFly = true;
    }

    public override void execute(ref PlayerState ps)
    {
        throw new System.NotImplementedException();
    }


    public override string GetName()
    {
        return "Add fly";
    }

    public override EffectTarget GetTargetType()
    {
        return EffectTarget.CARD;
    }

    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
    }

    
}
