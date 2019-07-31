using UnityEngine;

namespace Assets.Scripts.Actors
{
    [RequireComponent(typeof(Animator))]
    public class ActorDisplayerController : MonoBehaviour
    {
        private Animator _animator;
        private float _defaultSpeed;

        public bool IsAnimating => _animator.speed > 0; 

        private void Awake()
        {
            Initialize();
        }

        public void SetAnimationState(bool isOn)
        {
            if(_animator == null)
            {
                Initialize();
            }
            _animator.speed = isOn ? _defaultSpeed : 0f;
        }

        private void Initialize()
        {
            _animator = GetComponent<Animator>();
            _animator.StartPlayback();
            _defaultSpeed = _animator.speed;
        }
    }
}
