using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Virus
{
    public interface IUISource
    {
        void RegisterButton(string key, Button button);
        void AssignButtonAction(string key, UIActionData action);
        void LockAllButtons();
        void UnlockAllButtons();
        UniTask UnlockAfterDelayAsync(int delay);
        void OpenSettingsScreen();
        void CloseSettingsScreen();
    }
}
