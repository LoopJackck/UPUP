using Gamekit2D;
using UnityEngine;

public class PlayerJumpCounter : MonoBehaviour
{
    public static int jumpCount = 0;

    private CharacterController2D controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpCount++;
        }
    }
}