using UnityEngine;
using UnityEngine.AI;

public class RandomWander : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float wanderRadius = 10f;
    public float timeBetweenWanders = 5f;
    public float rotationSpeed = 5f;

    private UnityEngine.AI.NavMeshAgent agent;
    private float wanderTimer;
    private float timer;
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = timeBetweenWanders;
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenWanders)
        {
            SetNewRandomDestination();
            timer = 0f;
        }

        FaceMovementDirection();
    }
    private void SetNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }


    private void FaceMovementDirection()
    {
        Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            Vector3 lookDirection = new Vector3(velocity.x, 0, velocity.z);

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  targetRotation,
                                                  rotationSpeed * Time.deltaTime);
        }
    }
}
