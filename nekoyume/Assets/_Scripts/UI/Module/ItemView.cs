using System;
using Nekoyume.Game.Item;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI.Module
{
    public class ItemView<T> : MonoBehaviour where T : Model.Item
    {
        protected static readonly Color DefaultColor = Color.white;
        protected static readonly Color DimColor = new Color(1f, 1f, 1f, 0.3f);

        public Image iconImage;

        public T Data { get; private set; }

        #region Mono
        
        protected virtual void Awake()
        {
            this.ComponentFieldsNotNullTest();
        }

        protected virtual void OnDestroy()
        {
            Clear();
        }

        #endregion

        public virtual void SetData(T value)
        {
            if (ReferenceEquals(value, null))
            {
                Clear();
                return;
            }

            Data = value;

            UpdateView();
        }

        public virtual void Clear()
        {
            Data = null;

            UpdateView();
        }
        
        protected virtual void SetDim(bool isDim)
        {
            iconImage.color = isDim ? DimColor : DefaultColor;
        }

        private void UpdateView()
        {
            if (ReferenceEquals(Data, null))
            {
                iconImage.enabled = false;
                
                return;
            }
            
            var sprite = ItemBase.GetSprite(Data.item.Value);
            if (ReferenceEquals(sprite, null))
            {
                throw new FailedToLoadResourceException<Sprite>();
            }

            iconImage.overrideSprite = sprite;
            iconImage.SetNativeSize();
            iconImage.enabled = true;
        }
    }
}
