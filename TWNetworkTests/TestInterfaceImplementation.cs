using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TWNetworkHelper;

namespace TWNetworkTests
{
    internal class TestInterfaceImplementation : InterfaceImplementer
    {
        public int Num1 { get; private set; } = -1;
        public double Num2 { get; private set; } = -1;

        public int Number { get; set; }
        public string Something { get; private set; } = null;
        public TestInterfaceImplementation() : base(typeof(TestInterface))
        {
        }

        private void Valami(int num1, double num2)
        { 
            Num1 = num1;
            Num2 = num2;
        }
        private bool Ez(string something)
        {
            if (something is null)
                return false;
            Something = something;
            return true;
        }

        private void Returning(ref object obj,int valami, out int something)
        {
            something = 2;
            obj = null;
            valami = 5;
        }
    }
}
