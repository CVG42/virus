using UnityEngine;

namespace Virus
{
    public class PlayLevelMusic : MonoBehaviour
    {
        [SerializeField] private string _musicName;

        private void Start()
        {
            AudioManager.Source.PlayLevelMusic(_musicName);
        }
    }
}
