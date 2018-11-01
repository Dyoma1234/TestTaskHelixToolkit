using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Test
{
    class Model:IModel
    {
        public byte[] LoadFromFile (string path)
        {
            try
            {
              byte[] buffer = null;
             
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        int x = 0;
                        buffer = new byte[fs.Length];
                        while (x != fs.Length)
                        {
                            x = fs.Read(buffer, 0, (int)fs.Length);
                        }
                    }
                
                return buffer;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
