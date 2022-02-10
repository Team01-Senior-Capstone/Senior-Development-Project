using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //
    [Header("Set in Inspector")]
    public GameObject pipe_1;
    public GameObject pipe_2;
    public GameObject pipe_3;
    public GameObject pipe_4;

    public int row;
    public int col;

    public GameObject worker;   

    public Vector3 middle;
    Material m_Material;
    Color unSelected;
    Color _selected;
    float curHeight;
    float pipeHeight;
    int pipeNum = 0;
    GameObject curPipe;
    public bool selectable = false;
    public GameObject Manager;
    GameManager gm;

    const int PIPE_SIZE = 2;

    void Start()
    {
        //Fetch the Material from the Renderer of the GameObject
        m_Material = GetComponent<Renderer>().material;
        unSelected = m_Material.color;
        _selected = unSelected;
        _selected.b += 30;
        middle = GetComponent<Renderer>().bounds.center;
        curHeight = transform.position.y + 1;
        gm = Manager.GetComponent<GameManager>();
        
        row = int.Parse(this.gameObject.name.Substring(0, 1));
        col = int.Parse(this.gameObject.name.Substring(3, 1));
    }

    void placeWorker(GameObject p, string whichWorker)
    {
        middle.y = curHeight;
        GameObject work = Instantiate(p, middle, Quaternion.Euler(new Vector3(0, 180, 0)));
        work.tag = whichWorker;
        worker = work;
    }

    private void OnMouseDown()
    {
        if (!selectable) return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            if (hit.transform != null)
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (gm.getAction() == Action.BUILD) {
                    if(gm.selectedWorker.tag == "1")
                    {
                        buildOnTile(gm.getGameCoreWorker1());
                    }
                    else
                    {
                        buildOnTile(gm.getGameCoreWorker2());
                    }

                    gm.toggleAction();
                }
                else if (gm.getAction() == Action.PLAY)
                {
                    if (this.worker != null)
                    {
                        gm.selectedWorker = null;
                        gm.selectedWorker_tile = null;
                        gm.returnToSelect();
                    }
                    else
                    {
                        //Debug.Log("Move to " + gameObject.name);
                        gm.selectedWorker.transform.position = middle;
                        worker = gm.selectedWorker;

                        if(gm.selectedWorker.tag == "1")
                        {
                            gm.g.game.movePlayer(gm.getGameCoreWorker1(), gm.getMe(),
                                                 gm.selectedWorker_tile.GetComponent<Tile>().row,
                                                 gm.selectedWorker_tile.GetComponent<Tile>().col,
                                                 row, col);
                        }
                        else 
                        {
                            gm.g.game.movePlayer(gm.getGameCoreWorker2(), gm.getMe(),
                                                 gm.selectedWorker_tile.GetComponent<Tile>().row,
                                                 gm.selectedWorker_tile.GetComponent<Tile>().col,
                                                 row, col);
                        }



                        gm.selectedWorker_tile.GetComponent<Tile>().worker = null;
                        gm.selectedWorker_tile = this.gameObject;
                        gm.toggleAction();
                    }
                }
                else if (gm.getAction() == Action.SELECT)
                {
                    gm.selectedWorker = worker;
                    gm.selectedWorker_tile = gameObject;

                    gm.toggleAction();
                }
                else if(gm.getAction() == Action.FIRST_MOVE)
                {
                    placeWorker(gm.getWorker1(), "1");
                    gm.gameCorePlaceWorker(row, col, 2);
                    gm.toggleAction();
                }
                else if (gm.getAction() == Action.SECOND_MOVE)
                {
                    placeWorker(gm.getWorker2(), "2");
                    gm.gameCorePlaceWorker(row, col, 2);
                    gm.toggleAction();
                }
            }
    }

    //Builds a pipe on the tile
    public void buildOnTile(Gamecore.Worker worker)
    {
        middle.y = curHeight;
        Debug.Log(curHeight);

        pipeNum++;
        if (curPipe != null)
        {
            Destroy(curPipe);
        }

        if (pipeNum == 1)
        {
            curPipe = Instantiate(pipe_1, middle, Quaternion.Euler(new Vector3(90, 0, 0)));
            curHeight += 1;
        }
        else if (pipeNum == 2)
        {
            curPipe = Instantiate(pipe_2, middle, Quaternion.Euler(new Vector3(90, 0, 0)));
            curHeight += 1;
        }
        else if (pipeNum == 3)
        {
            curPipe = Instantiate(pipe_3, middle, Quaternion.Euler(new Vector3(0, 0, 0)));
            // curHeight += 1;
        }
        //Pipe's size does not increase; do not increase curHeight
        else if (pipeNum == 4)
        {
            curPipe = Instantiate(pipe_4, middle, Quaternion.Euler(new Vector3(0, 180, 0)));
        }
        curPipe.transform.SetParent(this.gameObject.transform);

        gm.g.game.workerBuild(worker, gm.getMe(),
                                 gm.selectedWorker_tile.GetComponent<Tile>().row,
                                 gm.selectedWorker_tile.GetComponent<Tile>().col,
                                 row, col);
        
    }

    void OnMouseOver()
    {
        if (!selectable) return;
        // Change the Color of the GameObject when the mouse hovers over it
        m_Material.color = _selected;
    }

    void OnMouseExit()
    {
        //Change the Color back to white when the mouse exits the GameObject
        m_Material.color = unSelected;
    }

    void OnDestroy()
    {
        //Destroy the instance
        Destroy(m_Material);
    }
}
