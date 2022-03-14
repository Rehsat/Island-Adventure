using UnityEngine;
using System;
using UnityEngine.Events;

namespace PixelCrew.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private HealthChangeEvent _onChange;

        public void ChangeHealth(int changeValue)
        {
            if (_health <= 0) return;
            _health += changeValue;
            _onHeal?.Invoke();
            _onChange?.Invoke(_health);
            if (_health <= 0)
            {
                _onDie?.Invoke();
            }
            else if (changeValue < 0)
            {
                _onDamage?.Invoke();
            }

        }

       [Serializable] public class HealthChangeEvent : UnityEvent<int>
       {
       }
        public void SetHealth(int health)
        {
            _health = health;
        }
    }
}
