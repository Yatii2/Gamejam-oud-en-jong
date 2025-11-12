using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class LocalPlayerController : MonoBehaviour
{
    private enum ControlScheme
    {
        WASD,
        Arrows,
        Auto
    }

    [Header("Movement")] [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;

    [Header("Input")]
    [Tooltip("Which scheme this player should use. Configure per-player: one object uses WASD, the other Arrows.")]
    [SerializeField]
    private ControlScheme controlScheme = ControlScheme.Auto;

    // Runtime objects
    private InputAction moveAction;
    private Vector2 moveValue;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        CreateMoveAction();
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
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
    }

    private void CreateMoveAction()
    {
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");

        if (controlScheme == ControlScheme.WASD || controlScheme == ControlScheme.Auto)
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

        moveAction.AddBinding("<Gamepad>/leftStick");

        moveAction.performed += ctx => moveValue = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveValue = Vector2.zero;
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