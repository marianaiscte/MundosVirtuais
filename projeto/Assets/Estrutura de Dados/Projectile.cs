using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 1f;
    public GameObject hitEffect;
    public float arcHeight = 0.5f; // Altura máxima da parábola

    private Vector3 startPosition;
    private float flightDuration;
    private float elapsedTime = 0f;

    void Start()
    {
        startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        flightDuration = distance / speed;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / flightDuration;

        // Interpolação linear da posição horizontal (x e z)
        Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);

        // Adiciona a altura parabólica (y)
        currentPosition.y += arcHeight * Mathf.Sin(Mathf.PI * t);

        transform.position = currentPosition;

        if (t >= 1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
