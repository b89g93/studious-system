using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UpdateAssistant
{
    class PythonUpdateFunctions
    {
        public const string FN_isManagerPath = "isManagerPath";
        public const string FN_isHasPostgresqlService = "isHasPostgresqlService";
        public const string FN_isManagerNode = "isManagerNode";
        public const string FN_terminateJavaProgress = "terminateJavaProgress";
        public const string FN_terminateCProgress = "terminateCProgress";
        public const string FN_xcopyFiles = "xcopyFiles";
        public const string FN_copyFile = "copyFile";
        public const string FN_recDeleteFolder = "recDeleteFolder";
        public const string FN_addPassToPGConf = "addPassToPGConf";
        public const string FN_excuteDBScript = "excuteDBScript";

        public static string script_header = @"# -*- coding: utf-8 -*-
successStatus='success'
updateProcesses = {}
def getUpdateProcesses():
    return updateProcesses
import os
import shutil
import time
updateProcesses[0] = ('update_TerminateProgress','终止进程')
updateProcesses[1] = ('update_Backup','备份原始版本')
updateProcesses[2] = ('update_Delete','清理原始版本')
updateProcesses[3] = ('update_Copy','升级拷贝')
updateProcesses[4] = ('update_DBUpdate','DB升级')
updateProcesses[5] = ('update_StartProgress','启动进程')
updateProcesses[6] = ('update_Clean','清理临时文件')
";
        private static string getTab(int tabCount)
        {
            string str = "";
            for (int i = 0; i < tabCount; i++)
            {
                str += "    ";
            }
            return str;
        }

        public static string call_Function(string functionName, int tabCount, params string[] param)
        {
            string tab = getTab(tabCount);
            string strParams = "";
            for (int i = 0; i < param.Length; )
            {
                strParams += param[i];
                i++;
                if (i < param.Length)
                {
                    strParams += ",";
                }
            }
            string function = functionName + "( " + strParams + " )\n";
            return tab + function;
        }

        public static string function_isManagerPath = @"def isManagerPath(installDir):
    dirs = os.listdir(installDir)
    isHasAppServer = False
    isHasDB = False
    for file in dirs:
        if(file == 'AppServer'):
            isHasAppServer = True
        elif(file == 'DB'):
            isHasDB = True
    return isHasAppServer and isHasDB
";

        public static string function_isHasPostgresqlService = @"def isHasPostgresqlService():
    output = os.popen('sc query postgresql')
    stdLine = output.read()
    return stdLine.find('RUNNING') > 0
";

        public static string function_isManagerNode = @"def isManagerNode(installDir):
    pathOk = isPathIsManagerPath(installDir)
    serviceOk = isHasPostgresqlService()
    print 'path OK == [ ' + str(pathOk) + ' ] Service OK [ ' + str(serviceOk) + ' ]'
    return pathOk and serviceOk
";

        public static string function_terminateJavaProgress = @"def terminateJavaProgress(windowTitle):
" +
"    killCmd = \"taskkill /f /im java.exe /fi \\\"WINDOWTITLE eq \" + windowTitle + \"\\\"\"\n" +
"    ret = os.system(killCmd)\n" +
@"    return ret
";
        public static string function_terminateCProgress = @"def terminateCProgress(name):
    print '终止进程'
    os.system('taskkill /F /IM ' + name)
";

        public static string function_xcopyFiles = @"def xcopyFiles(sourceDir, targetDir):
" +
"    tempCopyFilesList = targetDir + \"tempFiles.copylist\"\n" +
"    com = \'xcopy \"\' + sourceDir + \'\" \"\' + targetDir + \'\" /r /s /h /E/I/D/Y > \"\' + tempCopyFilesList +\'\"\'\n" +
@"    os.system(com)
    os.remove(tempCopyFilesList)
";

        public static string function_copyFile = @"def copyFile(sourceFile, targetDir):
    tempCopyFileList = targetDir  + '\\tempFile.copylist'
" +
"    com = \'xcopy \"\' + sourceFile + \'\" \"\' + targetDir + \'\"  /f /y /c >  \"\' + tempCopyFileList + \'\"\'\n" +
@"    print com
    os.system(com)
    os.remove(tempCopyFileList)
";

        public static string function_recDeleteFolder = @"def recDeleteFolder(src):
    if os.path.isfile(src):  
        try:
            print 'remove ' + src
            os.remove(src)  
        except:
            print 'remove ' + src + 'failed'
            pass 
    elif os.path.isdir(src):  
        for item in os.listdir(src):  
            itemsrc=os.path.join(src,item)  
            delete_file_folder(itemsrc)  
        try:
            print 'remove ' + src
            os.rmdir(src)  
        except:
            print 'remove ' + src + 'failed'
            pass 
";

        public static string function_addPassToPGConf = @"def addPassToPGConf():
    appDataPath = os.environ['APPDATA']
    print 'App Data Path [ ' + appDataPath + ' ]'
    postgreDataPath = os.path.join(appDataPath,'postgresql')
    if( False == os.path.exists( postgreDataPath ) ):
        os.makedirs(postgreDataPath)
    confPath = os.path.join(postgreDataPath,'pgpass.conf')
    print confPath
    if( False == os.path.exists( confPath ) ):
        f = open(confPath,'w')
        f.write('127.0.0.1:5432:*:postgres:postgres\n')
        f.close()
    else:
        f = open(confPath,'a')
        f.write('127.0.0.1:5432:*:postgres:postgres\n')
        f.close()
";

        public static string function_excuteDBScript = @"def excuteDBScript(installPath,uncompressPath,dbUpdateScriptFile):
    addPassToPGConf()
    upgrade = os.path.join(uncompressPath,dbUpdateScriptFile)
    sqlexePath = os.path.join(installPath,'DB\\9.3\\bin')
    curpath = os.path.abspath(os.curdir)
    os.chdir(sqlexePath)
" +
"    init = \'psql.exe -h 127.0.0.1 -p 5432 -U postgres -d leovideo -f \"\' + upgrade+ \'\"\'\n" +
@"    print init
    output = os.popen(init)
    ret = output.read()
    os.chdir(curpath)
    print ret
";

        public static string update_TerminateProgress(string[] cProgress, string[] javaProgress)
        {
            string function = "def update_TerminateProgress(installPath,uncompressPath):\n";
            function += "    print '终止进程'\n";
            if (cProgress != null)
            {
                foreach (string pro in cProgress)
                {
                    function += call_Function(FN_terminateCProgress, 1, pro);
                }
            }

            if (javaProgress != null)
            {
                foreach (string pro in javaProgress)
                {
                    function += call_Function(FN_terminateJavaProgress, 1, pro);
                }
            }
            
            function += @"    time.sleep(1)
    return success
";
            return function;
        }

        public static string getStringArray(string[] strs)
        {
            if (strs == null)
            {
                return @"[ '' ]";
            }
            string strArray = "[ ";
            foreach (string str in strs)
            {
                strArray += "\"";
                strArray += str;
                strArray += "\"";
                strArray += ",";
            }
            strArray += "\"\" ]";
            return strArray;
        }
        public static string update_Backup(string[] needBackDirs, string[] needIgnorFiles)
        {
            string function = "def update_Backup(installPath,uncompressPath):\n";
            function += "    print \'备份原始版本\'\n";
            function += ("    needBackDirs = "+ getStringArray(needBackDirs) + "\n");
            function += ("    needIgnorFiles = " + getStringArray(needIgnorFiles) + "\n");
            function += "    installPath = installPath + '\\\\'\n";
            function += "    localtime = time.strftime('%Y-%m-%d %H-%M-%S',time.localtime())\n";
            function += "    allFiles = os.listdir(installPath)\n";
            function += "    localPath = uncompressPath\n";
            function += "    localPathParent = os.path.abspath(os.path.join(os.path.dirname(localPath)))\n";
            function += @"    backUpPath = os.path.join(str(localPathParent),str(localtime),'Backup')
    if not os.path.exists(backUpPath):
        print 'Create backUp Path [ ' + backUpPath + ' ]'
        os.makedirs(backUpPath)
    for eachFile in allFiles:
        absPathFile = os.path.join(installPath,eachFile)
        
        if os.path.isdir(absPathFile):
            if eachFile in needBackDirs:
                targetPath = os.path.join(backUpPath,eachFile)
                if not os.path.exists(targetPath):
                    print 'Create Path [ ' + targetPath + ' ]'
                    os.makedirs(targetPath)
                print 'Copy ' + absPathFile + ' to ' + targetPath
                xcopyFiles(absPathFile,targetPath)
        else:
            if eachFile not in needIgnorFiles:
                targetPath = os.path.join(backUpPath,eachFile)
                print 'Copy ' + absPathFile + ' to' + targetPath
                copyFile(absPathFile,backUpPath)
    return 'success'
";
            return function;
            
        }

        public static string update_Delete(string[] needCleanDirs)
        {
            string function = "def update_Delete(installPath,uncompressPath):\n";
            function += "    print '清理原始版本'\n";
            function += "    installPath = installPath + '\\\\'\n";
            function += "    allFile = os.listdir(installPath)\n";
            function += ("    deleteArr = " + getStringArray(needCleanDirs) + "\n");
            function += @"    for eachFile in allFile:
        if eachFile in deleteArr:
            deletePath = os.path.join(installPath,eachFile)
            recDeleteFolder(deletePath)
    time.sleep(1)
    return 'success'
";
            return function;
        }


        public static string function_updateCopy = @"def updateCopy(uncompressPath,installPath,needCopyDirs,needCopyFiles):
    source = uncompressPath + '\\'
    fileList = os.listdir(source)
    target = installPath + '\\'
    if not os.path.exists(target):
        os.makedirs(target)
    for fileItem in fileList:
        fileItemAbsPath = os.path.join(source,fileItem)
        if os.path.isdir(fileItemAbsPath):
            if fileItem in needCopyDirs:
                targetPath = os.path.join(target,fileItem)
                if not os.path.exists(targetPath):
                    os.makedirs(targetPath)
                xcopyFiles(fileItemAbsPath,targetPath)
        else:
            if fileItem in needCopyFiles:
                copyFile(fileItemAbsPath,target)
    return 'success'
";
        public static string update_Copy(string[] needCopyDirs, string[] needCopyFiles)
        {
            string needCopyDirArr = getStringArray(needCopyDirs);
            string needCopyFileArr = getStringArray(needCopyFiles);
            string function = @"def update_Copy(installPath,uncompressPath):
    print '升级拷贝'
    needCopyDirs = " + needCopyDirArr + "\n    needCopyFiles = " + needCopyFileArr + @"
    updateCopy(installPath,uncompressPath,needCopyDirs,needCopyFiles)
    return 'success'
";
            return function;
        }

        public static string update_DBUpdate(string[] dbScriptFiles)
        {
            string[] scriptFileNames = new string[dbScriptFiles.Length];
            for(int i=0;i<dbScriptFiles.Length;i++)
            {
                scriptFileNames[i] = System.IO.Path.GetFileName(dbScriptFiles[i]);   
            }
            string scriptFileArr = getStringArray(scriptFileNames);
            string function = "def update_DBUpdate(installPath,uncompressPath):\n";
            function += "    print 'DB升级'\n";
            function += ("    scriptFileArr = " + scriptFileArr + "\n");
            function += "    if(isManagerNode(installPath)):\n";
            function += "        for scriptFile in scriptFileArr:\n";
            function += "            excuteDBScript(installPath,uncompressPath,scriptFile)\n";
            function += "    time.sleep(3)\n";
            function += "    return 'success'\n";
            return function;
        }

        public static string update_StartProgress(string[] needStartAppNames, string[] needStartWindowsServices)
        {
            string startProgres = "";
            string startWinService = "";
            foreach(string fileName in needStartAppNames)
            {
                startProgres += "    os.system('" + fileName + "')\n";   
            }
            foreach (string service in needStartWindowsServices)
            {
                startWinService += "    os.system('sc start " + service + "')\n";
            }
            string function = @"def update_StartProgress(installPath,uncompressPath):
    print '启动进程'
    installPath = installPath + '\\'
    os.chdir(installPath)
" + startProgres + startWinService + @"
    time.sleep(2)
    return 'success'
";
            return function;
        }

        public static string update_Clean(string[] needCleanTempDirs, string[] needCleanTempFiles)
        {
            string function = @"def update_Clean(installPath,uncompressPath):
    print '清理临时文件'
    installPath = installPath + '\\'
    localPath = uncompressPath
    localPathParent = os.path.abspath(os.path.join(os.path.dirname(localPath)))
    print localPathParent
    allFile = os.listdir(localPathParent)
    for eachFile in allFile:
        print eachFile
        backupIndex = eachFile.find('Backup')
        if(backupIndex > 0):
            needDeleteDir = localPathParent + '\\' + eachFile;
            print needDeleteDir
            shutil.rmtree(needDeleteDir)
    needCleanTempDirs = " + getStringArray(needCleanTempDirs) + @"
    needCleanTempFiles = " + getStringArray(needCleanTempFiles) + @"
    for dirItem in needCleanTempDirs:
        deleteDir = os.path.join(installPath,dirItem)
        recDeleteFolder(deleteDir)
    for fileItem in needCleanTempFiles:
        deleteFile = os.path.join(installPath,fileItem)
        os.remove(deleteFile)
    time.sleep(1)
    return 'success'
";
            return function;
        }
        
    }
}
