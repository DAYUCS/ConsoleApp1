using System;

namespace ClassLibrary1
{
    public class Class1 : MarshalByRefObject
    {
        public Class1() { }

        public int Sum(int a, int b)
        {
            dynamic d = a + 1;
            return d + b;
        }
    }
}
