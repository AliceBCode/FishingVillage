using FishingVillage.Gameplay;
using FishingVillage.Interactable;
using UnityEngine;

namespace FishingVillage.Player
{
    public class ConstrainedMovementState : MovementState
    {
        private IConstrainablePath _path;
        private float _currentPathT;

        public override PlayerState Type => PlayerState.Constrained;

        public ConstrainedMovementState(PlayerController context) : base(context) { }

        public void SetPath(ConstrainableRopePath path)
        {
            _path = path;
        }

        public override void Enter()
        {
            if (_path == null)
            {
                ctx.SetNormal();
                return;
            }
            
            ctx.velocity = Vector3.zero;
            _currentPathT = _path.GetClosestT(ctx.transform.position);
        }

        public override void Exit()
        {
            _path?.Release();
            _path = null;
        }

        public override void Update()
        {
            if (ctx.jumpBufferTimer > 0)
            {
                ctx.velocity = new Vector3(ctx.velocity.x, ctx.jumpForce, ctx.velocity.z);
                ctx.jumpBufferTimer = 0;
                ctx.SetNormal();
            }
        }

        public override void FixedUpdate()
        {
            if (_path == null) return;

            float moveInput = ctx.Input.MoveInput.x;
            float pathLength = Vector3.Distance(_path.GetPositionAt(0), _path.GetPositionAt(1));
            float tSpeed = ctx.moveSpeed / pathLength;

            _currentPathT += moveInput * tSpeed * Time.fixedDeltaTime;
            _currentPathT = Mathf.Clamp01(_currentPathT);

            Vector3 targetPos = _path.GetPositionAt(_currentPathT);
            Vector3 moveDelta = targetPos - ctx.transform.position;
            
            ctx.Controller.Move(moveDelta);
        }
    }
}