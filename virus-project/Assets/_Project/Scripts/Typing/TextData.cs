using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(menuName = "Typing/TextData", fileName = "NewText")]
    public class TextData : ScriptableObject
    {
        [TextArea(3, 10)] public string textToType;
    }
}
