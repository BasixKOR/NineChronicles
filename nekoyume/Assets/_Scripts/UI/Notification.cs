using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EasingCore;
using Nekoyume.Model.Mail;
using Nekoyume.UI.Scroller;
using UniRx;
using UnityEngine;

namespace Nekoyume.UI
{
    /// <summary>
    /// Usage: Just to call the `Notification.Push()` method anywhere.
    /// </summary>
    public class Notification : SystemInfoWidget
    {
        private class ReservationModel
        {
            public MailType mailType;
            public string message;
            public long requiredBlockIndex;
            public Guid itemId;
        }

        private const float InternalTimeToAddCell = 1f;
        private const float LifeTimeOfEachNotification = 3f;

        private static readonly int LifeTimeOfEachNotificationCeil =
            Mathf.CeilToInt(LifeTimeOfEachNotification);

        private static readonly List<NotificationCell.ViewModel> SharedModel =
            new List<NotificationCell.ViewModel>();

        private static readonly ConcurrentQueue<NotificationCell.ViewModel> AddQueue =
            new ConcurrentQueue<NotificationCell.ViewModel>();

        private static readonly Subject<Unit> OnEnqueueToAddQueue = new Subject<Unit>();

        private static readonly List<ReservationModel> ReservationList =
            new List<ReservationModel>();

        [SerializeField]
        private NotificationScroll scroll = null;

        private float _lastTimeToAddCell;
        private Coroutine _coAddCell;
        private Coroutine _coRemoveCell;

        #region Control

        public static void Push(MailType mailType, string message)
        {
            AddQueue.Enqueue(new NotificationCell.ViewModel
            {
                mailType = mailType,
                message = message
            });
            OnEnqueueToAddQueue.OnNext(Unit.Default);
        }

        public static void Reserve(
            MailType mailType,
            string message,
            long requiredBlockIndex,
            Guid itemId)
        {
            ReservationList.Add(new ReservationModel
            {
                mailType = mailType,
                message = message,
                requiredBlockIndex = requiredBlockIndex,
                itemId = itemId
            });
        }

        public static void CancelReserve(Guid itemUsableItemId)
        {
            if (!ReservationList.Any())
            {
                return;
            }

            var message = ReservationList
                .FirstOrDefault(m => m.itemId == itemUsableItemId);
            if (message is null)
            {
                return;
            }

            ReservationList.Remove(message);
        }

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            scroll.UpdateData(SharedModel);

            OnEnqueueToAddQueue
                .Where(_ => _coAddCell is null)
                .Subscribe(_ => _coAddCell = StartCoroutine(CoAddCell()))
                .AddTo(gameObject);

            CloseWidget = null;
            Game.Game.instance.Agent.BlockIndexSubject
                .ObserveOnMainThread()
                .Subscribe(SubscribeBlockIndex)
                .AddTo(gameObject);
        }

        #endregion

        private IEnumerator CoAddCell()
        {
            while (AddQueue.TryDequeue(out var viewModel))
            {
                var deltaTime = Time.time - _lastTimeToAddCell;
                if (deltaTime < InternalTimeToAddCell)
                {
                    yield return new WaitForSeconds(InternalTimeToAddCell - deltaTime);
                }

                // NOTE: `LifeTimeOfEachNotificationCeil`개 이상의 노티가 쌓이기 시작하면
                // 노티가 더해지는 연출과 시간이 다 된 노티가 사라지는 연출이 겹치게 되는데, 이러면 연출에 문제가 생깁니다.
                // 그래서 최대 개수를 `LifeTimeOfEachNotificationCeil`로 제한합니다.
                while (SharedModel.Count >= LifeTimeOfEachNotificationCeil)
                {
                    yield return null;
                }

                viewModel.addedTime = Time.time;
                SharedModel.Add(viewModel);
                scroll.UpdateData(SharedModel);
                _lastTimeToAddCell = Time.time;

                if (_coRemoveCell is null)
                {
                    _coRemoveCell = StartCoroutine(CoRemoveCell());
                }
            }

            _coAddCell = null;
        }

        private IEnumerator CoRemoveCell()
        {
            while (SharedModel.Count > 0)
            {
                var target = SharedModel[0];
                var deltaTime = Time.time - target.addedTime;
                if (deltaTime < LifeTimeOfEachNotification)
                {
                    yield return new WaitForSeconds(LifeTimeOfEachNotification - deltaTime);
                }

                var observable = scroll.PlayCellRemoveAnimation(0).First();
                scroll.ScrollTo(1, .5f, Ease.InCirc);
                yield return new WaitForSeconds(.5f);
                yield return observable.ToYieldInstruction();
                SharedModel.RemoveAt(0);
                scroll.UpdateData(SharedModel);
            }

            _coRemoveCell = null;
        }

        #region Subscribe

        {
            {
            }

        }

        {
            {
            }

            {
            }

        }

        private static void SubscribeBlockIndex(long blockIndex)
        {
            foreach (var reservationModel in ReservationList
                .Where(i => i.requiredBlockIndex <= blockIndex)
                .ToList())
            {
                Push(reservationModel.mailType, reservationModel.message);
                ReservationList.Remove(reservationModel);
            }
        }

        #endregion
    }
}
