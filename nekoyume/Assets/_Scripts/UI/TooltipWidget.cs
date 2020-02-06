using System;
using System.Collections.Generic;
using Nekoyume.EnumType;
using Nekoyume.Game;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace Nekoyume.UI
{
    public abstract class TooltipWidget<T> : Widget where T : Model.Tooltip
    {
        public static readonly float2 DefaultOffsetFromTarget = new float2(10f, 0f);
        public static readonly float2 DefaultOffsetFromParent = new float2(10f, 10f);
        
        public RectTransform panel;
    
        private readonly List<IDisposable> _disposablesForModel = new List<IDisposable>();

        public override WidgetType WidgetType => WidgetType.Tooltip;
        
        public T Model { get; private set; }
        
        public abstract PivotPresetType PivotPresetType { get; }
        public virtual float2 OffsetFromTarget => DefaultOffsetFromTarget;
        public virtual float2 OffsetFromParent => DefaultOffsetFromParent;

        public void Show(T value)
        {
            if (value is null)
            {
                Close();
                
                return;
            }
            
            _disposablesForModel.DisposeAllAndClear();
            Model = value;

            Model.target.Subscribe(rect => SubscribeTarget(rect)).AddTo(_disposablesForModel);
            Show();
        }
        
        public override void Close(bool ignoreCloseAnimation = false)
        {
            _disposablesForModel.DisposeAllAndClear();
            Model = null;
            
            base.Close(ignoreCloseAnimation);
        }
        
        protected virtual void SubscribeTarget(RectTransform target)
        {
            panel.MoveToRelatedPosition(target, PivotPresetType, OffsetFromTarget);
            UpdateAnchoredPosition();
        }

        protected virtual void UpdateAnchoredPosition()
        {
            panel.MoveInsideOfParent(OffsetFromParent);
        }
    }
}
