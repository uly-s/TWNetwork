using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNetworkTests
{
    internal interface TestInterface
    {
        int Number { get; set; }
        void Valami(int num1,double num2);
        bool Ez(string something);

        void Returning(ref object obj,out int something);
    }
}
