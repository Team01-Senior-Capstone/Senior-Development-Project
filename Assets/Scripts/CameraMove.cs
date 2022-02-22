using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3[] positions;
    Quaternion[] rotations;
    int index;

    public void Start()
    {
        positions = new Vector3[] { new Vector3(0, 0, -15), new Vector3(15,10, 0), new Vector3(0, 10, 14), new Vector3(-15, 10, 0) };
        rotations = new Quaternion[] { Quaternion.Euler(42.5f, 0f, 0f), Quaternion.Euler(42.5f, -90f, 0f), Quaternion.Euler(42.5f, 180f, 0f), Quaternion.Euler(42.5f, 90f, 0f) };
    }


    public void toggleRight()
    {
        index++;
        if(index >= positions.Length)
        {
            index = 0;
        }
        Vector3 newPos = new Vector3(positions[index].x, this.transform.position.y, positions[index].z);
        this.transform.position = newPos;
        this.transform.rotation = rotations[index];
    }

    public void toggleLeft()
    {
        index--;
        if (index < 0)
        {
            index = positions.Length-1;
        }
        Vector3 newPos = new Vector3(positions[index].x, this.transform.position.y, positions[index].z);
        this.transform.position = newPos;
        this.transform.rotation = rotations[index];
    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            toggleLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            toggleRight();
        }
    }
}
