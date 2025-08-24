namespace ThunderClientPatcher
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public static class BackupManager
    {
        public static bool CreateBackup(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string backupPath = GetBackupPath(filePath);

                if (File.Exists(backupPath))
                {
                    return false;
                }

                File.Copy(filePath, backupPath);
                MessageBox.Show("Backup created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool RestoreBackup(string filePath)
        {
            try
            {
                string backupPath = GetBackupPath(filePath);

                if (!File.Exists(backupPath))
                {
                    MessageBox.Show("No backup found to restore!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (File.Exists(filePath))
                    File.Delete(filePath);

                File.Copy(backupPath, filePath);
                MessageBox.Show("Backup restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Restore failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private static string GetBackupPath(string originalPath)
        {
            string directory = Path.GetDirectoryName(originalPath);
            string fileName = "backup_" + Path.GetFileName(originalPath);
            return Path.Combine(directory, fileName);
        }
    }
}
