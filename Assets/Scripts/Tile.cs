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
        Debug.Log("Placed " + worker + " on " + this.name);
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

                    Gamecore.TileBuildInfo b = gm.game.gameController.workerBuild(workerFunc(), gm.getMe(),
                                 gm.selectedWorker_tile.GetComponent<Tile>().row,
                                 gm.selectedWorker_tile.GetComponent<Tile>().col,
                                 row, col);

                    Debug.Log("Build successful for me? " + b.wasBuildSuccessful());

                    int fromTileRow = gm.selectedWorker_tile.GetComponent<Tile>().row;
                    int fromTileCol = gm.selectedWorker_tile.GetComponent<Tile>().col;
                    Move m = new Move(gm.game.gameController.getGameboard()[fromTileRow, fromTileCol], gm.game.gameController.getGameboard()[row, col], Gamecore.MoveAction.Build, workerFunc());
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
                        Gamecore.WorkerMoveInfo workMove = gm.game.gameController.movePlayer(workerFunc(), gm.getMe(),
                                                 gm.selectedWorker_tile.GetComponent<Tile>().row,
                                                 gm.selectedWorker_tile.GetComponent<Tile>().col,
                                                 row, col);
                        //Debug.Log(workMove.wasMoveSuccessful());
                        int fromTileRow = gm.selectedWorker_tile.GetComponent<Tile>().row;
                        int fromTileCol = gm.selectedWorker_tile.GetComponent<Tile>().col;
                        Move m = new Move(gm.game.gameController.getGameboard()[fromTileRow, fromTileCol], gm.game.gameController.getGameboard()[row, col], Gamecore.MoveAction.Move, workerFunc());
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
                    Move m = new Move(null, gm.game.gameController.getGameboard()[row, col], Gamecore.MoveAction.Move, gm.getGameCoreWorker1());
                    gm.move1 = m;
                    placeWorker(gm.getWorker1(), "1");
                    Debug.Log("Worker? " + worker);
                    gm.gameCorePlaceWorker(row, col, 1);
                    gm.toggleAction();

                }
                else if (gm.getAction() == Action.SECOND_MOVE)
                {
                    Move m = new Move(null, gm.game.gameController.getGameboard()[row, col], Gamecore.MoveAction.Move, gm.getGameCoreWorker2());
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
        foreach (Gamecore.Tile ti in gm.game.gameController.getOccupiedTiles())
        {
            Debug.Log(ti.getRow() + ", " + ti.getCol() + " is occupied");
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
            character_cur_height += 1;
        }
        //Pipe's size does not increase; do not increase curHeight
        else if (pipeNum == 4)
        {
            Vector3 v = middle;
            v.y += 1.25f;
            curPipe = Instantiate(pipe_4, v, Quaternion.Euler(new Vector3(90, 180, 0)));
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

        Quaternion q = worker.transform.rotation;

        Vector3 relativePos = getCharacterSpawn() - worker.transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        worker.transform.rotation = rotation;


        float speed = 7.5f;

        if (fromTile.pipeNum == 0 && pipeNum == 0)
        {
            //anim.SetTrigger("isMoving");
            //anim.SetBool("isMoving", true);
            anim.Play("Run");
            while (worker.transform.position != getCharacterSpawn())
            {

                worker.transform.position = Vector3.MoveTowards(worker.transform.position, getCharacterSpawn(), Time.deltaTime * speed);
                yield return null;
            }
            anim.Play("Wait");
        }
        else
        {
            // Debug.Log("Current pos: " + worker.transform.position);
            // for(int i = 0; i < 10; i++)
            // {
            //     worker.transform.position = SampleParabola(worker.transform.position, getCharacterSpawn(), 0f, .1f);
            //     Debug.Log("Current pos: " + worker.transform.position);
            // }
            //// Debug.Log(SampleParabola(worker.transform.position, getCharacterSpawn(), 2f, .5f));
            // Debug.Log("Target: " + getCharacterSpawn());

            int deg = 90;
            float counter = 0;

            if (fromTile.pipeNum == pipeNum)
            {
                deg = 180;
            }
            
            relativePos = middle - worker.transform.position;
            rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            worker.transform.rotation = rotation;
            int max = 0;
            Vector3 start = worker.transform.position;
            speed = 1;
            anim.Play("Jump_");
            while(worker.transform.position != getCharacterSpawn()) {
                //    Vector3 temp = Vector3.MoveTowards(worker.transform.position, getCharacterSpawn(), Time.deltaTime * speed);

                //    //push the character up more;
                //    if (getCharacterSpawn().y > worker.transform.position.y) {
                //        temp.y += .5f;
                //Debug.Log("Foo");

                // Still firing
                Vector3 pos = new Vector3(
                        worker.transform.position.x,
                        start.y + Mathf.Sin(Mathf.PI * 2 * counter / 360) ,
                        worker.transform.position.z
                 );
                pos.y += .75f;
                counter += speed;
                // Move the transform
                worker.transform.position = Vector3.Slerp(worker.transform.position, pos, 1f);
                // worker.transform.Translate(worker.transform.forward * speed);
                worker.transform.position += worker.transform.forward * Time.deltaTime * 8;
                yield return new WaitForSeconds(.003f);
                if(counter >= deg)
                {
                    break;
                }
            }
            //    worker.transform.position = temp;
            anim.Play("Wait");
            worker.transform.position = getCharacterSpawn();

            //    yield return null;
            Vector3 temp = getCharacterSpawn();
            //while (System.Math.Round(worker.transform.position.y, 4) != System.Math.Round(temp.y, 4) ||
            //       System.Math.Round(worker.transform.position.x, 4) != System.Math.Round(temp.x, 4) ||
            //       System.Math.Round(worker.transform.position.z, 4) != System.Math.Round(temp.z, 4))
            //{
            //    ////worker.transform.position = getCharacterSpawn();
            //    Vector3 center = (worker.transform.position + getCharacterSpawn()) * 0.5F;

            //    // move the center a bit downwards to make the arc vertical
            //    center -= new Vector3(0, 1, 0);

            //    // Interpolate over the arc relative to center
            //    Vector3 riseRelCenter = worker.transform.position - center;
            //    Vector3 setRelCenter = getCharacterSpawn() - center;

            //    // The fraction of the animation that has happened so far is
            //    // equal to the elapsed time divided by the desired time for
            //    // the total journey.
            //    float fracComplete = (Time.time - 1) / 1;
            //    worker.transform.position = Vector3.Slerp(worker.transform.position, getCharacterSpawn(), (Time.time - 1) / 1);
            //    worker.transform.position += center;

            //    //worker.transform.position = Vector3.MoveTowards(worker.transform.position, SampleParabola(worker.transform.position, getCharacterSpawn(), 0f, .01f), Time.deltaTime * .5f);
            //    Debug.Log("Current pos: " + worker.transform.position);

            //    max++;
            //    if (max >= 100000)
            //    {
            //        Debug.Log("Went to long");
            //        Debug.Log(worker.transform.position == getCharacterSpawn());
            //        Debug.Log("Target: " + getCharacterSpawn());
            //        Debug.Log("Y: " + (worker.transform.position.y != temp.y));
            //        Debug.Log("X: " + (worker.transform.position.x != temp.x));
            //        Debug.Log(temp.x);
            //        Debug.Log(worker.transform.position.x);
            //        Debug.Log("Z: " + (worker.transform.position.z != temp.z));
            //        worker.transform.position = getCharacterSpawn();
            //        break;
            //    }
            //}
        }
        worker.transform.rotation = q;
    }



    Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirection = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirection);
            //Vector3 up = Vector3.Cross(right, travelDirection);
            Vector3 up = Vector3.Cross(right, levelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }

    void removeSelectable()
    {
        selectable = false;
        m_Material.color = unSelected;
    }

    bool isSelected;
    public void removeSelect()
    {
        isSelected = false;
        m_Material.color = unSelected;
    }
    public void keepSelect()
    {
        isSelected = true;
        m_Material.color = _selected;
    }

    void OnMouseOver()
    {
        if (!selectable) return;
        m_Material.color = _selected;
        //foreach (Transform child in transform)
        //{
        //    // Change the Color of the GameObject when the mouse hovers over it
        //    //m_Material.color = _selected;
        //    child.GetComponent<Renderer>().material.color = _selected;
        //}
    }

    void OnMouseExit()
    {
        //Change the Color back to white when the mouse exits the GameObject
        if (isSelected) return;
        m_Material.color = unSelected;
        //foreach (Transform child in transform)
        //{
        //    // Change the Color of the GameObject when the mouse hovers over it
        //    //m_Material.color = _selected;
        //    child.GetComponent<Renderer>().material.color = unSelected;
        //}
    }

    void OnDestroy()
    {
        //Destroy the instance
        Destroy(m_Material);
    }
}
