using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{

    [SerializeField]
    int height;

    [SerializeField]
    bool isMaxHeight = false;

    [SerializeField]
    PiranhaPlant piranhaPlant = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init() {
        this.height = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseHeight () {

        if (this.height < 3)
            this.height++;
        else {
            PiranhaPlant curPlant = new PiranhaPlant();
            piranhaPlant = Instantiate(curPlant);
            isMaxHeight = true;
        }
    }

    public bool IsMaxHeight () {
        return this.isMaxHeight;
    }
}
