using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace dk.calc.functions
{
    public static class helper_functions
    {
        public static bool is_operator(string input)
        {
            // check if input is an operator
            List<string> operator_list = new List<string>() { "+", "-", "/", "*" };
            List<string> exp_op_list = new List<string>() { "^", "log", "root" };

            if (operator_list.Contains(input) ||
                exp_op_list.Contains(input))
                return true;
            else 
                return false;
        }

        public static bool is_clap(string input)
        {
            // check if input is a clap
            List<char> clap_list = new List<char>() { '(', ')' };
            if (input.Length == 1)
            {
                if (clap_list.Contains(input[0]))
                    return true;
                else
                    return false;
            }
            else
                return false;

        }

        public static bool is_digit(string input)
        {
            if (input.Length == 1 && Char.IsDigit(input[0]))
                return true;
            else
                return false;
        }

        public static int factorial(int x) {
            if (x < 0)
                return -1;
            else if (x == 1 || x == 0)
                return 1;
            else
                return x * factorial(x - 1);
        }

        public static double get_epsilon() { return 0.00000001; }

        public static T CopyObj<T>(this object source) {
            using (MemoryStream stream = new MemoryStream()) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);

            }
             
        }
    }



}
