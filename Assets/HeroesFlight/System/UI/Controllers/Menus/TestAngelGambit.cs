using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class TestAngelGambit : MonoBehaviour
{
    [SerializeField] private AngelEffectManager angelEffectManager;
    [SerializeField] private AngelPermanetCardMenu angelPermanetCardMenu;
    [SerializeField] private AngelGambitMenu angelGambitMenu;

    private void Start()
    {

        angelEffectManager.OnPermanetCard = angelPermanetCardMenu.AcivateCardPermanetEffect;

        angelGambitMenu.CardExit = angelEffectManager.Exists;

        angelGambitMenu.OnCardSelected = angelEffectManager.AddAngelCardSO;


        angelGambitMenu.OnCreated();
        angelPermanetCardMenu.OnCreated();
        angelGambitMenu.Close();
        angelPermanetCardMenu.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!angelEffectManager.EffectActive)
            {
                angelGambitMenu.Open();
            }
            else
            {
                angelEffectManager.ComplectedLevel();
            }
        }
    }
}
