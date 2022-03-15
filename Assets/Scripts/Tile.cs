using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject pipe_1, pipe_2, pipe_3, pipe_4, curPipe, Manager;
    GameObject worker;
    int row, col, pipeNum = 0;

    public Vector3 middle;

    //What color the tile turns on mouse over
    Material m_Material;
    Color unSelected, _selected;

    //Where to spawn in pipes and characters
    float pipe_cur_height, character_cur_height, pipeHeight;

    bool selectable = false, isSelected;

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
        gm.poof(getCharacterSpawn());
        work.tag = whichWorker;
        worker = work;
        if(whichWorker == "1")
        {
            gm.worker_1 = work;
        }
        else
        {
            gm.worker_2 = work;
        }
    }

    private void OnMouseDown()
    {
        if (!selectable || !isSelectable()) return;
        //Debug.Log(gm.getAction());
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        if (Physics.Raycast(ray, out hit) && hit.collider.transform != null)
        {
            if (gm.getAction() == Action.BUILD) {
                removeSelectable();
                buildOnTile();
                System.Func<Gamecore.Worker> workerFunc;
                if (gm.selectedWorker.tag == "1")
                {
                    workerFunc = gm.getGameCoreWorker1;

                }
                else
                {
                    workerFunc = gm.getGameCoreWorker2;
                }

                Gamecore.TileBuildInfo b = gm.game.getGameController().workerBuild(workerFunc(), gm.getMe(),
                                gm.selectedWorker_tile.GetComponent<Tile>().row,
                                gm.selectedWorker_tile.GetComponent<Tile>().col,
                                row, col);


                int fromTileRow = gm.selectedWorker_tile.GetComponent<Tile>().row;
                int fromTileCol = gm.selectedWorker_tile.GetComponent<Tile>().col;
                gm.selectedWorker_tile.GetComponent<Tile>().removeSelect();
                Move m = new Move(gm.game.getGameController().getGameboard()[fromTileRow, fromTileCol], gm.game.getGameController().getGameboard()[row, col], Gamecore.MoveAction.Build, workerFunc());
                gm.move2 = m;

                gm.toggleAction();
            }
            else if (gm.getAction() == Action.PLAY)
            {
                if (this.worker != null)
                {
                    removeSelect();
                    gm.selectedWorker = null;
                    gm.selectedWorker_tile = null;
                    gm.returnToSelect();
                }
                else
                {
                    removeSelectable();
                    moveToTile(gm.selectedWorker, gm.selectedWorker_tile.GetComponent<Tile>());

                    worker = gm.selectedWorker;
                    System.Func<Gamecore.Worker> workerFunc;
                    if(gm.selectedWorker.tag == "1")
                    {
                        workerFunc = gm.getGameCoreWorker1;
                        
                    }
                    else 
                    {
                        workerFunc = gm.getGameCoreWorker2;
                    }
                    Gamecore.WorkerMoveInfo workMove = gm.game.getGameController().movePlayer(workerFunc(), gm.getMe(),
                                                gm.selectedWorker_tile.GetComponent<Tile>().row,
                                                gm.selectedWorker_tile.GetComponent<Tile>().col,
                                                row, col);
                    //Debug.Log(workMove.wasMoveSuccessful());
                    int fromTileRow = gm.selectedWorker_tile.GetComponent<Tile>().row;
                    int fromTileCol = gm.selectedWorker_tile.GetComponent<Tile>().col;
                    Move m = new Move(gm.game.getGameController().getGameboard()[fromTileRow, fromTileCol], gm.game.getGameController().getGameboard()[row, col], Gamecore.MoveAction.Move, workerFunc());
                    gm.move1 = m;

                    gm.selectedWorker_tile.GetComponent<Tile>().removeSelect();
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
                Move m = new Move(null, gm.game.getGameController().getGameboard()[row, col], Gamecore.MoveAction.Move, gm.getGameCoreWorker1());
                gm.move1 = m;
                placeWorker(gm.getWorker1(), "1");
                gm.gameCorePlaceWorker(row, col, 1);
                gm.toggleAction();

            }
            else if (gm.getAction() == Action.SECOND_MOVE)
            {
                Move m = new Move(null, gm.game.getGameController().getGameboard()[row, col], Gamecore.MoveAction.Move, gm.getGameCoreWorker2());
                gm.move2 = m;

                placeWorker(gm.getWorker2(), "2");
                gm.gameCorePlaceWorker(row, col, 2);
                gm.toggleAction();

            }
        }
    }

    //Debugging
    public void printOccupiedSpaces(string s)
    {
        Debug.Log(s);
        foreach (Gamecore.Tile ti in gm.game.getGameController().getOccupiedTiles())
        {
            Debug.Log(ti.getRow() + ", " + ti.getCol() + " is occupied");
        }
    }

    public void undoPipeBuild()
    {
        

        Destroy(curPipe);
        if(pipeNum == 1)
        {
            //pipe_cur_height -= 1;
            character_cur_height -= 1;
        }
        else if (pipeNum == 2)
        {
            curPipe = Instantiate(pipe_1, middle, Quaternion.Euler(new Vector3(90, 0, 0)));
            pipe_cur_height -= 2;
            character_cur_height -= 2;
        }
        else if (pipeNum == 3)
        {
            curPipe = Instantiate(pipe_2, middle, Quaternion.Euler(new Vector3(90, 0, 0)));
            pipe_cur_height -= 1;
            character_cur_height -= 1;
        }
        else if (pipeNum == 4)
        {
            curPipe = Instantiate(pipe_3, middle, Quaternion.Euler(new Vector3(0, 0, 0)));
            character_cur_height -= 1;
        }
        //Pipe's size does not increase; do not increase curHeight
        else if (pipeNum == 5)
        {
            Vector3 v = middle;
            v.y -= 1.25f;
            curPipe = Instantiate(pipe_4, v, Quaternion.Euler(new Vector3(90, 180, 0)));
        }
        curPipe.transform.SetParent(this.gameObject.transform);
        middle.y = pipe_cur_height;
        pipeNum--;
    }

    //Builds a pipe on the tile
    public void buildOnTile()
    {
        middle.y = pipe_cur_height;
        //Debug.Log(curHeight);

        AudioManager.playBuildSound();
        gm.poof(middle);
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
            character_cur_height += 1;
        }
        //Pipe's size does not increase; do not increase curHeight
        else if (pipeNum == 4)
        {
            Vector3 v = middle;
            v.y += 1.25f;
            curPipe = Instantiate(pipe_4, v, Quaternion.Euler(new Vector3(90, 180, 0)));
        }
        else if(pipeNum == 5)
        {
            Debug.Log("BOOOOM");
        }
        curPipe.transform.SetParent(this.gameObject.transform);

        
        
    }
 

    public void moveToTile(GameObject worker, Tile fromTile)
    {
        StartCoroutine(moveWorkerTo(worker, fromTile));
        //worker.transform.position = getCharacterSpawn();
        this.worker = worker;
        fromTile.worker = null;
    }

    IEnumerator moveWorkerTo(GameObject worker, Tile fromTile)
    {
        Animator anim = worker.GetComponent<Animator>();

        


        float walkSpeed = 7.5f;
        float pipeSpeed = 4f;

        if (fromTile.pipeNum == 0 && pipeNum == 0)
        {
            Quaternion q = worker.transform.rotation;

            Vector3 relativePos = getCharacterSpawn() - worker.transform.position;

            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            worker.transform.rotation = rotation;

            anim.Play("Run");
            while (worker.transform.position != getCharacterSpawn())
            {

                worker.transform.position = Vector3.MoveTowards(worker.transform.position, getCharacterSpawn(), Time.deltaTime * walkSpeed);
                yield return null;
            }


            worker.transform.rotation = q;
            anim.Play("Wait");
        }
        else
        {

            AudioManager.playPipeSound();
            while (worker.transform.position != fromTile.middle)
            {

                worker.transform.position = Vector3.MoveTowards(worker.transform.position, fromTile.middle, Time.deltaTime * pipeSpeed);
                yield return null;
            }
            //anim.Play("Wait");
            worker.transform.position = middle;
            //yield return new WaitForSeconds(.75f);
            while (worker.transform.position != getCharacterSpawn())
            {

                worker.transform.position = Vector3.MoveTowards(worker.transform.position, getCharacterSpawn(), Time.deltaTime * pipeSpeed);
                yield return null;
            }

        }
    }

    public bool isSelectable()
    {
        return GameObject.Find("Help") == null && GameObject.Find("Server").GetComponent<NetworkServer>().connected && GameObject.Find("Disconnect") == null;
    }


    void removeSelectable()
    {
        selectable = false;
        m_Material.color = unSelected;
        foreach (Renderer ri in GetComponentsInChildren<Renderer>())
        {
            ri.material.color = unSelected;
        }
    }

    public void removeSelect()
    {
        isSelected = false;
        m_Material.color = unSelected;
        foreach (Renderer ri in GetComponentsInChildren<Renderer>())
        {
            ri.material.color = unSelected;
        }
    }
    public void keepSelect()
    {
        isSelected = true;
        m_Material.color = _selected;
        foreach (Renderer ri in GetComponentsInChildren<Renderer>())
        {
            ri.material.color = _selected;
        }
    }

    void OnMouseOver()
    {
        if (!selectable || !isSelectable()) return;
        m_Material.color = _selected;
        foreach(Renderer ri in GetComponentsInChildren<Renderer>())
        {
            ri.material.color = _selected;
        }
    }

    void OnMouseExit()
    {
        if (isSelected) return;
        m_Material.color = unSelected;
        foreach(Renderer ri in GetComponentsInChildren<Renderer>())
        {
            ri.material.color = unSelected;
        }
    }

    void OnDestroy()
    {
        Destroy(m_Material);
    }

    public void setSelectable (bool selectable) {

        this.selectable = selectable;
    }

    public bool getSelectable () {
        return this.selectable;
    }

    public int getRow () {
        return this.row;
    }

    public int getCol () {
        return this.col;
    }

    public GameObject getWorker () {
        return this.worker;
    }

    public void setWorker (GameObject w) {
        this.worker = w;
    }
}
