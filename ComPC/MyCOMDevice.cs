using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO.Ports;
using System.Text;
using System.Runtime.InteropServices;

public static class gkioutil {
    public static void output_array(String notes, byte[] arr)
    {
        String DbgMsg = notes + " - " + arr.Length + " : ";

        for (int i = 0; i < arr.Length; i++) {
            DbgMsg += String.Format("{0:X2} ", arr[i]);
        }

        DbgMsg += " --- ";

        Debug.Log(DbgMsg);
    }
}

public class GkioPort
{
    [DllImport("gkio", EntryPoint = "gkio_open", CallingConvention = CallingConvention.Cdecl)]
    public static extern int gkio_open();

    [DllImport("gkio", EntryPoint = "gkio_read", CallingConvention = CallingConvention.Cdecl)]
    public static extern int gkio_read(byte[] read_buf, int read_buf_size);

    [DllImport("gkio", EntryPoint = "gkio_write", CallingConvention = CallingConvention.Cdecl)]
    public static extern int gkio_write(byte[] write_buf, int write_buf_size);

    [DllImport("gkio", EntryPoint = "gkio_close", CallingConvention = CallingConvention.Cdecl)]
    public static extern void gkio_close();

    public GkioPort() {
        IsOpen_ = false;
        Debug.Log("GkioPort() created.");
    }

    ~GkioPort()
    {
        Debug.Log("GkioPort() destroyed.");
    }

    private bool IsOpen_ = false;
    public bool IsOpen { get { return IsOpen_; } }

    static public int gkio_write_read(byte[] write_buf, byte[] read_buf)
    {
        int ret;

        ret = gkio_write(write_buf, write_buf.Length);

        if (ret >= 0) {
            ret = gkio_read(read_buf, read_buf.Length);
        }

        return ret;
    }

    public void Open()
    {
        // 返回找到的 GKIO 板数量。目前没找到返回 0，找到 1 个或多个，都返回 1。只支持使用第一块板
        int num_of_gkio = gkio_open();

        IsOpen_ = (num_of_gkio != 0);

        Debug.Log("Open() called and num_of_gkio is : " + num_of_gkio);
    }

    public void Close()
    {
        if (IsOpen_) {
            gkio_close();
        }
    }

    private static byte[] wbuf = new byte[16];
    private static byte[] rbuf = new byte[16];

    // 把摩托艇程序写入的 buffer 数据包，转换成 GKIO 识别的包格式 wbuf
    void EncodeToGkio(byte[] buffer, byte[] wbuf)
    {
        
    }

    // 扣币
    private void SubCoin(byte numToSub)
    {
        if (numToSub <= 0) {
            return;
        }

        byte[] CMD_SUB_COIN = {  // 减币指令，币数待填写
            0x61, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        CMD_SUB_COIN[3] = numToSub;

        // 返回的 rbuf[0] 应该是 0x82
        gkio_write_read(CMD_SUB_COIN, rbuf);
    }

    // 初始化方向盘的状态，回中位置等。一般方向盘的位置在 0xAA 附近
    public void InitWheel(uint knuWheelCenter)
    {
        byte[] CMD_SET_WHEEL = {  // 方向盘力反馈指令
                0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

        // 目前 Io 板上的 STM32 芯片的 ADC 硬件测量结果不可能大于 0xFFE
        const int MAX_ADC_RANGE = 0xFFE;
        if (knuWheelCenter > MAX_ADC_RANGE) {
            knuWheelCenter = MAX_ADC_RANGE;
        }

        // knuIo 板的方向盘中间位置，是 12 bit 的，Gkio 板只支持 8 bit 数据
        byte GkioCenter = (byte)(knuWheelCenter >> 4);
        // 方向盘作用边界
        CMD_SET_WHEEL[1] = (byte)(GkioCenter - 1);
        CMD_SET_WHEEL[2] = GkioCenter;

        // 特定模式下，还可以在 3 和 4 字节继续细分方向盘力道, 范围 0 到 0x0388
        CMD_SET_WHEEL[3] = 0x03;
        CMD_SET_WHEEL[4] = 0x64;

        // 设置为正常模式，力道强
        CMD_SET_WHEEL[5] = 0x03;

        // 返回的 rbuf[0] 应该是 0x21
        //int ret = gkio_write_read(CMD_SET_WHEEL, rbuf);

        int ret1 = gkio_write(CMD_SET_WHEEL, 16);
        int ret2 = gkio_read(rbuf, 16);

        Debug.Log("InitWheel() called. knuWheelCenter : " + knuWheelCenter +
            ", GkioCenter : " + GkioCenter);
        Debug.Log("CMD_SET_WHEEL[1] : " + CMD_SET_WHEEL[1] +
            ", CMD_SET_WHEEL[2] : " + CMD_SET_WHEEL[2] +
            ", ret1 : " + ret1 + ", ret2 : " + ret2);
    }

    // force 在 0 到 900 之间
    private static void setMoterPower(uint force)
    {
        byte[] CMD_WHEEL_FORCE = {  // 方向盘电机功率。0 为关闭，0x388 为全功率
            0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        if (force > 900) {
            force = 900;
        }

        CMD_WHEEL_FORCE[1] = (byte)(force / 256);
        CMD_WHEEL_FORCE[2] = (byte)(force % 256);

        // 返回的 rbuf[0] 应该是 0x28
        gkio_write_read(CMD_WHEEL_FORCE, rbuf);
    }

    private static void shakeWheel()
    {
        // 振动一下方向盘，碰到石头了什么的
        byte[] CMD_SHAKE_WHEEL = {
            0x24, 0x02, 0x00, 0x00, 0x05, 0x03, 0xE8, 0x0A,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        // 返回的 rbuf[0] 应该是 0x28
        gkio_write_read(CMD_SHAKE_WHEEL, rbuf);
    }

    private static void turnOnLed(byte idx)
    {
        byte[] CMD_LED_ON = {
            0x61, 0x81, 0x00, 0xff, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        if (idx < 0 || idx > 10) {
            idx = 0;
        }

        CMD_LED_ON[2] = idx;

        // 返回的 rbuf[0] 应该是 0x81
        gkio_write_read(CMD_LED_ON, rbuf);
    }

    private static void turnOffLed(byte idx)
    {
        byte[] CMD_LED_OFF = {
            0x61, 0x81, 0x00, 0xff, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        if (idx < 0 || idx > 10) {
            idx = 0;
        }

        CMD_LED_OFF[2] = idx;

        // 返回的 rbuf[0] 应该是 0x81
        gkio_write_read(CMD_LED_OFF, rbuf);
    }

    enum GASCELL_STAT { ON = 0, OFF = 1 };
    private static void setGascell(byte gascell)
    {
        if ((gascell & 0x01) > 0) {
            turnOnLed(1);
        }
        else {
            turnOffLed(1);
        }

        if ((gascell & 0x02) > 0) {
            turnOnLed(2);
        }
        else {
            turnOffLed(2);
        }

        if ((gascell & 0x04) > 0) {
            turnOnLed(3);
        }
        else {
            turnOffLed(3);
        }

        if ((gascell & 0x08) > 0) {
            turnOnLed(4);
        }
        else {
            turnOffLed(4);
        }

        if ((gascell & 0x40) > 0) {
            turnOnLed(5);
        }
        else {
            turnOffLed(5);
        }
    }

    private void setLight(byte light)
    {
        switch (light) {
            case 0x00:
                turnOffLed(0);
                break;
            case 0x55:
            case 0xaa:
                turnOnLed(0);
                break;
        }
    }

    // KnuIo 板数据包
    public void Write(byte[] knuPkt, int offset, int count)
    {
        // knuPkt[2 - 3] 是减币
        if (knuPkt[2] == 0xaa) {
            SubCoin(knuPkt[3]);
        }

        InitWheel(pcvr.SteerValCen);

        /*
        // knuPkt[4] 是气囊和开始灯
        setGascell(knuPkt[4]);

        // knuPkt[6] 是方向盘振动和力
        const uint MOTOR_POWER_OFF = 0;
        const uint MOTOR_FULL_POWER = 900;

        switch (knuPkt[6]) {
            case 0x00:  // Demo 状态下，关闭电机电源，方向盘不自动回中
                setMoterPower(MOTOR_POWER_OFF);
                break;
            case 0x55:  // 振动一次
                shakeWheel();
                break;            
            case 0xaa:  // 开始游戏，打开电机电源，方向盘自动回中
                setMoterPower(MOTOR_FULL_POWER);
                break;
            default:    // 到这里说明出错，为了安全，先关闭电机
                setMoterPower(MOTOR_POWER_OFF);
                break;
        }

        // knuPkt[7] 是尾灯
        setLight(knuPkt[7]);
        */
    }

    private void FillPktHeadAndTail(byte[] knuPkt)
    {
        // 指定的包头
        knuPkt[0] = 0x01;
        knuPkt[1] = 0x55;

        // 指定的包尾 'ABCD'
        knuPkt[23] = 0x41;
        knuPkt[24] = 0x42;
        knuPkt[25] = 0x43;
        knuPkt[26] = 0x44;
    }

    private void FillChecksum(byte[] knuPkt)
    {
        // 偶数表示 KnuIo 板正常，奇数表示不正常
        knuPkt[22] = 0x02;

        // 校验 11 - 14 字节
        {
            byte tmpXorChecksum = 0x00;

            tmpXorChecksum ^= knuPkt[11];
            tmpXorChecksum ^= knuPkt[12];
            tmpXorChecksum ^= knuPkt[13];

            knuPkt[10] = tmpXorChecksum;
        }

        // 校验 15 - 17 字节
        {
            byte tmpXorChecksum = 0x00;

            tmpXorChecksum ^= knuPkt[15];
            tmpXorChecksum ^= knuPkt[16];
            tmpXorChecksum ^= knuPkt[17];

            knuPkt[14] = tmpXorChecksum;
        }

        // 总校验
        {
            byte tmpXorChecksum = 0x00;
            for (int idx = 2; idx < knuPkt.Length - 4; idx++) {
                if (idx == 8 || idx == 21) {
                    continue;
                }
                tmpXorChecksum ^= knuPkt[idx];
            }

            // 0x41 和 0x42 没什么特别，协议就是这么规定的
            tmpXorChecksum ^= 0x41; // EndRead_1;
            tmpXorChecksum ^= 0x42; // EndRead_2;

            knuPkt[21] = tmpXorChecksum;
        }
    }

    // 把读到的 GKIO 包 gkio[] 转换成摩托艇程序能识别的格式，放到 buf[] 中
    private void DecodeGkioToKnuIo(byte[] gkio, byte[] knuPkt)
    {
        Array.Clear(knuPkt, 0, knuPkt.Length);

        // 填充包头包尾
        FillPktHeadAndTail(knuPkt);

        // GKIO 当前的 coin 数量，直接转换到摩托艇需要的格式
        {
            byte gkioCoin = gkio[1];
            knuPkt[8] = gkioCoin;
        }

        // GKIO 的 4 - 7 字节都可以定义为按钮。我们选用 gkio[4] 和 gkio[5]，并转换到摩托艇需要的格式
        // 各 bit 为 1 表示按下，为 0 表示弹起
        {
            byte gkioButton = 0x00;

            // 注意 GKIO 是 1 表示未按，0 表示按下，所以要 ~ 取反
            // 第 4 个字节，玩家可用按钮，1 - 测试, 3 - 下移, 4 - 投币
            byte tmpButton = (byte)(~gkio[4]);

            const byte MASK_TEST = 0x02;  // 0000,0010
            if ((byte)(tmpButton & MASK_TEST) == MASK_TEST) {
                // gkio 的 bit 1 对应 knuio 的 bit 4
                gkioButton |= 0x10; // 0001,0000
            }

            const byte MASK_MOVE = 0x08; // 0000,1000
            if ((byte)(tmpButton & MASK_MOVE) == MASK_MOVE) {
                // gkio 的 bit 3 对应 knuio 的 bit 5
                gkioButton |= 0x20; // 0010,0000
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

            // gkio[7] 是板子上的拨码开关，支持使用拨码开关操作，便于调试和场地客人排错
            // 合成为 1 个 byte，放到摩托艇的 button 状态字节
            knuPkt[9] = (byte)((~gkio[7]) | gkioButton);
        }

        // GKIO 读出的 3 个 8 bit 电位器数据 byte[8] byte[9] byte[10]，要转换成摩托艇的
        // 4 bit 高位 +8 bit 低位的 12 位格式。为了扩大精度到 12 bit，最后 4 bit 取 0
        {
            byte wheel = gkio[8];
            knuPkt[6] = (byte)(wheel >> 4); // 方向盘
            knuPkt[7] = (byte)(wheel << 4);

            byte thrust = gkio[9];
            knuPkt[2] = (byte)(thrust >> 4);  // 油门
            knuPkt[3] = (byte)(thrust << 4);

            // 摩托艇没有刹车            
        }

        FillChecksum(knuPkt);
    }

    private byte[] gkio_read_buf = new byte[16];
    public void Read(byte[] buf, int offset, int count)
    {
        if (buf.Length != 27) {
            return;
        }

        // 最后读取一次 GKIO 状态
        {
            byte[] CMD_GET_STAT = {
                    0x61, 0x8A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };
            gkio_write_read(CMD_GET_STAT, gkio_read_buf);            
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

public class MyCOMDevice : MonoBehaviour
{
    public class ComThreadClass
	{
		public string ThreadName;
        //static SerialPort _SerialPortTmp;
        private static GkioPort _SerialPort;
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
                /*
                // 禁止这条，可玩 1 局，然后 IO 板失灵
				if (XkGameCtrl.IsLoadingLevel) {
					Thread.Sleep(100);
					continue;
				}

                // 禁止后面这 2 条，全程可玩，但不知道有没有副作用?
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
                */

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
        /*
		if (!pcvr.bIsHardWare) {
			return;
		}
		ComThreadClass.IsReadMsgComTimeOut = true;
		RestartComPort();
        */
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