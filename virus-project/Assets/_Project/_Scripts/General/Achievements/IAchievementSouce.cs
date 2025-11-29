using System;
using Cysharp.Threading.Tasks;

namespace Virus
{
    public interface IAchievementSouce
    {
        event Action<AchievementAttributes> OnAchievementUnlocked;

        bool IsUnlocked(string id);
        bool Unlock(string id);
        AchievementAttributes GetDefinition(string id);
        void SetOnlineMode(bool online);
        UniTask SyncWithServer();
    }
}
