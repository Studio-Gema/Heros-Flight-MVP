using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

namespace UISystem
{
    public class LoadingMenu : BaseMenu<LoadingMenu>
    {
        private event Action OnComplete;

        public override void OnCreated()
        {

        }

        public override void OnOpened()
        {

        }

        public void Load(Action OnComplete = null)
        {

        }

        public override void OnClosed()
        {

        }

        public override void ResetMenu()
        {

        }
    }
}