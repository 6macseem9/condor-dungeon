using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class SpawnPoint : MonoBehaviour
{
    private bool _spawnLock;
    private EnemySpawner _enemySpawner;

    private void Start()
    {
        _enemySpawner = GetComponentInParent<EnemySpawner>();
    }

    public void Spawn(int enemyIndex)
    {
        if (_spawnLock) return;
        if (_enemySpawner == null) return;

        _enemySpawner.Spawn(enemyIndex, transform.position);
        _spawnLock = true;
    }

    public void ResetSpawn()
    {
        if(_spawnLock) _spawnLock = false;
    }
}
