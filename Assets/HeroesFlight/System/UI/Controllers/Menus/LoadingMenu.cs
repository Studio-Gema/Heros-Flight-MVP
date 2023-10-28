using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class LoadingMenu : BaseMenu<LoadingMenu>
    {
        [SerializeField] Image progressImage;
        [SerializeField] TextMeshProUGUI progresstext;
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

        public void UpdateLoadingBar(float progress)
        {
            progressImage.fillAmount = progress;
            int percent = (int)(progress * 100);
            progresstext.text = $"{percent}%";

        }

        public override void OnClosed()
        {
            progressImage.fillAmount = 0;
            CloseMenu();
        }

        public override void ResetMenu()
        {
            progresstext.text = string.Empty;
        }
    }
}
