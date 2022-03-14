using System.Collections;
using PixelCrew.Components;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Creatures
{
    public class Creature : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private bool _invertScale;
        [SerializeField] protected float JumpLength;
        [SerializeField] private float _damageVelocity;
        [SerializeField] private float _speed;
        [SerializeField] private int _damage;

        [Header("Checkers")]
        [SerializeField] protected LayerCheck GroundCheck;
        [SerializeField] protected LayerMask GroundLayer;
        [SerializeField] protected SpawnListComponent Particles;
        [SerializeField] private CheckCircleOverlaps _attackRange;

        protected Vector2 Direction;
        protected Rigidbody2D Rigidbody;
        protected Animator Animator;
        protected PlaySoundComponent Sounds;
        private bool _isJumping;
        protected bool _isGrounded;

        private static readonly int IsRunningKey = Animator.StringToHash("is-running");
        private static readonly int OnGroundKey = Animator.StringToHash("on-ground");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            Sounds = GetComponent<PlaySoundComponent>();
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }
        protected virtual void Update()
        {
            _isGrounded = GroundCheck.IsTouchingLayer;
        }
        private void FixedUpdate()
        {
            var xVelocity = Direction.x * _speed;
            var yVelocity = CalculateYVelocity();
            Rigidbody.velocity = new Vector2(xVelocity, yVelocity);

            Animator.SetBool(IsRunningKey, Direction.x != 0);
            Animator.SetBool(OnGroundKey, _isGrounded);
            Animator.SetFloat(VerticalVelocityKey, Rigidbody.velocity.y);

            UpdateSpriteDirection();
        }
        protected virtual float CalculateYVelocity()
        {
            var yVelocity = Rigidbody.velocity.y;
            var jumping = Direction.y > 0;
            if (jumping)
            {
                _isJumping = true;
                bool isFalling = Rigidbody.velocity.y <= 0.001f;
                yVelocity = isFalling ? CalculateJumpVelocity(yVelocity) : yVelocity;
            }
            else if (Rigidbody.velocity.y > 0 && _isJumping)
            {
                yVelocity *= 0.3f;
            }
            return yVelocity;
        }
        protected virtual float CalculateJumpVelocity(float yVelocity)
        {
            if (_isGrounded)
            {
                yVelocity += JumpLength;
                Sounds.Play("Jump");
                Particles.Spawn("Jump");
            }
            return yVelocity;
        }

        public void UpdateSpriteDirection()
        {
            var multiplier = _invertScale ? -1 : 1;
            if (Direction.x > 0)
            {
                transform.localScale = new Vector3(multiplier, 1, 1);
            }
            else if (Direction.x < 0)
            {
                transform.localScale = new Vector3(-1 * multiplier, 1, 1);
            }
        }
        public virtual void TakeDamage()
        {
            _isJumping = false;
            Animator.SetTrigger(Hit);
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, _damageVelocity);
        }
        public virtual void Attack()
        {
            Animator.SetTrigger(AttackKey);
            Sounds.Play("Melee");
        }
        public void OnDoAttack()
        {
            _attackRange.Check();
        }
    }
}
