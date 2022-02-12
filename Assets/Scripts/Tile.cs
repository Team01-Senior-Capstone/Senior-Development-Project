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

    //What color the tile turns on mouse over
    Material m_Material;
    Color unSelected;
    Color _selected;

    //Where to spawn in pipes and characters
    float pipe_cur_height;
    float character_cur_height;
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
        pipe_cur_height = transform.position.y + 1;
        character_cur_height = transform.position.y + .5f;
        gm = Manager.GetComponent<GameManager>();
        
        row = int.Parse(this.gameObject.name.Substring(0, 1));
        col = int.Parse(this.gameObject.name.Substring(3, 1));
    }

    //Gets what elevation to move/spawn character to
    public Vector3 getCharacterSpawn()
    {
        Vector3 spawn = middle;
        spawn.y = character_cur_height;
        return spawn;
    }

    void placeWorker(GameObject p, string whichWorker)
    {
        GameObject work = Instantiate(p, getCharacterSpawn(), Quaternion.Euler(new Vector3(0, 180, 0)));
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

                    buildOnTile();
                    if (gm.selectedWorker.tag == "1")
                    {
                        gm.g.game.workerBuild(gm.getGameCoreWorker1(), gm.getMe(),
                                 gm.selectedWorker_tile.GetComponent<Tile>().row,
                                 gm.selectedWorker_tile.GetComponent<Tile>().col,
                                 row, col);
                        
                    }
                    else
                    {
                        gm.g.game.workerBuild(gm.getGameCoreWorker2(), gm.getMe(),
                                 gm.selectedWorker_tile.GetComponent<Tile>().row,
                                 gm.selectedWorker_tile.GetComponent<Tile>().col,
                                 row, col);
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
                        Debug.Log("Move to " + gameObject.name);
                    
                        gm.selectedWorker.transform.position = getCharacterSpawn();
                        worker = gm.selectedWorker;

                        if(gm.selectedWorker.tag == "1")
                        {
                            Gamecore.WorkerMoveInfo m = gm.g.game.movePlayer(gm.getGameCoreWorker1(), gm.getMe(),
                                                 gm.selectedWorker_tile.GetComponent<Tile>().row,
                                                 gm.selectedWorker_tile.GetComponent<Tile>().col,
                                                 row, col);
                        }
                        else 
                        {
                            Gamecore.WorkerMoveInfo m = gm.g.game.movePlayer(gm.getGameCoreWorker2(), gm.getMe(),
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
                    gm.gameCorePlaceWorker(row, col, 1);
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
    public void buildOnTile()
    {
        middle.y = pipe_cur_height;
        //Debug.Log(curHeight);

        pipeNum++;
        if (curPipe != null)
        {
            Destroy(curPipe);
        }

        if (pipeNum == 1)
        {
            curPipe = Instantiate(pipe_1, middle, Quaternion.Euler(new Vector3(90, 0, 0)));
            pipe_cur_height += 1;
            character_cur_height += 2;
        }
        else if (pipeNum == 2)
        {
            curPipe = Instantiate(pipe_2, middle, Quaternion.Euler(new Vector3(90, 0, 0)));
            pipe_cur_height += 1;
            character_cur_height += 1;
        }
        else if (pipeNum == 3)
        {
            curPipe = Instantiate(pipe_3, middle, Quaternion.Euler(new Vector3(0, 0, 0)));
            // curHeight += 1;
        }
        //Pipe's size does not increase; do not increase curHeight
        else if (pipeNum == 4)
        {
            curPipe = Instantiate(pipe_4, middle, Quaternion.Euler(new Vector3(90, 180, 0)));
        }
        curPipe.transform.SetParent(this.gameObject.transform);

        
        
    }

    public void moveToTile(GameObject worker, Tile fromTile)
    {
        worker.transform.position = getCharacterSpawn();
        this.worker = worker;
        fromTile.worker = null;
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
