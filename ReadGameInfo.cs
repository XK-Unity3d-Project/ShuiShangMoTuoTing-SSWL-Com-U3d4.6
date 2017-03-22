#define USE_HANDLE_JSON

using UnityEngine;
using System;
using System.Collections.Generic;

using Knu;

public class ReadGameInfo : MonoBehaviour
{
    const int STM32_ADC_MAX = 4096;

    const string CONFIG_FILE_NAME = "mtt.conf";

    private void Awake()
    {
        knuconfig.loadFromFile(CONFIG_FILE_NAME, ItemToProperty);
    }

    public void Load()
    {
        knuconfig.loadFromFile(CONFIG_FILE_NAME, ItemToProperty);
    }

    public void Save()
    {
        knuconfig.saveToFile(CONFIG_FILE_NAME, PropertyToItem);
    }

    const int Default_CoinToStart = 1;
    const int Default_GameMode = MODE_OPERATOR;
    const int Default_InsertCoin = 0;
    const int Default_GameRecord = 0;
    const int Default_PlayerSpeedMin = 0;
    const int Default_AudioVolume = 7;
    const int Default_SteerMin = STM32_ADC_MAX;  // 这是方向盘左极限，实际机台的电位器读数反而最大
    const int Default_SteerMax = 0;
    const int Default_SteerCenter = (Default_SteerMax + Default_SteerMin) / 2;
    const int Default_ThrustMin = 0;
    const int Default_ThrustMax = STM32_ADC_MAX;
    const int Default_BrakeMin = 0;
    const int Default_BrakeMax = STM32_ADC_MAX;

    const int Default_Grade = 2;
    const GameTextType Default_Language = GameTextType.Chinese;

    const int Default_SteerForce = 25;

    // knuconfig 会遍历配置文件的每一行，依次解析出设定项目和值，并传递给此函数
    // 此函数根据情况，将字符串解析成属性的真实类型
    private void ItemToProperty(string name, string value)
    {
        try {
            int tmp;

            switch (name) {
                case "CoinToStart":
                    CoinToStart = int.TryParse(value, out tmp) ? tmp : Default_CoinToStart;
                    break;
                case "GameMode":
                    GameMode = int.TryParse(value, out tmp) ? tmp : Default_GameMode;
                    break;
                case "InsertCoin":
                    InsertCoin = int.TryParse(value, out tmp) ? tmp : Default_InsertCoin;
                    break;
                case "GameRecord":
                    GameRecord = int.TryParse(value, out tmp) ? tmp : Default_GameRecord;
                    break;
                case "PlayerSpeedMin":
                    PlayerSpeedMin = int.TryParse(value, out tmp) ? tmp : Default_PlayerSpeedMin;
                    break;
                case "AudioVolume":
                    AudioVolume = int.TryParse(value, out tmp) ? tmp : Default_AudioVolume;
                    break;
                case "SteerMin":
                    SteerMin = int.TryParse(value, out tmp) ? tmp : Default_SteerMin;
                    break;
                case "SteerCenter":
                    SteerCenter = int.TryParse(value, out tmp) ? tmp : Default_SteerCenter;
                    break;
                case "SteerMax":
                    SteerMax = int.TryParse(value, out tmp) ? tmp : Default_SteerMax;
                    break;
                case "ThrustMin":
                    ThrustMin = int.TryParse(value, out tmp) ? tmp : Default_ThrustMin;
                    break;
                case "ThrustMax":
					ThrustMax = int.TryParse(value, out tmp) ? tmp : Default_ThrustMax;
                    break;
                case "BrakeMin":
                    BrakeMin = int.TryParse(value, out tmp) ? tmp : Default_BrakeMin;
                    break;
                case "BrakeMax":
                    BrakeMax = int.TryParse(value, out tmp) ? tmp : Default_BrakeMax;
                    break;
                case "Grade":
                    Grade = int.TryParse(value, out tmp) ? tmp : Default_Grade;
                    break;
                case "Language":
                    if (int.TryParse(value, out tmp) &&              // 字符串为合法整数
                        Enum.IsDefined(typeof(GameTextType), tmp))   // 且可用 GameTextType 类型表示
                    {
                        Language = (GameTextType)tmp;
                    } else {
                        Language = Default_Language;
                    }
                    break;
                case "SteerForce":
                    SteerForce = int.TryParse(value, out tmp) ? tmp : Default_SteerForce;
                    break;
            }
        }
        catch {
            // 解码配置文件时出错，下一行继续解码。场地可使用 "恢复工厂默认" 功能修正此问题
        }
    }

    // 把属性的值，保存到字符串数组，准备写入配置文件
    private string[] PropertyToItem()
    {
        List<string> item = new List<string>();

        // 高版本 C# 可以用防拼写错误的类型安全写法 nameof(volumn)，unity3d 的 mono 不支持这个语法
        item.Add(String.Format("{0} = {1}", "CoinToStart", CoinToStart));
        item.Add(String.Format("{0} = {1}", "GameMode", GameMode));
        item.Add(String.Format("{0} = {1}", "InsertCoin", InsertCoin));
        item.Add(String.Format("{0} = {1}", "GameRecord", GameRecord));
        item.Add(String.Format("{0} = {1}", "PlayerSpeedMin", PlayerSpeedMin));
        item.Add(String.Format("{0} = {1}", "AudioVolume", AudioVolume));
        item.Add(String.Format("{0} = {1}", "SteerMin", SteerMin));
        item.Add(String.Format("{0} = {1}", "SteerCenter", SteerCenter));
        item.Add(String.Format("{0} = {1}", "SteerMax", SteerMax));
        item.Add(String.Format("{0} = {1}", "ThrustMin", ThrustMin));
        item.Add(String.Format("{0} = {1}", "ThrustMax", ThrustMax));
        item.Add(String.Format("{0} = {1}", "BrakeMin", BrakeMin));
        item.Add(String.Format("{0} = {1}", "BrakeMax", BrakeMax));
        item.Add(String.Format("{0} = {1}", "Grade", Grade));

        // enum 类型，保存到文件的时候，还是转 int 方便
        item.Add(String.Format("{0} = {1}", "Language", (int)Language));

        item.Add(String.Format("{0} = {1}", "SteerForce", SteerForce));

        return item.ToArray();
    }

    private static ReadGameInfo Instance = null;
    static public ReadGameInfo GetInstance()
    {
        if (Instance == null) {
            GameObject obj = new GameObject("_ReadGameInfo");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<ReadGameInfo>();
        }

        return Instance;
    }

    private int START_COIN_ = Default_CoinToStart;
    private int CoinToStart
    {
        get { return Math.Max(1, START_COIN_); }
        set { START_COIN_ = Math.Max(value, 1); }
    }

    const int MODE_OPERATOR = 0;
    const int MODE_FREEPLAY = 1;
    private int GAME_MODE_ = Default_GameMode;
    private int GameMode
    {
        get { return GAME_MODE_; }
        set { GAME_MODE_ = (value == MODE_FREEPLAY) ? MODE_FREEPLAY : MODE_OPERATOR; }
    }

    private int INSERT_COIN_ = Default_InsertCoin;
    private int InsertCoin
    {
        get { return Math.Max(INSERT_COIN_, 0); }
        set { INSERT_COIN_ = Math.Max(value, 0); }
    }

    private int GAME_RECORD_ = Default_GameRecord;
    private int GameRecord
    {
        get { return Math.Max(GAME_RECORD_, 0); }
        set { GAME_RECORD_ = Math.Max(value, 0); }
    }    

    private int PLAYER_SPEED_MIN_ = Default_PlayerSpeedMin;
    private int PlayerSpeedMin
    {
        get { return PLAYER_SPEED_MIN_; }
        set { PLAYER_SPEED_MIN_ = Mathf.Clamp(value, 0, 80); }
    }

    private int AUDIO_VOLUME_ = Default_AudioVolume;
    private int AudioVolume
    {
        get { return Mathf.Clamp(AUDIO_VOLUME_, 0, 10); }
        set { AUDIO_VOLUME_ = Mathf.Clamp(value, 0, 10); }
    }

    // 注意，摩托艇的方向盘，转到最左边，电位器读数 SteerMin 反而最大；转到最右边，电位器读数 SteerMax 其实最小
    private int STEER_MIN_ = Default_SteerMin;     // BikeDirMin;
    public int SteerMin
    {
        get { return STEER_MIN_; }
        set { STEER_MIN_ = Mathf.Clamp(value, 0, STM32_ADC_MAX); }
    }

    private int STEER_CENTER_ = Default_SteerCenter;  // BikeDirCen;
    public int SteerCenter
    {
        get { return STEER_CENTER_; }
        set { STEER_CENTER_ = Mathf.Clamp(value, 0, STM32_ADC_MAX); }
    }

    private int STEER_MAX_ = Default_SteerMax;
    public int SteerMax
    {
        get { return STEER_MAX_; }
        set { STEER_MAX_ = Mathf.Clamp(value, 0, STM32_ADC_MAX); }
    }

    private int THRUST_MIN_ = Default_ThrustMin;    // BikePowerMin
    public int ThrustMin
    {
        get { return THRUST_MIN_; }
        //set { THRUST_MIN_ = Mathf.Clamp(value, 0, THRUST_MAX_); } //对于油门反接的情况存在问题.
		set { THRUST_MIN_ = Mathf.Clamp(value, 0, STM32_ADC_MAX); } //适配油门反接逻辑.
    }

    private int THRUST_MAX_ = Default_ThrustMax;
    public int ThrustMax
    {
        get { return THRUST_MAX_; }
		//set { THRUST_MAX_ = Mathf.Clamp(value, THRUST_MIN_, STM32_ADC_MAX); } //对于油门反接的情况存在问题.
		set { THRUST_MAX_ = Mathf.Clamp(value, 0, STM32_ADC_MAX); } //适配油门反接逻辑.
    }

    private int BRAKE_MIN_ = Default_BrakeMin;     // BikeShaCheMin
    public int BrakeMin
    {
        get { return BRAKE_MIN_; }
        set { BRAKE_MIN_ = Mathf.Clamp(value, 0, BRAKE_MAX_); }
    }

    private int BRAKE_MAX_ = Default_BrakeMax;
    public int BrakeMax
    {
        get { return BRAKE_MAX_; }
        set { BRAKE_MAX_ = Mathf.Clamp(value, BRAKE_MIN_, STM32_ADC_MAX); }
    }

    private int GRADE_ = Default_Grade;
    public int Grade
    {
        get { return GRADE_; }
        set {
            switch (value) {
                case 1:  // Easy
                case 2:  // Normal
                case 3:  // Difficult
                    GRADE_ = value;
                    break;
                default:
                    GRADE_ = 2;
                    break;
            }
        }
    }

    private GameTextType LANGUAGE_ = Default_Language;
    public GameTextType Language
    {
        get { return LANGUAGE_; }
        set { LANGUAGE_ = value; }
    }

    // SteerForce 是一个百分数的分子部分
    private int STEER_FORCE_ = Default_SteerForce;
	public static int SteerForceVal = Default_SteerForce;
    public int SteerForce
    {
        get { return STEER_FORCE_; }
        set {
			STEER_FORCE_ = Mathf.Clamp(value, 0, 100);
			SteerForceVal = STEER_FORCE_;
		}
    }

    void InitGameInfo()
	{
        Load();
	}

    public void FactoryReset()
    {
        CoinToStart = Default_CoinToStart;
        GameMode = Default_GameMode;
        InsertCoin = Default_InsertCoin;
        GameRecord = Default_GameRecord;
        PlayerSpeedMin = Default_PlayerSpeedMin;
        AudioVolume = Default_AudioVolume;

        SteerMin = Default_SteerMin;
        SteerMax = Default_SteerMax;
        SteerCenter = Default_SteerCenter;

        ThrustMin = Default_ThrustMin;
        ThrustMax = Default_ThrustMax;

        BrakeMin = Default_BrakeMin;
        BrakeMax = Default_BrakeMax;

        Grade = Default_Grade;

        // 在没有加到设定界面之前，保持不变，否则执行一次，英文节目会永久性变成中文
        // Language = GameTextType.Chinese;

        // 在没有加到设定界面之前，保持不变，否则执行一次，又会变成 Default_SteerForce 力量
        // SteerForce = Default_SteerForce;
    }

    /* 为了兼容性保留的废弃旧函数 */

    //[Obsolete("ReadStarCoinNumSet has been deprecated. Use property CoinToStart instead.")]
    public string ReadStarCoinNumSet()
    {
        return Convert.ToString(CoinToStart);
    }
    //[Obsolete("WriteStarCoinNumSet has been deprecated. Use property CoinToStart instead.")]
    public void WriteStarCoinNumSet(string value)
    {
        int tmp_coin_to_start;

        bool ParseSuccess = int.TryParse(value, out tmp_coin_to_start);

        if (ParseSuccess) {
            CoinToStart = tmp_coin_to_start;
        }
        else {
            tmp_coin_to_start = 0;
        }
    }

    //[Obsolete("ReadGameStarMode has been deprecated. Use property GameMode instead.")]
    public string ReadGameStarMode()
    {
        return (GameMode == MODE_FREEPLAY) ? "FREE" : "oper";
    }
    //[Obsolete("WriteGameStarMode has been deprecated. Use property GameMode instead.")]
    public void WriteGameStarMode(string mode_str)
    {
        GameMode = mode_str.Equals("FREE") ? MODE_FREEPLAY : MODE_OPERATOR;
    }

    //[Obsolete("ReadInsertCoinNum has been deprecated. Use property InsertCoin instead.")]
    public string ReadInsertCoinNum()
    {
        return Convert.ToString(InsertCoin);
    }
    //[Obsolete("WriteInsertCoinNum has been deprecated. Use property InsertCoin instead.")]
    public void WriteInsertCoinNum(string value)
    {
        int tmp_InsertCoin;

        bool ParseSuccess = int.TryParse(value, out tmp_InsertCoin);

        if (!ParseSuccess) {
            InsertCoin = tmp_InsertCoin;
        }
        else {
            InsertCoin = 0;
        }
    }

    //[Obsolete("ReadPlayerMinSpeedVal has been deprecated. Use property PlayerSpeedMin instead.")]
    public int ReadPlayerMinSpeedVal()
    {
        return PlayerSpeedMin;
    }
    //[Obsolete("WritePlayerMinSpeedVal has been deprecated. Use property PlayerSpeedMin instead.")]
    public void WritePlayerMinSpeedVal(int value)
    {
        PlayerSpeedMin = value;
    }

    //[Obsolete("ReadGameRecord has been deprecated. Use property GameRecord instead.")]
    public int ReadGameRecord()
    {
        return GameRecord;
    }
    //[Obsolete("WriteGameRecord has been deprecated. Use property GameRecord instead.")]
    public void WriteGameRecord(int value)
    {
        GameRecord = value;
    }

    //[Obsolete("WriteGameAudioVolume has been deprecated. Use property AudioVolume instead.")]
    public void WriteGameAudioVolume(int value)
    {
        AudioVolume = value;
    }
    //[Obsolete("ReadGameAudioVolume has been deprecated. Use property AudioVolume instead.")]
    public int ReadGameAudioVolume()
    {
        return AudioVolume;
    }
}