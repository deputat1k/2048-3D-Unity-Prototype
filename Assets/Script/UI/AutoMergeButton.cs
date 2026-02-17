using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;
using Cube2048.Core.Interfaces; // 🔥 Підключаємо інтерфейси

namespace Cube2048.UI
{
    public class AutoMergeButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        private IAutoMergeService mergeService; // 🔥 Було Controller, стало Service (Інтерфейс)

        [Inject]
        public void Construct(IAutoMergeService mergeService) // 🔥 Інтерфейс у конструкторі
        {
            this.mergeService = mergeService;
        }

        private void Start()
        {
            button.onClick.AddListener(() => OnClick().Forget());
        }

        private void Update()
        {
            if (mergeService != null)
            {
                // Ми використовуємо властивості з інтерфейсу
                button.interactable = mergeService.HasPair && !mergeService.IsMerging;
            }
        }

        private async UniTaskVoid OnClick()
        {
            if (!button.interactable) return;
            button.interactable = false;
            await mergeService.TriggerMerge();
        }
    }
}