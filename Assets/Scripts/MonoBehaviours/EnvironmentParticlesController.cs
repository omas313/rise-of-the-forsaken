using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentParticlesController : MonoBehaviour
{
    [SerializeField] ParticleSystem _acidRain;
    [SerializeField] ParticleSystem _fog;
    [SerializeField] ParticleSystem _backFog;
    [SerializeField] ParticleSystem _particles;

    public void SetEnvironmentParticles(BattleDataDefinition battleDataDefinition)
    {
        if (battleDataDefinition.AcidRain)
            _acidRain.Play();
        else
            _acidRain.Stop();

        if (battleDataDefinition.Fog)
            _fog.Play();
        else
            _fog.Stop();

        if (battleDataDefinition.BackFog)
            _backFog.Play();
        else
            _backFog.Stop();

        if (battleDataDefinition.Particles)
            _particles.Play();
        else
            _particles.Stop();
    }

}
