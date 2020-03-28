using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;
using eldritch.cards;
public class AddMana : Effect
{
    
    public override void execute(ref Card target)
    {
        throw new System.NotImplementedException();
    }

    public override void execute(ref PlayerState ps)
    {
        ps.mana++;
    }


    public override string GetName()
    {
        return "Add mana";
    }

    public override EffectTarget GetTargetType()
    {
        return EffectTarget.SELF;
    }

    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
    }

}
