using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace UpdateAssistant
{
    public partial class Form1 : Form
    {
        private static int FILE_TYPE = 0;
        private static int DIR_TYPE = 1;  //文件类型
        private Stack<string> m_selectUpdateSourcePathStack;        //路径栈
        private ImageList m_fileImageList;        //系统提供的图标
        private ImageList m_imgList;              //文件夹图标
        private int m_sourceDirCount = 0;

        public Form1()
        {
            InitializeComponent();
            initConfig();
        }

        public void initConfig()
        {
            // 初始化imageList,drivesList
            m_fileImageList = new ImageList();
            m_imgList = new ImageList();
            m_imgList.Images.Add(Image.FromFile("image/folder.ico"));
            m_imgList.Images.Add(Image.FromFile("image/disk.png"));

            this.m_selectUpdateSourcePathStack = new Stack<string>();
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

        private void loadDirectoryInfosToListView(string rootPath)
        {
            m_selectUpdateSourcePathStack.Clear();
            // 记录访问信息
            if (m_selectUpdateSourcePathStack.Count == 0 || m_selectUpdateSourcePathStack.Peek() != rootPath)
                m_selectUpdateSourcePathStack.Push(rootPath);

            //locationTextBox.Text = rootPath;
            // 清空listview
            listViewUpdateSourceDirInfo.Items.Clear();
            listViewUpdateSourceDirInfo.View = View.Tile;
            listViewUpdateSourceDirInfo.TileSize = new Size(600, 25);
            m_fileImageList.Images.Clear();
            m_fileImageList.Images.Add(m_imgList.Images[0]);
            listViewUpdateSourceDirInfo.LargeImageList = m_fileImageList;
            string[] directoryInfo = Directory.GetDirectories(rootPath);
            //加载文件夹
            List<ListViewItem> deletedList = new List<ListViewItem>();
            for (int i = 0; i < directoryInfo.Length; i++)
            {
                ListViewItem item = new ListViewItem();
                try
                {
                    item.Text = directoryInfo[i].Substring(directoryInfo[i].LastIndexOf('\\') + 1);
                    item.Name = directoryInfo[i];
                    item.Tag = DIR_TYPE;
                    item.ImageIndex = 0;
                    listViewUpdateSourceDirInfo.Items.Add(item);
                    // 判断是否有访问权限
                    Directory.GetDirectories(directoryInfo[i]);
                }
                catch (Exception)
                {   // 没有访问权限，则记录
                    deletedList.Add(item);
                }
            }
            // 删除没有访问权限的节点
            foreach (ListViewItem item in deletedList)
                item.Remove();

            //// 加载文件
            m_sourceDirCount = listViewUpdateSourceDirInfo.Items.Count;
            string[] fileInfo = Directory.GetFiles(rootPath);
            for (int i = 0; i < fileInfo.Length; i++)
            {
                listViewUpdateSourceDirInfo.Items.Add(fileInfo[i].Substring(fileInfo[i].LastIndexOf('\\') + 1));
                Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(fileInfo[i]);
                m_fileImageList.Images.Add(icon);
                listViewUpdateSourceDirInfo.Items[i + m_sourceDirCount].ImageIndex = i + 1;
                listViewUpdateSourceDirInfo.Items[i + m_sourceDirCount].Tag = FILE_TYPE;
                listViewUpdateSourceDirInfo.Items[i + m_sourceDirCount].Name = fileInfo[i];
            }
        }

        private void btnAddToUpdatePackageList_Click(object sender, EventArgs e)
        {
            listViewUpdatePackageInfo.LargeImageList = m_fileImageList;
            foreach (ListViewItem item in listViewUpdateSourceDirInfo.SelectedItems)
            {
                ListViewItem targetItem = new ListViewItem();
                targetItem.Text = item.Name;
                targetItem.Name = item.Name;
                targetItem.Tag = item.Tag;
                targetItem.ImageIndex = item.ImageIndex;

                listViewUpdatePackageInfo.Items.Add(targetItem);
            }
        }

        private void btnSelectUpdateFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                textBoxSourceDir.Text = foldPath;
                loadDirectoryInfosToListView(foldPath);
            }
        }


        private void btnDeleteUpdateFileName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewUpdatePackageInfo);
        }

        private void selectFileToTextbox(TextBox textBox)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = "C://";

            fileDialog.Filter = "all files (*.*)|*.*|All files (*.*)|*.*";

            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = true;

            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = fileDialog.FileName;
            }
        }

        private void btnAddDBUpdateScript_1_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_1);
        }

        private void btnAddDBUpdateScript_2_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_2);
        }

        private void btnAddDBUpdateScript_3_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_3);
        }
        

        private void btnAddDBUpdateScript_5_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_5);
        }

        private void btnAddDBUpdateScript_6_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_6);
        }

        private void btnAddDBUpdateScript_7_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_7);
        }

        private void btnAddDBUpdateScript_8_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_8);
        }

        private void btnAddDBUpdateScript_9_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_9);
        }

        private void btnAddDBUpdateScript_10_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_10);
        }

        private void btnAddDBUpdateScript_4_Click(object sender, EventArgs e)
        {
            selectFileToTextbox(textBoxDBUpdateScript_4);
        }

        private void btnAddStartProcessName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewStartExecuteProcessName, textBoxStartProcessName.Text);
        }

        private void btnDeleteProcessName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewStartExecuteProcessName);
        }

        private void btnStartWindowsServiceName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewStartWindowsServiceName, textBoxStartWindowsServiceName.Text);
        }

        private void btnDeleteWindowsServiceName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewStartWindowsServiceName);
        }

        private void btnAddCleanTempDirName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewCleanTempDir, textBoxCleanTempDirName.Text);
        }
        private void btnDeleteCleanTempDirName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewCleanTempDir);
        }

        private void btnAddCleanTempFileName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.AddItem(listViewCleanTempFile, textBoxCleanTempFileName.Text);
        }

        private void btnDeleteCleanTempFileName_Click(object sender, EventArgs e)
        {
            UpdateInfoList.DeleteSelectedItems(listViewCleanTempFile);
        }
        
    }
}
