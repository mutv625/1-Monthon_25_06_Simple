using UnityEngine;

// このスクリプトにはAnimatorとAudioSourceが必須であることを示す
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class AnimationAutoDestroy : MonoBehaviour
{
    [Tooltip("ここで再生する効果音(SE)ファイル")]
    public AudioClip soundEffect;

    [Tooltip("アニメーションの再生速度（1が標準）")]
    public float animationSpeed = 1.0f;

    void Start()
    {
        // 自身のAnimatorコンポーネントを取得
        Animator animator = GetComponent<Animator>();
        // 再生速度を設定
        animator.speed = this.animationSpeed;

        // 自身のAudioSourceを取得し、効果音を再生
        AudioSource audioSource = GetComponent<AudioSource>();
        if (soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
        
        // アニメーションの長さを取得し、再生速度を考慮して消滅までの時間を計算
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        float destroyTime = (animationSpeed > 0) ? animationLength / animationSpeed : animationLength;
        
        // 計算した時間後に、このGameObject自身を破壊する
        Destroy(gameObject, destroyTime);
    }
}