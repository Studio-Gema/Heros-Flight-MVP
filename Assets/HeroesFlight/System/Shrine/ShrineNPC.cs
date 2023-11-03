using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineNPC : InteractiveNPC
{
    [SerializeField] private ShrineNPCType shrineNPCType;

    private ShrineNPCFee shrineNPCFee;
    public ShrineNPCType GetShrineNPCType ()=> shrineNPCType;

    public void Initialize(ShrineNPCFee shrineNPCFee, Action OnInteract)
    {
        this.shrineNPCFee = shrineNPCFee;
        this.OnInteract += OnInteract;
        shrineNPCFee.OnInteracted = InteractionComplected;
    }

    protected override void Interact()
    {
        if (!shrineNPCFee.Unlocked) return;
        base.Interact();
    }
}
