using UnityEngine;
using TMPro;

public class Cube : MonoBehaviour
{
    [Header("Game Settings")]
    public int Value = 2;
    private int ID;

    [Header("Visual References")]
    [SerializeField] private TMP_Text[] valueTexts;
    [SerializeField] private Renderer cubeRenderer;

    private Rigidbody rb;
    private bool hasMerged = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ID = GetInstanceID();

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        // Оновлюємо текст 
        if (valueTexts != null)
        {
            foreach (var textItem in valueTexts)
            {
                if (textItem != null)
                    textItem.text = Value.ToString();
            }
        }

        // Оновлюємо колір
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = GetColorForValue(Value);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasMerged) return;

        Cube otherCube = collision.gameObject.GetComponent<Cube>();

        if (otherCube != null)
        {
            if (Value == otherCube.Value && this.ID < otherCube.GetID())
            {
                hasMerged = true;
                otherCube.hasMerged = true;
                Merge(this, otherCube);
            }
        }
    }

    // Допоміжний метод щоб отримати ID іншого кубика 
    public int GetID()
    {
        return ID;
    }

    private void Merge(Cube cube1, Cube cube2)
    {
        Vector3 pos = Vector3.Lerp(cube1.transform.position, cube2.transform.position, 0.5f);

        Cube newCube = Instantiate(this, pos, Quaternion.identity);
        newCube.GetComponent<Collider>().enabled = true;

        int points = cube1.Value / 2;
        ScoreManager.Instance.AddScore(points);

        newCube.Value = cube1.Value * 2;
        newCube.hasMerged = false;
        newCube.UpdateVisuals();

        Rigidbody newRb = newCube.GetComponent<Rigidbody>();
        if (newRb != null)
        {
            // Для Unity 6
            newRb.linearVelocity = Vector3.zero;
            newRb.angularVelocity = Vector3.zero;
            newRb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }

        Destroy(cube1.gameObject);
        Destroy(cube2.gameObject);
    }

    private Color GetColorForValue(int value)
    {
        switch (value)
        {
            case 2: return new Color(0.9f, 0.9f, 0.9f);
            case 4: return new Color(1f, 0.9f, 0.6f);
            case 8: return new Color(1f, 0.7f, 0.4f);
            case 16: return new Color(1f, 0.5f, 0.3f);
            case 32: return new Color(1f, 0.3f, 0.3f);
            case 64: return new Color(1f, 0f, 0f);
            default: return Color.magenta;
        }
    }
}