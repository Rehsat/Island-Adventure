using UnityEngine;
using UnityEngine.SceneManagement;
using PixelCrew.Model;

namespace PixelCrew.Components
{
    public class ReloadLevel : MonoBehaviour
    {
        public void Reload()
        {
            var session = FindObjectOfType<GameSession>();
            Destroy(session);
            var _scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(_scene.name);
        }
    }
}
