using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class NetworkServer : MonoBehaviour
{
	//Flag
	bool moved;
	public string _tag1;
	public string _tag2;


	PhotonView pv;
	Tuple<Move, Move> moves;

	public void sendMoves(Tuple<Move, Move> moves)
	{
		pv.RPC("acceptMove", RpcTarget.Others, moves);
	}

	public void sendTags(string t1, string t2)
	{
		pv.RPC("acceptTags", RpcTarget.Others, t1, t2);
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

	[PunRPC]
	public void acceptTags(string tag1, string tag2)
	{
		_tag1 = tag1;
		_tag2 = tag2;
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
