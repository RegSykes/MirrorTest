using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Impulse : Ability
{
    [SerializeField] private float Distance = 15f;

    [SyncVar]
    private float SyncDistance = 15f;

    private float Timeout = 3f;
    private float SmoothFactor = 0.1f;
    private float AccuracyRadius = 1f;
    private Collider OwnerCollider;

    private Character OwnerCharacter;
    private Rigidbody OwnerRigidbody;
    private Transform OwnerTransform;
    private Vector3 TargetPosition;

    private ParticleSystem OwnerParticleSystemTrail;

    private Coroutine ActivateImpulseCoroutine;

    private void Awake()
    {
        if(OwnerCollider == null) OwnerCollider = GetComponent<SphereCollider>();
        OwnerRigidbody = GetComponent<Rigidbody>();
        OwnerTransform = GetComponent<Transform>();
        OwnerCharacter = GetComponent<Character>();
        OwnerParticleSystemTrail = GetComponentInChildren<ParticleSystem>();
        OwnerParticleSystemTrail.gameObject.SetActive(false);
    }

    [Command]
    public override void Use() => RpcOnUse();

    [ClientRpc]
    private void RpcOnUse() => ActivateImpulseCoroutine = StartCoroutine(ActivateImpulseIE(SyncDistance, Timeout));

    private void SetInProcess(bool inProcess) 
    { 
        InProcess = inProcess;
        OwnerParticleSystemTrail.gameObject.SetActive(inProcess);
    }

    private IEnumerator ActivateImpulseIE(float distance, float timeout)
    {
        TargetPosition = OwnerTransform.position + OwnerCharacter.characterController.velocity.normalized * distance;
        //TargetPosition = OwnerTransform.position + OwnerTransform.forward * distance;
        SetInProcess(true);
        yield return new WaitForSeconds(timeout);
        SetInProcess(false);
    }

    private void FixedUpdate()
    {
        if (InProcess && isOwned) 
        { 
            OwnerTransform.position = Vector3.Lerp(OwnerTransform.position, TargetPosition, SmoothFactor); 
            if(Vector3.Distance(OwnerTransform.position, TargetPosition) < AccuracyRadius)
            {
                StopCoroutine(ActivateImpulseCoroutine);
                SetInProcess(false);
            }
        }
        OnServerSyncDistance();
    }

    [ServerCallback]
    private void OnServerSyncDistance()
    {
        if(Distance != SyncDistance)
        {
            SyncDistance = Distance;
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!InProcess) return;

        if (other is not SphereCollider) return;

        ICanBeHitByImpulse canYou = other.GetComponent<ICanBeHitByImpulse>();
        if(canYou != null && other != OwnerCollider && !canYou.UnderEffect)
        {
            canYou.GotHitByImpulse();
            OwnerCharacter.IncrementScore();
        }
    }
}
