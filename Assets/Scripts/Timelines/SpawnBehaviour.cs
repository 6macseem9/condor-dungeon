using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpawnBehaviour : PlayableBehaviour
{
    public int EnemyIndex;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        SpawnPoint spawnPoint = playerData as SpawnPoint;
        spawnPoint.Spawn(EnemyIndex);
    }
}
