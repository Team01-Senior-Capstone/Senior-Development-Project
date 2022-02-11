using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Network : Opponent
{
	string roomName;
	bool host;
	NetworkServer ns;

	public Network(string roomName, bool h)
	{
		if(host)
		{
			//open Room
		}
		else
		{
			//join the room
		}
	}

	public override Tuple<Move, Move> GetMove(Gamecore.GameController gc) {
		ns.get();
		return ns.getMoves();
	}

	public override Tuple<Move, Move> GetWorkerPlacements(Gamecore.GameController gc)
	{
		ns.get();
		return ns.getMoves();
	}

	public void sendMoves(Tuple<Move, Move> moves)
	{
		this.photonView.RPC("acceptMove", PhotonTargets.All, moves);
	}

}
