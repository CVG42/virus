using System;

namespace Virus
{
    public interface ILocalizationSource
    {
        event Action OnLanguageChanged;
        event Action<string> OnLanguageChanging;

        string GetLocalizedText(string key);
        void SetLanguage(string language);
        string CurrentLanguage { get; }
    }
}
