using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jump;
    [SerializeField] private float fall;
    private bool _canJump;

    private void Update()
    {
        Vector2 direction = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        if (direction != Vector2.zero)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
