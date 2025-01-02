using System.Collections;
using UnityEngine;

namespace Squirrel_Hero.Demo
{
    public class LoopAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationClip animationClip;
        [SerializeField] private DefaultAnimations defaultAnimation;
        [SerializeField] private float timeBetweenAnimations = 1.0f;
    
        private Animator _animator;
        private int _animationHash;
        private int _defaultAnimationHash;

        private enum DefaultAnimations
        {
            Idle,
            Fall,
            Hit
        }

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();

            if (defaultAnimation == DefaultAnimations.Idle)
                _defaultAnimationHash = Animator.StringToHash("Idle");
            else if (defaultAnimation == DefaultAnimations.Fall)
                _defaultAnimationHash = Animator.StringToHash("Fall");
            else if (defaultAnimation == DefaultAnimations.Hit)
                _defaultAnimationHash = Animator.StringToHash("Hit");
        }

        private void Start()
        {
            if (_animator == null)
            {
                Debug.LogError("LoopAnimationWithNoTransitions script requires an Animator component.");
                enabled = false;
                return;
            }

            if (string.IsNullOrEmpty(animationClip.name))
            {
                Debug.LogError("Specify the name of the animation state in the Animator Controller.");
                enabled = false;
                return;
            }
        
            _animationHash = Animator.StringToHash(animationClip.name);

            StartCoroutine(PlayAnimation());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(PlayAnimation());
        }

        private IEnumerator PlayAnimation()
        {
            _animator.Play(_animationHash, -1, 0f);
        
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            yield return new WaitForSeconds(timeBetweenAnimations);
        
            PlayDefaultAnimation();
        }
    
        private void PlayDefaultAnimation()
        {
            _animator.Play(_defaultAnimationHash, -1, 0f);
        }
    }
}
