using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pelumi.Juicer;
using HeroesFlight.System.UI.Reward;

public class DailyRewardUI : MonoBehaviour
{
    public enum State
    {
        NotReady,
        Ready,
        Claimed    
    }

    public Action<int> OnRewardButtonClicked;

    [SerializeField] private State state;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Transform container;
    [SerializeField] private RewardView[] singleRewardViews;

    [Header ("NotReady")]
    [SerializeField] private GameObject notReadyContent;

    [Header("Ready")]
    [SerializeField] private GameObject readyContent;

    [Header("Claimed")]
    [SerializeField] private GameObject claimedContent;

    private int rewardIndex;
    private AdvanceButton advanceButton;

    JuicerRuntime readyEffect;


    private void Awake()
    {
        advanceButton = GetComponent<AdvanceButton>();  
        advanceButton.onClick.AddListener(() => OnRewardButtonClicked?.Invoke(rewardIndex));

        readyEffect = container.transform.JuicyScale(1.2f, 1.0f);
        readyEffect.SetLoop(-1);
    }

    public void SetVisual(List<RewardVisualEntry> rewardVisual)
    {
        for (int i = 0; i < rewardVisual.Count; i++)
        {
            singleRewardViews[i].SetVisual(rewardVisual[i]);
        }
    }

    public void SetRewardIndex (int rewardIndex)
    {
        this.rewardIndex = rewardIndex;
        dayText.text =  "Day " + (rewardIndex + 1).ToString();
    }

    public void SetState(State state)
    {
        this.state = state;

        if(state == State.Ready)
        {
            readyEffect.Start();
        }
        else
        {
            readyEffect.Stop();
        }

        advanceButton.interactable = state == State.Ready;
        notReadyContent .SetActive(state == State.NotReady);
        readyContent.SetActive(state == State.Ready);
        claimedContent.SetActive(state == State.Claimed);
    }
}
