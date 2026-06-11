using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelAbandonParticles : MonoBehaviour
    {
        public void Play(Vector3 center, Color color)
        {
            for (int i = 0; i < 32; i++)
                CreateParticle(center, color);

            Debug.Log("[Module Z] Partículas cúbicas de abandono creadas.");
        }

        private void CreateParticle(Vector3 center, Color color)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            particle.name = "Abandon_Cube_Particle";

            particle.transform.position = center + Random.insideUnitSphere * 0.8f;
            particle.transform.localScale = Vector3.one * Random.Range(0.18f, 0.35f);

            Renderer renderer = particle.GetComponent<Renderer>();
            renderer.material.color = color;

            Rigidbody rb = particle.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 0.2f;

            rb.AddForce(
                new Vector3(
                    Random.Range(-3f, 3f),
                    Random.Range(4f, 7f),
                    Random.Range(-3f, 3f)
                ),
                ForceMode.Impulse
            );

            Destroy(particle, 2.5f);
        }
    }
}