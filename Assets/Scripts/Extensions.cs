using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public static float DistanceTo(this NavMeshAgent nav, Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        nav.CalculatePath(destination, path);

        var corners = path.corners;
        float distance = 0;
        for (int i = 1; i < corners.Length; i++)
        {
            distance += Vector3.Distance(corners[i - 1], corners[i]);
        }
        return distance;
    }
    public static T[] Slice<T>(this T[] source, int index, int length)
    {
        T[] slice = new T[length];
        Array.Copy(source, index, slice, 0, length);
        return slice;
    }

    public static bool Contains<T>(this List<T> haystack, List<T> needles)
    {
        foreach (T needle in needles)
        {
            if (!haystack.Contains(needle))
            {
                return false;
            }
        }
        return true;
    }

    public static string Print<T>(this List<T> list)
    {
        string res = "[";
        foreach (T item in list)
        {
            res += item.ToString();
            res += ", ";
        }
        return res + "]";
    }
    public static T RandomChoice<T>(this List<T> list)
    {
        return list[Random.Range(0,list.Count)];
    }
    public static T RandomChoice<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    public static void ShowCompletely(this CanvasGroup canv,bool show)
    {
        canv.alpha = show ? 1 : 0;
        canv.blocksRaycasts = show;
        canv.interactable = show;
    }
    public static Vector3 GetCenter(this List<Unit> list)
    {
        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;
        foreach (var obj in list)
        {
            totalX += obj.transform.position.x;
            totalY += obj.transform.position.y;
            totalZ += obj.transform.position.z;
        }
        return new Vector3(totalX, totalY, totalZ) / list.Count;
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
    public static Tweener DelayOneFrame(TweenCallback func, bool realTime = false)
    {
        return Delay(0.01f,func,realTime);
    }

    public static Tweener Repeat(float interval, int times, TweenCallback func, bool realTime = false)
    {
        float timer = 0;
        Tweener tween = DOTween.To(() => timer, x => timer = x, interval, interval).SetUpdate(realTime);
        tween.SetLoops(times);
        tween.onStepComplete = func;
        return tween;
    }
}

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string name;
    public NamedArrayAttribute(string name) { this.name = name; }
}



[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        try
        {
            EditorGUI.PropertyField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).name));
        }
        catch
        {
            EditorGUI.PropertyField(rect, property, label);
        }
    }
}