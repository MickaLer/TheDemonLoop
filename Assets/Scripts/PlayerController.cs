using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float doubleJumpPower;
    [SerializeField] private float dashPower;
    [SerializeField] private GameObject attackHitBox;
    private bool _canJump;
    private bool _doubleJump;

    private Rigidbody2D _rigidbody;
    private float _directionLooking = 1;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private IEnumerator Start()
    {
        while (true)
        {
            Vector2 direction = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
            _rigidbody.linearVelocity = new Vector2(direction.x * moveSpeed, _rigidbody.linearVelocity.y);
            if (direction.x != 0)
            {
                _directionLooking = direction.x;
                transform.rotation = Quaternion.Euler(0, direction.x > 0 ? 0 : 180, 0);
            }
            
            //ACTIONS
            if (InputSystem.actions.FindAction("Jump").triggered && (_canJump ||_doubleJump)) yield return StartCoroutine(Jump());
            if (InputSystem.actions.FindAction("Dash").triggered) yield return StartCoroutine(Dash());
            if (InputSystem.actions.FindAction("Attack").triggered) yield return StartCoroutine(Attack());
            
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Jump()
    {
        _rigidbody.linearVelocity = Vector2.zero;
        _doubleJump = _canJump;
        _canJump = false;
        _rigidbody.AddForce(Vector2.up * (_doubleJump ? jumpPower : doubleJumpPower), ForceMode2D.Impulse);
        yield return null;
    }

    private IEnumerator Dash()
    {
        _rigidbody.AddForce(Vector2.right * (_directionLooking * dashPower), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        _rigidbody.linearVelocity = Vector2.zero;
        yield return null;
    }
    
    private IEnumerator Attack()
    {
        attackHitBox.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackHitBox.SetActive(false);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _canJump = true;
        _doubleJump = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        _canJump = false;
    }
}
