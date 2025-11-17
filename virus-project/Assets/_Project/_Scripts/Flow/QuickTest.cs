using Cysharp.Threading.Tasks;
using UnityEngine;
using Virus.Flow;

namespace Virus
{
    public class QuickTest : MonoBehaviour
    {
        [SerializeField] private LoadingBar loadingScreen;

        async void Start()
        {
            if (loadingScreen == null) return;

            await loadingScreen.ShowLoadingScreen();

            for (int i = 0; i <= 100; i++)
            {
                loadingScreen.UpdateProgress(i / 100f);
                await UniTask.Delay(100);
            }

            await UniTask.Delay(1000);
            FlowManager.Source.LoadScene("Level");
        }
    }
}
