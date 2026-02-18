using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;
using Cube2048.Core.Interfaces;

namespace Cube2048.UI
{
    public class AutoMergeButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        private IAutoMergeService mergeService;

        [Inject]
        public void Construct(IAutoMergeService mergeService)
        {
            this.mergeService = mergeService;
        }

        private void Start()
        {
            button.onClick.AddListener(() => OnClick().Forget());

            // 🔥 ПІДПИСКА НА ПОДІЮ
            mergeService.OnStatusChanged += UpdateButtonState;

            // Початковий стан (вимкнено при старті)
            UpdateButtonState(false);
        }

        private void OnDestroy()
        {
            if (mergeService != null)
            {
                // 🔥 ВІДПИСКА (Обов'язково!)
                mergeService.OnStatusChanged -= UpdateButtonState;
            }
        }

        // Цей метод викликається сам, коли контролер щось вирішив
        private void UpdateButtonState(bool isInteractable)
        {
            button.interactable = isInteractable;
        }

        private async UniTaskVoid OnClick()
        {
            if (!button.interactable) return;

            // Вимикаємо одразу, щоб не клікнути двічі
            button.interactable = false;

            await mergeService.TriggerMerge();
        }
    }
}