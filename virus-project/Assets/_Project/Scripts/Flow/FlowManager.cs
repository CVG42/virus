using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Virus.Flow
{
    public class FlowManager : Singleton<IFlowSource>, IFlowSource
    {
        [SerializeField] private CanvasGroup _levelTransitionCanvasGroup;

        private async UniTask LoadSceneAsync(string sceneName)
        {
            _levelTransitionCanvasGroup.gameObject.SetActive(true);
            await _levelTransitionCanvasGroup.DOFade(1, 2).AsyncWaitForCompletion();
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).ToUniTask();
            await _levelTransitionCanvasGroup.DOFade(0, 1.5f).AsyncWaitForCompletion();
            _levelTransitionCanvasGroup.gameObject.SetActive(false);
        }

        public void LoadScene(string sceneName)
        {
            LoadSceneAsync(sceneName).Forget();
        }
    }
}
