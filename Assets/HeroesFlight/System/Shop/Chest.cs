﻿using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : MonoBehaviour
{
    public enum ChestType
    {
        Regular,
        Rare,
        Epic
    }

    [SerializeField] private ChestType chestType;
    [SerializeField] private RewardPackSO rewards;
    [SerializeField] float gemchestPrice;
    [SerializeField] float goldchestPrice;

    [Header("Timed Reward")]
    [SerializeField] TimeType timeType;
    [SerializeField] float nextRewardTimeAdded = 20f;
    [SerializeField] float checkingInterval = 2f;

    private TimedReward timedReward;
    public ChestType GetChestType => chestType;
    public RewardPackSO GetRewards => rewards;

    public float GetGemChestPrice => gemchestPrice;
    public float GetGoldChestPrice => goldchestPrice;

    void Start()
    {
        SetUpNormalChest();
    }

    void SetUpNormalChest()
    {
        if (chestType == ChestType.Regular)
        {
            timedReward = new TimedReward();
            timedReward.OnInternetConnected = () =>
            {
                Debug.Log("Internet Connected");
            };

            timedReward.OnInternetDisconnected = () =>
            {
                Debug.Log("Internet Disconnected");
            };

            timedReward.OnRewardReadyToBeCollected = () =>
            {
                Debug.Log("Reward Ready To Be Collected");
            };

            timedReward.RewardPlayer = (LastRewardClaimDate) =>
            {
                // enable button
                Debug.Log("Reward Player");
            };

            timedReward.OnTimerUpdateed = (time) =>
            {
               // Debug.Log("Timer Updated" + time);
            };

            string todaysDate = DateTime.Now.ToString();

            timedReward.Init(this, todaysDate, TimeType.Seconds, nextRewardTimeAdded, checkingInterval);
        }
    }

    public List<Reward> OpenChest()
    {
       return rewards.GetReward();
    }

    public void OpenNormalChestWithAds()
    {
        if (chestType == ChestType.Regular && timedReward.IsRewardReady())
        {
            timedReward.ClaimTimedReward();
        }
    }
}
