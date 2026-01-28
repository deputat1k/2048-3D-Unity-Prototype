using UnityEngine;
using TMPro;

public class Cube : MonoBehaviour
{
    [Header("Settings")]
    public int Value = 2;
    [SerializeField] private GameSettings settings;

    [Header("Visuals")]
    [SerializeField] private TMP_Text[] valueTexts;
    [SerializeField] private Renderer cubeRenderer;

    private Rigidbody rb;
    private bool hasMerged = false;

    private float minX;
    private float maxX;

    private CubeSpawner spawner;
    private ScoreBank scoreBank;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        UpdateVisuals();
    }

    public void Construct(CubeSpawner spawner, ScoreBank scoreBank)
    {
        this.spawner = spawner;
        this.scoreBank = scoreBank;
    }

    public void Init(float leftLimit, float rightLimit)
    {
        minX = leftLimit;
        maxX = rightLimit;
    }

    public void UpdateVisuals()
    {
        if (valueTexts != null)
        {
            foreach (var text in valueTexts) text.text = Value.ToString();
        }
        if (cubeRenderer != null && settings != null)
        {
            cubeRenderer.material.color = GetColorForValue(Value);
        }
    }

    private Color GetColorForValue(int value)
    {
        if (settings == null || settings.CubeColors == null) return Color.white;
        int index = (int)Mathf.Log(value, 2) - 1;
        if (index >= 0 && index < settings.CubeColors.Length) return settings.CubeColors[index];
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
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
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
            rb.AddForce(Vector3.up * settings.MergePushForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Cube otherCube = collision.gameObject.GetComponent<Cube>();

        if (hasMerged) return;

        if (otherCube != null && otherCube.Value == Value &&
            this.GetInstanceID() < otherCube.GetInstanceID())
        {
            Merge(otherCube);
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

        if (scoreBank != null)
        {
            scoreBank.AddScore(1);
        }
    }
}