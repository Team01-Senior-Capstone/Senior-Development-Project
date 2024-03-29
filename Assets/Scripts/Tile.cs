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
    Color unSelected, _selected, _highlited;

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
        _highlited = m_Material.color;
        _highlited.g += 1f;
        _highlited.b += 1f;
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
        string character = p.tag;
        GameObject work = Instantiate(p, getCharacterSpawn(), Quaternion.Euler(new Vector3(0, 180, 0)));
        gm.poof(getCharacterSpawn());
        work.tag = whichWorker;
        worker = work;
        if(whichWorker == "1")
        {
            gm.worker_1 = work;
            gm.worker1_tag = character;
            gm.worker_1.tag = character;
        }
        else
        {
            gm.worker_2 = work;
            gm.worker2_tag = character;
            gm.worker_2.tag = character;
        }
        work.tag = character;
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
                gm.waiting = true;
                GameObject.Find("Help Manager").GetComponent<HelpManager>().toggleHelpString();
                
                gm.toggleAction();
            }
            else if (gm.getAction() == Action.PLAY)
            {
                if (this.worker != null)
                {
                    if (this.worker == gm.selectedWorker)
                    {
                        removeSelect();
                        gm.selectedWorker = null;
                        gm.selectedWorker_tile = null;
                        gm.returnToSelect();
                    }
                    else
                    {
                        gm.selectedWorker_tile.GetComponent<Tile>().removeSelect();
                        gm.selectedWorker_tile = this.gameObject;
                        gm.selectedWorker = this.worker;
                        gm.actionSelect();
                    }
                }
                else
                {
                    removeSelectable();
                    moveToTile(gm.selectedWorker, gm.selectedWorker_tile.GetComponent<Tile>());

                    worker = gm.selectedWorker;
                    System.Func<Gamecore.Worker> workerFunc;
                    if(gm.selectedWorker.tag == gm.worker1_tag)
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
                    GameObject.Find("Help Manager").GetComponent<HelpManager>().toggleHelpString();

                    gm.toggleAction();
                }
            }
            else if (gm.getAction() == Action.SELECT)
            {
                gm.selectedWorker = worker;
                gm.selectedWorker_tile = gameObject;
                GameObject.Find("Help Manager").GetComponent<HelpManager>().toggleHelpString();

                gm.toggleAction();
            }
            else if(gm.getAction() == Action.FIRST_MOVE)
            {
                Move m = new Move(null, gm.game.getGameController().getGameboard()[row, col], Gamecore.MoveAction.Move, gm.getGameCoreWorker1());
                gm.move1 = m;
                placeWorker(gm.getWorker1(), "1");
                gm.gameCorePlaceWorker(row, col, 1);
                GameObject.Find("Help Manager").GetComponent<HelpManager>().toggleHelpString();

                gm.toggleAction();

            }
            else if (gm.getAction() == Action.SECOND_MOVE)
            {
                Move m = new Move(null, gm.game.getGameController().getGameboard()[row, col], Gamecore.MoveAction.Move, gm.getGameCoreWorker2());
                gm.move2 = m;

                placeWorker(gm.getWorker2(), "2");
                gm.gameCorePlaceWorker(row, col, 2);
                GameObject.Find("Help Manager").GetComponent<HelpManager>().toggleHelpString();
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
    //public float[] pipeHeights = [-1, -2, -2.5, -3.75];
    public void undoPipeBuild()
    {

        int cpyPipeNum = pipeNum;

        pipeNum--;
        Destroy(curPipe);
        if(cpyPipeNum == 1)
        {
            pipe_cur_height -= 1;
            character_cur_height = transform.position.y + .5f;
        }
        else if (cpyPipeNum == 2)
        {
            //pipe_cur_height = transform.position.y + 1;
            pipe_cur_height -= 1.5f; //1.5
            pipeNum--;
            buildOnTile();
            //middle.y = pipe_cur_height;
            //curPipe = Instantiate(pipe_1, middle, Quaternion.Euler(new Vector3(90, 0, 0)));
            
            character_cur_height-= 3;
        }
        else if (cpyPipeNum == 3)
        {
            pipe_cur_height -= 1;
            //middle.y = pipe_cur_height;
            //curPipe = Instantiate(pipe_2, middle, Quaternion.Euler(new Vector3(90, 0, 0)));

            character_cur_height -= 1f;
            pipeNum--;
            buildOnTile();
        }
        else if (cpyPipeNum == 4)
        {
            //curPipe = Instantiate(pipe_3, middle, Quaternion.Euler(new Vector3(0, 0, 0)));
            character_cur_height -= 1;
            pipe_cur_height -= .5f;
            pipeNum--;
            buildOnTile();
        }
        //Pipe's size does not increase; do not increase curHeight
        else if (cpyPipeNum == 5)
        {
            pipeNum--;
            buildOnTile();
            //Vector3 v = middle;
            //v.y -= 1.25f;
            //curPipe = Instantiate(pipe_4, v, Quaternion.Euler(new Vector3(90, 180, 0)));
        }
        //curPipe.transform.SetParent(this.gameObject.transform);
        
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
            pipe_cur_height += .5f;
            character_cur_height += 1f;
        }
        else if (pipeNum == 3)
        {
            pipe_cur_height += .5f;
            curPipe = Instantiate(pipe_3, middle, Quaternion.Euler(new Vector3(0, 0, 0)));
            character_cur_height += .5f;
        }
        //Pipe's size does not increase; do not increase curHeight
        else if (pipeNum == 4)
        {
            Vector3 v = middle;
            v.y += .75f;
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
        AudioManager.playCharacterRandom(worker.tag);
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
        bool sel = GameObject.Find("Help") == null  && GameObject.Find("Disconnect") == null && GameObject.Find("SettingsPopUp") == null && (GameObject.Find("Server").GetComponent<NetworkServer>().connected || !gm.game.netWorkGame);
        //Debug.Log("Connected: " + GameObject.Find("Server").GetComponent<NetworkServer>().connected);
        //Debug.Log(sel);
        return sel;
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
        foreach (Renderer ri in GetComponentsInChildren<Renderer>())
        {
            ri.material.color = _selected;
        }
    }

    void OnMouseExit()
    {
        if (isSelected) return;

        m_Material.color = highlighted ? _highlited : unSelected;
        foreach (Renderer ri in GetComponentsInChildren<Renderer>())
        {
            ri.material.color = highlighted ? _highlited : unSelected;
        }
    }
    bool highlighted = false;
    public void highlight()
    {
        highlighted = true;
        m_Material.color = _highlited;
        foreach (Renderer ri in GetComponentsInChildren<Renderer>())
        {
            ri.material.color = _highlited;
        }
    }
    public void unhighlight()
    {
        highlighted = false;
        m_Material.color = unSelected;
        foreach (Renderer ri in GetComponentsInChildren<Renderer>())
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
