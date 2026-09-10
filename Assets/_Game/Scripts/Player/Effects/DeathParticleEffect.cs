using UnityEngine;

public class DeathParticleEffect : MonoBehaviour
{
    private ParticleSystem particle;
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    void Start()
    {
        particle.Play();
    }

    void Update()
    {
        if (!particle.IsAlive()) Destroy(gameObject);
    }
}
