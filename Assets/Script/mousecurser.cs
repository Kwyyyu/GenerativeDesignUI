using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mousecurser : MonoBehaviour
{
    public GameObject transform_icon;
    public GameObject transform_target;

    private void Update()
    {
        Get_MouseInput();
        Update_Moving();
    }

    private void Get_MouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Input.mousePosition;
            transform_target.transform.position = mousePos;

            string message = mousePos.ToString();
            Debug.Log(message);
        }
    }

    private void Update_Moving()
    {
        transform_icon.transform.position = Vector3.Lerp(transform_icon.transform.position, transform_target.transform.position, 0.02f);
    }
}
