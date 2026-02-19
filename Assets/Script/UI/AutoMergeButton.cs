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

            mergeService.OnStatusChanged += UpdateButtonState;

            UpdateButtonState(false);

        }

        private void OnDestroy()
        {
            if (mergeService != null)
            {
                mergeService.OnStatusChanged -= UpdateButtonState;
            }
        }

        private void UpdateButtonState(bool isInteractable)
        {
            button.interactable = isInteractable;
        }

        private async UniTaskVoid OnClick()
        {
            if (!button.interactable) return;

            button.interactable = false;

            await mergeService.TriggerMerge();

        }
    }
}