using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ThunderClientPatcher
{
    public partial class MainForm : Form
    {
        private const string FUNC_QF_PATTERN = @"(static\s+QF\s*\(\s*t\s*\)\s*\{)(\s*var\s+e\s*=\s*\{\s*isError:\s*!0,\s*hasSubs:\s*!1,\s*offline:\s*!1\s*\};[\s\S]*?return\s+e;\s*)(\})";
        private const string FUNC_UQ_PATTERN = @"(static\s+uq\s*\(\s*\)\s*\{)(\s*return\s+g\.rg\s*==\s*this\.Jj\(m\.Ts\.Zj\)[\s\S]*?h\.Ha\.Cb;\s*)(\})";

        private readonly string FUNC_QF_PATCHED_B64 = "JDENCiAgICB2YXIgZSA9IHsNCiAgICAgICAgaXNFcnJvcjogZmFsc2UsDQogICAgICAgIGhhc1N1YnM6IHRydWUsDQogICAgICAgIG9mZmxpbmU6IHRydWUsDQogICAgICAgIHRva2VuOiAiRkFLRVRva2VuQkxBQkxBQkxBIiwNCiAgICAgICAgbmFtZTogIlxuRW50ZXJwcmlzZSBFZGl0aW9uIHwgVW5vZmZpY2lhbCBMaWNlbnNlIFByb3Zpc2lvbmVkIGJ5IEdpdEh1YkBSRTB4MCIsDQogICAgICAgIHJlZmNvZGU6ICJGQ0syMDI1QllQQVNTIiwNCiAgICAgICAgcGxhbjogIkVudGVycHJpc2UgVmVyc2lvbiBMaWZldGltZSAgXG4gUHJvdmlkZWQgQnkgR2l0aHViQFJFMHgwIiwNCiAgICAgICAgZXhwaXJ5OiAiMjA5OS0xMi0zMVQyMzo1OTo1OS45OTlaIg0KICAgIH07DQogICAgcmV0dXJuIGU7DQokMw==";
        private readonly string FUNC_UQ_PATCHED_B64 = "JDEKICAgICAgICAgICAgcmV0dXJuIGguSGEuUGI7CiQz";

        private string _extensionBasePath;
        private const string EXTENSION_PUBLISHER = "rangav";
        private const string EXTENSION_NAME = "vscode-thunder-client";

        private const string PACKAGE_JSON_FILE_NAME = "package.json";
        private const string EXTENSION_JS_PATH = "dist\\extension.js";

        private readonly List<string> _supportedVersions = new List<string>
        {
            "2.37.5","2.37.6","2.37.7"
        };

        private string _version;

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
            ShowLegalDisclaimer();
            CheckExtension();

        }

        private void CheckExtension()
        {
            try
            {
                if (!FindExtension() && !GetExtensionRootDirectory())
                {
                    SetStatus("Thunder Client Not Found");
                    return;
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
                return false;

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
                string qfReplacement = Encoding.UTF8.GetString(Convert.FromBase64String(FUNC_QF_PATCHED_B64));
                string uqReplacement = Encoding.UTF8.GetString(Convert.FromBase64String(FUNC_UQ_PATCHED_B64));

                content = Regex.Replace(content, FUNC_QF_PATTERN, qfReplacement, RegexOptions.Singleline);
                content = Regex.Replace(content, FUNC_UQ_PATTERN, uqReplacement, RegexOptions.Singleline);

                File.WriteAllText(extensionJsFullPath, content);
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
        "This patcher only works with Thunder Client v2.37.5-8.\n\n" +
        "Please download the supported version from the official source or\n" +
        "Download the supported version from my Github repo Github@RE0x0\n" +
        "Do you want to proceed anyway?"
, "Unsupported Version",
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