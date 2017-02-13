using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateAssistant
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAddTerminateProcessName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewTermintProgress, textBoxTerminateProgressName.Text);
        }

        private void btnDeleteTerminateProcessName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewTermintProgress);
        }

        private void btnAddTerminateJavaProcessName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewTerminatJavaProgress, textBoxTerminateJavaProgressName.Text);
        }

        private void btnDeleteTerminateJavaProcessName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewTerminatJavaProgress);
        }

        private void btnAddBackupDirName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewBackupDirName, textBoxBackUpDirName.Text);
        }

        private void btnDeleteBackupDirName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewBackupDirName);
        }

        private void btnAddIgnoreBackupFile_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewIgnoreBackupFileName, textBoxIgnoreBackupFile.Text);
        }

        private void btnDeleteIgnoreBackupFile_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewIgnoreBackupFileName);
        }

        private void btnAddCleanDirName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewCleanDirName, textBoxCleanDirName.Text);
        }

        private void btnDeleteCleanDirName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewCleanDirName);
        }

        private void btnSelectUpdateFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSelectUpdateFile_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = "C://";

            fileDialog.Filter = "all files (*.*)|*.*|All files (*.*)|*.*";

            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = true;

            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (String fileName in fileDialog.SafeFileNames)
                {
                    UpdateInfoList.AddItem(listViewUpdateFileName, fileName);
                }
                

            }
        }

        private void btnDeleteUpdateFileName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewUpdateFileName);
        }
    }
}
