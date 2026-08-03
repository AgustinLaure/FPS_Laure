using System.Collections;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    [SerializeField] private LayerMask targets;
    [SerializeField] private float healAmount;
    [SerializeField] private AudioSource pickSound;
    [SerializeField] private GameObject renderer;
    [SerializeField] private GameObject colliderContainer;
    [SerializeField] private float rotationSpeed;

    private SphereCollider collider;

    private float currentRotation = 0;

    private Coroutine destroyCoroutine;

    private void Awake()
    {
        collider = colliderContainer.GetComponent<SphereCollider>();
    }

    private void Update()
    {
        currentRotation += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(new Vector3(0f, currentRotation, 0f));

        if (destroyCoroutine == null)
        {
            Collider[] hits = Physics.OverlapSphere(colliderContainer.transform.TransformPoint(collider.center), collider.radius, targets, QueryTriggerInteraction.Collide);

            if (hits.Length > 0)
            {
                HealthPoints tagetHealthPoints = hits[0].transform.parent.GetComponent<HealthPoints>();

                if (tagetHealthPoints != null)
                {
                    tagetHealthPoints.Heal(healAmount);

                    destroyCoroutine = StartCoroutine(DestroyCoroutine());
                }
            }
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        bool isSoundPlaying = true;

        pickSound.Play();
        colliderContainer.SetActive(false);
        renderer.SetActive(false);

        while (isSoundPlaying)
        {
            isSoundPlaying = pickSound.isPlaying;
            yield return null;
        }

        Destroy(gameObject);
    }
}
