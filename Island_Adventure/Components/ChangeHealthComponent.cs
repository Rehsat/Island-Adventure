using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components
{
    public class ChangeHealthComponent : MonoBehaviour
    {
        [SerializeField] private int _changeValue;

        public void ChangeHealth (GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if(healthComponent != null)
            {
                healthComponent.ChangeHealth(_changeValue);
            }
        }
    }
}
