using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;
using eldritch.cards;
public class Destroy : Effect
{
    
    public override void execute(ref Card target)
    {
        //DuelFunctions.destroyMinion((GameObject) target);
    }

    public override void execute(ref PlayerState ps)
    {
        throw new System.NotImplementedException();
    }


    public override string GetName()
    {
        return "Destroy";
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