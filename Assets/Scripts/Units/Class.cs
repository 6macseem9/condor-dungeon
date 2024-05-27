using UnityEngine;



[CreateAssetMenu(fileName = "Class", menuName = "Unit Class", order = 1)]
public class Class : ScriptableObject
{
    public string ClassName;
    [TextArea]public string ClassDescription;
    public Color ClassColor;
    public AnimationClip Turnaround;
    [Space(7)]

    public Stats Stats;
}
