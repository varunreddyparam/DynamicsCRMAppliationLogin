using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateCRMRecords;

namespace CrudOperations
{
    public class Context : IDisposable
    {
        [ThreadStatic]
        private static Context _instance;
        [ThreadStatic]
        private static int _instanceCounter;

        public Service Service
        {
            get; set;
        }

        public static Context Instance()
        {
            if (_instanceCounter == 0)
            {
                _instance = new Context();
            }
            _instanceCounter++;
            return _instance;
        }

        private static void Release()
        {
            _instanceCounter--;
            if (_instanceCounter == 0)
            {
                if (_instance != null)
                    _instance.Dispose();
                _instance = null;
            }
        }

        public void Dispose()
        {
            Release();
        }


    }
}
