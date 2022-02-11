using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class NetworkServer : MonoBehaviour
{
	//Flag
	bool moved;
	Tuple<Move, Move> moves;
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
