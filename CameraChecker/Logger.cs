using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraCheckerApp
{
    class Logger
    {
        public static void WriteLog(String lines)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("camerachecker.log", true);
            String dateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            file.WriteLine(dateTime + " " + lines);

            file.Close();

        }
    }
}
