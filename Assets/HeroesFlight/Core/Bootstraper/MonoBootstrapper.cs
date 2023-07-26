﻿using HeroesFlight.Core.Application;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Input;
using HeroesFlight.System.NPC;
using HeroesFlight.System.UI;
using StansAssets.Foundation.Patterns;
using UnityEngine;


namespace HeroesFlight.Core.Bootstrapper
{

   
    public class MonoBootstrapper : MonoBehaviour,IBootstrapper
    {
        IApplication m_Application;
        ServiceLocator m_ServiceLocator;

        void Awake()
        {
            m_Application = new HeroesFlightApplication();
            m_Application.Start(this);
        }

        public ServiceLocator ResolveServices()
        {
            m_ServiceLocator = new ServiceLocator();
            IInputSystem inputSystem = new InputSystem();
            CharacterSystemInterface characterSystem = new CharacterSystem(inputSystem);
            NpcSystemInterface npcSystem = new NpcSystem();
            GamePlaySystemInterface gamePlaySystem = new GamePlaySystem(characterSystem,npcSystem);
            IUISystem uiSystem = new UiSystem(gamePlaySystem);

            m_ServiceLocator.Register(uiSystem);
            m_ServiceLocator.Register(gamePlaySystem);
            m_ServiceLocator.Register(inputSystem);
            m_ServiceLocator.Register(characterSystem);
            m_ServiceLocator.Register(npcSystem);
            return m_ServiceLocator;
        }
    }
}