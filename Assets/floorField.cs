using UnityEngine;

public class floorField : MonoBehaviour
{
    public GameObject Stagefield;

    void Start()
    {
        Stagefield.SetActive(false);
    }


    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player"))
        {
            Stagefield.SetActive(true);
        }


    }
}
