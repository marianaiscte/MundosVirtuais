using UnityEngine;

//Script que controlar o movimento de projéteis no jogo
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
        startPosition = transform.position; //guarda posição inicial
        float distance = Vector3.Distance(startPosition, targetPosition); //calcula a distância entre a posição inicial e final
        flightDuration = distance / speed;//calcula a duração de voo do projetil com base na distância e velocidade
    }

    void Update()
    {
        elapsedTime += Time.deltaTime; //aumenta o tempo desde o inicio do voo
        float t = elapsedTime / flightDuration;

        // Interpolação linear da posição horizontal (x e z), que move o projetil na horizontal
        Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);

        // Adiciona a altura parabólica (y)
        currentPosition.y += arcHeight * Mathf.Sin(Mathf.PI * t);

        transform.position = currentPosition;

        // Verifica se atingiu o alvo
        if (t >= 1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (hitEffect != null)
        {   //Inicializa o efeito de embate
            Instantiate(hitEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
