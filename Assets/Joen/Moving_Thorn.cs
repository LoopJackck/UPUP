using System.Collections;
using Gamekit2D;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Moving_Thorn : MonoBehaviour
{
    public float deactivate_Time = 0.5f; 
    public float active_Time = 2f;

    public float scaleSpeed = 50f;

    private Vector2 Max_Scale = new Vector2(1.154001f, 1.598883f);
    private Vector2 Min_Scale = new Vector2(1.154001f, 0.3011661f);

    private bool IsActive = false;
    public BoxCollider2D BoxColl;

    public Damager D;

    void Start()
    {
        StartCoroutine(ative_Thorn());
        BoxColl = GetComponent<BoxCollider2D>();
    }

 
    void Update()
    {
        //if(IsActive)
        //{
        //    transform.localScale = Vector2.Lerp(transform.localScale, Max_Scale, Time.deltaTime * scaleSpeed);
        //}
        //else
        //{
        //    transform.localScale = Vector2.Lerp(transform.localScale, Min_Scale, Time.deltaTime * scaleSpeed);
        //}


        float targetY = IsActive ? Max_Scale.y : Min_Scale.y;
        transform.localScale = new Vector2(
            transform.localScale.x,
            Mathf.Lerp(transform.localScale.y, targetY, Time.deltaTime * scaleSpeed)
        );
    }

    IEnumerator ative_Thorn()
    {

        while (true)
        {
            IsActive = false;
            //BoxColl.isTrigger = true;
            D.gameObject.SetActive(true);
            //D.hittableLayers = LayerMask.NameToLayer("Nothing");

            yield return new WaitForSeconds(deactivate_Time);

            IsActive = true;
            //  BoxColl.isTrigger = false;
            D.gameObject.SetActive(false);
            //D.hittableLayers = LayerMask.NameToLayer("Player");

            yield return new WaitForSeconds(active_Time);

        }



        yield return null;
    }

}
