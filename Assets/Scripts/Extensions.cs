using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    public static bool IsEnemy(this Unit unit)
    {
        return unit.CompareTag("EnemyUnit");
    }

    public static Vector3 Clamp(this Vector3 vector, float min, float max)
    {
        if (vector.x > max) vector.x = max;
        if (vector.x < min) vector.x = min;
        if (vector.z > max) vector.z = max;
        if (vector.z < min) vector.z = min;

        return vector;
    }

    public static void AddPressAnimation(this Button button)
    {
        button.onClick.AddListener(() =>
        {
            button.transform.DOKill();
            button.transform.DOScale(0.8f, 0);
            button.transform.DOScale(1f, 0.2f);
        });
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

    public static float DistanceTo(this NavMeshAgent nav, Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        nav.CalculatePath(destination, path);
        
        var corners = path.corners;
        float distance = 0;
        for (int i= 1; i<corners.Length;i++)
        {
            distance += Vector3.Distance(corners[i - 1], corners[i]);
        }
        return distance;
    }
}