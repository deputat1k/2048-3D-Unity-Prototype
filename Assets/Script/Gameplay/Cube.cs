using UnityEngine;
using TMPro;
using Zenject;
using Cube2048.Data;
using Cube2048.Core.Interfaces;

namespace Cube2048.Gameplay
{
    public class Cube : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int initialValue = 2;
        [SerializeField] private GameSettings settings;

        [Header("Visuals")]
        [SerializeField] private TMP_Text[] valueTexts;
        [SerializeField] private Renderer cubeRenderer;

        public int Value { get; private set; }

        // 🔥 НОВА ВЛАСТИВІСТЬ: Чи знаходиться куб у грі (вже вилетів)
        public bool IsLaunched { get; private set; } = false;

        private ICubeSpawner spawner;
        private IScoreService scoreService;

        private Rigidbody rb;
        private bool hasMerged = false;
        private float minX;
        private float maxX;

        [Inject]
        public void Construct(ICubeSpawner spawner, IScoreService scoreService)
        {
            this.spawner = spawner;
            this.scoreService = scoreService;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
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
            if (value <= 0) return Color.white;
            int index = (int)Mathf.Log(value, 2) - 1;
            if (index >= 0 && index < settings.CubeColors.Length) return settings.CubeColors[index];
            return Color.white;
        }

        public void Move(float deltaX)
        {
            // Рухати можна тільки якщо ще не вистрелили
            if (IsLaunched) return;

            if (settings == null) return;
            Vector3 pos = transform.position;
            pos.x += deltaX * settings.MoveSpeed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }

        public void Shoot()
        {
            // 🔥 ПОЗНАЧАЄМО, ЩО КУБ У ГРІ
            IsLaunched = true;

            if (rb != null && settings != null)
            {
                rb.AddForce(Vector3.forward * settings.PushForce, ForceMode.Impulse);
            }
        }

        public void ResetCube()
        {
            hasMerged = false;
            // 🔥 СКИДАЄМО СТАН: Куб знову "на старті" (чекає пострілу)
            IsLaunched = false;

            if (rb != null)
            {
                rb.isKinematic = false;
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
            // Якщо куб народився від злиття, він автоматично вважається "у грі"
            IsLaunched = true;

            if (rb != null && settings != null)
            {
                Vector3 randomDir = Random.insideUnitSphere * 0.5f;
                randomDir.y = 0;
                rb.AddForce((Vector3.up + randomDir) * settings.MergePushForce, ForceMode.Impulse);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasMerged) return;
            if (collision.gameObject == null) return;

            if (collision.gameObject.TryGetComponent<Cube>(out Cube otherCube))
            {
                if (otherCube.hasMerged) return;

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
                scoreService.AddScore(newValue);
            }
        }
    }
}