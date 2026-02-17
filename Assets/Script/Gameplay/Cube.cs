using UnityEngine;
using TMPro;
using Zenject;
using Cube2048.Data;
using Cube2048.Core.Interfaces; // Підключаємо інтерфейси

namespace Cube2048.Gameplay
{
    public class Cube : MonoBehaviour
    {
        [Header("Settings")]
        // Використовуємо backing field для властивості
        [SerializeField] private int initialValue = 2;
        [SerializeField] private GameSettings settings;

        [Header("Visuals")]
        [SerializeField] private TMP_Text[] valueTexts;
        [SerializeField] private Renderer cubeRenderer;

        // Властивість Value (Публічна для читання, приватна для запису)
        public int Value { get; private set; }

        // 🔥 Тільки одна змінна для Спавнера (Інтерфейс)
        private ICubeSpawner spawner;

        // 🔥 Тільки одна змінна для Очок (Інтерфейс)
        private IScoreService scoreService;

        private Rigidbody rb;
        private bool hasMerged = false;
        private float minX;
        private float maxX;

        [Inject]
        // 🔥 Конструктор приймає ТІЛЬКИ інтерфейси
        public void Construct(ICubeSpawner spawner, IScoreService scoreService)
        {
            this.spawner = spawner;
            this.scoreService = scoreService;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            // Ініціалізуємо Value значенням з інспектора при старті
            Value = initialValue;
        }

        private void Start()
        {
            UpdateVisuals();
        }

        public void Init(float leftLimit, float rightLimit)
        {
            minX = leftLimit;
            maxX = rightLimit;
        }

        // Метод для оновлення значення (наприклад, при спавні з пулу)
        public void SetValue(int newValue)
        {
            Value = newValue;
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            if (valueTexts != null)
            {
                foreach (var text in valueTexts)
                    if (text != null) text.text = Value.ToString();
            }

            if (cubeRenderer != null && settings != null)
            {
                cubeRenderer.material.color = GetColorForValue(Value);
            }
        }

        private Color GetColorForValue(int value)
        {
            if (settings == null || settings.CubeColors == null) return Color.white;

            // value має бути > 0
            if (value <= 0) return Color.white;

            int index = (int)Mathf.Log(value, 2) - 1;

            if (index >= 0 && index < settings.CubeColors.Length)
                return settings.CubeColors[index];

            return Color.white;
        }

        public void Move(float deltaX)
        {
            if (settings == null) return;
            Vector3 pos = transform.position;
            pos.x += deltaX * settings.MoveSpeed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }

        public void Shoot()
        {
            if (rb != null && settings != null)
            {
                rb.AddForce(Vector3.forward * settings.PushForce, ForceMode.Impulse);
            }
        }

        public void ResetCube()
        {
            hasMerged = false;
            if (rb != null)
            {
                rb.isKinematic = false;
                // Для Unity 6 використовуй linearVelocity, для старих - velocity
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Bounce()
        {
            if (rb != null && settings != null)
            {
                // Додаємо трохи рандому, щоб не стояли стовпчиком
                Vector3 randomDir = Random.insideUnitSphere * 0.5f;
                randomDir.y = 0;
                rb.AddForce((Vector3.up + randomDir) * settings.MergePushForce, ForceMode.Impulse);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasMerged) return;
            if (collision.gameObject == null) return;

            // Намагаємось отримати компонент Cube
            if (collision.gameObject.TryGetComponent<Cube>(out Cube otherCube))
            {
                if (otherCube.hasMerged) return;

                // Перевірка: однакове значення і ID (щоб зливався тільки один, а не обидва одразу)
                if (otherCube.Value == Value && this.GetInstanceID() < otherCube.GetInstanceID())
                {
                    Merge(otherCube);
                }
            }
        }

        private void Merge(Cube otherCube)
        {
            hasMerged = true;
            otherCube.hasMerged = true;

            int newValue = Value * 2;

            Vector3 spawnPos = (transform.position + otherCube.transform.position) / 2f;
            spawnPos.y += 0.5f;

            // Використовуємо інтерфейси
            if (spawner != null)
            {
                spawner.ReturnToPool(this);
                spawner.ReturnToPool(otherCube);

                Cube newCube = spawner.SpawnSpecific(spawnPos, newValue);
                if (newCube != null)
                {
                    newCube.Bounce();
                }
            }

            if (scoreService != null)
            {
                scoreService.AddScore(newValue); // Додаємо значення куба, а не 1
            }
        }
    }
}