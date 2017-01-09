using System.IO;
using System.Text;

namespace Knu
{
    public delegate void DelegateItemToProperty(string name, string value);
    public delegate string[] DelegatePropertyToItem();

    class knuconfig
    {
        protected const string CONF_DIRECTORY = @"./conf/";
        protected string CONF_FILE;

        public knuconfig(string conf_file_name, DelegateItemToProperty runItemToProperty, DelegatePropertyToItem runPropertyToItem)
        {
            initConfigFile(conf_file_name);

            ItemToProperty = runItemToProperty;
            PropertyToItem = runPropertyToItem;
        }

        protected void initConfigFile(string conf_file_name)
        {
            CONF_FILE = CONF_DIRECTORY + conf_file_name;
            if (!Directory.Exists(CONF_DIRECTORY)) {
                Directory.CreateDirectory(CONF_DIRECTORY);
            }

            /*if (!File.Exists(CONF_FILE)) {
                File.Create();
            }*/
        }

        /* Windows only
        [DllImport("Kernel32.dll", EntryPoint = "FlushFileBuffers")]
        static extern int FlushFileBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle hFile);
        */

        public void loadFromFile()
        {
            try {
                if (File.Exists(CONF_FILE)) {
                    string[] confLines = File.ReadAllLines(CONF_FILE, Encoding.UTF8);

                    // 应检查文件有效性，头部尾部什么的
                    foreach (string e in confLines) {
                        string name, value;

                        bool isValidLine = DecodeLine(e, out name, out value);

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

        public void saveToFile()
        {
            string[] tmp_item = PropertyToItem();

            //Console.WriteLine("coin_to_start : {0}, volumn : {1}, free_play : {2}", coin_to_start, volumn, free_play);

            // .net 4.0 或以上，可以直接 fs.Flush(true)，直接写到物理磁盘
            // unity3d 自带的早期版本，可指定文件为 WriteThrough，即不缓冲，直接写入磁盘
            FileStream fs = new FileStream(CONF_FILE, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough);
            StreamWriter sr = new StreamWriter(fs, Encoding.UTF8);

            try {
                sr.WriteLine(@"# game config file");
                sr.WriteLine(@"# format : <item>=<value> or <item> = <value>, for instance, coin_to_start=5 / coin_to_start = 5");
                sr.WriteLine(@"# True/False for boolean, 94 for int, 14.27 for float, string is just string");

                sr.WriteLine("");

                foreach (string e in tmp_item) {
                    sr.WriteLine(e);
                }

                sr.WriteLine("");

                sr.WriteLine("# END");
            }
            finally {
                // 文件缓冲写到文件系统，文件系统缓冲写到物理磁盘
                sr.Flush();
                //fs.Flush();
                //FlushFileBuffers(fs.SafeFileHandle); // 怕 WriteThourgh 不生效
                sr.Close(); // 会自动 Close 了 fs
            }
        }

        // 各种字符串合法性检查，防止手写配置文件出错。基本的格式规则放在这里，强制所有配置文件统一格式
        private bool DecodeLine(string e, out string name, out string value)
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

        // 遍历从配置文件读出的字符串，依次解析出设定项目和值，保存到属性，供程序读写
        private DelegateItemToProperty ItemToProperty;

        // 把属性的值，保存到字符串数组，准备写入配置文件
        private DelegatePropertyToItem PropertyToItem;
    }
}
