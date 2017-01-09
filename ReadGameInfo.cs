#define USE_HANDLE_JSON

using UnityEngine;
using System;
using System.Collections.Generic;

using Knu;

public class ReadGameInfo : MonoBehaviour
{
    const int STM32_ADC_MAX = 4096;

    private knuconfig kconf_;

    private void Awake()
    {
        kconf_ = new knuconfig(@"mtt.conf", ItemToProperty, PropertyToItem);
        kconf_.loadFromFile();
    }

    public void Load()
    {
        kconf_.loadFromFile();
    }

    public void Save()
    {
        kconf_.saveToFile();
    }

    const int Default_CoinToStart = 1;
    const int Default_GameMode = MODE_OPERATOR;
    const int Default_InsertCoin = 0;
    const int Default_GameRecord = 0;
    const int Default_PlayerSpeedMin = 0;
    const int Default_AudioVolume = 7;
    const int Default_SteerMin = 0;
    const int Default_SteerMax = STM32_ADC_MAX;
    const int Default_SteerCenter = (Default_SteerMax + Default_SteerMin) / 2;
    const int Default_ThrustMin = 0;
    const int Default_ThrustMax = STM32_ADC_MAX;
    const int Default_BrakeMin = 0;
    const int Default_BrakeMax = STM32_ADC_MAX;

    const int Default_Grade = 2;

    // knuconfig 会遍历配置文件的每一行，依次解析出设定项目和值，并传递给此函数
    // 此函数根据情况，将字符串解析成属性的真实类型
    private void ItemToProperty(string name, string value)
    {
        try {
            int tmp;

            if (String.Equals(name, "CoinToStart")) {
                CoinToStart = int.TryParse(value, out tmp) ? tmp : Default_CoinToStart;
                return;
            }
            if (String.Equals(name, "GameMode")) {
                GameMode = int.TryParse(value, out tmp) ? tmp : Default_GameMode;
                return;
            }
            if (String.Equals(name, "InsertCoin")) {
                InsertCoin = int.TryParse(value, out tmp) ? tmp : Default_InsertCoin;
                return;
            }
            if (String.Equals(name, "GameRecord")) {
                GameRecord = int.TryParse(value, out tmp) ? tmp : Default_GameRecord;
                return;
            }
            if (String.Equals(name, "PlayerSpeedMin")) {
                PlayerSpeedMin = int.TryParse(value, out tmp) ? tmp : Default_PlayerSpeedMin;
                return;
            }
            if (String.Equals(name, "AudioVolume")) {
                AudioVolume = int.TryParse(value, out tmp) ? tmp : Default_AudioVolume;
                return;
            }
            if (String.Equals(name, "SteerMin")) {
                SteerMin = int.TryParse(value, out tmp) ? tmp : Default_SteerMin;
                return;
            }
            if (String.Equals(name, "SteerCenter")) {
                SteerCenter = int.TryParse(value, out tmp) ? tmp : Default_SteerCenter;
                return;
            }
            if (String.Equals(name, "SteerMax")) {
                SteerMax = int.TryParse(value, out tmp) ? tmp : Default_SteerMax;
                return;
            }
            if (String.Equals(name, "ThrustMin")) {
                ThrustMin = int.TryParse(value, out tmp) ? tmp : Default_ThrustMin;
                return;
            }
            if (String.Equals(name, "ThrustMax")) {
                ThrustMax = int.TryParse(value, out tmp) ? tmp : Default_ThrustMax;
                return;
            }
            if (String.Equals(name, "BrakeMin")) {
                BrakeMin = int.TryParse(value, out tmp) ? tmp : Default_BrakeMin;
                return;
            }
            if (String.Equals(name, "BrakeMax")) {
                BrakeMax = int.TryParse(value, out tmp) ? tmp : Default_BrakeMax;
                return;
            }
            if (String.Equals(name, "Grade")) {
                Grade = int.TryParse(value, out tmp) ? tmp : Default_Grade;
                return;
            }
            if (String.Equals(name, "Language")) {
                bool success = int.TryParse(value, out tmp);
                if (success) {
                    Language = (tmp == 0) ? GameTextType.Chinese : GameTextType.English;
                } else {
                    Language = Default_Language;
                }
                return;
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
        item.Add(String.Format("{0} = {1}", "Language", Language == GameTextType.Chinese ? 0 : 1));

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

    private int START_COIN = Default_CoinToStart;
    private int CoinToStart
    {
        get { return Math.Max(1, START_COIN); }
        set { START_COIN = Math.Max(value, 1); }
    }

    const int MODE_OPERATOR = 0;
    const int MODE_FREEPLAY = 1;
    private int GAME_MODE = Default_GameMode;
    private int GameMode
    {
        get { return GAME_MODE; }
        set { GAME_MODE = (value == MODE_FREEPLAY) ? MODE_FREEPLAY : MODE_OPERATOR; }
    }

    private int INSERT_COIN = Default_InsertCoin;
    private int InsertCoin
    {
        get { return Math.Max(INSERT_COIN, 0); }
        set { INSERT_COIN = Math.Max(value, 0); }
    }

    private int GAME_RECORD = Default_GameRecord;
    private int GameRecord
    {
        get { return Math.Max(GAME_RECORD, 0); }
        set { GAME_RECORD = Math.Max(value, 0); }
    }    

    private int PLAYER_SPEED_MIN = Default_PlayerSpeedMin;
    private int PlayerSpeedMin
    {
        get { return PLAYER_SPEED_MIN; }
        set { PLAYER_SPEED_MIN = Mathf.Clamp(value, 0, 80); }
    }

    private int AUDIO_VOLUME = Default_AudioVolume;
    private int AudioVolume
    {
        get { return Mathf.Clamp(AUDIO_VOLUME, 0, 10); }
        set { AUDIO_VOLUME = Mathf.Clamp(value, 0, 10); }
    }    

    private int STEER_MIN = Default_SteerMin;     // BikeDirMin;
    public int SteerMin
    {
        get { return Mathf.Clamp(STEER_MIN, 0, STEER_CENTER); }
        set { STEER_MIN = Mathf.Clamp(value, 0, STEER_CENTER); }
    }

    private int STEER_CENTER = Default_SteerCenter;  // BikeDirCen;
    public int SteerCenter
    {
        get { return STEER_CENTER; }
        set { STEER_CENTER = Mathf.Clamp(value, STEER_MIN, STEER_MAX); }
    }

    private int STEER_MAX = Default_SteerMax;
    public int SteerMax
    {
        get { return STEER_MAX; }
        set { STEER_MAX = Mathf.Clamp(value, STEER_CENTER, STM32_ADC_MAX); }
    }

    private int THRUST_MIN = Default_ThrustMin;    // BikePowerMin
    public int ThrustMin
    {
        get { return THRUST_MIN; }
        set { THRUST_MIN = Mathf.Clamp(value, 0, THRUST_MAX); }
    }

    private int THRUST_MAX = Default_ThrustMax;
    public int ThrustMax
    {
        get { return THRUST_MAX; }
        set { THRUST_MAX = Mathf.Clamp(value, THRUST_MIN, STM32_ADC_MAX); }
    }

    private int BRAKE_MIN = Default_BrakeMin;     // BikeShaCheMin
    public int BrakeMin
    {
        get { return BRAKE_MIN; }
        set { BRAKE_MIN = Mathf.Clamp(value, 0, THRUST_MAX); }
    }

    private int BRAKE_MAX = Default_BrakeMax;
    public int BrakeMax
    {
        get { return BRAKE_MAX; }
        set { BRAKE_MAX = Mathf.Clamp(value, BRAKE_MIN, STM32_ADC_MAX); }
    }

    private int GRADE = Default_Grade;
    public int Grade
    {
        get { return GRADE; }
        set {
            switch (value) {
                case 1:  // Easy
                case 2:  // Normal
                case 3:  // Difficult
                    GRADE = value;
                    break;
                default:
                    GRADE = 2;
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