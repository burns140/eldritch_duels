using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;
using eldritch.cards;
public class Insight : Effect
{
    

    public override void execute(ref Card target)
    {
        throw new System.NotImplementedException();
    }

    public override void execute(ref PlayerState ps)
    {
        Card cardToShow = ps.library[0];
        // TODO: DISPLAY THIS CARD ON THE SCREEN SOMEWHERE
    }


    public override string GetName()
    {
        return "Insight";
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