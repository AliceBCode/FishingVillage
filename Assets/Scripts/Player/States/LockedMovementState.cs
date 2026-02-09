using UnityEngine;

namespace FishingVillage.Player
{
    public class LockedMovementState : MovementState
    {
        public override PlayerState Type => PlayerState.LockedInPlace;

        public LockedMovementState(PlayerController context) : base(context) { }

        public override void Enter()
        {
            ctx.velocity = Vector3.zero;
        }
        
    }
}