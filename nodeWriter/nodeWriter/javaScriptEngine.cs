using Jint;
using Jint.Native;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace nodeWriter
{

    public class JScore
    {
        private Engine engine;
        private List<Program> JSprograms = new List<Program>();

        public JScore()
        {
            try
            {
                engine = new Engine().SetValue("log", new Action<object>(Console.WriteLine));
            }
            catch (Exception i)
            {
                //Log.RecordError(i);
            }
        }



        public bool executeJSCode(string code)
        {
            try
            {
                engine.Execute(code);
                return true;
            }
            catch (Exception i)
            {
                //Log.RecordError(i);
                return false;
            }

        }

        public void setValues(List<Tuple<string, string, object>> setValues, ref Engine e)
        {
            foreach (var val in setValues)
            {
                switch (val.Item2)
                {
                    case "int32":
                        e.SetValue(val.Item1, Convert.ToInt32(val.Item3));
                        break;

                    case "int64":
                        e.SetValue(val.Item1, Convert.ToInt64(val.Item3));
                        break;

                    case "double":
                        e.SetValue(val.Item1, Convert.ToDouble(val.Item3));
                        break;

                    case "string":
                        e.SetValue(val.Item1, Convert.ToString(val.Item3));
                        break;

                    case "float":
                        e.SetValue(val.Item1, float.Parse((string)val.Item3));
                        break;

                    case "bool":
                        e.SetValue(val.Item1, Convert.ToBoolean(val.Item3));
                        break;

                    case "object":
                        e.SetValue(val.Item1, val.Item3);
                        break;

                    case "Undefined":
                        e.SetValue(val.Item1, JsValue.Undefined);
                        break;
                }
            }
        }

        public bool executeJSCode(string code, JSInputVal input)
        {
            try
            {
                var e = engine.Execute(code);
                return true;
            }
            catch (Exception i)
            {
                //Log.RecordError(i);
                return false;
            }

        }

        public object executeJSprogramWithReturn(string code)
        {
            try
            {
                Engine e = engine.Execute(code);
                var ret = e.GetCompletionValue().ToObject();
                return ret;
            }
            catch (Exception i)
            {
                return null;
            }
        }

        public object executeJSprogramWithReturn(Program p)
        {
            try
            {
                Engine e = engine.Execute(p);
                var ret = e.GetCompletionValue().ToObject();
                return ret;
            }
            catch (Exception i)
            { 
                return null;
            }
        }
    }

    public class JSInputVal
    {

    }


}
