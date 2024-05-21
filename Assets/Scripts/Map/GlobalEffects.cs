using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEffects : MonoBehaviour
{
    public static GlobalEffects Instance;
    [field: SerializeField] public ParticleSystem HealParticles { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;
    }
}
