using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

using System.IO;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class scriptSendData : MonoBehaviour {

	public static bool bClients = false;
	private static int hostIdMaster;
	private static int iConnectionIdCounter = -1;

	public static void ConnectToClients()
	{
		byte error;
		if (!bClients) {
			bClients = true;
			GlobalConfig gconfig = new GlobalConfig ();
			gconfig.ReactorModel = ReactorModel.FixRateReactor;
			gconfig.ThreadAwakeTimeout = 1;
			ConnectionConfig config = new ConnectionConfig ();
			config.AddChannel (QosType.ReliableSequenced);
			HostTopology topology = new HostTopology (config, 10);
			NetworkTransport.Init (gconfig);
			hostIdMaster = NetworkTransport.AddHost (topology, 8888);

			iConnectionIdCounter = NetworkTransport.Connect (hostIdMaster, "192.168.1.17", 8888, 0, out error);
			if ((NetworkError)error != NetworkError.Ok)
				UnityEngine.Debug.LogError ("Error Counter : " + (NetworkError)error);
		}
	}

	public static void SendSocketMessage(string sMessage) {
		byte error;
		int recHostId;
		int recConnectionId;
		int recChannelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;

		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream(buffer);
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, sMessage);

		NetworkEventType recNetworkEventLeft = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
		NetworkTransport.Send(hostIdMaster, iConnectionIdCounter, 0, buffer, bufferSize, out error);
		if ((NetworkError) error != NetworkError.Ok)
			UnityEngine.Debug.LogError("Error Counter : " + (NetworkError) error);
	}
}