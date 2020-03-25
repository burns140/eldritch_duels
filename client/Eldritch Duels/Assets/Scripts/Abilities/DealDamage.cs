using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch.cards;
using eldritch;
public class DealDamage : Effect
{
    
    public override void execute(ref Card target)
    {
        throw new System.NotImplementedException();
    }

    public override void execute(ref PlayerState ps)
    {
        ps.hp--;
    }


    public override string GetName()
    {
        return "Damage Opp";
    }

    public override EffectTarget GetTargetType()
    {
        return EffectTarget.OPPONENT;
    }

    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
    }

    
}
