using System;
using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllIn1EffectTrigger : MonoBehaviour
{
    [Header("Settings")] [SerializeField] string propertyName;
    [SerializeField] bool startOnEnable;

    [SerializeField] float duration;
    [SerializeField] float from;
    [SerializeField] float to;

    [SerializeField] bool loop;
    [SerializeField] float stepDelay;
    [SerializeField] private int loopsCount;
    private JuicerRuntime effect;
    private Material material;
   

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;

        effect = material.JuicyFloatProperty(propertyName, to, duration);

        if (loop)
        {
            effect.SetLoop(-1);
            effect.SetStepDelay(1f);
        }
    }

    private void OnEnable()
    {
        material.SetFloat(propertyName, from);

        if (startOnEnable)
        {
            effect.Start();
        }
    }

    public void StartEffect()
    {
        if (loop)
        {
            if (loopsCount > 0)
            {
                effect.SetLoop(loopsCount);
            }
            else
            {
                effect.SetLoop(-1);
            }

            effect.SetStepDelay(stepDelay);
        }

        effect.Start();
    }
}