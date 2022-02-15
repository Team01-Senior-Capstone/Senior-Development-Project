using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

public class Network : Opponent
{
	NetworkServer ns;
	bool host;
	string roomName;
	GameObject server;

	public Network(string roomName, bool h)
	{

		this.roomName = roomName;
		host = h;
		Debug.Log(h);
		server = new GameObject("Server");
		GameObject.DontDestroyOnLoad(server.gameObject);
		server.AddComponent<NetworkServer>();
		ns = server.GetComponent<NetworkServer>();
		ns.host = this.host;
		ns.roomName = this.roomName;
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

	public override Tuple<string, string> GetWorkerTags()
	{
		return new Tuple<string, string>(ns._tag1, ns._tag2);
	}
	public override bool GetReady()
	{
		return ns.ready;
	}

	public override void SendReady(bool r)
	{
		ns.sendReady(r);
	}
	public override void SendWorkerPlacements(Tuple<Move, Move> m)
	{
		ns.sendMoves(m);
	}
	public override void SendWorkerTags(string s1, string s2)
	{
		ns.sendTags(s1, s2);
	}

	public override void SendMoves(Tuple<Move, Move> moves)
	{
		ns.sendMoves(moves);
	}


}
