﻿using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New RewardPack", menuName = "Shop/RewardPack")]
public class RewardPack : ScriptableObject
{
    [SerializeField] private Reward[] fixedRewards;
    [SerializeField] private Reward[] chancedRewards;

    private List<Reward> rewardToGive = new List<Reward>();

    public Reward[] GetFixedRewards() => fixedRewards;
    public List<Reward> GetRewardsToGive() => rewardToGive;

    public void GiveReward()
    {
        rewardToGive.Clear();
        GiveAllFixedRewards();
        GenerateRewardByChance();
        RewardPlayer();
    }

    private void GiveAllFixedRewards()
    {
        foreach (Reward reward in chancedRewards)
        {
            rewardToGive.Add(reward);
        }
    }

    private void GenerateRewardByChance()
    {
        foreach (Reward reward in chancedRewards)
        {
            float randomRoll = Random.Range(1, 101);
            if (randomRoll <= reward.GetChance())
            {
                rewardToGive.Add(reward);
            }
        }
    }

    public void RewardPlayer()
    {
        foreach (Reward reward in rewardToGive)
        {
            reward.GiveReward();
        }
    }

    public void GenerateRandomRewardByChance(Reward[] rewardArray, int currrentNumberOfReward)
    {
        float totalChance = 0;
        foreach (Reward reward in rewardArray) totalChance += reward.GetChance();

        for (int i = 0; i < currrentNumberOfReward; i++)
        {
            float randomRoll = Random.Range(1, totalChance + 1);
            float pos = 0;

            foreach (Reward reward in rewardArray)
            {
                if (randomRoll <= reward.GetChance() + pos)
                {
                    rewardToGive.Add(reward);
                    break;
                }
                pos += reward.GetChance();
            }
        }
    }
}