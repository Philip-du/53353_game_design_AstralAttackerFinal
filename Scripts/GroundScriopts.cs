using UnityEngine;

public class GroundScriopts : MonoBehaviour
{
    public GameObject enemyBombPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "EnemyBomb")
        {
           
            // destroy the bullet
            Destroy(collision.gameObject);
        }
    }

}
