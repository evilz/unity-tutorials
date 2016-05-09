using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public float Speed;

    private int count;
    public Text CountText;
    public Text WinText;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        _rigidbody = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        WinText.text = string.Empty;
    }

    void FixedUpdate()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");

        var accelerationMove = new Vector3(Input.acceleration.x, 0, -Input.acceleration.z);
        var movement = new Vector3(moveHorizontal,0.0f,moveVertical);
        movement += accelerationMove;

        _rigidbody.AddForce(movement * Speed);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText(); 
        }
    }

    void SetCountText()
    {
        CountText.text = "Count:" + count;
        if (count >= 12)
        {
            WinText.text = "You Win";
        }
    }
}
