
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3[] positions;
    Quaternion[] rotations, birdEyeRotations;
    int index;
    bool canBirdEye;

    public void Start()
    {
        positions = new Vector3[] { new Vector3(0, 10, -13), new Vector3(14.5f,10, 0), new Vector3(1f, 10, 13.5f), new Vector3(-13f, 10, 0) };
        rotations = new Quaternion[] { Quaternion.Euler(42.5f, 0f, 0f), Quaternion.Euler(42.5f, -90f, 0f), Quaternion.Euler(42.5f, 180f, 0f), Quaternion.Euler(42.5f, 90f, 0f) };
        birdEyeRotations = new Quaternion[] { Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(89f, 90f, 0f), Quaternion.Euler(90f, 180f, 0f), Quaternion.Euler(91f, -90f, 0f)};
        canBirdEye = true;
    }


    public void toggleRight()
    {
        index++;
        if(index >= positions.Length)
        {
            index = 0;
        }

        if (canBirdEye) {
            Vector3 newPos = new Vector3(positions[index].x, this.transform.position.y, positions[index].z);
            this.transform.position = newPos;
            this.transform.rotation = rotations[index];
        }
        else {
            this.transform.rotation = birdEyeRotations[index];
        }
    }

    public void toggleLeft()
    {
        index--;
        if (index < 0)
        {
            index = positions.Length-1;
        }
        
        if (canBirdEye) {
            Vector3 newPos = new Vector3(positions[index].x, this.transform.position.y, positions[index].z);
            this.transform.position = newPos;
            this.transform.rotation = rotations[index];
        }
        else {
            this.transform.rotation = birdEyeRotations[index];
        }
    }

    public void toggleUp () {

        this.transform.position = new Vector3(0f, 15f, -0.5f);
        this.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        GameObject light = GameObject.FindGameObjectWithTag("Light");
        canBirdEye = false;
    }
    
    public void toggleDown () {

        this.transform.position = new Vector3(0f, 10f, -13f);
        this.transform.rotation = Quaternion.Euler(42.5f, 0f, 0f);
        canBirdEye = true;
    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            toggleLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            toggleRight();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && canBirdEye) {
            toggleUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !canBirdEye) {
            toggleDown();
        }
    }
}
