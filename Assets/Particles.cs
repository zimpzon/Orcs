using UnityEngine;

public class Particles : MonoBehaviour, IKillableObject
{
    public static Particles I;

    public ParticleSystem Acid;

    private void Awake()
    {
        I = this;
    }

    public void SpawnAcid(Vector2 pos)
    {
        Acid.transform.position = pos;
        Acid.Emit(1);
    }

    public void Kill()
    {
        Acid.Clear();
    }
}
