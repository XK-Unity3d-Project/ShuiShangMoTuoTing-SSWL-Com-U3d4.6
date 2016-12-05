using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO.Ports;
using System.Text;
using System.Runtime.InteropServices;

public class MyCOMDevice : MonoBehaviour
{
	public class ComThreadClass
	{
		public string ThreadName;
		static SerialPort _SerialPort;
		public static int BufLenRead = 27;
		public static int BufLenReadEnd = 4;
		public static  int BufLenWrite = 23;
		public static byte[] ReadByteMsg = new byte[BufLenRead];
		public static byte[] WriteByteMsg = new byte[BufLenWrite];
		static string RxStringData;
		static string _NewLine = "ABCD"; //0x41 0x42 0x43 0x44
		public static int ReadTimeout = 0x0050; //单位为毫秒.
		public static int WriteTimeout = 0x07d0;
		public static bool IsStopComTX;
		public static bool IsReadMsgComTimeOut;
		public static string ComPortName = "COM1";
		public static bool IsReadComMsg;
		public static bool IsTestWRPer;
		public static int WriteCount;
		public static int ReadCount;
		public static int ReadTimeOutCount;
		public ComThreadClass(string name)
		{
			ThreadName = name;
			OpenComPort();
		}

		public static void OpenComPort()
		{
			if (!pcvr.bIsHardWare) {
				return;
			}

			if (_SerialPort != null) {
				return;
			}

			_SerialPort = new SerialPort(ComPortName, 38400, Parity.None, 8, StopBits.One);
			_SerialPort.NewLine = _NewLine;
			_SerialPort.Encoding = Encoding.GetEncoding("iso-8859-1");
			_SerialPort.ReadTimeout = ReadTimeout;
			_SerialPort.WriteTimeout = WriteTimeout;
			if (_SerialPort != null)
			{
				try
				{
					if (_SerialPort.IsOpen)
					{
						_SerialPort.Close();
						Debug.Log("Closing port, because it was already open!");
					}
					else
					{
						_SerialPort.Open();
						if (_SerialPort.IsOpen) {
							IsFindDeviceDt = true;
							Debug.Log("COM open sucess");
						}
					}
				}
				catch (Exception exception)
				{
					if (XkGameCtrl.IsGameOnQuit || ComThread == null) {
						return;
					}
					Debug.Log("error:COM already opened by other PRG... " + exception);
				}
			}
			else
			{
				Debug.Log("Port == null");
			}
		}

		public void Run()
		{
			do
			{
				if (XkGameCtrl.IsLoadingLevel) {
					Thread.Sleep(100);
					continue;
				}

				IsTestWRPer = false;
				if (IsReadMsgComTimeOut) {
					CloseComPort();
					break;
				}

				if (IsStopComTX) {
					IsReadComMsg = false;
					Thread.Sleep(1000);
					continue;
				}

				COMTxData();
				if (pcvr.IsJiaoYanHid || !pcvr.IsPlayerActivePcvr) {
					Thread.Sleep(100);
				}
				else {
					Thread.Sleep(25);
				}

				COMRxData();
				if (pcvr.IsJiaoYanHid || !pcvr.IsPlayerActivePcvr) {
					Thread.Sleep(100);
				}
				else {
					Thread.Sleep(25);
				}
				IsTestWRPer = true;
			}
			while (_SerialPort.IsOpen);
			CloseComPort();
			Debug.Log("Close run thead...");
		}

		void COMTxData()
		{
			if (XkGameCtrl.IsGameOnQuit) {
				return;
			}

			try
			{
				IsReadComMsg = false;
				_SerialPort.Write(WriteByteMsg, 0, WriteByteMsg.Length);
				_SerialPort.DiscardOutBuffer();
				WriteCount++;
			}
			catch (Exception exception)
			{
				if (XkGameCtrl.IsGameOnQuit || ComThread == null) {
					return;
				}
				Debug.Log("Tx error:COM!!! " + exception);
			}
		}

		void COMRxData()
		{
			if (XkGameCtrl.IsGameOnQuit) {
				return;
			}

			try
			{
				RxStringData = _SerialPort.ReadLine();
				ReadByteMsg = _SerialPort.Encoding.GetBytes(RxStringData);
				_SerialPort.DiscardInBuffer();
				ReadCount++;
				IsReadComMsg = true;
				ReadMsgTimeOutVal = 0f;
				CountOpenCom = 0;
			}
			catch (Exception exception)
			{
				if (XkGameCtrl.IsGameOnQuit || ComThread == null) {
					return;
				}

				Debug.Log("Rx error:COM..." + exception);
				IsReadMsgComTimeOut = true;
				IsReadComMsg = false;
				ReadTimeOutCount++;
			}
		}

		public static void CloseComPort()
		{
			IsReadComMsg = false;
			if (_SerialPort == null || !_SerialPort.IsOpen) {
				return;
			}
			_SerialPort.DiscardOutBuffer();
			_SerialPort.DiscardInBuffer();
			_SerialPort.Close();
			_SerialPort = null;
		}
	}

	static ComThreadClass _ComThreadClass;
	static Thread ComThread;
	public static bool IsFindDeviceDt;
	public static float ReadMsgTimeOutVal;
	static float TimeLastVal;
	const float TimeUnitDelta = 0.1f; //单位为秒.
	public static uint CountRestartCom;
	public static uint CountOpenCom;
	static MyCOMDevice _Instance;
	public static MyCOMDevice GetInstance()
	{
		if (_Instance == null) {
			GameObject obj = new GameObject("_MyCOMDevice");
			DontDestroyOnLoad(obj);
			_Instance = obj.AddComponent<MyCOMDevice>();
		}
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		StartCoroutine(OpenComThread());
	}

	IEnumerator OpenComThread()
	{
		if (!pcvr.bIsHardWare) {
			yield break;
		}

		ReadMsgTimeOutVal = 0f;
		ComThreadClass.IsReadMsgComTimeOut = false;
		ComThreadClass.IsReadComMsg = false;
		ComThreadClass.IsStopComTX = false;
		if (_ComThreadClass == null) {
			_ComThreadClass = new ComThreadClass(ComThreadClass.ComPortName);
		}
		else {
			ComThreadClass.CloseComPort();
		}
		
		if (ComThread != null) {
			CloseComThread();
		}
		yield return new WaitForSeconds(2f);

		ComThreadClass.OpenComPort();
		if (ComThread == null) {
			ComThread = new Thread(new ThreadStart(_ComThreadClass.Run));
			ComThread.Start();
		}
	}

	void RestartComPort()
	{
		if (!ComThreadClass.IsReadMsgComTimeOut) {
			return;
		}
		CountRestartCom++;
		CountOpenCom++;
		ScreenLog.Log("Restart ComPort "+ComThreadClass.ComPortName+", time "+(int)Time.realtimeSinceStartup);
		ScreenLog.Log("CountRestartCom: "+CountRestartCom);
		StartCoroutine(OpenComThread());
	}

	void CheckTimeOutReadMsg()
	{
		ReadMsgTimeOutVal += TimeUnitDelta;
		float timeMinVal = CountOpenCom < 6 ? 0.5f : 4f;
		if (CountOpenCom > 20) {
			timeMinVal = 10f;
		}

		if (ReadMsgTimeOutVal > timeMinVal) {
			ScreenLog.Log("CheckTimeOutReadMsg -> The app should restart to open the COM!");
			ComThreadClass.IsReadMsgComTimeOut = true;
			RestartComPort();
		}
	}

	/**
	 * 强制重启串口通讯,目的是清理串口缓存信息.
	 */
	public void ForceRestartComPort()
	{
		if (!pcvr.bIsHardWare) {
			return;
		}
		ComThreadClass.IsReadMsgComTimeOut = true;
		RestartComPort();
	}

	void Update()
	{
		//test...
//		if (Input.GetKeyUp(KeyCode.T)) {
//			ForceRestartComPort();
//		}
//		if (Input.GetKeyUp(KeyCode.T)) {
//			XkGameCtrl.IsLoadingLevel = !XkGameCtrl.IsLoadingLevel;
//		}
		//test end...
		
		if (!pcvr.bIsHardWare || XkGameCtrl.IsLoadingLevel || ComThreadClass.IsReadComMsg) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeLastVal < TimeUnitDelta) {
			return;
		}
		TimeLastVal = Time.realtimeSinceStartup;
		CheckTimeOutReadMsg();
	}

//	void OnGUI()
//	{
//		string strA = "IsReadComMsg "+ComThreadClass.IsReadComMsg
//			+", ReadMsgTimeOutVal "+ReadMsgTimeOutVal.ToString("f2");
//		GUI.Box(new Rect(0f, 0f, 400f, 25f), strA);
//	}

	void CloseComThread()
	{
		if (ComThread != null) {
			ComThread.Abort();
			ComThread = null;
		}
	}

	void OnApplicationQuit()
	{
		Debug.Log("OnApplicationQuit...Com");
		XkGameCtrl.IsGameOnQuit = true;
		ComThreadClass.CloseComPort();
		Invoke("CloseComThread", 2f);
	}
}