using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

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
        private string COMPRESS_SCREPT_FILE = "temp.bat";

        private UpdatePackageConfig config = new UpdatePackageConfig();

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
            m_imgList.Images.Add(Image.FromFile("image/errFile.ico"));
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
            m_fileImageList.Images.Add(m_imgList.Images[1]); 
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
                listViewUpdateSourceDirInfo.Items[i + m_sourceDirCount].ImageIndex = i + 2;
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

        private void setPackageContents(UpdatePackageConfig config)
        {
            List<string> packageDirs = new List<string>();
            List<string> packageFiles = new List<string>();
            foreach (ListViewItem item in listViewUpdatePackageInfo.Items)
            {
                if (Convert.ToInt32(item.Tag) == DIR_TYPE)
                {
                    packageDirs.Add(item.Text);
                }
                else
                {
                    packageFiles.Add(item.Text);
                }
            }
            config.UpdatePackageContaint = new UpdatePackageConfigUpdatePackageContaint();
            config.UpdatePackageContaint.Dirs = packageDirs.ToArray();
            config.UpdatePackageContaint.Files = packageFiles.ToArray();
        }

        void setDBScriptFile(UpdatePackageConfig config)
        {
            List<string> listFiles = new List<string>();
            
            string dbScript_1 = textBoxDBUpdateScript_1.Text;
            string dbScript_2 = textBoxDBUpdateScript_2.Text;
            string dbScript_3 = textBoxDBUpdateScript_3.Text;
            string dbScript_4 = textBoxDBUpdateScript_4.Text;
            string dbScript_5 = textBoxDBUpdateScript_5.Text;
            string dbScript_6 = textBoxDBUpdateScript_6.Text;
            string dbScript_7 = textBoxDBUpdateScript_7.Text;
            string dbScript_8 = textBoxDBUpdateScript_8.Text;
            string dbScript_9 = textBoxDBUpdateScript_9.Text;
            string dbScript_10 = textBoxDBUpdateScript_10.Text;

            if(!dbScript_1.Equals(""))
            {
                listFiles.Add(dbScript_1);
            }
            if (!dbScript_2.Equals(""))
            {
                listFiles.Add(dbScript_2);
            }
            if (!dbScript_3.Equals(""))
            {
                listFiles.Add(dbScript_3);
            }
            if (!dbScript_4.Equals(""))
            {
                listFiles.Add(dbScript_4);
            }
            if (!dbScript_5.Equals(""))
            {
                listFiles.Add(dbScript_5);
            }
            if (!dbScript_6.Equals(""))
            {
                listFiles.Add(dbScript_6);
            }
            if (!dbScript_7.Equals(""))
            {
                listFiles.Add(dbScript_7);
            }
            if (!dbScript_8.Equals(""))
            {
                listFiles.Add(dbScript_8);
            }
            if (!dbScript_9.Equals(""))
            {
                listFiles.Add(dbScript_9);
            }
            if (!dbScript_10.Equals(""))
            {
                listFiles.Add(dbScript_10);
            }
            config.UpdateDBScriptFiles = listFiles.ToArray();
        }

        private void SaveAs(string fileName,string fileContent)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(fileContent);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        private void menuItemSaveAs_Click(object sender, EventArgs e)
        {
            
            config = new UpdatePackageConfig();

            //需要结束的进程
            List<string> listStr = UpdateInfoList.GetListContents(listViewTermintProgress);
            config.NeedTerminateProcessNames = listStr.ToArray();
            //需要结束的JAVA进程
            listStr = UpdateInfoList.GetListContents(listViewTerminatJavaProgress);
            config.NeedTerminateJavaProcessNames = listStr.ToArray();
            //需要备份的文件夹
            listStr = UpdateInfoList.GetListContents(listViewBackupDirName);
            config.NeedBackupDirNames = listStr.ToArray();
            //不需要备份的文件
            listStr = UpdateInfoList.GetListContents(listViewIgnoreBackupFileName);
            config.NeedIgnoreBackupFileNames = listStr.ToArray();

            //安装目录下需要清理的文件夹
            listStr = UpdateInfoList.GetListContents(listViewCleanDirName);
            config.NeedCleanInstallPathDirNames = listStr.ToArray();

            //需要启动的进程
            listStr = UpdateInfoList.GetListContents(listViewStartExecuteProcessName);
            config.NeedStartProgressNames = listStr.ToArray();

            //需要启动的windows服务
            listStr = UpdateInfoList.GetListContents(listViewStartWindowsServiceName);
            config.NeedStartWindowsServices = listStr.ToArray();

            //需要清理的临时文件夹
            listStr = UpdateInfoList.GetListContents(listViewCleanTempDir);
            config.NeedCleanTempDirs = listStr.ToArray();

            //需要清理的临时文件
            listStr = UpdateInfoList.GetListContents(listViewCleanTempFile);
            config.NeedCleanTempFiles = listStr.ToArray();

            setPackageContents(config);

            setDBScriptFile(config);

            string strConfig = XmlUtil.Serializer(config.GetType(), config);

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "project upl files   (*.upl)|*.upl";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream;
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    using (StreamWriter sw = new StreamWriter(myStream))
                    {
                        sw.Write(strConfig);
                    }

                    myStream.Close();
                }
            } 
            
        }

        private void fillConfigUI(string configFile)
        {
            FileStream fs = new FileStream(configFile, FileMode.Open);
            StreamReader sw = new StreamReader(fs);

            string xml = sw.ReadToEnd();

            sw.Close();
            fs.Close();

            config = (UpdatePackageConfig)XmlUtil.Deserialize(typeof(UpdatePackageConfig), xml);
            if (config == null)
            {
                MessageBox.Show("工程文件一毁坏！");
                return;
            }

            if (config.NeedTerminateProcessNames != null)
            {
                UpdateInfoList.AddItems(listViewTermintProgress, config.NeedTerminateProcessNames);
            }

            if (config.NeedTerminateJavaProcessNames != null)
            {
                UpdateInfoList.AddItems(listViewTerminatJavaProgress, config.NeedTerminateJavaProcessNames);
            }

            if (config.NeedBackupDirNames != null)
            {
                UpdateInfoList.AddItems(listViewBackupDirName, config.NeedBackupDirNames);
            }

            if (config.NeedIgnoreBackupFileNames != null)
            {
                UpdateInfoList.AddItems(listViewIgnoreBackupFileName, config.NeedIgnoreBackupFileNames);
            }

            if (config.NeedCleanInstallPathDirNames != null)
            {
                UpdateInfoList.AddItems(listViewCleanDirName, config.NeedCleanInstallPathDirNames);
            }

            if (config.NeedStartProgressNames != null)
            {
                UpdateInfoList.AddItems(listViewStartExecuteProcessName, config.NeedStartProgressNames);
            }

            if (config.NeedStartWindowsServices != null)
            {
                UpdateInfoList.AddItems(listViewStartWindowsServiceName, config.NeedStartWindowsServices);
            }

            if (config.NeedCleanTempDirs != null)
            {
                UpdateInfoList.AddItems(listViewCleanTempDir, config.NeedCleanTempDirs);
            }

            if (config.NeedCleanTempFiles != null)
            {
                UpdateInfoList.AddItems(listViewCleanTempFile, config.NeedCleanTempFiles);
            }

            if (config.UpdateDBScriptFiles != null)
            {
                for (int i = 0; i < config.UpdateDBScriptFiles.Length; i++)
                {
                    this.Controls.Find("textBoxDBUpdateScript_" + (i + 1).ToString(), true)[0].Text = config.UpdateDBScriptFiles[i];
                }
            }

            if (config.UpdatePackageContaint != null)
            {
                m_fileImageList.Images.Clear();
                m_fileImageList.Images.Add(m_imgList.Images[0]);
                m_fileImageList.Images.Add(m_imgList.Images[1]);
                if (listViewUpdatePackageInfo.LargeImageList == null)
                {
                    listViewUpdatePackageInfo.LargeImageList = m_fileImageList;
                }
                if (config.UpdatePackageContaint.Dirs != null)
                {
                    foreach (string dir in config.UpdatePackageContaint.Dirs)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Tag = DIR_TYPE;
                        item.Name = dir;
                        item.Text = dir;
                        item.ImageIndex = 0;
                        listViewUpdatePackageInfo.Items.Add(item);
                    }
                }
                if (config.UpdatePackageContaint.Files != null)
                {
                    int iconFileIndex = 1 ;
                    foreach (string file in config.UpdatePackageContaint.Files)
                    {
                        
                        ListViewItem item = new ListViewItem();
                        item.Tag = FILE_TYPE;
                        item.Name = file;
                        item.Text = file;
                        try
                        {
                            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(file);
                            m_fileImageList.Images.Add(icon);
                            item.ImageIndex = iconFileIndex + 1;
                            iconFileIndex++;
                        }
                        catch (Exception ex)
                        {
                            item.ImageIndex = 1;
                        }

                        
                        listViewUpdatePackageInfo.Items.Add(item);
                    }

                }
            }
        }

        private void cleanUI()
        {
            listViewTermintProgress.Items.Clear();
            listViewTerminatJavaProgress.Items.Clear();
            listViewBackupDirName.Items.Clear();
            listViewIgnoreBackupFileName.Items.Clear();
            listViewCleanDirName.Items.Clear();
            listViewUpdatePackageInfo.Items.Clear();
            listViewStartExecuteProcessName.Items.Clear();
            listViewStartWindowsServiceName.Items.Clear();
            listViewCleanTempDir.Items.Clear();
            listViewCleanTempFile.Items.Clear();

            textBoxDBUpdateScript_1.Text = "";
            textBoxDBUpdateScript_2.Text = "";
            textBoxDBUpdateScript_3.Text = "";
            textBoxDBUpdateScript_4.Text = "";
            textBoxDBUpdateScript_5.Text = "";
            textBoxDBUpdateScript_6.Text = "";
            textBoxDBUpdateScript_7.Text = "";
            textBoxDBUpdateScript_8.Text = "";
            textBoxDBUpdateScript_9.Text = "";
            textBoxDBUpdateScript_10.Text = "";

            m_fileImageList.Images.Clear();

            
        }
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = "C://";

            fileDialog.Filter = "upl files (*.upl)|*.upl|All files (*.*)|*.*";

            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = true;

            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                cleanUI();
                fillConfigUI(fileDialog.FileName);
            }

           

        }

        private void menuItemNew_Click(object sender, EventArgs e)
        {
            cleanUI();
        }

        private void menuItemQuit_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认退出吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                this.Close();
            }
            
        }

        private void toolStripOpen_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem_Click(sender, e);
        }

        private void toolStripSaveAs_Click(object sender, EventArgs e)
        {
            menuItemSaveAs_Click(sender, e);
        }

        private bool executeBatFile(string batFile)
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = batFile;
                proc.StartInfo.Arguments = string.Format("10");//this is argument
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                return false;
            }
            return true;
        }
        private void createCompressPackage(string packageName)
        {
            bool bRet = createCompressScript(packageName);
            if (bRet)
            {
                //直径打包脚本生成压缩包
                executeBatFile(COMPRESS_SCREPT_FILE);
            }
            
        }

        private string getFileName(string path)
        {
            int index = path.LastIndexOf("\\");
            if (index < 0)
            {
                return null;
            }
            return path.Substring(index+1);
        }

        private string getFilePath(string path)
        {
            int index = path.LastIndexOf("\\");
            if (index < 0)
            {
                return null;
            }
            return path.Substring(0, index+1);
        }

        private bool createCompressScript(string packageName)
        {
            if (config.UpdatePackageContaint.Dirs.Length == 0 &&
                config.UpdatePackageContaint.Files.Length == 0)
            {
                MessageBox.Show("内容列表为空");
                return false;
            }

            string rarExe = Directory.GetCurrentDirectory() + "\\WinRAR\\Rar.exe";
            string strContent = "";
            foreach (string dir in config.UpdatePackageContaint.Dirs)
            {
                string rootDriver = dir.Substring(0, 2);
                strContent += ("cd " + dir + "\\..");
                strContent += ("\r\n");
                strContent += (rootDriver + "\r\n");
                strContent += (rarExe + " a " + packageName + " " + getFileName(dir));
                strContent += ("\r\n");
            }
            foreach (string file in config.UpdatePackageContaint.Files)
            {
                string filePath = getFilePath(file);
                if (filePath != null)
                {
                    string rootDriver = file.Substring(0, 2);
                    strContent += ("cd " + filePath);
                    strContent += ("\r\n");
                    strContent += (rootDriver + "\r\n");
                    strContent += (rarExe + " a " + packageName + " " + getFileName(file));
                    strContent += ("\r\n");
                }
            }

            FileStream tempScript = new FileStream(COMPRESS_SCREPT_FILE, FileMode.Create);
            StreamWriter sw = new StreamWriter(tempScript, System.Text.Encoding.Default);
            sw.Write(strContent);
            sw.Close();
            tempScript.Close();

            return true;
            
            
        }


        private void menuItemCreateUpdatePackage_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "zip files   (*.zip)|*.zip";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string updatePackageName = saveFileDialog.FileName;
                createCompressPackage(updatePackageName);
            }

        }

        

        private void toolStripCreateUpdatePackage_Click(object sender, EventArgs e)
        {
            //menuItemCreateUpdatePackage_Click(sender,e);
            PythonScriptCreater.createScript();
            
        }

    }
}
