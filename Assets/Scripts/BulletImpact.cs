using UnityEngine;

public class BulletImpactScript : MonoBehaviour
{
    [SerializeField] private float maxTimeAlive;
    private float currentTimeAlive = 0f;

    private void Update()
    {
        currentTimeAlive += Time.deltaTime;

        if (currentTimeAlive >= maxTimeAlive)
        {
            Destroy(gameObject);
        }
    }
}
