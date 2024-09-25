using UnityEngine;
using DG.Tweening;

public class TitleBaseController : MonoBehaviour
{
    [SerializeField] private Transform baseObject; // BaseオブジェクトのTransformをアサイン

    // 動作設定
    [SerializeField] private float moveDistance = 1.0f; // 上下運動の幅
    [SerializeField] private float moveDuration = 2.0f; // 上下運動の時間
    [SerializeField] private float rotateDuration = 3.0f; // 回転の一周にかかる時間

    private bool isAnimating = false; // アニメーションの状態を保持
    private Tween moveTween; // 上下運動のTween
    private Tween rotateTween; // 回転運動のTween

    private bool isMainMenu;
    public bool IsMainMenu
    {
        get { return isMainMenu; }
        set
        {
            if (isMainMenu != value)
            {
                isMainMenu = value;
                UpdateAnimationState();
            }
        }
    }

    private void Start()
    {
        IsMainMenu = true; // 初期状態をメインメニューとして設定
    }

    private void UpdateAnimationState()
    {
        if (isMainMenu && !isAnimating)
        {
            // アニメーション開始
            StartAnimation();
        }
        else if (!isMainMenu && isAnimating)
        {
            // アニメーション停止
            StopAnimation();
        }
    }

    public void StartAnimation()
    {
        // 上下運動のTweenを開始
        moveTween = baseObject.DOMoveY(baseObject.position.y + moveDistance, moveDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // 回転運動のTweenを開始
        rotateTween = baseObject.DORotate(new Vector3(0, 360, 0), rotateDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

        isAnimating = true; // アニメーション中の状態に更新
    }

    public void StopAnimation()
    {
        // 上下運動と回転運動を停止
        moveTween.Kill();
        rotateTween.Kill();

        isAnimating = false; // アニメーション停止状態に更新
    }
}