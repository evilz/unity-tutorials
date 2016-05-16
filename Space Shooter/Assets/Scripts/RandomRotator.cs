using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{

    public float Tumble;

    private void Start()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.angularVelocity = Random.insideUnitSphere*Tumble;
    }
}
