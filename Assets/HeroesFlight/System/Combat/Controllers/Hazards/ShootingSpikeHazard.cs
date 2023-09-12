using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootingSpikeHazard : EnironmentHazard
{
    [Header("Shooting Spike Settings")]
    [SerializeField] private float warningLineDuration;
    [SerializeField] private float warningLineWidth;
    [SerializeField] private float healthPercentage = 15;
    [SerializeField] private WarningLine warningLine;
    [SerializeField] private HazardArrow hazardArrow;
    private bool isTriggered;

    private void Start()
    {
        StartCoroutine(Runtime());
    }

    private void Trigger()
    {
        isTriggered = true;
        warningLine.Trigger(() =>
        {
            HazardArrow arrow = ObjectPoolManager.SpawnObject(hazardArrow, warningLine.transform.position, Quaternion.identity);
            arrow.SetupArrow(healthPercentage, warningLine.GetFowardDirection);
            isTriggered = false;
        }, warningLineDuration,warningLineWidth);
    }


    public IEnumerator Runtime()
    {
        while (true)
        {
            yield return ActivateCooldown();
            Trigger();
            yield return new WaitUntil(() => !isTriggered);
            yield return null;
        }
    }
}
