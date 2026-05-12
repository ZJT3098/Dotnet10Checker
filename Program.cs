using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNet10Checker
{
    /// <summary>
    /// 程序入口点
    /// </summary>
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 启用视觉样式（XP风格）
            Application.EnableVisualStyles();
            // 设置文本渲染兼容性
            Application.SetCompatibleTextRenderingDefault(false);
            // 运行主窗体
            Application.Run(new MainForm());
        }
    }

    /// <summary>
    /// 语言枚举
    /// </summary>
    enum Language { zhCN, zhTW, en }

    /// <summary>
    /// 多语言字符串管理类
    /// </summary>
    static class Strings
    {
        private static int _currentIndex;

        /// <summary>
        /// 当前语言索引
        /// </summary>
        public static int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
                CurrentLang = (Language)value;
            }
        }

        /// <summary>
        /// 当前语言
        /// </summary>
        public static Language CurrentLang { get; private set; } = Language.zhCN;

        /// <summary>
        /// 获取指定键的语言字符串
        /// </summary>
        /// <param name="key">字符串键</param>
        /// <returns>对应语言的字符串</returns>
        public static string Get(string key)
        {
            switch (CurrentLang)
            {
                case Language.zhCN:
                    return zhCN.TryGetValue(key, out var v1) ? v1 : "";
                case Language.zhTW:
                    return zhTW.TryGetValue(key, out var v2) ? v2 : "";
                case Language.en:
                    return en.TryGetValue(key, out var v3) ? v3 : "";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 简体中文字典
        /// </summary>
        static Dictionary<string, string> zhCN = new Dictionary<string, string>
        {
            { "Title", ".NET 10+ 运行时检测" },
            { "Subtitle", "检查 .NET 10 或更高版本是否已安装" },
            { "Ready", "就绪" },
            { "Check", "检测运行时" },
            { "Scanning", "扫描中..." },
            { "NotFound", "✗ 未找到 .NET 10+" },
            { "Found", "✓ 已检测到 .NET 10+！" },
            { "NotFoundInPath", "✗ 未在 PATH 中找到 dotnet.exe" },
            { "NoRuntimes", "✗ 未检测到 .NET 5.0+ 运行时" },
            { "InstalledRuntimes", "已安装的运行时 (.NET 5.0+)：" },
            { "Success", "✓ 成功：找到 {0} 个 .NET 10+ 运行时！" },
            { "Failed", "✗ 失败：未检测到 .NET 10+ 运行时" },
            { "FailedHint", "  请安装 .NET 10 或更高版本" },
            { "Error", "错误" },
            { "Language", "语言" },
            { "Version", "版本" },
            { "Title2", "=== .NET 10+ 运行时检测 ===" },
            { "DotnetExe", "dotnet.exe:" },
            { "Download", "下载 .NET 10" }
        };

        /// <summary>
        /// 繁体中文字典
        /// </summary>
        static Dictionary<string, string> zhTW = new Dictionary<string, string>
        {
            { "Title", ".NET 10+ 執行階段檢測" },
            { "Subtitle", "檢查 .NET 10 或更高版本是否已安裝" },
            { "Ready", "就緒" },
            { "Check", "檢測執行階段" },
            { "Scanning", "掃描中..." },
            { "NotFound", "✗ 未找到 .NET 10+" },
            { "Found", "✓ 已檢測到 .NET 10+！" },
            { "NotFoundInPath", "✗ 未在 PATH 中找到 dotnet.exe" },
            { "NoRuntimes", "✗ 未檢測到 .NET 5.0+ 執行階段" },
            { "InstalledRuntimes", "已安裝的執行階段 (.NET 5.0+)：" },
            { "Success", "✓ 成功：找到 {0} 個 .NET 10+ 執行階段！" },
            { "Failed", "✗ 失敗：未檢測到 .NET 10+ 執行階段" },
            { "FailedHint", "  請安裝 .NET 10 或更高版本" },
            { "Error", "錯誤" },
            { "Language", "語言" },
            { "Version", "版本" },
            { "Title2", "=== .NET 10+ 執行階段檢測 ===" },
            { "DotnetExe", "dotnet.exe:" },
            { "Download", "下載 .NET 10" }
        };

        /// <summary>
        /// 英文字典
        /// </summary>
        static Dictionary<string, string> en = new Dictionary<string, string>
        {
            { "Title", ".NET 10+ Runtime Checker" },
            { "Subtitle", "Checks if .NET 10 or higher is installed" },
            { "Ready", "Ready" },
            { "Check", "Check Runtimes" },
            { "Scanning", "Scanning..." },
            { "NotFound", "✗ .NET 10+ NOT found" },
            { "Found", "✓ .NET 10+ detected!" },
            { "NotFoundInPath", "✗ dotnet.exe not found in PATH" },
            { "NoRuntimes", "✗ No .NET 5.0+ runtimes detected" },
            { "InstalledRuntimes", "Installed Runtimes (.NET 5.0+):" },
            { "Success", "✓ SUCCESS: {0} .NET 10+ runtime(s) found!" },
            { "Failed", "✗ FAILED: No .NET 10+ runtime detected" },
            { "FailedHint", "  Please install .NET 10 or higher" },
            { "Error", "Error" },
            { "Language", "Language" },
            { "Version", "Version" },
            { "Title2", "=== .NET 10+ Runtime Detection ===" },
            { "DotnetExe", "dotnet.exe:" },
            { "Download", "Download .NET 10" }
        };
    }

    /// <summary>
    /// 主窗体
    /// </summary>
    class MainForm : Form
    {
        // UI 控件声明
        private TextBox txtResult;          // 结果显示文本框
        private Button btnCheck;            // 检测按钮
        private Button btnDownload;         // 下载按钮
        private ProgressBar progressBar;   // 进度条
        private Label lblStatus;            // 状态标签
        private ComboBox cboLanguage;       // 语言选择下拉框
        private Label lblLanguage;          // 语言标签
        private Label titleLabel;           // 标题标签
        private Label subLabel;             // 副标题标签
        private Language currentLang = Language.zhCN;  // 当前语言
        private const string DotNet10DownloadUrl = "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/10.0.7/windowsdesktop-runtime-10.0.7-win-x86.exe";  // .NET 10 下载链接

        /// <summary>
        /// 构造函数，初始化窗体和控件
        /// </summary>
        public MainForm()
        {
            // 窗体基本设置
            Text = Strings.Get("Title");
            ClientSize = new Size(500, 400);
            StartPosition = FormStartPosition.CenterScreen;  // 居中显示
            BackColor = Color.FromArgb(30, 30, 30);           // 深色背景
            FormBorderStyle = FormBorderStyle.FixedDialog;    // 固定大小对话框
            MaximizeBox = false;                              // 禁用最大化

            // 初始化标题标签
            titleLabel = new Label
            {
                Text = Strings.Get("Title"),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // 初始化副标题标签
            subLabel = new Label
            {
                Text = Strings.Get("Subtitle"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(20, 55),
                AutoSize = true
            };

            // 初始化语言标签
            lblLanguage = new Label
            {
                Text = Strings.Get("Language"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(320, 30),
                AutoSize = true
            };

            // 初始化语言下拉框
            cboLanguage = new ComboBox
            {
                Location = new Point(320, 52),
                Size = new Size(160, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,  // 只读下拉框
                Font = new Font("Segoe UI", 9)
            };
            cboLanguage.Items.AddRange(new object[] { "简体中文", "繁體中文", "English" });
            cboLanguage.SelectedIndex = 0;  // 默认简体中文
            cboLanguage.SelectedIndexChanged += CboLanguage_SelectedIndexChanged;

            // 初始化状态标签
            lblStatus = new Label
            {
                Text = Strings.Get("Ready"),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(20, 85),
                AutoSize = true
            };

            // 初始化检测按钮
            btnCheck = new Button
            {
                Text = Strings.Get("Check"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 115),
                Size = new Size(130, 35)
            };
            btnCheck.FlatAppearance.BorderSize = 0;  // 无边框
            btnCheck.Click += BtnCheck_Click;

            // 初始化下载按钮
            btnDownload = new Button
            {
                Text = Strings.Get("Download"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),  // 绿色
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(160, 115),
                Size = new Size(160, 35),
                Visible = false  // 默认隐藏
            };
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.Click += BtnDownload_Click;

            // 初始化进度条
            progressBar = new ProgressBar
            {
                Location = new Point(160, 125),
                Size = new Size(320, 20),
                Style = ProgressBarStyle.Marquee,  // 滚动条样式
                Visible = false
            };

            // 初始化结果文本框
            txtResult = new TextBox
            {
                Location = new Point(20, 165),
                Size = new Size(460, 210),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = Color.LightGreen,
                BorderStyle = BorderStyle.FixedSingle
            };

            // 将所有控件添加到窗体
            Controls.AddRange(new Control[] { titleLabel, subLabel, lblLanguage, cboLanguage, lblStatus, btnCheck, btnDownload, progressBar, txtResult });
        }

        /// <summary>
        /// 下载按钮点击事件
        /// </summary>
        private void BtnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                // 使用浏览器打开下载链接
                Process.Start(new ProcessStartInfo
                {
                    FileName = DotNet10DownloadUrl,
                    UseShellExecute = true
                });
            }
            catch { }
        }

        /// <summary>
        /// 语言选择改变事件
        /// </summary>
        private void CboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Strings.CurrentIndex = cboLanguage.SelectedIndex;
            currentLang = Strings.CurrentLang;
            UpdateUI();
        }

        /// <summary>
        /// 更新界面文字
        /// </summary>
        private void UpdateUI()
        {
            Text = Strings.Get("Title");
            titleLabel.Text = Strings.Get("Title");
            subLabel.Text = Strings.Get("Subtitle");
            lblStatus.Text = Strings.Get("Ready");
            lblStatus.ForeColor = Color.FromArgb(150, 150, 150);
            btnCheck.Text = Strings.Get("Check");
            lblLanguage.Text = Strings.Get("Language");
        }

        /// <summary>
        /// 检测按钮点击事件（异步）
        /// </summary>
        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            // 禁用按钮，显示进度条
            btnCheck.Enabled = false;
            progressBar.Visible = true;
            lblStatus.Text = Strings.Get("Scanning");
            lblStatus.ForeColor = Color.Yellow;  // 黄色表示扫描中
            txtResult.Text = "";
            txtResult.ForeColor = Color.LightGreen;

            try
            {
                // 在后台线程执行检测
                CheckResult result = await Task.Run(() => CheckRuntimes());
                txtResult.Text = result.Text;

                if (result.Success)
                {
                    // 检测成功
                    lblStatus.Text = Strings.Get("Found");
                    lblStatus.ForeColor = Color.Lime;  // 绿色表示成功
                    txtResult.ForeColor = Color.Lime;
                    btnDownload.Visible = false;  // 隐藏下载按钮
                }
                else
                {
                    // 检测失败
                    lblStatus.Text = Strings.Get("NotFound");
                    lblStatus.ForeColor = Color.Red;  // 红色表示失败
                    txtResult.ForeColor = Color.Tomato;
                    // 只有在没有任何运行时时才显示下载按钮
                    btnDownload.Visible = !result.HasRuntimes;
                }
            }
            catch (Exception ex)
            {
                // 显示错误信息
                txtResult.Text = Strings.Get("Error") + ": " + ex.Message;
                txtResult.ForeColor = Color.Tomato;
                lblStatus.Text = Strings.Get("Error");
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                // 恢复按钮，隐藏进度条
                btnCheck.Enabled = true;
                progressBar.Visible = false;
            }
        }

        /// <summary>
        /// 执行运行时检测
        /// </summary>
        /// <returns>检测结果</returns>
        private CheckResult CheckRuntimes()
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.AppendLine(Strings.Get("Title2"));
            output.AppendLine();

            // 查找 dotnet.exe 路径
            string dotnetPath = FindDotnetExecutable();
            if (dotnetPath == null)
            {
                output.AppendLine(Strings.Get("NotFoundInPath"));
                return new CheckResult { Text = output.ToString(), Success = false, HasRuntimes = false };
            }

            output.AppendLine(Strings.Get("DotnetExe") + " " + dotnetPath);
            output.AppendLine();

            // 获取已安装的运行时列表
            List<RuntimeInfo> runtimes = GetInstalledRuntimes(dotnetPath);
            if (runtimes.Count == 0)
            {
                output.AppendLine(Strings.Get("NoRuntimes"));
                return new CheckResult { Text = output.ToString(), Success = false, HasRuntimes = false };
            }

            output.AppendLine(Strings.Get("InstalledRuntimes"));
            output.AppendLine("---------------------------");

            // 筛选 .NET 10+ 运行时
            List<RuntimeInfo> dotNet10Plus = runtimes.Where(r => r.VersionMajor >= 10).ToList();

            // 按版本降序排列显示
            foreach (RuntimeInfo runtime in runtimes.OrderByDescending(r => r.VersionMajor).ThenByDescending(r => r.Name))
            {
                string indicator = runtime.VersionMajor >= 10 ? "[10+]" : "    ";
                output.AppendLine(indicator + " " + runtime.Name);
                output.AppendLine("      " + Strings.Get("Version") + ": " + runtime.Version);
            }

            output.AppendLine();
            output.AppendLine(new string('-', 30));

            if (dotNet10Plus.Any())
            {
                // 存在 .NET 10+ 运行时
                output.AppendLine();
                output.AppendLine(string.Format(Strings.Get("Success"), dotNet10Plus.Count));
                return new CheckResult { Text = output.ToString(), Success = true, HasRuntimes = true };
            }
            else
            {
                // 不存在 .NET 10+ 运行时
                output.AppendLine();
                output.AppendLine(Strings.Get("Failed"));
                output.AppendLine(Strings.Get("FailedHint"));
                return new CheckResult { Text = output.ToString(), Success = false, HasRuntimes = true };
            }
        }

        /// <summary>
        /// 查找系统中的 dotnet.exe 路径
        /// </summary>
        /// <returns>dotnet.exe 路径，未找到返回 null</returns>
        private string FindDotnetExecutable()
        {
            // 首先从 PATH 环境变量中查找
            string pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (pathEnv != null)
            {
                string[] paths = pathEnv.Split(Path.PathSeparator);
                foreach (string path in paths)
                {
                    string dotnetExe = Path.Combine(path.Trim(), "dotnet.exe");
                    if (File.Exists(dotnetExe)) return dotnetExe;
                }
            }

            // 查找默认安装目录 (64位)
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string defaultPath = Path.Combine(programFiles, "dotnet", "dotnet.exe");
            if (File.Exists(defaultPath)) return defaultPath;

            // 查找 32位程序目录
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string x86Path = Path.Combine(programFilesX86, "dotnet", "dotnet.exe");
            if (File.Exists(x86Path)) return x86Path;

            return null;
        }

        /// <summary>
        /// 获取系统已安装的 .NET 运行时列表
        /// </summary>
        /// <param name="dotnetPath">dotnet.exe 路径</param>
        /// <returns>运行时信息列表</returns>
        private List<RuntimeInfo> GetInstalledRuntimes(string dotnetPath)
        {
            List<RuntimeInfo> runtimes = new List<RuntimeInfo>();

            // 有效的运行时名称集合（不区分大小写）
            HashSet<string> validRuntimes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Microsoft.NETCore.App",
                "Microsoft.WindowsDesktop.App",
                "Microsoft.AspNetCore.App"
            };

            try
            {
                // 配置进程启动信息
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = dotnetPath,
                    Arguments = "--list-runtimes",
                    RedirectStandardOutput = true,  // 重定向标准输出
                    RedirectStandardError = true,   // 重定向错误输出
                    UseShellExecute = false,
                    CreateNoWindow = true           // 不显示窗口
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    // 设置 3 秒超时
                    if (!process.WaitForExit(3000))
                    {
                        process.Kill();  // 超时则终止进程
                        throw new InvalidOperationException("dotnet 命令超时");
                    }

                    string output = process.StandardOutput.ReadToEnd();

                    // 检查命令是否成功执行
                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd();
                        throw new InvalidOperationException("dotnet 命令执行失败: " + error);
                    }

                    // 解析输出内容
                    string[] lines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            string name = parts[0];
                            string version = parts[1];

                            // 检查是否为有效的运行时
                            if (validRuntimes.Contains(name))
                            {
                                string[] versionParts = version.Split('.');
                                if (versionParts.Length >= 1)
                                {
                                    int majorVersion;
                                    if (int.TryParse(versionParts[0], out majorVersion) && majorVersion >= 5)
                                    {
                                        // 避免重复添加
                                        RuntimeInfo existing = runtimes.FirstOrDefault(r => r.Name == name && r.Version == version);
                                        if (existing == null)
                                        {
                                            runtimes.Add(new RuntimeInfo
                                            {
                                                Name = name,
                                                Version = version,
                                                VersionMajor = majorVersion
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                throw;  // 重新抛出已知异常
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("执行 dotnet --list-runtimes 失败", ex);
            }

            return runtimes;
        }
    }

    /// <summary>
    /// 运行时信息数据类
    /// </summary>
    class RuntimeInfo
    {
        /// <summary>运行时名称</summary>
        public string Name { get; set; }
        /// <summary>运行时版本</summary>
        public string Version { get; set; }
        /// <summary>主版本号</summary>
        public int VersionMajor { get; set; }
    }

    /// <summary>
    /// 检测结果数据类
    /// </summary>
    class CheckResult
    {
        /// <summary>结果文本</summary>
        public string Text { get; set; }
        /// <summary>是否检测到 .NET 10+</summary>
        public bool Success { get; set; }
        /// <summary>是否存在任何运行时</summary>
        public bool HasRuntimes { get; set; }
    }
}