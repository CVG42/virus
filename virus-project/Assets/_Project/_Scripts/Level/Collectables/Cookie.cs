using UnityEngine;

namespace Virus
{
    public class Cookie : MonoBehaviour
    {
        [SerializeField] GameObject _cookieModel;
        [SerializeField] ParticleSystem _particles;

        private bool _isCollected = false;

        private void Start()
        {
            _cookieModel.SetActive(true);
        }

        private void Collect()
        {
            _isCollected = true;

            CollectablesManager.Source.AddCookie();
            InjectionManager.Source.AddProgress(1);

            _cookieModel.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return;

            if (other.gameObject.CompareTag("Player"))
            {
                _particles.Play();
                AudioManager.Source.PlayCookieSFX();
                Collect();
            }
        }
    }
}
