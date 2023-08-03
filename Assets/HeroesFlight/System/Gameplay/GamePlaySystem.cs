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

namespace HeroesFlight.System.Gameplay
{
    public class GamePlaySystem : GamePlaySystemInterface
    {
        public GamePlaySystem(CharacterSystemInterface characterSystem, NpcSystemInterface npcSystem)
        {
            this.npcSystem = npcSystem;
            this.characterSystem = characterSystem;
            npcSystem.OnEnemySpawned += HandleEnemySpawned;
            GameTimer = new CountDownTimer();
        }

        public event Action OnNextLvlLoadRequest;
        public CountDownTimer GameTimer { get; private set; }
        public AngelEffectManager EffectManager { get;private set; }

        public int CurrentLvlIndex => container.CurrentLvlIndex;

        public event Action<bool> OnMinibossSpawned;
        public event Action<float> OnMinibossHealthChange;

        public event Action<int> OnRemainingEnemiesLeft;
        public event Action<DamageModel> OnCharacterDamaged;
        public event Action<DamageModel> OnEnemyDamaged;
        public event Action<int> OnCharacterHealthChanged;
        public event Action<int> OnCharacterComboChanged;
        public event Action<GameplayState> OnGameStateChange;
        List<IHealthController> GetExistingEnemies() => activeEnemyHealthControllers;
        List<IHealthController> activeEnemyHealthControllers = new();
        IHealthController miniBoss;
        IHealthController characterHealthController;
        CharacterAttackController characterAttackController;
        CharacterSystemInterface characterSystem;
        CameraControllerInterface cameraController;
        NpcSystemInterface npcSystem;
        GameplayContainer container;
        GameplayState currentState;
        float timeSinceLastStrike;
        float timeToResetCombo = 3f;
        int characterComboNumber;
        int enemiesToKill;
        int wavesAmount;
        Coroutine combotTimerRoutine;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            cameraController = scene.GetComponentInChildren<CameraControllerInterface>();
            EffectManager = scene.GetComponentInChildren<AngelEffectManager>();
            container = scene.GetComponentInChildren<GameplayContainer>();
            container.Init();
            container.OnPlayerEnteredPortal += HandlePlayerTriggerPortal;
            container.SetStartingIndex(0);
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
            characterAttackController = null;
            characterHealthController = null;
        }

        public void ResetLogic()
        {
            activeEnemyHealthControllers.Clear();
            enemiesToKill = 0;
            GameTimer.Stop();
            ChangeState(GameplayState.Ended);
            OnMinibossSpawned?.Invoke(false);
            CoroutineUtility.Stop(combotTimerRoutine);
        }

        bool CheckCurrentModel(out SpawnModel currentLvlModel)
        {
            currentLvlModel = container.GetCurrentLvlModel();
            if (currentLvlModel == null)
            {
                ChangeState(GameplayState.Won);
                return false;
            }

            return true;
        }

        void CreateLvL(SpawnModel currentLvlModel)
        {
            characterSystem.SetCharacterControllerState(true);
            if (currentLvlModel.MiniBosses.Count > 0)
            {
                CreateMiniboss(currentLvlModel);
            }

            npcSystem.SpawnRandomEnemies(currentLvlModel);
        }

        public void ReviveCharacter()
        {
            characterHealthController.Revive();
            GameTimer.Resume();
            ChangeState(GameplayState.Ongoing);
        }

        void SetupCharacter()
        {
            var characterController = characterSystem.CreateCharacter();
            characterHealthController =
                characterController.CharacterTransform.GetComponent<CharacterHealthController>();
            characterAttackController =
                characterController.CharacterTransform.GetComponent<CharacterAttackController>();
            characterAttackController.SetCallback(GetExistingEnemies);
            characterHealthController.OnDeath += HandleCharacterDeath;
            characterHealthController.OnBeingDamaged += HandleCharacterDamaged;
            characterHealthController.Init();
            characterSystem.SetCharacterControllerState(false);
            cameraController.SetTarget(characterController.CharacterTransform);
            npcSystem.InjectPlayer(characterController.CharacterTransform);
            EffectManager.Initialize(  characterController.CharacterTransform.GetComponent<CharacterStatController>());
        }

        void CreateMiniboss(SpawnModel currentLvlModel)
        {
            var miniboss = npcSystem.SpawnMiniBoss(currentLvlModel);
            miniBoss = miniboss.GetComponent<IHealthController>();
            miniBoss.OnBeingDamaged += HandleEnemyDamaged;
            miniBoss.OnBeingDamaged += HandleMinibosshealthChange;
            miniBoss.OnDeath += HandleEnemyDeath;
            miniBoss.Init();
            activeEnemyHealthControllers.Add(miniBoss);
            OnMinibossSpawned?.Invoke(true);
            cameraController.SetCameraShakeState(false);
        }

        void HandleMinibosshealthChange(DamageModel damageModel)
        {
            OnMinibossHealthChange?.Invoke(miniBoss.CurrentHealthProportion);
            if (miniBoss.CurrentHealthProportion <= 0)
            {
                miniBoss.OnBeingDamaged -= HandleMinibosshealthChange;
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
            if (currentState != GameplayState.Ongoing)
                return;

            iHealthController.OnBeingDamaged -= HandleEnemyDamaged;
            iHealthController.OnDeath -= HandleEnemyDeath;
            activeEnemyHealthControllers.Remove(iHealthController);
            enemiesToKill--;
            OnRemainingEnemiesLeft?.Invoke(enemiesToKill);
            if (enemiesToKill <= 0)
            {
                if (container.FinishedLoop)
                {
                    CoroutineUtility.WaitForSeconds(1f, () =>
                    {
                        ChangeState(GameplayState.Won);
                    });

                    return;
                }

                CoroutineUtility.WaitForSeconds(1f, () =>
                {
                    container.EnablePortal();
                });
            }
        }

        void HandleCharacterDamaged(DamageModel damageModel)
        {
            if (currentState != GameplayState.Ongoing)
                return;

            OnCharacterDamaged?.Invoke(damageModel);
        }

        void HandleCharacterDeath(IHealthController obj)
        {
            Debug.LogError($"character died and game state is {currentState}");
            if (currentState != GameplayState.Ongoing)
                return;

            //freezes engine?  
            // GameTimer.Pause();
            CoroutineUtility.WaitForSeconds(1f, () =>
            {
                ChangeState(GameplayState.Lost);
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

        void ChangeState(GameplayState newState)
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
        
            OnGameStateChange?.Invoke(currentState);
            cameraController.SetCameraShakeState(currentModel.MiniBosses.Count > 0);
            GameTimer.Start(3, null,
                () =>
                {
                    CreateLvL(currentModel);
                    GameTimer.Start(180, null,
                        () =>
                        {
                            if (currentState != GameplayState.Ongoing)
                                return;

                            ChangeState(GameplayState.Lost);
                        }, characterAttackController);
                }, characterAttackController);
        }

        public void StartGameLoop(SpawnModel currentModel)
        {
            SetupCharacter();
            currentState = GameplayState.Ongoing;
            OnGameStateChange?.Invoke(currentState);
            cameraController.SetCameraShakeState(currentModel.MiniBosses.Count > 0);
            GameTimer.Start(3, null,
                () =>
                {
                    CreateLvL(currentModel);
                    GameTimer.Start(180, null,
                        () =>
                        {
                            if (currentState != GameplayState.Ongoing)
                                return;

                            ChangeState(GameplayState.Lost);
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
    }
}