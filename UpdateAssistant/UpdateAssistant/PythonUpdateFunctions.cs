using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateAssistant
{
    class PythonUpdateFunctions
    {
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
            string function = functionName + "( " + strParams + " )";
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

    }
}
