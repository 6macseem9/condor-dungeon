using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] public Unit[] Enemies { get; private set; } 

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void Spawn(int index, Vector3 position)
    {
        if (Enemies.Length == 0) return;

        var instance = Instantiate(Enemies[index], position, Quaternion.identity);
        instance.Spawn();
    }
}
