using UnityEngine;

namespace PixelCrew.Components
{ 
    public class SpawnComponent : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameObject _prefab;

        [ContextMenu ("Spawn")]
        public void Spawn()
        {
           var instantinate = Instantiate(_prefab, _target.position, Quaternion.identity);
            instantinate.transform.localScale = transform.lossyScale;
        }
    }
}
