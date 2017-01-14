/*
 Windows 下 unity3d 的 PlayerPrefs 把数据保存在注册表里（linux 下在 ~/.config/ 目录下），
 注册表是一个带缓存的大数据库，并不一定马上写入磁盘。这就存在一个比较严重的缺陷，就是写入的
 配置数据，在突然断电后，可能部分设定，比如几币一玩，没有保存，下次重新开机，还是旧数据。

 现改为用 ./conf/mtt.conf 文本文件作为配置文件，用记事本打开后，类似这样

 CoinToStart = 5
 Volume = 0
 FreePlay = 0

 写的时候，有留意以下几点
 1) 仅在游戏启动时，读取配置的时候，以只读方式在几十毫秒内打开文件。
    仅在退出设置画面时候，写入配置文件的几十毫秒内，机台断电，文件内容有可能是不完整的。
    其它时候基本没有损坏文件的可能。
 2) 通过 FileOptions.WriteThrough 参数， 指定文件内容不缓存，立即保存到物理硬盘。
    防止短时间内机台断电后，新数据没有保存，重新开机还是旧数据
 3) xml 格式也可做到，但文件比较复杂，不容易阅读
 4) json 格式最佳，但 unity3d 4.x 没带

 通过委托 delegate 实现。而不是继承一个子类覆盖 ItemToProperty / PropertyToItem 这
 2 个方法，主要是因为用到 knuconfig 的对象，一般已经继承了其它类，比如 MonoBehaviour

初步测试结果

 一块高科 USB IO 板，使用一个控制气囊的输出针，用 2 条线，直接接电脑主板上的 RESET 针
 脚和 GND 针脚测试程序，每次开机后自动启动，先读取配置文件中的 RESET 次数记录，然后 +1
 保存，最后指令 IO 板，发出 RESET 信号，将主板重启。

 一个周末大约循环重启 3800 次左右，配置文件正常
 */

using System.IO;
using System.Text;

namespace Knu
{
    // 遍历从配置文件读出的字符串，依次解析出设定项目和值，保存到属性，供程序读写
    public delegate void DelegateItemToProperty(string name, string value);

    // 把属性的值，保存到字符串数组，准备写入配置文件
    public delegate string[] DelegatePropertyToItem();

    static class knuconfig
    {
        const string CONF_DIRECTORY = @"./conf/";

        static private string GetFullPathOfConfFile(string conf_file_name)
        {
            string conf_full_path = CONF_DIRECTORY + conf_file_name;
            return conf_full_path;
        }

        static private void initConfigFile(string conf_file_name)
        {
            if (!Directory.Exists(CONF_DIRECTORY)) {
                Directory.CreateDirectory(CONF_DIRECTORY);
            }

            /* 出厂没有配置文件，自动取默认值。进设定菜单一次，退出的时候自动保存文件，就有配置文件了。
             * if (!File.Exists(CONF_FILE)) {
                File.Create();
            }*/
        }

        /* Windows 下有效
        [DllImport("Kernel32.dll", EntryPoint = "FlushFileBuffers")]
        static extern int FlushFileBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle hFile);
        */

        static public void loadFromFile(string conf_file_name, DelegateItemToProperty ItemToProperty)
        {
            initConfigFile(conf_file_name);

            string CONF_FILE = GetFullPathOfConfFile(conf_file_name);
            try {
                if (File.Exists(CONF_FILE)) {
                    string[] confLines = File.ReadAllLines(CONF_FILE, Encoding.UTF8);

                    // 应检查文件有效性，头部尾部标记什么的。不过先略过
                    foreach (string line in confLines) {
                        string name, value;

                        bool isValidLine = DecodeLine(line, out name, out value);

                        if (isValidLine) {
                            ItemToProperty(name, value);
                        }
                    }
                }
            }
            catch {
                // 解码配置文件时出错，停止继续解码。场地可使用 "恢复工厂默认" 功能修正此问题
            }
        }

        static public void saveToFile(string conf_file_name, DelegatePropertyToItem PropertyToItem)
        {
            initConfigFile(conf_file_name);

            string[] tmp_item = PropertyToItem();

            //Console.WriteLine("coin_to_start : {0}, volumn : {1}, free_play : {2}", coin_to_start, volumn, free_play);

            // .net 4.0 或以上，可以直接 fs.Flush(true)，直接写到物理磁盘
            // Winddows 下可以直接 import 标准的 Win32 函数 FlushFileBuffers，指令文件立即写入物理磁盘，Linux 下为标准 C 库
            // 的 int fflush ( FILE * stream ); 函数
            // unity3d 自带的早期版本，可指定文件为 WriteThrough，即不缓冲，直接写入磁盘
            string CONF_FILE = GetFullPathOfConfFile(conf_file_name);
            FileStream file = new FileStream(CONF_FILE, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough);
            StreamWriter fileWriter = new StreamWriter(file, Encoding.UTF8);

            try {
                // 尽量预分配能放得下整个配置文件的内容，免得执行过程中还要分配第二次。
                // 和反复调用 fileWriter.WriteLine()，每次写一行，这样生成的最终文件，他们的 md5sum 是一样的。
                StringBuilder strBuf = new StringBuilder(2048);

                strBuf.AppendLine(@"# game config file");
                strBuf.AppendLine(@"# format : <item>=<value> or <item> = <value>, for instance, coin_to_start=5 / coin_to_start = 5");
                strBuf.AppendLine(@"# True/False for boolean, 94 for int, 14.27 for float, string is just string");

                strBuf.AppendLine("");

                foreach (string line in tmp_item) {
                    strBuf.AppendLine(line);
                }

                strBuf.AppendLine("");

                strBuf.AppendLine("# END");

                // 用 StringBuilder 构造内容，最后一次性写入，比每次写一行，效率高
                fileWriter.Write(strBuf.ToString());
            }
            finally {
                // 文件缓冲写到文件系统，文件系统缓冲写到物理磁盘
                fileWriter.Flush();
                //file.Flush();
                //FlushFileBuffers(file.SafeFileHandle); // 怕 WriteThourgh 不生效
                fileWriter.Close(); // 会自动 Close 了 fs
            }
        }

        // 各种字符串合法性检查，防止手写配置文件出错。基本的格式规则放在这里，强制所有配置文件统一格式
        static private bool DecodeLine(string e, out string name, out string value)
        {
            name = null;
            value = null;

            if ((e.Length < 3) ||          // 字符串长度太短，不可能是有效设置，比如是空行
                (e.StartsWith("#")) ||     // 以 # 开头的是注释，跳过
                (e.IndexOf('=') == -1))    // 没有 = 号，不是有效的设置
            {
                return false;
            }

            string[] param = e.Split('=');
            if ((param == null) ||         // 没找到 = 符号
                (param.Length != 2))       // 必须刚好由 = 号分为 2 部分，即只有 1 个 = 符号
            {
                return false;
            }

            // 去首尾空格
            name = param[0].Trim();
            value = param[1].Trim();

            return true;
        }
    }
}
