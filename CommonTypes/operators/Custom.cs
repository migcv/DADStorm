using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CommonTypes.operators {
    public class Custom : RemoteOperator {
        private string dll, className, method;
        private Type t;
        private MethodInfo m;

        public Custom() { }

        public Custom(string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] parameters)
            : base("CUSTOM", inputSources, outputSources, routing, logLevel, parameters){
            try{
                if (parameters.Length == 3){
                    this.dll = "../../../input/" + parameters[0];
                    this.className = parameters[1];
                    this.method = parameters[2];
                }
            }
            catch { }
        }

        public override void doOperation(Tuple input) {
            try{

                byte[] code = File.ReadAllBytes(dll);
                Assembly assembly = Assembly.Load(code);

                foreach (Type type in assembly.GetTypes()){
                    if (type.IsClass == true){
                        if (type.FullName.EndsWith("." + className)){
                            // create an instance of the object
                            object ClassObj = Activator.CreateInstance(type);
                            object resultObject = type.InvokeMember("CustomOperation",
                                BindingFlags.Default | BindingFlags.InvokeMethod, null, ClassObj,
                                new Object[] { input });
                            result = (Tuple)resultObject;
                        }
                    }
                }
            }
            catch {
				result = input;
			}
        }
    }
}
