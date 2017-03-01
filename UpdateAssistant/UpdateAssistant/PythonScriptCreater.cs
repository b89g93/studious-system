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
        public static bool createScript()
        {
            FileStream tempScript = new FileStream(PY_SCRIPT_FILE, FileMode.Create);
            StreamWriter sw = new StreamWriter(tempScript, System.Text.Encoding.UTF8);
            sw.Write(PythonUpdateFunctions.script_header);
            sw.Write(PythonUpdateFunctions.function_isManagerPath);
            sw.Write(PythonUpdateFunctions.function_isHasPostgresqlService);
            sw.Write(PythonUpdateFunctions.function_isManagerNode);

            sw.Write(PythonUpdateFunctions.call_Function("isManagerNode", 2, "\"C:\\\\\""));
            sw.Close();
            tempScript.Close();
            return true;
        }
    }
}
