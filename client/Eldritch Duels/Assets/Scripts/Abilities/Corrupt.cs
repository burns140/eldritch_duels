using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;
using eldritch.cards;

public class Corrupt : Effect
{
    public override void execute(ref Card target)
    {
        throw new System.NotImplementedException();
    }

    public override void execute(ref PlayerState ps)
    {
        ps.corruption++;
    }

    public override string GetName()
    {
        return "Corrupt";
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
