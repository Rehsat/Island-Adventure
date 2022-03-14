using System;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components
{
    public class OnCollisionEnter : MonoBehaviour
    {

        [SerializeField] private string _tag;
        [SerializeField] private EnterEvent _action;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag(_tag))
            {
                _action?.Invoke(collision.gameObject);
            }
        }

        [Serializable]
        private class EnterEvent : UnityEvent<GameObject>
        {

        }

    }
}
