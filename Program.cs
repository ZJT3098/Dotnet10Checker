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
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    enum Language { zhCN, zhTW, en }

    static class Strings
    {
        private static int _currentIndex;
        public static int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
                CurrentLang = (Language)value;
            }
        }
        public static Language CurrentLang { get; private set; } = Language.zhCN;

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

    class MainForm : Form
    {
        private TextBox txtResult;
        private Button btnCheck;
        private Button btnDownload;
        private ProgressBar progressBar;
        private Label lblStatus;
        private ComboBox cboLanguage;
        private Label lblLanguage;
        private Label titleLabel;
        private Label subLabel;
        private Language currentLang = Language.zhCN;
        private const string DotNet10DownloadUrl = "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/10.0.7/windowsdesktop-runtime-10.0.7-win-x86.exe";

        public MainForm()
        {
            Text = Strings.Get("Title");
            ClientSize = new Size(500, 400);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 30);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            titleLabel = new Label
            {
                Text = Strings.Get("Title"),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            subLabel = new Label
            {
                Text = Strings.Get("Subtitle"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(20, 55),
                AutoSize = true
            };

            lblLanguage = new Label
            {
                Text = Strings.Get("Language"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(320, 30),
                AutoSize = true
            };

            cboLanguage = new ComboBox
            {
                Location = new Point(320, 52),
                Size = new Size(160, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cboLanguage.Items.AddRange(new object[] { "简体中文", "繁體中文", "English" });
            cboLanguage.SelectedIndex = 0;
            cboLanguage.SelectedIndexChanged += CboLanguage_SelectedIndexChanged;

            lblStatus = new Label
            {
                Text = Strings.Get("Ready"),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(20, 85),
                AutoSize = true
            };

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
            btnCheck.FlatAppearance.BorderSize = 0;
            btnCheck.Click += BtnCheck_Click;

            btnDownload = new Button
            {
                Text = Strings.Get("Download"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(160, 115),
                Size = new Size(160, 35),
                Visible = false
            };
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.Click += BtnDownload_Click;

            progressBar = new ProgressBar
            {
                Location = new Point(160, 125),
                Size = new Size(320, 20),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

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

            Controls.AddRange(new Control[] { titleLabel, subLabel, lblLanguage, cboLanguage, lblStatus, btnCheck, btnDownload, progressBar, txtResult });
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = DotNet10DownloadUrl,
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private void CboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Strings.CurrentIndex = cboLanguage.SelectedIndex;
            currentLang = Strings.CurrentLang;
            UpdateUI();
        }

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

        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            btnCheck.Enabled = false;
            progressBar.Visible = true;
            lblStatus.Text = Strings.Get("Scanning");
            lblStatus.ForeColor = Color.Yellow;
            txtResult.Text = "";
            txtResult.ForeColor = Color.LightGreen;

            try
            {
                CheckResult result = await Task.Run(() => CheckRuntimes());
                txtResult.Text = result.Text;

                if (result.Success)
                {
                    lblStatus.Text = Strings.Get("Found");
                    lblStatus.ForeColor = Color.Lime;
                    txtResult.ForeColor = Color.Lime;
                    btnDownload.Visible = false;
                }
                else
                {
                    lblStatus.Text = Strings.Get("NotFound");
                    lblStatus.ForeColor = Color.Red;
                    txtResult.ForeColor = Color.Tomato;
                    btnDownload.Visible = !result.HasRuntimes;
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = Strings.Get("Error") + ": " + ex.Message;
                txtResult.ForeColor = Color.Tomato;
                lblStatus.Text = Strings.Get("Error");
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnCheck.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private CheckResult CheckRuntimes()
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.AppendLine(Strings.Get("Title2"));
            output.AppendLine();

            string dotnetPath = FindDotnetExecutable();
            if (dotnetPath == null)
            {
                output.AppendLine(Strings.Get("NotFoundInPath"));
                return new CheckResult { Text = output.ToString(), Success = false, HasRuntimes = false };
            }

            output.AppendLine(Strings.Get("DotnetExe") + " " + dotnetPath);
            output.AppendLine();

            List<RuntimeInfo> runtimes = GetInstalledRuntimes(dotnetPath);
            if (runtimes.Count == 0)
            {
                output.AppendLine(Strings.Get("NoRuntimes"));
                return new CheckResult { Text = output.ToString(), Success = false, HasRuntimes = false };
            }

            output.AppendLine(Strings.Get("InstalledRuntimes"));
            output.AppendLine("---------------------------");

            List<RuntimeInfo> dotNet10Plus = runtimes.Where(r => r.VersionMajor >= 10).ToList();

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
                output.AppendLine();
                output.AppendLine(string.Format(Strings.Get("Success"), dotNet10Plus.Count));
                return new CheckResult { Text = output.ToString(), Success = true, HasRuntimes = true };
            }
            else
            {
                output.AppendLine();
                output.AppendLine(Strings.Get("Failed"));
                output.AppendLine(Strings.Get("FailedHint"));
                return new CheckResult { Text = output.ToString(), Success = false, HasRuntimes = true };
            }
        }

        private string FindDotnetExecutable()
        {
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

            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string defaultPath = Path.Combine(programFiles, "dotnet", "dotnet.exe");
            if (File.Exists(defaultPath)) return defaultPath;

            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string x86Path = Path.Combine(programFilesX86, "dotnet", "dotnet.exe");
            if (File.Exists(x86Path)) return x86Path;

            return null;
        }

        private List<RuntimeInfo> GetInstalledRuntimes(string dotnetPath)
        {
            List<RuntimeInfo> runtimes = new List<RuntimeInfo>();
            HashSet<string> validRuntimes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Microsoft.NETCore.App",
                "Microsoft.WindowsDesktop.App",
                "Microsoft.AspNetCore.App"
            };

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = dotnetPath,
                    Arguments = "--list-runtimes",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    if (!process.WaitForExit(3000))
                    {
                        process.Kill();
                        throw new InvalidOperationException("dotnet command timed out");
                    }

                    string output = process.StandardOutput.ReadToEnd();

                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd();
                        throw new InvalidOperationException("dotnet command failed: " + error);
                    }

                    string[] lines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            string name = parts[0];
                            string version = parts[1];

                            if (validRuntimes.Contains(name))
                            {
                                string[] versionParts = version.Split('.');
                                if (versionParts.Length >= 1)
                                {
                                    int majorVersion;
                                    if (int.TryParse(versionParts[0], out majorVersion) && majorVersion >= 5)
                                    {
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
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to run dotnet --list-runtimes", ex);
            }

            return runtimes;
        }
    }

    class RuntimeInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public int VersionMajor { get; set; }
    }

    class CheckResult
    {
        public string Text { get; set; }
        public bool Success { get; set; }
        public bool HasRuntimes { get; set; }
    }
}