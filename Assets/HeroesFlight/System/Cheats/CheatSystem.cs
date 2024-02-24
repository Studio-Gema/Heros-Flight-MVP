﻿using System;
using HeroesFlight.System.Cheats.Data;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Inventory;
using HeroesFlight.System.NPC;
using HeroesFlight.System.Stats.Handlers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Cheats
{
    public class CheatSystem : CheatSystemInterface
    {
        public CheatSystem(GamePlaySystemInterface gamePlaySystemInterface, DataSystemInterface dataSystemInterface,
            CombatSystemInterface combatSystemInterface, TraitSystemInterface traitSystemInterface,
            NpcSystemInterface npcSystemInterface,InventorySystemInterface inventorySystemInterface)
        {
            gamePlaySystem = gamePlaySystemInterface;
            dataSystem = dataSystemInterface;
            combatSystem = combatSystemInterface;
            traitSystem = traitSystemInterface;
            npcSystem = npcSystemInterface;
            inventorySystem = inventorySystemInterface;
            profile = Resources.Load<CheatsDataProfile>(Setup_File_Location);
        }

        GamePlaySystemInterface gamePlaySystem;
        DataSystemInterface dataSystem;
        CombatSystemInterface combatSystem;
        TraitSystemInterface traitSystem;
        NpcSystemInterface npcSystem;
        InventorySystemInterface inventorySystem;

        private const string GOLD_CURRENCY_KEY = "GP";
        private const string GEMS_CURRENCY_KEY = "GEM";

        private const string Setup_File_Location = "Cheats/CheatsDataProfile";
        private CheatsDataProfile profile;

        public void Init(Scene scene = default, Action onComplete = null) { }
        public void Reset() { }

        /// <summary>
        /// Adds currency to the currency manager.
        /// </summary>
        public void AddCurrency()
        {
            dataSystem.CurrencyManager.AddCurrency(GEMS_CURRENCY_KEY,10000);
            dataSystem.CurrencyManager.AddCurrency(GOLD_CURRENCY_KEY,10000);
        }

        /// <summary>
        /// Adds predefined items to the inventory system.
        /// </summary>
        public void AddItems()
        {
            inventorySystem.AddPredefinedItems();
        }

        /// <summary>
        /// Unlocks all traits for the user.
        /// </summary>
        public void UnlockTraits()
        {
            traitSystem.UnlockAllTraits();
        }

        /// <summary>
        /// Kills all enemies in the game.
        /// </summary>
        public void KillAllEnemies()
        {
            npcSystem.KillAllSpawnedEntities();
        }

        /// <summary>
        /// Makes the player immortal.
        /// </summary>
        public void MakePlayerImmortal()
        {
            combatSystem.MakePlayerImmortal();
        }
    }
}