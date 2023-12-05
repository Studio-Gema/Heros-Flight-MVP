using System;
using HeroesFlight.System.Stats.Handlers;
using StansAssets.Foundation.Extensions;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Stats
{
    public class ProgressionSystem : ProgressionSystemInterface
    {
        public ProgressionSystem(DataSystemInterface dataSystem)
        {
            currencyHandler = new InRunCurrencyHandler();
          //  traitHandler = new TraitHandler();
            this.dataSystem = dataSystem;
        }

        DataSystemInterface dataSystem;
        InRunCurrencyHandler currencyHandler;
       // private TraitHandler traitHandler;
        public BoosterManager BoosterManager { get; private set; }
 
        public void Init(Scene scene = default, Action onComplete = null)
        {
            BoosterManager = scene.GetComponentInChildren<BoosterManager>();
        }

        public void Reset()
        {
            currencyHandler.ResetValues();
        }

        public void AddCurrency(string key, int amount, Action OnComplete = null)
        {
            currencyHandler.AddCurrency(key, amount, OnComplete);
        }

        public int GetCurrency(string key)
        {
          return  currencyHandler.GetCurrencyAmount(key);
        }

        public void SaveRunResults()
        {
            dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gold, currencyHandler.GetCurrencyAmount(CurrencyKeys.Gold));
            dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Experience, currencyHandler.GetCurrencyAmount(CurrencyKeys.Experience));
            currencyHandler.ResetValues();
        }

        public void ResetCurrency(string experience)
        {
            currencyHandler.ResetValue(experience);
        }

        public void CollectRunCurrency()
        {
            //HeroProgression.AddExp(GetCurrency(CurrencyKeys.RunExperience));
            ResetCurrency(CurrencyKeys.RunExperience);
        }
    }
}