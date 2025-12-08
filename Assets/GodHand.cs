using UnityEngine;

public class GodHand : MonoBehaviour
{
    public GameObject Point_1;
    public GameObject Point_2;
    public GameObject Point_3;
    public GameObject Point_4;
    public GameObject Point_5;


    public GameObject Ellen;


    void Start()
    {
        
    }

    
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            MovingLikeGOD(Point_1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            MovingLikeGOD(Point_2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MovingLikeGOD(Point_3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MovingLikeGOD(Point_4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            MovingLikeGOD(Point_5);
        }

    }



    void MovingLikeGOD(GameObject GO_point)
    {
        Ellen.transform.position = GO_point.transform.position;
    }
}
