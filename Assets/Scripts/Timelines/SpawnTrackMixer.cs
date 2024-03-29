using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpawnTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        SpawnPoint spawnPoint = playerData as SpawnPoint;
        bool shouldReset = true;

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if (inputWeight > 0)
            {
                ScriptPlayable<SpawnBehaviour> inputPlayable = (ScriptPlayable<SpawnBehaviour>)playable.GetInput(i);

                SpawnBehaviour input = inputPlayable.GetBehaviour();
                spawnPoint.Spawn(input.EnemyIndex);
                shouldReset = false;
            }
        }

        if (shouldReset) spawnPoint.ResetSpawn();
    }
}
