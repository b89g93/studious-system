using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UpdateAssistant
{
    class PythonScriptCreater
    {
        public static string PY_SCRIPT_FILE = "upgrade.py";
        public static bool createScript(UpdatePackageConfig config)
        {
            FileStream tempScript = new FileStream(PY_SCRIPT_FILE, FileMode.Create);
            StreamWriter sw = new StreamWriter(tempScript, System.Text.Encoding.UTF8);
            sw.Write(PythonUpdateFunctions.script_header);
            sw.Write(PythonUpdateFunctions.function_isManagerPath);
            sw.Write(PythonUpdateFunctions.function_isHasPostgresqlService);
            sw.Write(PythonUpdateFunctions.function_isManagerNode);

            sw.Write(PythonUpdateFunctions.function_terminateJavaProgress);
            sw.Write(PythonUpdateFunctions.function_terminateCProgress);
            sw.Write(PythonUpdateFunctions.function_xcopyFiles);

            sw.Write(PythonUpdateFunctions.function_copyFile);
            sw.Write(PythonUpdateFunctions.function_recDeleteFolder);
            sw.Write(PythonUpdateFunctions.function_addPassToPGConf);
            sw.Write(PythonUpdateFunctions.function_excuteDBScript);
            sw.Write(PythonUpdateFunctions.function_updateCopy);

            sw.Write(PythonUpdateFunctions.update_TerminateProgress(config.NeedTerminateProcessNames,config.NeedTerminateJavaProcessNames));
            sw.Write(PythonUpdateFunctions.update_Backup(config.NeedBackupDirNames, config.NeedIgnoreBackupFileNames));
            sw.Write(PythonUpdateFunctions.update_Delete(config.NeedCleanInstallPathDirNames));
            sw.Write(PythonUpdateFunctions.update_Copy(config.UpdatePackageContaint.Dirs, config.UpdatePackageContaint.Files));
            sw.Write(PythonUpdateFunctions.update_DBUpdate(config.UpdateDBScriptFiles));
            sw.Write(PythonUpdateFunctions.update_StartProgress(config.NeedStartProgressNames, config.NeedStartWindowsServices));
            sw.Write(PythonUpdateFunctions.update_Clean(config.NeedCleanTempDirs, config.NeedCleanTempFiles));

            sw.Close();
            tempScript.Close();
            return true;
        }
    }
}
