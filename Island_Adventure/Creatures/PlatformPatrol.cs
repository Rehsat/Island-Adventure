using System.Collections;
using PixelCrew;
using UnityEngine;

namespace PixelCrew.Creatures
{
    public class PlatformPatrol : Patrol
    {
        [SerializeField] private LayerCheck _notAbyssCheck;
        [SerializeField] private float _direction = 1;
        private Creature _creature;
       

        private void Awake()
        {
            _creature = GetComponent<Creature>();
        }

        public override IEnumerator DoPatrol()
        {
           var direction = new Vector2(_direction, 0);
           while (enabled)
           {
                if (!_notAbyssCheck.IsTouchingLayer)
                {
                    direction.x = -direction.x;
                }
                _creature.SetDirection(direction.normalized);
                yield return null;
            }

        }
    }
}
