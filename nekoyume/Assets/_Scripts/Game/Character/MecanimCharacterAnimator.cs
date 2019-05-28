using System.Linq;
using UnityEngine;

namespace Nekoyume.Game.Character
{
    public class MecanimCharacterAnimator : CharacterAnimator<Animator>
    {
        private static readonly Vector3 Vector3Zero = Vector3.zero;
        
        private int _baseLayerIndex;
        
        public MecanimCharacterAnimator(CharacterBase root) : base (root)
        {
        }

        public override void ResetTarget(GameObject value)
        {
            base.ResetTarget(value);

            Animator.speed = TimeScale;
            
            _baseLayerIndex = Animator.GetLayerIndex("Base Layer");
        }

        public override bool AnimatorValidation()
        {
            // Reference.
            // if (ReferenceEquals(_anim, null)) 이 라인일 때와 if (_anim == null) 이 라인일 때의 결과가 달라서 주석을 남겨뒀어요.
            // ReferenceEquals(left, null) 함수는 left 변수의 메모리에 담긴 포인터가 null인지 검사하고,
            // `left == null` 식은 left 변수의 메모리에 담긴 포인터가 가리키는 메모리의 값이 null인지 검사합니다.
            return Animator != null;
        }

        public override Vector3 GetHUDPosition()
        {
            return Vector3Zero;
        }

        public override void Appear()
        {
            if (!AnimatorValidation())
            {
                return;
            }
            
            Animator.Play(nameof(CharacterAnimation.Type.Appear));
        }

        public override void Idle()
        {
            if (!AnimatorValidation())
            {
                return;
            }
            
            Animator.Play(nameof(CharacterAnimation.Type.Idle));
            Animator.SetBool(nameof(CharacterAnimation.Type.Run), false);
            Animator.SetBool(nameof(CharacterAnimation.Type.Die), false);
        }

        public override void Run()
        {
            if (!AnimatorValidation())
            {
                return;
            }
            
            Animator.Play(nameof(CharacterAnimation.Type.Run));
            Animator.SetBool(nameof(CharacterAnimation.Type.Run), true);
        }

        public override void StopRun()
        {
            if (!AnimatorValidation())
            {
                return;
            }
            
            Animator.SetBool(nameof(CharacterAnimation.Type.Run), false);
        }

        public override void Attack()
        {
            if (!AnimatorValidation())
            {
                return;
            }

            Animator.Play(nameof(CharacterAnimation.Type.Attack), _baseLayerIndex, 0f);
        }

        /// <summary>
        /// Casting 애니메이션을 재생하는 함수입니다.
        /// 이후 리소스 명을 변경하도록 요청해서 `CharacterAnimation.Type.Casting`을 `CharacterAnimation.Type.Cast`로 변경하겠습니다.
        /// </summary>
        public override void Cast()
        {
            if (!AnimatorValidation())
            {
                return;
            }

            Animator.Play(nameof(CharacterAnimation.Type.Casting), _baseLayerIndex, 0f);
        }

        public override void Hit()
        {
            if (!AnimatorValidation())
            {
                return;
            }

            Animator.Play(nameof(CharacterAnimation.Type.Hit), _baseLayerIndex, 0f);
        }

        public override void Die()
        {
            if (!AnimatorValidation())
            {
                return;
            }
            
            Animator.SetBool(nameof(CharacterAnimation.Type.Die), true);
        }

        public override void Disappear()
        {
            if (!AnimatorValidation())
            {
                return;
            }
            
            Animator.Play(nameof(CharacterAnimation.Type.Disappear));
        }
    }
}
