using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "Game Settings", order = 51)]
public class GameSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float MoveSpeed = 0.2f;
    public float XLimit = 1.5f;

    [Header("Mechanics")]
    public float PushForce = 20f;
    public float MergePushForce = 3f; 

    [Header("Colors")]
    public Color[] CubeColors;
}