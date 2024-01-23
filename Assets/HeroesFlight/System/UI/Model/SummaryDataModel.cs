﻿using System;
using System.Collections.Generic;
using HeroesFlight.System.UI.Reward;

namespace HeroesFlight.System.UI.Model
{
    public class SummaryDataModel
    {
        public SummaryDataModel(Tuple<int, float> numberOfLevelsGained, int goldGained, TimeSpan timeSpent, int currentLvl,List<RewardVisualEntry> rewards,CharacterSO characterSO)
        {
            NumberOfLevelsGained = numberOfLevelsGained;
            GoldGained = goldGained;
            TimeSpent = timeSpent;
            CurrentLvl = currentLvl;
            rewardVisualEntries = rewards;
            CurrentCharacterSO = characterSO;
        }
        public Tuple<int, float> NumberOfLevelsGained;
        public int GoldGained;
        public int CurrentLvl;
        public TimeSpan TimeSpent;
        public List<RewardVisualEntry> rewardVisualEntries = new();
        public CharacterSO CurrentCharacterSO;
    }
}