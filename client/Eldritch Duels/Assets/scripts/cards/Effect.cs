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
    public abstract class Effect
    {
        protected GameObject target = null;
        public virtual void SetTarget(GameObject target)
        {
            if (target != null && target.tag.Equals("targetable"))
            {
                this.target = target;
            }
        }
        public abstract void execute();
        public abstract string GetName();
    }
}
