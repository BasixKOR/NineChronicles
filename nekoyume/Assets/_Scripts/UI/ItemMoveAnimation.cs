﻿using Nekoyume.Game.VFX;
using UnityEngine;
using UnityEngine.UI;
using Nekoyume.Game;
using DG.Tweening;
using Nekoyume.Game.Controller;
using System.Collections;
using Nekoyume.UI.Module;
using UnityEngine.UI.Extensions;

namespace Nekoyume.UI
{
    public class ItemMoveAnimation : AnimationWidget
    {
        private Vector3 _endPosition;
        private float _middleXGap;
        public Image itemImage = null;
        
        public static ItemMoveAnimation Show(Sprite itemSprite, Vector3 startWorldPosition, Vector3 endWorldPosition, 
            bool moveToLeft = false, float animationTime = 1f, float middleXGap = 0f, bool endPointIsInventory = false)
        {
            var result = Create<ItemMoveAnimation>(true);

            result.Show();
            
            result.IsPlaying = true;
            result.itemImage.sprite = itemSprite;
            var rect = result.RectTransform;
            rect.anchoredPosition = startWorldPosition.ToCanvasPosition(ActionCamera.instance.Cam, MainCanvas.instance.Canvas);

            result._endPosition = endWorldPosition;
            result._animationTime = animationTime;
            result._middleXGap = middleXGap;

            result.StartCoroutine(result.CoPlay(moveToLeft, endPointIsInventory));
            return result;
        }

        private IEnumerator CoPlay(bool moveToLeft, bool endPointIsInventory)
        {
            VFXController.instance.Create<ItemMoveVFX>(transform.position);

            Tweener tweenScale = transform.DOScale(1.2f, 0.1f).SetEase(Ease.OutSine);
            yield return new WaitWhile(tweenScale.IsPlaying);
            
            yield return new  WaitForSeconds(0.5f);

            Vector3 midPath;
            if (moveToLeft)
                midPath = new Vector3(transform.position.x - _middleXGap, (_endPosition.y + transform.position.y) / 2, _endPosition.z);
            else
                midPath = new Vector3(transform.position.x + _middleXGap, (_endPosition.y + transform.position.y) / 2, _endPosition.z);

            Vector3[] path = new Vector3[] { transform.position, midPath, _endPosition };

            Tweener tweenMove;
            tweenMove = transform.DOPath(path, _animationTime, PathType.CatmullRom).SetEase(Ease.OutSine);
            yield return new WaitForSeconds(_animationTime - 0.5f);
            if(endPointIsInventory)
                Find<BottomMenu>().PlayGetItemAnimation();
            yield return new WaitWhile(tweenMove.IsPlaying);
            itemImage.enabled = false;
            
            if(!endPointIsInventory)
            {
                var vfx = VFXController.instance.Create<ItemMoveVFX>(_endPosition);

                yield return new WaitWhile(() => vfx.gameObject.activeSelf);
            }
            IsPlaying = false;

            Close();
        }
    }
}
