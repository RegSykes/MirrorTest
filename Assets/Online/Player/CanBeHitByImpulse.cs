using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanBeHitByImpulse
{
    void GotHitByImpulse();
    bool UnderEffect { get; }
}

public class CanBeHitByImpulse : NetworkBehaviour, ICanBeHitByImpulse
{
    public bool UnderEffect { private set; get; }

    [SerializeField] private float EffectTime = 3f;

    [SyncVar]
    private float SyncEffectTime = 3f;

    [SerializeField] private Color HitColor = Color.red;

    private Color backupColor;
    private Renderer myRenderer;
    private Coroutine effectCoroutine;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        backupColor = myRenderer.material.color;
    }

    [ClientRpc]
    public void GotHitByImpulseRpc() => effectCoroutine = StartCoroutine(StartHitEffectIE(SyncEffectTime, HitColor, backupColor));

    private IEnumerator StartHitEffectIE(float effectTime, Color hitColor, Color _backupColor)
    {
        UnderEffect = true;
        myRenderer.material.color = hitColor;
        yield return new WaitForSeconds(effectTime);
        myRenderer.material.color = _backupColor;
        UnderEffect = false;
    }

    public void GotHitByImpulse()
    {
        UnderEffect = true;
        GotHitByImpulseRpc();
    }

    private void FixedUpdate() => OnServerSyncEffectTime();

    [ServerCallback]
    private void OnServerSyncEffectTime()
    {
        if(EffectTime != SyncEffectTime)
        {
            SyncEffectTime = EffectTime;
        }
    }
}
