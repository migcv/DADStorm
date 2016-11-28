﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CommonTypes.operators {
    public class Custom : RemoteOperator {
        private string dll, className, method;

        public Custom(string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] parameters)
            : base("CUSTOM", inputSources, outputSources, routing, logLevel, parameters){
        }

        public override void doOperation(Tuple input) {
           if (parameters.Length == 3)                {
                this.dll = "C:/Users/Admin/Documents/GitHub/DADStorm/input/" + parameters[0];
                this.className = parameters[1];
                this.method = parameters[2];
            }

            byte[] code = File.ReadAllBytes(dll);
            Assembly assembly = Assembly.Load(code);
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass == true)
                    {
                        if (type.FullName.EndsWith("." + className))
                        {
                            // create an instance of the object
                            object ClassObj = Activator.CreateInstance(type);
                            object resultObject = type.InvokeMember(method,
                                BindingFlags.Default | BindingFlags.InvokeMethod, null, ClassObj,
                                new Object[] { input });
                            result = (Tuple)resultObject;
                        }
                    }
                }
            }
            catch
            (ReflectionTypeLoadException ex){
                // now look at ex.LoaderExceptions - this is an Exception[], so:
                foreach (Exception inner in ex.LoaderExceptions){
                    // write details of "inner", in particular inner.Message
                    Console.WriteLine(inner.Message);
                    result = input;
                }

            }
        }
    }
}
