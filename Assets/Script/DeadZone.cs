using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private float timer = 0f;
    private bool isCubeInside = false;

    private void OnTriggerStay(Collider other)
    {
        // ѕерев≥р€Їмо, чи це кубик
        if (other.GetComponent<Cube>() != null)
        {
            // якщо кубик прол≥таЇ повз не рахуЇмо
            // –ахуЇмо, т≥льки €кщо в≥н майже зупинивс€
            if (other.attachedRigidbody.linearVelocity.magnitude < 0.1f) 
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;
            }

            // якщо кубик лежить тут вже 2 секунди
            if (timer > 1f)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // якщо кубик вилет≥в ≥з зони - скидаЇмо таймер
        if (other.GetComponent<Cube>() != null)
        {
            timer = 0f;
        }
    }
}