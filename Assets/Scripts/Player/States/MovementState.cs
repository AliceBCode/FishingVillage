namespace FishingVillage.Player
{
    public abstract class MovementState
    {
        protected readonly PlayerController ctx;

        public abstract PlayerState Type { get; }

        protected MovementState(PlayerController context)
        {
            ctx = context;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}