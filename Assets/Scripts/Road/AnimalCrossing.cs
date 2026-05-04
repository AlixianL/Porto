using UnityEngine;
using System.Collections;

public class AnimalCrossing : MonoBehaviour
{
    public GameObject animal;
    public float speed = 5f;

    public bool leftToRight = true;

    
    public float destroyDelay = 3f;

    private bool shouldMove = false;
    private bool hasStartedDestroyTimer = false;

    void Update()
    {
        if (shouldMove && animal != null)
        {
            Vector3 direction = leftToRight ? Vector3.right : Vector3.left;
            animal.transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarMovement"))
        {
            shouldMove = true;

       
            if (!hasStartedDestroyTimer)
            {
                hasStartedDestroyTimer = true;
                StartCoroutine(DestroyAfterDelay());
            }
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);

        if (animal != null)
        {
            Destroy(animal);
        }
    }
}