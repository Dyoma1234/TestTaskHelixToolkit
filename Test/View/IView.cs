using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public interface IView
    {
        string GetFilePath();
        void ShowError(string message);
    }
}
