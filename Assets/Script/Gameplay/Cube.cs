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
        public bool IsLaunched { get; private set; } = false;

        private ICubeSpawner spawner;
        private IScoreService scoreService;
        private Rigidbody rb;
        private bool hasMerged = false;

     
        private float minX;
        private float maxX;

        private float targetX;
        private float currentX;
        private float currentVelocity;

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
            targetX = transform.position.x;
            currentX = transform.position.x;
        }

        public void Init(float leftLimit, float rightLimit)
        {
            minX = leftLimit;
            maxX = rightLimit;
            targetX = transform.position.x;
            currentX = transform.position.x;
            currentVelocity = 0f;
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
        private void Update()
        {
            if (!IsLaunched && settings != null)
            {
             
                float smoothTimeSeconds = 0.1f;

                currentX = Mathf.SmoothDamp(currentX, targetX, ref currentVelocity, smoothTimeSeconds);

                Vector3 pos = transform.position;
                pos.x = currentX;
                transform.position = pos;
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
            if (IsLaunched || settings == null) return;

            targetX += deltaX * settings.MoveSpeed * Time.deltaTime;
            targetX = Mathf.Clamp(targetX, minX, maxX);
        }

        public void Shoot()
        {
            IsLaunched = true;
            targetX = transform.position.x;
            currentX = transform.position.x;

            if (rb != null && settings != null)
            {
                rb.AddForce(Vector3.forward * settings.PushForce, ForceMode.Impulse);
            }
        }

        public void ResetCube()
        {
            hasMerged = false;
            IsLaunched = false;

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(true);

            targetX = transform.position.x;
            currentX = transform.position.x;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Bounce()
        {
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