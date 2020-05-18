using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class Mass : MonoBehaviour
{
    public int playerShooterNum=1;
    public Rigidbody rBody;
    public Collider collider;
    public Light explosionLight;
    [HideInInspector]public CoroutineManager coroutineManagerInstance;
    public LayerMask AnimalMask;                        // Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem explosionParticles;         // Reference to the particles that will play on explosion.
    public GameObject MassExplosion;
    public AudioSource ExplosionAudio;                // Reference to the audio that will play on explosion.
    public float MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a animal.
    public float ExplosionForce = 1000f;              // The amount of force added to a animal at the centre of the explosion.
    public float MaxLifeTime = 4f;                    // The time in seconds before the mass is removed.
    public float ExplosionRadius = 5f;                // The maximum distance away from the explosion animals can be and are still affected.


    private void Start ()
    {
        //rBody = GetComponent<Rigidbody>();
        // If it isn't destroyed by then, destroy the mass after it's lifetime.
        Destroy (gameObject, MaxLifeTime);
    }

    private void OnTriggerEnter (Collider other)
    {
        var otherPlayNum = ((AnimalMovement) other.GetComponent(
            typeof(AnimalMovement)));
        if (otherPlayNum != null && otherPlayNum.PlayerNumber == playerShooterNum)
        {
            print(otherPlayNum.PlayerNumber);
            Physics.IgnoreCollision(collider, other);
        }
        else
        {
            print("lol"); //"Animal"+GetComponent<AnimalManager>().PlayerNumber+" took damage.");
            // Collect all the colliders in a sphere from the mass's current position to a radius of the explosion radius.
            Collider[] colliders =
                Physics.OverlapSphere(transform.position, ExplosionRadius, AnimalMask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                print(i);
                // ... and find their rigidbody.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                // If they don't have a rigidbody or theyre mass clones or theyre the original animal, go on to the next collider
                if (!targetRigidbody || targetRigidbody.name.CompareTo("Mass") == 0
                                     || targetRigidbody.name.CompareTo("Mass (Clone)") == 0
                                     || ((AnimalMovement) targetRigidbody.GetComponent(
                                         typeof(AnimalMovement))).PlayerNumber == playerShooterNum)
                    continue;

                // Add an explosion force.
                targetRigidbody.AddExplosionForce(ExplosionForce, transform.position,
                    ExplosionRadius);

                // Find the AnimalHealth script associated with the rigidbody.
                AnimalHealth targetHealth = targetRigidbody.GetComponent<AnimalHealth>();

                // If there is no AnimalHealth script attached to the gameobject, go on to the next collider.
                if (!targetHealth)
                    continue;

                // Calculate the amount of damage the target should take based on it's distance from the mass.
                float damage = CalculateDamage(targetRigidbody.position);

                // Deal this damage to the animal.
                targetHealth.TakeDamage(damage);
            }

            // Unparent the particles from the mass.
            MassExplosion.transform.parent = null;

            // Play the particle system.
            explosionParticles.Play();
            //turn on light and quickly interpolate for a flash effect
            explosionLight.enabled = true;
            coroutineManagerInstance.newCoroutine(MassExplosionFlash());
            // Play the explosion sound effect.
            ExplosionAudio.Play();

            // Once the particles have finished, destroy the gameobject they are on.

            Destroy(explosionParticles.gameObject, explosionParticles.main.duration);
            // Destroy the mass.
            Destroy(gameObject);
        }
    }
    private IEnumerator MassExplosionFlash()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);
        while (explosionLight.intensity>0.1)
        {
            explosionLight.intensity = Mathf.Lerp(explosionLight.intensity, 0, 0.27f);
            yield return wait;
        }
        print("end");
    }

    private float CalculateDamage (Vector3 targetPosition)
    {
        // Create a vector from the mass to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the mass to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (ExplosionRadius - explosionDistance) / ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * MaxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max (0f, damage);

        return damage;
    }
}
