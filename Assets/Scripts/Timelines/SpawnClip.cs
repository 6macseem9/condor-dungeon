using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpawnClip : PlayableAsset
{
    public int EnemyIndex;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SpawnBehaviour>.Create(graph);

        var behaviour = playable.GetBehaviour();
        behaviour.EnemyIndex = EnemyIndex;

        return playable;
    }
}
