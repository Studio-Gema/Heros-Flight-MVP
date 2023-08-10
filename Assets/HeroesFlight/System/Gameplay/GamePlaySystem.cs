﻿using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Container;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace HeroesFlight.System.Gameplay
{
    public class GamePlaySystem : GamePlaySystemInterface
    {
        public GamePlaySystem(IDataSystemInterface dataSystemInterface, CharacterSystemInterface characterSystem, NpcSystemInterface npcSystem)
        {
            this.dataSystemInterface = dataSystemInterface;
            this.npcSystem = npcSystem;
            this.characterSystem = characterSystem;
            npcSystem.OnEnemySpawned += HandleEnemySpawned;
            GameTimer = new CountDownTimer();
        }

        public event Action OnNextLvlLoadRequest;
        public CountDownTimer GameTimer { get; private set; }
        public AngelEffectManager EffectManager { get; private set; }

        public BoosterManager BoosterManager { get; private set; }

        public BoosterSpawner BoosterSpawner { get; private set; }

        public CurrencySpawner CurrencySpawner { get; private set; }

        public int CurrentLvlIndex => container.CurrentLvlIndex;

        public event Action<float> OnUltimateChargesChange;
        public event Action<bool> OnMinibossSpawned;
        public event Action<float> OnMinibossHealthChange;

        public event Action<int> OnRemainingEnemiesLeft;
        public event Action<DamageModel> OnCharacterDamaged;
        public event Action<float, Transform> OnCharacterHeal;
        public event Action<DamageModel> OnEnemyDamaged;
        public event Action<int> OnCharacterHealthChanged;
        public event Action<int> OnCharacterComboChanged;
        public event Action<GameState> OnGameStateChange;
        public event Action<BoosterSO, float, Transform> OnBoosterActivated;
        public event Action<int> OnCoinsCollected;
        public event Action<BoosterContainer> OnBoosterContainerCreated;

        IDataSystemInterface dataSystemInterface;
        List<IHealthController> GetExistingEnemies() => activeEnemyHealthControllers;
        List<IHealthController> activeEnemyHealthControllers = new();
        IHealthController miniBoss;
        IHealthController characterHealthController;
        CharacterAttackController characterAttackController;
        CharacterAbilityInterface characterAbility;
        CharacterSystemInterface characterSystem;
        CameraControllerInterface cameraController;
        NpcSystemInterface npcSystem;
        GameplayContainer container;
        GameState currentState;
        float timeSinceLastStrike;
        float timeToResetCombo = 3f;
        int characterComboNumber;
        int enemiesToKill;
        int wavesAmount;
        Coroutine combotTimerRoutine;
        int collectedGold;
        float collectedXp;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            cameraController = scene.GetComponentInChildren<CameraControllerInterface>();
            EffectManager = scene.GetComponentInChildren<AngelEffectManager>();
            container = scene.GetComponentInChildren<GameplayContainer>();
            BoosterManager = scene.GetComponentInChildren<BoosterManager>();
            BoosterSpawner = scene.GetComponentInChildren<BoosterSpawner>();
            BoosterManager.OnBoosterActivated += HandleBoosterActivated;
            BoosterManager.OnBoosterContainerCreated += HandleBoosterWithDurationActivated;

            CurrencySpawner = scene.GetComponentInChildren<CurrencySpawner>();
            CurrencySpawner.Initialize(this);

            container.Init();
            container.OnPlayerEnteredPortal += HandlePlayerTriggerPortal;
            container.SetStartingIndex(0);
            OnUltimateChargesChange?.Invoke(0);
            OnComplete?.Invoke();
        }

        public void Reset()
        {
            ResetLogic();
            ResetPlayerSubscriptions();
            container.SetStartingIndex(0);
        }


        void ResetPlayerSubscriptions()
        {
            characterAttackController.SetCallback(null);
            characterHealthController.OnDeath -= HandleCharacterDeath;
            characterHealthController.OnBeingDamaged -= HandleCharacterDamaged;
            characterHealthController.OnHeal -= HandleCharacterHeal;
            characterAttackController = null;
            characterHealthController = null;
            characterAbility = null;
        }

        public void ResetLogic()
        {
            activeEnemyHealthControllers.Clear();

            // EffectManager.ResetAngelEffects();
            enemiesToKill = 0;
            GameTimer.Stop();
            ChangeState(GameState.Ended);
            OnMinibossSpawned?.Invoke(false);
            if (combotTimerRoutine != null)
                CoroutineUtility.Stop(combotTimerRoutine);
        }

        public void EnablePortal()
        {
            container.EnablePortal();
        }

        public void UseCharacterSpecial()
        {
            cameraController.SetCameraState(GameCameraType.Skill);
            characterHealthController.SetInvulnerableState(true);
            characterSystem.SetCharacterControllerState(false);
            characterAttackController.ToggleControllerState(false);
            characterAbility.UseAbility(null, () =>
            {
                cameraController.SetCameraState(GameCameraType.Character);
                characterSystem.SetCharacterControllerState(true);
                characterAttackController.ToggleControllerState(true);
                characterHealthController.SetInvulnerableState(false);
            });
        }

        bool CheckCurrentModel(out SpawnModel currentLvlModel)
        {
            currentLvlModel = container.GetCurrentLvlModel();
            if (currentLvlModel == null)
            {
                ChangeState(GameState.Won);
                return false;
            }

            return true;
        }

        void CreateLvL(SpawnModel currentLvlModel)
        {
            if (currentLvlModel.MiniBosses.Count > 0)
            {
                CreateMiniboss(currentLvlModel);
            }

            npcSystem.SpawnRandomEnemies(currentLvlModel);
        }

        public void CreateCharacter()
        {
            SetupCharacter();
        }

        public void ReviveCharacter()
        {
            characterHealthController.Revive();
            GameTimer.Resume();
            ChangeState(GameState.Ongoing);
        }

        void SetupCharacter()
        {
            var characterController = characterSystem.CreateCharacter();
            characterHealthController =
                characterController.CharacterTransform.GetComponent<CharacterHealthController>();
            characterAttackController =
                characterController.CharacterTransform.GetComponent<CharacterAttackController>();
            characterAbility=characterController.CharacterTransform.GetComponent<AbilityBaseCharacter>();
            characterAbility.Init(characterController.CharacterSO.AnimationData.UltimateAnimations,
                characterController.CharacterSO.UltimateData.Charges);
            characterAttackController.SetCallback(GetExistingEnemies);
            characterHealthController.OnDeath += HandleCharacterDeath;
            characterHealthController.OnBeingDamaged += HandleCharacterDamaged;
            characterHealthController.OnHeal += HandleCharacterHeal;
            characterHealthController.Init();
            characterSystem.SetCharacterControllerState(false);
            cameraController.SetTarget(characterController.CharacterTransform);
            npcSystem.InjectPlayer(characterController.CharacterTransform);
            EffectManager.Initialize(characterController.CharacterTransform.GetComponent<CharacterStatController>());
            BoosterManager.Initialize(characterController.CharacterTransform.GetComponent<CharacterStatController>());
            CurrencySpawner.SetPlayer(characterController.CharacterTransform);
        }

        void CreateMiniboss(SpawnModel currentLvlModel)
        {
            var miniboss = npcSystem.SpawnMiniBoss(currentLvlModel);
            miniBoss = miniboss.GetComponent<IHealthController>();
            miniBoss.OnBeingDamaged += HandleEnemyDamaged;
            miniBoss.OnBeingDamaged += HandleMinibossHealthChange;
            miniBoss.OnDeath += HandleEnemyDeath;
            miniBoss.Init();
            activeEnemyHealthControllers.Add(miniBoss);
            OnMinibossSpawned?.Invoke(true);
            cameraController.SetCameraShakeState(false);
        }

        void HandleMinibossHealthChange(DamageModel damageModel)
        {
            OnMinibossHealthChange?.Invoke(miniBoss.CurrentHealthProportion);
            if (miniBoss.CurrentHealthProportion <= 0)
            {
                miniBoss.OnBeingDamaged -= HandleMinibossHealthChange;
            }
        }

        void HandleEnemySpawned(AiControllerBase obj)
        {
            var healthController = obj.GetComponent<AiHealthController>();
            healthController.OnBeingDamaged += HandleEnemyDamaged;
            healthController.OnDeath += HandleEnemyDeath;
            healthController.Init();
            activeEnemyHealthControllers.Add(healthController);
        }

        void HandleEnemyDeath(IHealthController iHealthController)
        {
            if (currentState != GameState.Ongoing)
                return;

            iHealthController.OnBeingDamaged -= HandleEnemyDamaged;
            iHealthController.OnDeath -= HandleEnemyDeath;
            activeEnemyHealthControllers.Remove(iHealthController);
            enemiesToKill--;
            characterAbility.UpdateAbilityCharges(5);
            OnUltimateChargesChange?.Invoke(characterAbility.CurrentCharge);
            BoosterSpawner.SpawnBoostLoot(container.MobDrop, iHealthController.currentTransform.position);

            CurrencySpawner.SpawnAtPosition(CurrencyKeys.Gold, 10, iHealthController.currentTransform.position);

            CurrencySpawner.SpawnAtPosition(CurrencyKeys.Experience, 10, iHealthController.currentTransform.position);

            OnRemainingEnemiesLeft?.Invoke(enemiesToKill);

            if (enemiesToKill <= 0)
            {
                GameTimer.Stop();

                if (container.FinishedLoop)
                {
                    characterAttackController.ToggleControllerState(false);
                    characterSystem.SetCharacterControllerState(false);

                    CoroutineUtility.WaitForSeconds(1f, () =>
                    {
                        ChangeState(GameState.Won);
                    });

                    return;
                }

                ChangeState(GameState.WaitingPortal);
            }
        }

        void HandleCharacterDamaged(DamageModel damageModel)
        {
            if (currentState != GameState.Ongoing)
                return;

            OnCharacterDamaged?.Invoke(damageModel);
        }

        private void HandleCharacterHeal(float arg1, Transform transform)
        {
            OnCharacterHeal?.Invoke(arg1, transform);
        }

        void HandleCharacterDeath(IHealthController obj)
        {
            Debug.LogError($"character died and game state is {currentState}");
            if (currentState != GameState.Ongoing)
                return;

            //freezes engine?  
            // GameTimer.Pause();
            CoroutineUtility.WaitForSeconds(1f, () =>
            {
                ChangeState(GameState.Lost);
            });
        }

        void HandleEnemyDamaged(DamageModel damageModel)
        {
            UpdateCharacterCombo();
            OnEnemyDamaged?.Invoke(damageModel);
        }

        void UpdateCharacterCombo()
        {
            timeSinceLastStrike = timeToResetCombo;
            characterComboNumber++;
            OnCharacterComboChanged?.Invoke(characterComboNumber);
        }

        IEnumerator CheckTimeSinceLastStrike()
        {
            while (true)
            {
                timeSinceLastStrike -= Time.deltaTime;
                if (timeSinceLastStrike <= 0)
                {
                    characterComboNumber = 0;
                    OnCharacterComboChanged?.Invoke(characterComboNumber);
                }

                yield return null;
            }
        }

        void ChangeState(GameState newState)
        {
            if (currentState == newState)
                return;
            currentState = newState;
            OnGameStateChange?.Invoke(currentState);
        }

        void HandlePlayerTriggerPortal()
        {
            OnNextLvlLoadRequest?.Invoke();
        }

        public void ContinueGameLoop(SpawnModel currentModel)
        {
            ChangeState(GameState.Ongoing);
            cameraController.SetCameraShakeState(currentModel.MiniBosses.Count > 0);
            characterSystem.SetCharacterControllerState(true);
            GameTimer.Start(3, null,
                () =>
                {
                    CreateLvL(currentModel);
                    GameTimer.Start(180, null,
                        () =>
                        {
                            if (currentState != GameState.Ongoing)
                                return;

                            ChangeState(GameState.Lost);
                        }, characterAttackController);
                }, characterAttackController);
        }

        public void StartGameLoop(SpawnModel currentModel)
        {
            ChangeState(GameState.Ongoing);
            cameraController.SetCameraShakeState(currentModel.MiniBosses.Count > 0);
            characterSystem.SetCharacterControllerState(true);
            GameTimer.Start(3, null,
                () =>
                {
                    CreateLvL(currentModel);
                    GameTimer.Start(180, null,
                        () =>
                        {
                            if (currentState != GameState.Ongoing)
                                return;

                            ChangeState(GameState.Lost);
                        }, characterAttackController);
                }, characterAttackController);
        }

        public SpawnModel PreloadLvl()
        {
            if (!CheckCurrentModel(out var currentLvlModel))
            {
                Debug.LogError("Current lvl loop model has 0 lvls");
                return null;
            }


            enemiesToKill = currentLvlModel.MiniBosses.Count == 0
                ? currentLvlModel.MobsAmount
                : currentLvlModel.MobsAmount + 1;
            OnRemainingEnemiesLeft?.Invoke(enemiesToKill);
            OnCharacterComboChanged?.Invoke(characterComboNumber);
            combotTimerRoutine = CoroutineUtility.Start(CheckTimeSinceLastStrike());
            return currentLvlModel;
        }

        private void HandleBoosterActivated(BoosterSO sO, float arg2, Transform transform)
        {
            OnBoosterActivated?.Invoke(sO, arg2, transform);
        }

        public void AddGold(int amount)
        {
            collectedGold += amount;
            OnCoinsCollected?.Invoke(collectedGold);
        }

        public void AddExperience(int amount)
        {
            collectedXp += amount;
        }

        public void StoreRunReward()
        {     
            dataSystemInterface.AddCurency(CurrencyKeys.Gold, collectedGold);
            dataSystemInterface.AddCurency(CurrencyKeys.Experience, collectedXp);
            collectedGold = 0;
            collectedXp = 0;
        }

        private void HandleBoosterWithDurationActivated(BoosterContainer container)
        {
            OnBoosterContainerCreated?.Invoke(container);
        }
    }
}