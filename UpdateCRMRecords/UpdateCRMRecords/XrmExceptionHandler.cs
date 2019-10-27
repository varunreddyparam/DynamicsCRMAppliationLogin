using System;
using System.Diagnostics;
using System.ServiceModel.Dispatcher;

namespace UpdateCRMRecords
{
    public class XrmExceptionHandler:ExceptionHandler
    {
        public override bool HandleException(Exception ex)
        {
            // This method contains logic to decide whether 
            // the exception is serious enough
            // to terminate the process.
            return ShouldTerminateProcess(ex);
        }

        public bool ShouldTerminateProcess(Exception ex)
        {
            // Write your logic here.
            return false;
        }

        public void CreateLog(string log, EventLogEntryType eventLog)
        {
            Console.WriteLine(log+ eventLog.ToString());
        }
    }
}
