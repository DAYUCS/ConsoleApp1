using CSScriptLibrary;
using System;
using System.Reflection;

namespace ConsoleApp1
{
    class Program
    {
        /* Load dll into remote application domain
        static void Main(string[] args)
        {
            string assemblyPath = "D:\\MSProjects\\ConsoleApp1\\ClassLibrary1\\bin\\Debug\\ClassLibrary1.dll";

            for (int i = 0; i < 100000; i++)
            {
                AppDomain appDomain = AppDomain.CreateDomain("remoteDomain");
                ObjectHandle objectHandle = appDomain.CreateInstanceFrom(assemblyPath, "ClassLibrary1.Class1");

                object obj = objectHandle.Unwrap();
                
                int result = (int)obj.GetType().InvokeMember("Sum", BindingFlags.InvokeMethod, Type.DefaultBinder, obj, new object[] {i, 1});

                Console.WriteLine("-- --");
                Console.WriteLine(result);

                appDomain.Unload();
            }

        }
        */

        /* Load dll into current application domain
        static void Main(string[] args)
        {
            const BindingFlags MyBinding = (BindingFlags)(-20);

            Assembly asm = Assembly.LoadFrom("D:\\MSProjects\\ConsoleApp1\\ClassLibrary1\\bin\\Debug\\ClassLibrary1.dll");
            object obj = asm.CreateInstance("ClassLibrary1.Class1", true, MyBinding, null, null, null, null);
         
            for (int i = 0; i < 100000; i++)
            {
                // Test load dll again and again...
                //Assembly asm = Assembly.LoadFrom("D:\\MSProjects\\ConsoleApp1\\ClassLibrary1\\bin\\Debug\\ClassLibrary1.dll");
                //object obj = asm.CreateInstance("ClassLibrary1.Class1", true, MyBinding, null, null, null, null);

                int result = (int)obj.GetType().GetMethod("Sum", MyBinding).Invoke(null, new object[] {i, 1});

                Console.WriteLine("-- --");
                Console.WriteLine(result);

                //asm = null;
                //obj = null;
            }

        }
        */

        /* CS Script Remote Application Domain example
        static void Main(string[] args)
        {
        
            CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;

            var code = @"int Sum(int a, int b)
                             {
                                 dynamic d = a + 1;
                                 return d + b;
                             }";

            

            for (int i = 0; i < 100000; i++)
            {
                var sum = CSScript.Evaluator
                            .CreateDelegateRemotely<int>(code);
                
                int result = sum(i, 1);

                Console.WriteLine("i:{0}  --  result:{1}", i, result);
                
                sum.UnloadOwnerDomain();
            }

        }
        */

        //* CS Script Current Application Domain example
        static void Main(string[] args)
        {

            CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;
            //CSScript.EvaluatorConfig.Access = EvaluatorAccess.Singleton;
            //CSScript.GlobalSettings.InMemoryAssembly = true;

            string code1 = @"using System;
                            public class Script1
                            {
                                public static int Sum(int a, int b)
                                {
                                    dynamic d = a + 1;
                                    return d + b;
                                }
                            }";

            string code2 = @"using System;
                            public class Script2
                            {
                                public static int Sum(int a, int b)
                                {
                                    dynamic d = a - 1;
                                    return d + b;
                                }
                            }";

            Assembly asm1 = CSScript.Evaluator.CompileCode(code1);
            Assembly asm2 = CSScript.Evaluator.CompileCode(code2);
            //string asm1File = asm10.Location;
            //string asm2File = asm20.Location;

            int result;
            for (int i = 0; i < 100000; i++)
            {
                if ((i % 2) == 0)
                {
                    Console.WriteLine("Script1");
                    //Assembly asm1 = CSScript.Evaluator.CompileCode(code1);
                    //Assembly asm1 = Assembly.LoadFrom(asm1File);
                    Type type = asm1.GetType("Script1");
                    object instance = Activator.CreateInstance(type);
                    result = (int) type.InvokeMember("Sum", BindingFlags.InvokeMethod, null, instance, new object[] { i, 1 });
                    //AsmHelper helper = new AsmHelper(asm1);
                    //object instance = helper.CreateObject("Script1");
                    //FastInvokeDelegate methodInvoker = helper.GetMethodInvoker("Script1.Sum", new object[] { i, 1 });
                    //result = (int) methodInvoker(instance, i, 1);
                } else 
                {
                    Console.WriteLine("Script2");
                    //Assembly asm2 = CSScript.Evaluator.CompileCode(code2);
                    //Assembly asm2 = Assembly.LoadFrom(asm2File);
                    Type type = asm2.GetType("Script2");
                    object instance = Activator.CreateInstance(type);
                    result = (int)type.InvokeMember("Sum", BindingFlags.InvokeMethod, null, instance, new object[] { i, 1 });
                    //AsmHelper helper = new AsmHelper(asm2);
                    //object instance = helper.CreateObject("Script2");
                    //FastInvokeDelegate methodInvoker = helper.GetMethodInvoker("Script2.Sum", new object[] { i, 1 });
                    //result = (int)methodInvoker(instance, i, 1);
                }

                Console.WriteLine("i:{0}  --  result:{1}", i, result);

            }

        }

    }
}
