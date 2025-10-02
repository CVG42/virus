using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Virus
{
    public class QuickTest : MonoBehaviour
    {
        [SerializeField] private LoadingBar loadingScreen;

        async void Start()
        {
            if (loadingScreen == null) return;

            // Auto-test on start
            await loadingScreen.ShowLoadingScreen();

            // Simulate loading
            for (int i = 0; i <= 100; i++)
            {
                loadingScreen.UpdateProgress(i / 100f);
                await UniTask.Delay(100);
            }

            await UniTask.Delay(1000); // Show completion for 1 second
            await loadingScreen.HideLoadingScreen();
        }
    }
}
