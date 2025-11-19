using System;

namespace Virus
{
    public interface IAchievementSouce
    {
        event Action<AchievementAttributes> OnAchievementUnlocked;

        bool IsUnlocked(string id);
        bool Unlock(string id);
        AchievementAttributes GetDefinition(string id);
    }
}
