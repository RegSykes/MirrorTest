using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
public class Character : NetworkBehaviour
{
    [SerializeField] public CharacterController characterController;
    [SerializeField] public Ability PlayerAbility;
    [SerializeField] public ScoreSystem CharacterScoreSystem;
    [SerializeField] public PlayerIdentity PlayerIdentityInGame;
    private void Awake()
    {
        if(!CharacterScoreSystem) CharacterScoreSystem = GetComponent<ScoreSystem>();
        if (!PlayerIdentityInGame) PlayerIdentityInGame = GetComponent<PlayerIdentity>();
    }

    [ServerCallback]
    public void IncrementScore() => CharacterScoreSystem.IncrementScoreRpc();

    void OnValidate()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        characterController.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<NetworkTransform>().syncDirection = SyncDirection.ClientToServer;
    }

    public override void OnStartLocalPlayer()
    {
        characterController.enabled = true;
    }

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float turnSensitivity = 5f;
    public float maxTurnSpeed = 100f;

    [Header("Diagnostics")]
    public float horizontal;
    public float vertical;
    public float turn;
    public float jumpSpeed;
    public bool isGrounded = true;
    public bool isFalling;
    public Vector3 velocity;

    void Update()
    {
        if (!isLocalPlayer || characterController == null || !characterController.enabled)
            return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Q and E cancel each other out, reducing the turn to zero
        if (Input.GetKey(KeyCode.Q))
            turn = Mathf.MoveTowards(turn, -maxTurnSpeed, turnSensitivity);
        if (Input.GetKey(KeyCode.E))
            turn = Mathf.MoveTowards(turn, maxTurnSpeed, turnSensitivity);
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
            turn = Mathf.MoveTowards(turn, 0, turnSensitivity);
        if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
            turn = Mathf.MoveTowards(turn, 0, turnSensitivity);

        if (isGrounded)
            isFalling = false;

        if ((isGrounded || !isFalling) && jumpSpeed < 1f && Input.GetKey(KeyCode.Space))
        {
            jumpSpeed = Mathf.Lerp(jumpSpeed, 1f, 0.5f);
        }
        else if (!isGrounded)
        {
            isFalling = true;
            jumpSpeed = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlayerAbility.Use();
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer || characterController == null || !characterController.enabled)
            return;

        transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);

        Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);
        direction = transform.TransformDirection(direction);
        direction *= moveSpeed;

        if (jumpSpeed > 0)
            characterController.Move(direction * Time.fixedDeltaTime);
        else
            characterController.SimpleMove(direction);

        isGrounded = characterController.isGrounded;
        velocity = characterController.velocity;
    }
}
