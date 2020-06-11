using System.Linq;
using Libplanet;
using Nekoyume.EnumType;
using Nekoyume.Game.Controller;
using Nekoyume.Helper;
using Nekoyume.Model.Item;
using Nekoyume.Model.State;
using TMPro;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nekoyume.UI
{
    public class AvatarTooltip : TooltipWidget<AvatarTooltip.ViewModel>
    {
        public class ViewModel : Model.Tooltip
        {
            public ViewModel(RectTransform target)
            {
                this.target.Value = target;
            }
        }

        [SerializeField]
        private Image avatarImage = null;

        [SerializeField]
        private TextMeshProUGUI levelText = null;

        [SerializeField]
        private TextMeshProUGUI titleText = null;

        [SerializeField]
        private TextMeshProUGUI nameAndHashText = null;

        [SerializeField]
        private Button avatarInfoButton = null;

        private AvatarState _selectedAvatarState = null;

        protected override PivotPresetType TargetPivotPresetType => PivotPresetType.TopRight;
        protected override float2 OffsetFromTarget => new float2(-20f, -30f);

        protected override void Awake()
        {
            base.Awake();
            avatarInfoButton
                .OnClickAsObservable()
                .Subscribe(OnClickAvatarInfo)
                .AddTo(gameObject);
        }

        protected override void Update()
        {
            base.Update();

            if (Input.touchCount == 0 &&
                !Input.anyKeyDown)
            {
                return;
            }

            var current = EventSystem.current;
            if (current.currentSelectedGameObject == avatarInfoButton.gameObject)
            {
                return;
            }

            Close();
        }

        public void Show(RectTransform target, Address avatarAddress)
        {
            var avatarState =
                new AvatarState(
                    (Bencodex.Types.Dictionary) Game.Game.instance.Agent.GetState(avatarAddress));
            Show(target, avatarState);
        }

        public void Show(RectTransform target, AvatarState avatarState)
        {
            avatarImage.sprite = SpriteHelper.GetCharacterIcon(avatarState.characterId);
            levelText.text = $"<color=#B38271>LV.{avatarState.level}</color>";

            var title = avatarState.inventory.Costumes.FirstOrDefault(costume =>
                costume.ItemSubType == ItemSubType.Title &&
                costume.equipped);
            titleText.text = title is null
                ? ""
                : title.GetLocalizedNonColoredName();
            nameAndHashText.text = avatarState.NameWithHash;
            _selectedAvatarState = avatarState;
            var currentAvatarAddress = Game.Game.instance.States.CurrentAvatarState.address;
            var isCurrentAvatar = currentAvatarAddress.Equals(_selectedAvatarState.address);
            avatarInfoButton.gameObject.SetActive(!isCurrentAvatar);
            Show(new ViewModel(target));
        }

        private void OnClickAvatarInfo(Unit unit)
        {
            AudioController.PlayClick();
            Find<FriendInfoPopup>().Show(_selectedAvatarState);
            Close();
        }
    }
}
