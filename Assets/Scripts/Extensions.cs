using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void ReplaceClip(this Animator animator,AnimationClip clip)
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, clip));
        aoc.ApplyOverrides(anims);
        animator.runtimeAnimatorController = aoc;
    }
}

public static class Util
{
    public static Tweener Delay(float time, TweenCallback func, bool realTime = false)
    {
        float timer = 0;
        Tweener tween = DOTween.To(() => timer, x => timer = x, time, time).SetUpdate(realTime);
        tween.onComplete = func;
        return tween;
    }

    public static Tweener Repeat(float time, int times, TweenCallback func, bool realTime = false)
    {
        float timer = 0;
        Tweener tween = DOTween.To(() => timer, x => timer = x, time, time).SetUpdate(realTime);
        tween.SetLoops(times);
        tween.onStepComplete = func;
        return tween;
    }
}