using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace Virus
{
    public class DialogueManager : Singleton<IDialogueSource>, IDialogueSource
    {
        [Header("Cinematic format")]
        [SerializeField] private Canvas _dialogueCinematicCanvas;
        [SerializeField] private Image _characterCinematicImage;
        [SerializeField] private TextMeshProUGUI _characterCinematicName;
        [SerializeField] private TextMeshProUGUI _dialogueCinematicArea;
        [SerializeField] private Button _dialogueButton;
        [SerializeField] private float _typingCinematicSpeed = 0.2f;
        [SerializeField] private RectTransform _dialogueCinematicRectTransform;
        [SerializeField] private Action _onCinematicDialogueEnd;

        [Header("Gameplay format")]
        [SerializeField] private Canvas _dialogueGameplayCanvas;
        [SerializeField] private Image _characterGameplayImage;
        [SerializeField] private TextMeshProUGUI _characterGameplayName;
        [SerializeField] private TextMeshProUGUI _dialogueGameplayArea;
        [SerializeField] private float _typingGameplaySpeed = 0.05f;
        [SerializeField] private RectTransform _dialogueGameplayRectTransform;
        [SerializeField] private Action _onGameplayDialogueEnd;

        public event Action OnCinematicDialogueEnd;

        private bool _isCinematicTyping = false, _isGameplayTyping = false, _skipTypingCinematic = false;
        private string _currentSentence = "";
        private readonly Queue<DialogueLine> _cinematicLines = new Queue<DialogueLine>();
        private readonly Queue<DialogueLine> _gameplayLines = new Queue<DialogueLine>();
        private CancellationTokenSource _gameplayCts;

        private void Start()
        {
            _dialogueButton.onClick.AddListener(DisplayNextCinematicDialogueLine);
        }

        private void OnDestroy()
        {
            InputManager.Source.OnConfirmButtonPressed -= DisplayNextCinematicDialogueLine;
            _gameplayCts?.Cancel();
            _gameplayCts?.Dispose();
        }

        public void StartCinematicDialogue(Dialogue dialogue, Action onDialogueEnd)
        {
            _onCinematicDialogueEnd = onDialogueEnd;
            CinematicDialogues(dialogue);
            EnableCinematicDialogue();
            DisplayNextCinematicDialogueLine();
        }

        private void CinematicDialogues(Dialogue dialogue)
        {
            InputManager.Source.OnConfirmButtonPressed -= DisplayNextCinematicDialogueLine;
            InputManager.Source.OnConfirmButtonPressed += DisplayNextCinematicDialogueLine;

            _cinematicLines.Clear();

            foreach (DialogueLine dialogueline in dialogue.DialogueLines)
            {
                _cinematicLines.Enqueue(dialogueline);
            }
        }

        private void EnableCinematicDialogue()
        {
            _dialogueCinematicCanvas.enabled = true;
            _dialogueCinematicRectTransform.DOAnchorPosY(0, 0.5f, true);
        }

        public void DisplayNextCinematicDialogueLine()
        {
            if (_isCinematicTyping)
            {
                _skipTypingCinematic = true;
                return;
            }

            if (_cinematicLines.Count == 0)
            {
                EndCinematicDialogue();
                return;
            }

            DialogueLine currentline = _cinematicLines.Dequeue();

            _characterCinematicImage.sprite = currentline.Character.Icon;
            _characterCinematicName.text = currentline.Character.Name.Localize();

            TypeCinematicSentence(currentline);
        }

        private async void TypeCinematicSentence(DialogueLine dialogueline)
        {
            _isCinematicTyping = true;
            _dialogueCinematicArea.text = "";
            _skipTypingCinematic = false;
            _currentSentence = dialogueline.Line.Localize();

            foreach (char letter in _currentSentence)
            {
                if (_skipTypingCinematic)
                {
                    _dialogueCinematicArea.text = _currentSentence;
                    break;
                }

                _dialogueCinematicArea.text += letter;

                await UniTask.Delay(TimeSpan.FromSeconds(_typingCinematicSpeed), DelayType.DeltaTime);
                // AudioManager.Source.TypingSFX();
            }
            _isCinematicTyping = false;
        }

        private void EndCinematicDialogue()
        {
            _onCinematicDialogueEnd?.Invoke();
            OnCinematicDialogueEnd?.Invoke();
            DisableCinematicDialogue().Forget();
            GameManager.Source.ChangeState(GameState.OnPlay);
        }

        private async UniTask DisableCinematicDialogue()
        {
            _dialogueCinematicRectTransform.DOAnchorPosY(-215, 0.5f, true).WaitForCompletion();
            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime);
            _dialogueCinematicCanvas.enabled = false;
        }

        public void StartGameplayDialogue(Dialogue dialogue, Action onDialogueEnd)
        {
            _gameplayCts?.Cancel();
            _gameplayCts = new CancellationTokenSource();

            _onGameplayDialogueEnd = onDialogueEnd;
            GameplayDialogues(dialogue);
            EnableGameplayDialogue();
            DisplayNextGameplayDialogueLine();
        }

        private void GameplayDialogues(Dialogue dialogue)
        {
            _gameplayLines.Clear();

            foreach (DialogueLine dialogueline in dialogue.DialogueLines)
            {
                _gameplayLines.Enqueue(dialogueline);
            }
        }

        private void EnableGameplayDialogue()
        {
            _dialogueGameplayCanvas.enabled = true;
            _dialogueGameplayRectTransform.DOAnchorPosX(0, 0.5f, true);
        }

        private void DisplayNextGameplayDialogueLine()
        {
            if (_gameplayLines.Count == 0)
            {
                EndGameplayCinematic();
                return;
            }

            DialogueLine currentline = _gameplayLines.Dequeue();

            _characterGameplayImage.sprite = currentline.Character.Icon;
            _characterGameplayName.text = currentline.Character.Name.Localize();

            TypeGameplaySentence(currentline);
        }

        private async void TypeGameplaySentence(DialogueLine dialogueline)
        {
            _isGameplayTyping = true;
            _dialogueGameplayArea.text = "";
            _currentSentence = dialogueline.Line.Localize();

            try
            {
                foreach (char letter in _currentSentence)
                {
                    _dialogueGameplayArea.text += letter;
                    await UniTask.Delay(TimeSpan.FromSeconds(_typingGameplaySpeed), DelayType.DeltaTime, cancellationToken: _gameplayCts.Token);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: _gameplayCts.Token);
                DisplayNextGameplayDialogueLine();
            }
            catch (OperationCanceledException)
            {
                // Dialogue was cancelled - this is normal when interrupting
            }
            finally
            {
                _isGameplayTyping = false;
            }
        }

        private void EndGameplayCinematic()
        {
            _onGameplayDialogueEnd?.Invoke();
            DisableGameplayDialogue().Forget();
        }

        private async UniTask DisableGameplayDialogue()
        {
            _dialogueGameplayRectTransform.DOAnchorPosX(640, 0.5f, true);
            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime);
            _dialogueGameplayCanvas.enabled = false;
        }
    }

    [Serializable]
    public class DialogueLine
    {
        public DialogueCharacter Character;
        [TextArea(3, 10)]
        public string Line;
    }
}
