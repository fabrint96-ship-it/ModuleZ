using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelVictoryParticles : MonoBehaviour
    {
        public void Play(Vector3 center, Color color)
        {
            for (int i = 0; i < 24; i++)
            {
                CreateParticle(center, color);
            }

            Debug.Log("[Module Z] Partículas cúbicas de victoria creadas.");
        }

        private void CreateParticle(Vector3 center, Color color)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            particle.name = "Victory_Cube_Particle";

            particle.transform.position = center + Random.insideUnitSphere * 0.6f;
            particle.transform.localScale = Vector3.one * Random.Range(0.12f, 0.28f);

            Renderer renderer = particle.GetComponent<Renderer>();
            renderer.material.color = color;

            Rigidbody rb = particle.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 0.2f;

            Vector3 force = new Vector3(
                Random.Range(-2.5f, 2.5f),
                Random.Range(3f, 6f),
                Random.Range(-2.5f, 2.5f)
            );

            rb.AddForce(force, ForceMode.Impulse);

            Destroy(particle, 1.5f);
        }
    }
}