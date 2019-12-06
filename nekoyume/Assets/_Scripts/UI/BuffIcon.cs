﻿using Nekoyume.Game;
using Nekoyume.Game.Controller;
using Nekoyume.Game.VFX;
using Nekoyume.Model;
using Nekoyume.TableData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI
{
    public class BuffIcon : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI remainedDurationText;
        public Game.Buff Data { get; set; }
        public CharacterBase character;

        public void Show(Game.Buff buff, bool isAdded)
        {
            Data = buff;
            image.enabled = true;
            remainedDurationText.enabled = true;
            var sprite = Data.RowData.GetIcon();
            image.overrideSprite = sprite;
            UpdateStatus(Data);

            if (isAdded)
            {
                var vfx = VFXController.instance.CreateAndChaseRectTransform<DropItemInventoryVFX>
                    (image.rectTransform);
            }
        }

        public void UpdateStatus(Game.Buff buff)
        {
            Data = buff;
            remainedDurationText.text = Data.remainedDuration.ToString();
        } 

        public void Hide()
        {
            image.enabled = false;
            remainedDurationText.enabled = false;
            image.overrideSprite = null;
        }
    }
}
