using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

[RequireComponent(typeof(Collider2D))]
public class LocalPlayerController : MonoBehaviour
{
    private enum ControlScheme
    {
        Wasd,
        Arrows,
        JoyStick,
        Auto
    }

    [Header("Movement")] [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Input")]
    [Tooltip("Which scheme this player should use. Configure per-player: one object uses WASD, the other Arrows.")]
    [SerializeField]
    private ControlScheme controlScheme = ControlScheme.Auto;

    // Runtime objects
    private InputAction moveAction;
    private InputAction dashAction;
    private InputAction wallhacksAction;
    private Vector2 moveValue;
    private PlayerDash playerDash;
    private Wallhacks wa;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        playerDash = GetComponent<PlayerDash>();
        wa = GetComponent<Wallhacks>();

        CreateMoveAction();
        CreateDashAction();
        CreateWallhacksAction();
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
        }

        if (dashAction != null)
        {
            dashAction.Enable();
        }

        if (wallhacksAction != null)
        {
            wallhacksAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
            moveAction.Dispose();
            moveAction = null;
        }

        if (dashAction != null)
        {
            dashAction.Disable();
            dashAction.Dispose();
            dashAction = null;
        }

        if (wallhacksAction != null)
        {
            wallhacksAction.Disable();
            wallhacksAction.Dispose();
            wallhacksAction = null;
        }
    }

    private void CreateMoveAction()
    {
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        if (controlScheme == ControlScheme.Wasd || controlScheme == ControlScheme.Auto)
        {
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
        }

        if (controlScheme == ControlScheme.Arrows || controlScheme == ControlScheme.Auto)
        {
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
        }

        if (controlScheme == ControlScheme.JoyStick || controlScheme == ControlScheme.Auto)
        {
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Gamepad>/leftStick/up")
                .With("Down", "<Gamepad>/leftStick/down")
                .With("Left", "<Gamepad>/leftStick/left")
                .With("Right", "<Gamepad>/leftStick/right");
        }

        moveAction.performed += ctx => moveValue = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveValue = Vector2.zero;
    }

    private void CreateDashAction()
    {
        dashAction = new InputAction("Dash", InputActionType.Button);
        if (controlScheme == ControlScheme.Wasd || controlScheme == ControlScheme.Auto)
        {
            dashAction.AddBinding("<Keyboard>/space");
        }

        if (controlScheme == ControlScheme.Arrows || controlScheme == ControlScheme.Auto)
        {
            dashAction.AddBinding("<Keyboard>/rightShift");
        }

        if (controlScheme == ControlScheme.JoyStick || controlScheme == ControlScheme.Auto)
        {
            dashAction.AddBinding("<Gamepad>/rightTrigger");
        }

        dashAction.performed += ctx =>
        {
            if (playerDash != null) playerDash.OnDashButtonPressed();
        };
    }

    private void CreateWallhacksAction()
    {
        wallhacksAction = new InputAction("Wallhacks", InputActionType.Button);
        if (controlScheme == ControlScheme.Wasd || controlScheme == ControlScheme.Auto)
        {
            wallhacksAction.AddBinding("<Keyboard>/leftShift");
        }

        if (controlScheme == ControlScheme.Arrows || controlScheme == ControlScheme.Auto)
        {
            wallhacksAction.AddBinding("<Keyboard>/rightControl");
        }

        if (controlScheme == ControlScheme.JoyStick || controlScheme == ControlScheme.Auto)
        {
            wallhacksAction.AddBinding("<Gamepad>/rightShoulder");
        }

        wallhacksAction.performed += ctx =>
        {
            if (wa != null) wa.OnPhaseButtonPressed();
        };
    }

    private void FixedUpdate()
    {
        if (moveAction == null)
        {
            return;
        }

        var read = moveAction.ReadValue<Vector2>();
        if (read != Vector2.zero)
        {
            moveValue = read;
            if (read == Vector2.up)
            {
                animator.SetBool("IsDown", false);
                animator.SetBool("IsLeft", false);
                animator.SetBool("IsUp", true);
                GetComponent<SpriteRenderer>().flipX = false;
            } else if  (read == Vector2.down)
            {
                animator.SetBool("IsDown", true);
                animator.SetBool("IsLeft", false);
                animator.SetBool("IsUp", false);
                GetComponent<SpriteRenderer>().flipX = false;
            } else if (read == Vector2.left)
            {
                animator.SetBool("IsDown", false);
                animator.SetBool("IsLeft", true);
                animator.SetBool("IsUp", false);
                GetComponent<SpriteRenderer>().flipX = false;
            } else if (read == Vector2.right)
            {
                animator.SetBool("IsDown", false);
                animator.SetBool("IsLeft", true);
                animator.SetBool("IsUp", false);
                GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        if (moveValue.sqrMagnitude > 1f)
        {
            moveValue = moveValue.normalized;
        }

        if (rb != null)
        {
            rb.linearVelocity = moveValue * speed;
        }
        else
        {
            transform.Translate((Vector3)moveValue * (speed * Time.fixedDeltaTime));
        }
    }

    public IEnumerator StartRebind(Action onComplete = null, string excludeControls = null)
    {
        if (moveAction == null)
        {
            Debug.LogWarning("Move action is null - cannot rebind");
            yield break;
        }

        moveAction.Disable();
        var rebind = moveAction.PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/leftButton")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                operation.Dispose();
                moveAction.Enable();
                onComplete?.Invoke();
            });
        rebind.Start();

        while (!rebind.completed)
        {
            yield return null;
        }
    }

    public void LogBindings()
    {
        if (moveAction == null)
        {
            Debug.Log("Move action is null");
            return;
        }

        Debug.Log($"Bindings for {name} ({controlScheme}):");
        for (int i = 0; i < moveAction.bindings.Count; i++)
        {
            var b = moveAction.bindings[i];
            Debug.Log($"[{i}] path={b.path} groups={b.groups} name={b.name}");
        }
    }
}