using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject PlayerExplosion;
    public int ScoreValue;
    private GameController _gameController;

    void Start()
    {
        GameObject gameControlleObject = GameObject.FindWithTag("GameController");
        if (gameControlleObject != null)
        {
            _gameController = gameControlleObject.GetComponent<GameController>();
        }
        if (_gameController == null)
        {
            Debug.Log("Cannot find GameController script !");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary") { return; }

        Instantiate(Explosion, transform.position, transform.rotation);

        if (other.tag == "Player")
        { 
            Instantiate(PlayerExplosion, other.transform.position, other.transform.rotation);
            _gameController.GameOver();
        }
        _gameController.AddScore(ScoreValue);
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
