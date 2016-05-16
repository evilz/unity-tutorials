using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public Boundary Boundary = new Boundary();
    public float Tilt;

    public GameObject Shot;
    public Transform ShotSpawn;
    public float FireRate;

    private float nextFire;



    private Rigidbody rigidbody;
    private AudioSource audio;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + FireRate;
            Instantiate(Shot, ShotSpawn.position, ShotSpawn.rotation);
            audio.Play();
        }
    }

    private void FixedUpdate()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");

        var movement = new Vector3(moveHorizontal, .0f, moveVertical);

        rigidbody.velocity = movement * Speed;

        rigidbody.position = new Vector3(
            Mathf.Clamp(rigidbody.position.x, Boundary.xMin, Boundary.xMax),
            0.0f,
            Mathf.Clamp(rigidbody.position.z, Boundary.zMin, Boundary.zMax)
         );

        rigidbody.rotation = Quaternion.Euler(0, 0, rigidbody.velocity.x * (-Tilt));
    }
}
