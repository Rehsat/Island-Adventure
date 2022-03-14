using UnityEngine;
using PixelCrew.Utils;
using PixelCrew.Components;
using PixelCrew.Model;

namespace PixelCrew.Creatures
{
    public class Hero : Creature
    {
        [SerializeField] private CheckCircleOverlaps _interactionCheck;
        [SerializeField] private float _interactionRadius;
        [SerializeField] private float _slamDownVelocity;

        [SerializeField] private LayerCheck _wallCheck;
        [SerializeField] private LayerMask _interactionLayer;
        [SerializeField] private ParticleSystem _hitParticle;
        [SerializeField] private Cooldown _throwCooldown;

        private static readonly int ThrowKey = Animator.StringToHash("throw");

        [SerializeField] private RuntimeAnimatorController _armed;
        [SerializeField] private RuntimeAnimatorController _unarmed;

        private GameSession _session;

        private int CoinsCount => _session.Data.Inventory.Count("Coins");
        private int SwordCount => _session.Data.Inventory.Count("Swords");

        private bool _allowDoubleJump;
        private bool _isOnWall;

        protected override void Awake()
        {
            base.Awake();
        }
        public void OnHealthChange(int currentHealth)
        {
            _session.Data.Hp = currentHealth;
        }
        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            var health = GetComponent<HealthComponent>();
            health.SetHealth(_session.Data.Hp);
            _session.Data.Inventory.OnChanged += OnInventoryChanged;

            UpdateHeroWeapon();
        }
        protected override void Update()
        {
            base.Update();
        }
        private void OnDestroy()
        {
            _session.Data.Inventory.OnChanged -= OnInventoryChanged;
        }
        private void OnInventoryChanged(string id, int value)
        {
            if (id == "Swords")
                UpdateHeroWeapon();
        }
        protected override float CalculateYVelocity()
        {
            var yVelocity = Rigidbody.velocity.y;
            var jumping = Direction.y > 0;
            if (_isGrounded || _isOnWall)
            {
                _allowDoubleJump = true;
            }
            if(!jumping && _isOnWall)
            {
                return 0f;
            }
            return base.CalculateYVelocity();
        }
        protected override float CalculateJumpVelocity(float yVelocity)
        {
            if (!_isGrounded &&_allowDoubleJump)
            {
                Particles.Spawn("Jump");
                Sounds.Play("Jump");
                _allowDoubleJump = false;
                return JumpLength;
            }
            return base.CalculateJumpVelocity(yVelocity);
        }

        public void AddInInventory(string id, int value)
        {
            _session.Data.Inventory.Add(id, value);
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            if (CoinsCount > 0)
            {
                SpawnCoins();
            }
        }
        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(CoinsCount, 5);
            _session.Data.Inventory.Remove("Coins", numCoinsToDispose);

            var burst = _hitParticle.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticle.emission.SetBurst(0, burst);

            _hitParticle.gameObject.SetActive(true);
            _hitParticle.Play();

        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(other.gameObject.IsInLayer(GroundLayer))
            {
                var contact = other.contacts[0];
                if(contact.relativeVelocity.y >= _slamDownVelocity)
                {
                    Particles.Spawn("SlamDown");
                }
            }
        }
       
        public override void Attack()
        {
            if (SwordCount <= 0) return;

            base.Attack();
        }
       
        public void Interact()
        {
            _interactionCheck.Check();
         
        }
        private void UpdateHeroWeapon()
        {
            Animator.runtimeAnimatorController = SwordCount > 0? _armed : _unarmed; 
        }
        public void Throw()
        {
            if (_throwCooldown.IsReady)
            {
                Animator.SetTrigger(ThrowKey);
                _throwCooldown.Reset();
            }
        }
        public void OnDoThrow()
        {
            Particles.Spawn("Throw");

            Sounds.Play("Range");
        }
    }
}
