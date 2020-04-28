using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eldritch.cards
{
    public enum EffectOrder
    {
        START_OF_TURN,
        ON_ATTACK,
        ON_PLAY,
        END_OF_TURN,
        NULL

    }

    public enum EffectTarget{
        SELF,
        OPPONENT,
        CARD,
        DRAW,
        NULL
    }
    public abstract class Effect
    {
        protected GameObject target = null;
        protected EffectTarget toTarget = EffectTarget.NULL;
        public virtual void SetTarget(GameObject target)
        {
            if (target != null && target.tag.Equals("targetable"))
            {
                this.target = target;
            }
        }
        public abstract void execute(ref Card target);
        public abstract void execute(ref PlayerState ps);
        public abstract string GetName();
        public abstract EffectTarget GetTargetType();
    }
}
