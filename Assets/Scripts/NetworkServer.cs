using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class NetworkServer : MonoBehaviour
{
	//Flag
	bool moved;
	PhotonView pv;
	Tuple<Move, Move> moves;

	public void sendMoves(Tuple<Move, Move> moves)
	{
		pv.RPC("acceptMove", RpcTarget.Others, moves);
	}

	public void Start()
	{
		if (gameObject.GetComponent<PhotonView>() == null)
		{
			pv = gameObject.AddComponent<PhotonView>();
			pv.ViewID = 1;
		}
	}
	//Event subscriber that sets the flag
	[PunRPC]
	public void acceptMove(Tuple<Move, Move> m)
	{
		moved = true;
		moves = m;
	}

	bool checkHappened()
	{
		return moved;
	}
	//Coroutine that waits until the flag is set
	IEnumerator WaitForEvent()
	{

		yield return new WaitUntil(checkHappened);
		moved = false;
	}

	//Main coroutine, that stops and waits somewhere within it's execution
	public IEnumerator get()
	{
		//Other stuff...
		yield return StartCoroutine(WaitForEvent());
		//Oher stuff...
	}

	public Tuple<Move, Move> getMoves()
	{
		return moves;
	}
}
