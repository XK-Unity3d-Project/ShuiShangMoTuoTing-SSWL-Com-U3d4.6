using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO.Ports;
using System.Text;
using System.Runtime.InteropServices;

public class MyCOMDevice : MonoBehaviour
{
    public static void output_array(String notes, byte[] arr)
    {
        String DbgMsg = notes + " - " + arr.Length + " : ";

        for (int i = 0; i < arr.Length; i++) {
            DbgMsg += String.Format("{0:X2} ", arr[i]);
        }

        DbgMsg += " --- ";

        Debug.Log(DbgMsg);
    }

    public class GkioPort
    {
        [DllImport("gkio", EntryPoint = "gkio_open", CallingConvention = CallingConvention.Cdecl)]
        extern static public int _gkio_open();

        static int gkio_open() {
            Debug.Log("gkio_open() called.");
            return _gkio_open();
        }

        [DllImport("gkio", EntryPoint = "gkio_read", CallingConvention = CallingConvention.Cdecl)]
        extern static public int _gkio_read(byte[] read_buf, int read_buf_size);

        static public int gkio_read(byte[] read_buf, int read_buf_size)
        {
            int ret;

            ret = _gkio_read(read_buf, read_buf_size);
            output_array("_gkio_read()", read_buf);

            return ret;
        }

        [DllImport("gkio", EntryPoint = "gkio_write", CallingConvention = CallingConvention.Cdecl)]
        extern static public int _gkio_write(byte[] write_buf, int write_buf_size);

        static int gkio_write(byte[] write_buf, int write_buf_size)
        {
            int ret;

            output_array("_gkio_write()", write_buf);
            ret = _gkio_write(write_buf, write_buf_size);

            return ret;
        }

        [DllImport("gkio", EntryPoint = "gkio_close", CallingConvention = CallingConvention.Cdecl)]
        extern static public void _gkio_close();

        static void gkio_close()
        {
            Debug.Log("gkio_close() called.");
            _gkio_close();
        }

        //public GkioPort() { IsOpen_ = false; }

        static private bool IsOpen_ = false;
        static public bool IsOpen { get { return IsOpen_; } }

        static public void Open()
        {
            // 返回找到的 GKIO 板数量。目前没找到返回 0，找到 1 个或多个，都返回 1。只支持使用第一块板
            int num_of_gkio = gkio_open();

            IsOpen_ = (num_of_gkio != 0);
        }

        static public void Close()
        {
            if (IsOpen_) {
                gkio_close();
            }
        }

        static private byte[] wbuf = new byte[16];
        static private byte[] rbuf = new byte[16];

        // 把摩托艇程序写入的 buffer 数据包，转换成 GKIO 识别的包格式 wbuf
        static void EncodeToGkio(byte[] buffer, byte[] wbuf)
        {

        }

        static void SubCoin(byte numToSub)
        {
            if (numToSub <= 0) {
                Debug.Log("SubCoin() called but num is O so just return.");
                return;
            }

            byte[] CMD_SUB_COIN = {  // 减币指令，币数待填写
                0x61, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            CMD_SUB_COIN[3] = numToSub;

            gkio_write(CMD_SUB_COIN, CMD_SUB_COIN.Length);

            // 返回的 rbuf[0] 应该是 0x82
            gkio_read(rbuf, rbuf.Length);
        }

        static void SetWheel(byte wheelStat)
        {
            /* 暂时禁止
            byte[] CMD_SET_WHEEL = {
                0x21, 0x02, 0x00, 0x00,  // 减少 1 个币
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            switch (wheelStat) {
                case 0x00: // 关闭方向盘力反馈功能
                    break;                
                case 0x55: // 振动方向盘
                    break;
                case 0xaa: // 打开方向盘力反馈功能
                    break;
            }

            gkio_write(CMD_SET_WHEEL, CMD_SET_WHEEL.Length);

            // 返回的 rbuf[0] 应该是 0x21
            gkio_read(rbuf, rbuf.Length);
            */
        }

        static public void Write(byte[] buf, int offset, int count)
        {
            // buffer[2 - 3] 是减币
            if (buf[2] == 0xaa) {
                SubCoin(buf[3]);
            }

            // buf[4] 是气囊和开始灯

            // buf[6] 是方向盘振动和力
            SetWheel(buf[6]);

            // buf[7] 是尾灯
        }

        static void FillPktHeadAndTail(byte[] buf)
        {
            // 指定的包头
            buf[0] = 0x01;
            buf[1] = 0x55;

            // 指定的包尾
            buf[23] = 0x41;
            buf[24] = 0x42;
            buf[25] = 0x43;
            buf[26] = 0x44;
        }

        // 把读到的 GKIO 包 gkio[] 转换成摩托艇程序能识别的格式，放到 buf[] 中
        static void DecodeGkioToKnuIo(byte[] gkio, byte[] speedboat)
        {
            Array.Clear(speedboat, 0, speedboat.Length);

            // 填充包头包尾
            FillPktHeadAndTail(speedboat);

            // GKIO 当前的 coin 数量，直接转换到摩托艇需要的格式
            {
                byte gkioCoin = gkio[1];
                speedboat[8] = gkioCoin;
            }

            // 转换 GKIO 的 4 - 7 字节都可以定义为按钮。我们选用 gkio[4]，并转换到摩托艇需要的格式
            // 各 bit 为 1 表示按下，为 0 表示弹起
            {
                byte gkioButton = 0x00;

                // 注意 GKIO 是 1 表示未按，0 表示按下，所以要 ~ 取反
                // 第 4 个字节，玩家可用按钮，1 - 测试, 3 - 下移, 4 - 投币
                byte tmpButton = (byte)(~gkio[4]);

                const byte MASK_TEST = 0x02;
                if ((byte)(tmpButton & MASK_TEST) == MASK_TEST) {
                    // gkio 的 bit 1 对应 knuio 的 bit 5
                    gkioButton |= 0x10;
                }

                const byte MASK_DOWN = 0x04;
                if ((byte)(tmpButton & MASK_DOWN) == MASK_DOWN) {
                    // gkio 的 bit 1 对应 knuio 的 bit 5
                    gkioButton |= 0x10;
                }

                // 投币是由固件直接转换成币数，放在第 1 个字节

                tmpButton = (byte)(~gkio[5]);
                const byte MASK_SPEAKER = 0x20;
                if ((byte)(tmpButton & MASK_SPEAKER) == MASK_SPEAKER) {
                    // gkio 的 bit 1 对应 knuio 的 bit 5
                    gkioButton |= 0x04;
                }

                tmpButton = (byte)(~gkio[5]);
                const byte MASK_START = 0x40;
                if ((byte)(tmpButton & MASK_START) == MASK_START) {
                    // gkio 的 bit 1 对应 knuio 的 bit 5
                    gkioButton |= 0x01;
                }

                // 合成为 1 个 byte，放到摩托艇的 button 状态字节
                //speedboat[9] = (byte)(gkioPlayerBtn | gkioServiceBtn);
                speedboat[9] = (byte)((~gkio[7]) | gkioButton);

                // GKIO : 80 00 00 00 FD FF FF FF 03 03 03 03 00 00 00 00
                // 解析出的 speedboat[9] : 2, gkioPlayerBtn : 2, gkioServiceBtn : 0
                /*Debug.Log("speedboat[9] : " + speedboat[9] + 
                    ", gkioPlayerBtn : " + gkioPlayerBtn +
                    ", gkioServiceBtn : " + gkioServiceBtn); */

                // Debug.Log("speedboat[9] : " + speedboat[9]);
            }

            // GKIO 读出的 3 个 8 bit 电位器数据 byte[8] byte[9] byte[10]，要转换成摩托艇的
            // 4 bit 高位 +8 bit 低位的 12 位格式。为了扩大精度，最后 4 bit 取 0
            {
                byte wheel = gkio[8];
                speedboat[6] = (byte)(wheel >> 4); // 方向盘
                speedboat[7] = (byte)(wheel << 4);

                byte thrust = gkio[9];
                speedboat[2] = (byte)(thrust >> 4);  // 油门
                speedboat[3] = (byte)(thrust << 4);

                /*
                byte breaker = gkio[9];
                speedboat[4] = (byte)(breaker >> 4);  // 刹车
                speedboat[5] = (byte)(breaker << 4);
                */                
            }

            // 偶数表示 KnuIo 板正常，奇数表示不正常
            speedboat[22] = 0x02;

            // 校验 11 - 14 字节
            {
                byte tmpXorChecksum = 0x00;

                tmpXorChecksum ^= speedboat[11];
                tmpXorChecksum ^= speedboat[12];
                tmpXorChecksum ^= speedboat[13];

                speedboat[10] = tmpXorChecksum;
            }

            // 校验 15 - 17 字节
            {
                byte tmpXorChecksum = 0x00;

                tmpXorChecksum ^= speedboat[15];
                tmpXorChecksum ^= speedboat[16];
                tmpXorChecksum ^= speedboat[17];

                speedboat[14] = tmpXorChecksum;
            }

            // 总校验
            {
                byte tmpXorChecksum = 0x00;
                for (int idx = 2; idx < speedboat.Length - 4; idx++) {
                    if (idx == 8 || idx == 21) {
                        continue;
                    }
                    tmpXorChecksum ^= speedboat[idx];
                }

                // 0x41 和 0x42 没什么特别，协议就是这么规定的
                tmpXorChecksum ^= 0x41; // EndRead_1;
                tmpXorChecksum ^= 0x42; // EndRead_2;

                speedboat[21] = tmpXorChecksum;
            }
        }

        static private byte[] gkio_read_buf = new byte[16];
        static public void Read(byte[] buf, int offset, int count)
        {
            // 最后读取一次 GKIO 状态
            {
                byte[] CMD_GET_STAT = {
                    0x61, 0x8A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };
                gkio_write(CMD_GET_STAT, CMD_GET_STAT.Length);
                gkio_read(gkio_read_buf, gkio_read_buf.Length);
            }

            if (buf.Length != 27) {
                return;
            }

            // 80 38 38 38 FF FF FF FF 03 03 03 03 00 00 00 00
            //output_array("READ() gkio buf : ", gkio_read_buf);

            DecodeGkioToKnuIo(gkio_read_buf, buf);

            // 01 55, 包头
            // 00 30 00 30 00 30, 三个电位器, 03 变 30，正确
            // 38, 币数
            // 00, 按钮状态，正常
            // 00 00 00 00, byte[10] 校验和
            // 00 00 00 00, byte[14] 校验和
            // 00 00 00, 随机值
            // 32, byte[21] 校验和，应该为 31
            // 02, 必须为偶数
            // 41 42 43 44 包尾
            //output_array("READ() KnuIo buf : ", buf);
        }
    }

	public class ComThreadClass
	{
		public string ThreadName;
        //static SerialPort _SerialPort;
        static private GkioPort _SerialPort;
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

            //_SerialPort = new SerialPort(ComPortName, 38400, Parity.None, 8, StopBits.One);
            _SerialPort = new GkioPort();
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
                _SerialPort.Read(ReadByteMsg, 0, ReadByteMsg.Length);
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

        /* czq
		if (ReadMsgTimeOutVal > timeMinVal) {
			ScreenLog.Log("CheckTimeOutReadMsg -> The app should restart to open the COM!");
			ComThreadClass.IsReadMsgComTimeOut = true;
			RestartComPort();
		}
        */
	}

	// 强制重启串口通讯,目的是清理串口缓存信息.
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