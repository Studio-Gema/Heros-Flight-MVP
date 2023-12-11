using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Complection Quest", menuName = "Quest/Level Complection Quest")]
public class LevelComplectionQuestSO : QuestSO
{
    public WorldType worldType;

    public override void Awake()
    {
        questType = QuestType.LevelComplection;
    }
}