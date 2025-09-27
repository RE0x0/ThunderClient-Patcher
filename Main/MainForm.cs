using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ThunderClientPatcher
{
    public partial class MainForm : Form
    {
        private Dictionary<string, string> _functionReplacements = new Dictionary<string, string>();
        private List<string> _supportedVersions = new List<string>();
        private string _extensionBasePath;
        private const string EXTENSION_PUBLISHER = "rangav";
        private const string EXTENSION_NAME = "vscode-thunder-client";
        private const string PACKAGE_JSON_FILE_NAME = "package.json";
        private const string EXTENSION_JS_PATH = "dist\\extension.js";
        private string _version;
        private string _currentAppVersion = "1.1.0";
        private string _latestAppVersion = "";
        private const string REPO_SETTINGS_URL = "https://raw.githubusercontent.com/RE0x0/ThunderClient-Patcher/refs/heads/main/settings.json";

        public MainForm()
        {
            InitializeComponent();
        }
        private void SetStatus(string status)
        {
            lbStatus.Text = $"Status : {status}";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
            ShowLegalDisclaimer();
            CheckExtension();
        }

        private void LoadSettings()
        {
            SetDefaultSettings();
        }
        private async Task<bool> FetchSettingsFromRepo()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    string jsonContent = await httpClient.GetStringAsync(REPO_SETTINGS_URL);

                    var serializer = new JavaScriptSerializer();
                    var settings = serializer.Deserialize<Dictionary<string, object>>(jsonContent);

                    if (settings.ContainsKey("function_replacements") &&
                        settings.ContainsKey("supported_versions") &&
                        settings.ContainsKey("app_version"))
                    {
                        var replacements = (Dictionary<string, object>)settings["function_replacements"];
                        var versionsList = (System.Collections.ArrayList)settings["supported_versions"];
                        _latestAppVersion = settings["app_version"].ToString();

                        _functionReplacements.Clear();
                        foreach (var replacement in replacements)
                        {
                            _functionReplacements.Add(replacement.Key, replacement.Value.ToString());
                        }

                        _supportedVersions.Clear();
                        foreach (var version in versionsList)
                        {
                            _supportedVersions.Add(version.ToString());
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatus("Failed to fetch settings from repo, using default settings");
            }
            return false;
        }
        private void CheckForAppUpdate()
        {
            if (_latestAppVersion != _currentAppVersion)
            {
                MessageBox.Show(
                    $"A new version of Thunder Client Patcher is available!\n\n" +
                    $"Current Version: {_currentAppVersion}\n" +
                    $"Latest Version: {_latestAppVersion}\n\n" +
                    "Please visit my GitHub repo to download the latest version.",
                    "Update Available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        private void SetDefaultSettings()
        {
            _functionReplacements = new Dictionary<string, string>
            {
                {
                    "static $F(){return g.Yv==this.Mj(m.Ss.Lj)?h.Ba.jb:g.Yv==this.Mj(m.Ss.qj)?h.Ba.Eb:g.Yv==this.Mj(m.Ss.zE)?h.Ba.Sb:g.Yv==this.Mj(m.Ss.LE)?h.Ba.Tb:g.Yv==this.Mj(m.Ss.Bj)?h.Ba.xb:h.Ba.kb}",
                    "static $F(){return h.Ba.xb;}"
                },
                {
                    "static MF(t){var e={isError:!0,hasSubs:!1,offline:!1};try{let n=l.Ej.Gj(t),i=JSON.parse(n);let r=this.TF(i,!0);r.xF?(e.errorMessage=r.pL,e.hasSubs=!1,l.oo.tf(r.pL,r.OF)):(t=i.token,e.token=t,e.name=i.name,e.hasSubs=i.active,e.refcode=i.refcode,e.plan=i.plan,e.offline=i.offline,e.expiry=i.expiry,e.isError=!1)}catch(t){e.errorMessage=h.Rp.Lv}return e}",
                    "static MF(t){var e={isError:false,hasSubs:true,offline:true,token:\"FakeTokenRE0x0\",name:\"\\nEnterprise Edition | Unofficial License Provisioned by GitHub@RE0x0\",refcode:\"TC2025BYPASSLOL\",plan:\"Enterprise Version\",expiry:\"2099-12-31T23:59:59.999Z\"};return e}"
                }
            };

            _supportedVersions = new List<string>
            {
                "2.37.8"
            };
        }
        private async void CheckExtension()
        {
            try
            {
                if (!FindExtension())
                {
                    SetStatus("Thunder Client Not Found, attempting to fetch latest settings...");
                    if (await FetchSettingsFromRepo())
                    {
                        SetStatus("Successfully fetched latest settings, revalidating extension...");
                        CheckForAppUpdate();
                        if (!FindExtension())
                        {
                            SetStatus("Thunder Client Not Found with updated settings, opening dialog...");
                            if (!GetExtensionRootDirectory())
                            {
                                SetStatus("Thunder Client Not Found");
                                return;
                            }
                        }
                    }
                    else
                    {
                        SetStatus("Failed to fetch settings, opening dialog...");
                        if (!GetExtensionRootDirectory())
                        {
                            SetStatus("Thunder Client Not Found");
                            return;
                        }
                    }
                }

                ReadPackageInfo();
                EnableControls();
                CreateBackup();
                SetStatus("Thunder Client found successfully");
                lbVersion.Text = $"Version : {_version}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"An error occurred: {ex.Message}");
            }
        }
        private bool FindExtension()
        {
            string extensionsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".vscode", "extensions");

            if (!Directory.Exists(extensionsPath))
                return false;

            foreach (string version in _supportedVersions)
            {
                string extensionDir = Path.Combine(extensionsPath, $"{EXTENSION_PUBLISHER}.{EXTENSION_NAME}-{version}");
                if (Directory.Exists(extensionDir) &&
                    File.Exists(Path.Combine(extensionDir, PACKAGE_JSON_FILE_NAME)))
                {
                    _extensionBasePath = extensionDir;
                    return true;
                }
            }

            return false;
        }
        private bool ValidateExtensionPath()
        {
            return Directory.Exists(_extensionBasePath) &&
                   File.Exists(Path.Combine(_extensionBasePath, PACKAGE_JSON_FILE_NAME));
        }
        private bool GetExtensionRootDirectory()
        {
            var folderDialog = new FolderBrowserDialog
            {
                Description = "Select the root directory of the Thunder Client extension",
                RootFolder = Environment.SpecialFolder.Desktop,
                ShowNewFolderButton = false
            };

            if (folderDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            _extensionBasePath = folderDialog.SelectedPath;

            if (ValidateExtensionPath())
                return true;

            ShowWarningMessage("The selected folder does not contain a valid Thunder Client extension.");
            return false;
        }
        private void ReadPackageInfo()
        {
            string jsonString = File.ReadAllText(Path.Combine(_extensionBasePath, PACKAGE_JSON_FILE_NAME));
            var serializer = new JavaScriptSerializer();
            var data = serializer.Deserialize<Dictionary<string, object>>(jsonString);

            if (data.TryGetValue("version", out object versionObj))
            {
                _version = versionObj.ToString();
            }
        }
        private bool Patch()
        {
            string extensionJsFullPath = Path.Combine(_extensionBasePath, EXTENSION_JS_PATH);
            try
            {
                if (BackupManager.CreateBackup(extensionJsFullPath))
                {
                    SetStatus("Backed up successfully");
                }
                string content = File.ReadAllText(extensionJsFullPath);
                foreach (var replacement in _functionReplacements)
                {
                    if (!content.Contains(replacement.Key))
                    {
                        return false;
                    }
                }
                string patchedContent = content;
                foreach (var replacement in _functionReplacements)
                {
                    patchedContent = patchedContent.Replace(replacement.Key, replacement.Value);
                }

                File.WriteAllText(extensionJsFullPath, patchedContent);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void EnableControls()
        {
            btnPatch.Enabled = true;
            btnRestore.Enabled = true;
        }

        private void ShowLegalDisclaimer()
        {
            MessageBox.Show(
                "⚠️  LEGAL DISCLAIMER - EDUCATIONAL USE ONLY\n\n" +
                "This application is created for educational and research purposes only.\n\n" +
                "I am NOT responsible for:\n" +
                "• Any illegal or unauthorized use of this software\n" +
                "• Any damages or issues caused by using this patcher\n" +
                "• Any violation of terms of service or licensing agreements\n\n" +
                "If you find Thunder Client useful and use it regularly,\n" +
                "please support the original author by purchasing a license\n" +
                "from the official source.\n\n" +
                "Use at your own risk. By continuing, you acknowledge that you understand\n" +
                "this software is for educational purposes only and you assume all responsibility.",
                "Legal Disclaimer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(message,
                "Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void btnPatch_Click(object sender, EventArgs e)
        {
            if (!_supportedVersions.Contains(_version))
            {
                var msgresult = MessageBox.Show(
                    "❌ Unsupported Version Detected!\n\n" +
                    "This patcher only works with Thunder Client v2.37.5-7.\n\n" +
                    "Please download the supported version from the official source or\n" +
                    "Download the supported version from my Github repo Github@RE0x0\n" +
                    "Do you want to proceed anyway?",
                    "Unsupported Version",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (msgresult != DialogResult.Yes)
                {
                    SetStatus("Operation cancelled by user");
                    return;
                }
            }

            bool result = Patch();
            SetStatus(result ? "Patched successfully" : "Something went wrong!");
        }
        private void CreateBackup()
        {
            if (BackupManager.CreateBackup(Path.Combine(_extensionBasePath, EXTENSION_JS_PATH)))
            {
                SetStatus("Backed up successfully");
            }
        }
        private void RestoreBackup()
        {
            if (BackupManager.RestoreBackup(Path.Combine(_extensionBasePath, EXTENSION_JS_PATH)))
            {
                SetStatus("Restored successfully");
            }
        }
        private void btnRestore_Click(object sender, EventArgs e)
        {
            RestoreBackup();
        }

        private void lbMyRepo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/RE0x0");
        }
    }
}