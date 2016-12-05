using UnityEngine;
using System.Collections;
using System.Threading;

public class TestComPort : MonoBehaviour {
	static bool IsFireP1;
	static int TimeVal;
	static string TestReadMsg = "";	
//	static string TestReadMsgA = "1PFire";	
	static string TestReadMsgB = "";	
	static string TestReadMsgC = "";	
	static string TestReadMsgD = "";	
	static string TestReadMsgE = "";
	static TestComPort _Instance;
	public static TestComPort GetInstance()
	{
		if (_Instance == null) {
			GameObject obj = new GameObject("_TestComPort");
			//DontDestroyOnLoad(obj);
			_Instance = obj.AddComponent<TestComPort>();
		}
		return _Instance;
	}

	void Awake()
	{
		if (_Instance != null) {
			Destroy(gameObject);
			return;
		}
		_Instance = this;
		//DontDestroyOnLoad(gameObject);
	}

	void OnGUI()
	{
		TestReadMsg = "";
		for (int i = 0; i < MyCOMDevice.ComThreadClass.ReadByteMsg.Length; i++) {
			TestReadMsg += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
		}

		if (MyCOMDevice.ComThreadClass.ReadByteMsg.Length < (MyCOMDevice.ComThreadClass.BufLenRead - MyCOMDevice.ComThreadClass.BufLenReadEnd)) {
//			Debug.Log("ReadBufLen: "+MyCOMDevice.ComThreadClass.ReadByteMsg.Length);
//			Debug.LogError("ReadMsgError: msg -> "+TestReadMsg);
			return;
		}

//		if ((MyCOMDevice.ComThreadClass.ReadByteMsg[28]&0x08) == 0x08 && !IsFireP1) {
//			IsFireP1 = true;
//			TestReadMsgA = "1PFire Down, byte28 -> " + MyCOMDevice.ComThreadClass.ReadByteMsg[28];
//		}
//		else if ((MyCOMDevice.ComThreadClass.ReadByteMsg[28]&0x08) == 0x00 && IsFireP1) {
//			IsFireP1 = false;
//			TestReadMsgA = "1PFire Up, byte28 -> " + MyCOMDevice.ComThreadClass.ReadByteMsg[28];
//		}

		float wrPer = MyCOMDevice.ComThreadClass.ReadCount != 0 ? (float)MyCOMDevice.ComThreadClass.WriteCount / MyCOMDevice.ComThreadClass.ReadCount : (float)MyCOMDevice.ComThreadClass.WriteCount;
		TestReadMsgB = "WriteBytes: " + MyCOMDevice.ComThreadClass.WriteCount;
		TestReadMsgC = "ReadBytes: " + MyCOMDevice.ComThreadClass.ReadCount;
		if (MyCOMDevice.ComThreadClass.IsTestWRPer) {
			TestReadMsgD = "W/R: " + wrPer;
		}

		if (MyCOMDevice.ComThreadClass.IsReadComMsg) {
			TimeVal = (int)Time.realtimeSinceStartup;
		}
		TestReadMsgE = MyCOMDevice.ComThreadClass.ComPortName + " -> time: "+TimeVal.ToString();
		
		GUI.Box(new Rect(0f, 10f, Screen.width, 30f), TestReadMsg);

//		TestReadMsgA = "IsReadComMsg "+MyCOMDevice.ComThreadClass.IsReadComMsg+", IsLoadingLevel"+MyCOMDevice.ComThreadClass.IsLoadingLevel;
//		GUI.Box(new Rect(0f, 40f, Screen.width, 30f), TestReadMsgA);
		GUI.Box(new Rect(0f, 70f, Screen.width, 30f), TestReadMsgB);
		GUI.Box(new Rect(0f, 100f, Screen.width, 30f), TestReadMsgC);
		GUI.Box(new Rect(0f, 130f, Screen.width, 30f), TestReadMsgD);
		GUI.Box(new Rect(0f, 160f, Screen.width, 30f), TestReadMsgE);
		
		if ((MyCOMDevice.ReadMsgTimeOutVal * 1000) >= MyCOMDevice.ComThreadClass.ReadTimeout) {
			int val = (int)MyCOMDevice.ReadMsgTimeOutVal;
			GUI.Box(new Rect(0f, 190f, Screen.width, 30f), "ReadMsgTimeOutVal: "+val
			        +", CountRestartCom "+MyCOMDevice.CountRestartCom);
		}
	}
	
	void OnApplicationQuit()
	{
		Debug.Log("Test***OnApplicationQuit...");
		//Debug.Log("TestReadMsgA: " + TestReadMsgA);
//		Debug.Log("TestReadMsgB: " + TestReadMsgB);
//		Debug.Log("TestReadMsgC: " + TestReadMsgC);
//		Debug.Log("TestReadMsgD: " + TestReadMsgD);
		Debug.Log("TestReadMsgE: " + TestReadMsgE);
	}
}