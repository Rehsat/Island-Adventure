using System.Collections;
using PixelCrew.Components;
using UnityEngine;

namespace PixelCrew.Creatures
{
    public class MobAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;
        [SerializeField] private LayerCheck _canAttack;

        [SerializeField] private float _alarmDelay = 0.5f;
        [SerializeField] private float _missHero = 0.5f;
        [SerializeField] private float _attackCooldown = 1f;
        private Coroutine _current;
        private GameObject _target;
        private Patrol _patrol;

        private static readonly int IsDeadKey = Animator.StringToHash("is-dead");

        private Animator _animator;
        private SpawnListComponent _particles;
        private Creature _creature;
        private bool _isDead;

        private void Awake()
        {
            _creature = GetComponent<Creature>();
            _animator = GetComponent<Animator>();
            _particles = GetComponent<SpawnListComponent>();
            _patrol = GetComponent<Patrol>();
        }

        private void Start()
        {
            StartState(_patrol.DoPatrol());
        }
        public void OnHeroVision(GameObject go)
        {
            if (_isDead) return;
            _target = go;

            StartState(AgroToHero());
        }
        private IEnumerator AgroToHero()
        {
            _particles.Spawn("Exclamation");
            yield return new WaitForSeconds(_alarmDelay);

            StartState(GoToHero());
        }
        IEnumerator GoToHero()
        {
            while (_vision.IsTouchingLayer)
            {
                if (_canAttack.IsTouchingLayer)
                {
                    StartState(Attack());
                }
                else
                {
                    SetDirectionToTarget();
                }
                yield return null;
            }
            _particles.Spawn("MissHero");
            yield return new WaitForSeconds(_missHero);
            StartState(_patrol.DoPatrol());
        }
        private void SetDirectionToTarget()
        {
            var direction = _target.transform.position - transform.position;
            direction.y = 0;
            _creature.SetDirection(direction.normalized);
        }
        private IEnumerator Attack()
        {
            while (_canAttack.IsTouchingLayer)
            {
                _creature.Attack();
                yield return new WaitForSeconds(_attackCooldown);
            }
            StartState(GoToHero());
        }
        private void StartState(IEnumerator coroutine)
        {
            _creature.SetDirection(Vector2.zero);
            if (_current != null)
                StopCoroutine(_current);
            

            _current = StartCoroutine(coroutine);
        }
        public void OnDie()
        {
            _creature.SetDirection(Vector2.zero);
            _isDead = true;
            _animator.SetBool(IsDeadKey, true);
            StopCoroutine(_current);
        }
    }
}