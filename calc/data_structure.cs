using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dk.calc.functions;

namespace dk.calc.dto
{
    [Serializable]
    abstract class CalcEquationElement {
        // abstract class to hold all input elements in one list
      
    }
    [Serializable]
    class CalcScientificOperator : CalcEquationElement {
        // special case of equation element which processes a number or equation enclosed by it
        // e.g. cos, sin, tan, faculty
        // consist of two elements, which enclose the number to process

        private string operator_ = "";

        public string get_operator() { return operator_; }
        Dictionary<string, string> op_output_dict;

        public CalcScientificOperator(string op) {
            operator_ = op;

            op_output_dict = new Dictionary<string, string> {
                { "s(", "sin(" },
                { ")s", ")"},
                { "c(", "cos(" },
                { ")c", ")"},
                { "t(", "tan(" },
                { ")t", ")"},
                { "n(", "(" },
                { ")n", ")!"},
                { "(", "("},
                { ")", ")"},
                { "e(", "("},
                { ")e", ")^"},
                { "l(", "ln("},
                { ")l", ")" },
                { "r(", "("},
                { ")r", ")root"}
            };

        }


        public override string ToString() {
            // return string representation of the internal operator
            if (op_output_dict.ContainsKey(operator_))
            {
                string op_to_string = op_output_dict[operator_];

                return op_to_string;
            } else
            {
                return "";
            }
     
        }
        

    }

    [Serializable]
    class CalcOperator : CalcEquationElement
    {
        // child class to implement operator input, including exponential, point and line operators, as well as paranthesis

        private string operator_ = "";

        public CalcOperator(string op) {
            operator_ = op;
        }

        override public string ToString()
        {
            return operator_.ToString();
        }

        public string get_operator() {
            return operator_;
        }

        public CalcOperator(CalcOperator copy_source) {
            this.operator_ = copy_source.operator_;
        }

        public bool is_point_operator() {
            if (operator_ == "*" ||
                operator_ == "/")
            {
                return true;
            }
            else
                return false;
        }

        public bool is_line_operator() {
            if (operator_ == "-" ||
                operator_ == "+")
            {
                return true;
            }
            else
                return false;

        }

        public bool is_exp_operator() {
            if (operator_ == "log" ||
                operator_ == "^" ||
                operator_ == "root")
            {
                return true;
            }
            else
                return false;

        }
    }
    [Serializable]
    class CalcNumber : CalcEquationElement
    {
        // child class to implement number input
        // internaly saved as string builder for consecutive addition of digits

        public CalcNumber() {
            number_sb_ = new StringBuilder();
            number_sb_.Append('0');
        }


        public CalcNumber(char n) {
            number_sb_ = new StringBuilder();
            number_sb_.Append(n);
        }

        public CalcNumber(double d) {
            number_sb_ = new StringBuilder();
            number_sb_.Append(d);
        }

        public void add_digit(char d) {
            if (number_sb_.Length == 1 && number_sb_.ToString() == "0")
            {
                number_sb_.Clear();
                number_sb_.Append(d);
            }
            else {
                number_sb_.Append(d);

            }

        }

        public double get_double() {
            
            return double.Parse(number_sb_.ToString());
        }

        public void add_comma() {
            if (!number_sb_.ToString().Contains(','))
                number_sb_.Append(',');
        }
        StringBuilder number_sb_;
        bool positive_ = true;

        override public string ToString() {
            if (positive_) {
                // scientific output if calcnumber's length > 20
                if (!number_sb_.ToString().Contains(",") && number_sb_.Length > 20) {
                    int length = number_sb_.Length;
                    var sb = new StringBuilder();
                    sb.Append(number_sb_[0]);
                    sb.Append(",");
                    for (var i = 1; i < 6; i++) {
                        sb.Append(number_sb_[i]);
                    }
                    sb.Append("e");
                    sb.Append(length - 1);

                    return sb.ToString();
                }
                return number_sb_.ToString();

            }
            else {
                string str = "-" + number_sb_.ToString();
                return str;
            }
                
        } 

    }
    [Serializable]
    class CalcInputEquation {

        // class to hold all equation elements as list
        // contains functions for calculation and substitution of "sublists" and the whole list

        string calc_output = "";

        Dictionary<string, Tuple<string, string>> sci_op_dict_;

        private List<CalcEquationElement> equation_elements;

        private string mode = "RAD";

        // discribes an equation
        public string to_string() {
            StringBuilder rtn = new StringBuilder();
            foreach (var i in equation_elements) {
                rtn.Append(i.ToString());
            }
            return rtn.ToString();
        }

        public override string ToString()
        {
            return this.to_string();
        }

        public CalcInputEquation() {
            equation_elements = new List<CalcEquationElement>();
            equation_elements.Add(new CalcNumber());

            // setup sci_op dictionary
            sci_op_dict_ = new Dictionary<string, Tuple<string, string>> {
                { "sin", Tuple.Create("s(",")s")},
                { "cos", Tuple.Create("c(",")c")},
                { "tan", Tuple.Create("t(",")t")},
                { "n!", Tuple.Create("n(",")n")},
                { "ln", Tuple.Create("l(",")l")}
            };
        }



        public void clear_last_entry() {
            equation_elements = new List<CalcEquationElement>();
            equation_elements.Add(new CalcNumber());

        }

        public void set_mode(string mod) {
            this.mode = mod;
        }

        public string get_mode() {
            return this.mode;
        }


        public void add_operator(string op) {
            // check last equation element if an operator is a vaild input

            if (equation_elements.Count() >0 && op == "(" && equation_elements.Last() is CalcNumber) {
                double num = (equation_elements.Last() as CalcNumber).get_double();
                if (num == 0.0) {
                    equation_elements.RemoveAt(equation_elements.Count() - 1);
                }
                equation_elements.Add(new CalcOperator(op));

                return;
            }
                
            if (equation_elements.Last() is CalcNumber || // last element is a number
                (equation_elements.Last() is CalcScientificOperator) || // last scientific operator 
                (equation_elements.Last() is CalcOperator && op == "(") ||
                (equation_elements.Last() is CalcOperator && (equation_elements.Last() as CalcOperator).get_operator() == ")"))
            {             
                equation_elements.Add(new CalcOperator(op));
            }
        }

        public void add_digit(char op) {
            // check previous elements for valid input

            if (equation_elements.Last() is CalcNumber)
            {
                (equation_elements.Last() as CalcNumber).add_digit(op);
            }
            else if (equation_elements.Last() is CalcOperator) {
                equation_elements.Add(new CalcNumber(op));
            }
        }

        public void add_number(double num) {
            if (equation_elements.Last() is CalcOperator)
                equation_elements.Add(new CalcNumber(num));
            else if (equation_elements.Count()>0 && equation_elements.Last() is CalcNumber && (equation_elements.Last() as CalcNumber).get_double() == 0.0) {
                equation_elements.RemoveAt(equation_elements.Count() - 1);
                equation_elements.Add(new CalcNumber(num));

            }
        }

        public void add_comma() {
            if (equation_elements.Last() is CalcNumber) {
                (equation_elements.Last() as CalcNumber).add_comma();
            }
        }

        public void add_sci_op(string sci_op) {
            // cos, sin, tan, n!, ln,
            // add braces and scientific operator

            // dict with sci_op and string representation for lookup


            // normal cases - n!, ln, cos, sin, tan
            var sci_op_elements = sci_op_dict_[sci_op];


            if (equation_elements.Count > 0 &&
                equation_elements.Last() is CalcNumber)
            {
                // if the last element is a number
                equation_elements.Insert(equation_elements.Count - 1, new CalcScientificOperator(sci_op_elements.Item1));
                equation_elements.Add(new CalcScientificOperator(sci_op_elements.Item2));

            }
            else if (equation_elements.Count > 0 &&
              equation_elements.Last() is CalcOperator &&
              (equation_elements.Last() as CalcOperator).get_operator() == ")")
            {
                // if the last element is a ending brace
                // ending braces, search for start and apply sci_op
                for (var i = equation_elements.Count - 1; i+1 > 0; i--)
                {
                    if (equation_elements[i] is CalcOperator &&
                        (equation_elements[i] as CalcOperator).get_operator() == "(")
                    {
                        equation_elements.Insert(i, new CalcScientificOperator(sci_op_elements.Item1));
                        equation_elements.Add(new CalcScientificOperator(sci_op_elements.Item2));
                    }

                }

            }
            else if (equation_elements.Last() is CalcScientificOperator) {
                //find start of the last element
                string last_op = (equation_elements.Last() as CalcScientificOperator).get_operator();
                char op_type = last_op[1];
                string start_op = op_type.ToString() + "(";



                //Console.WriteLine("look sci reverse " + start_op);

                for (var i = equation_elements.Count - 1; i+1 > 0; i--) {
                    if (equation_elements[i] is CalcScientificOperator &&
                        (equation_elements[i] as CalcScientificOperator).get_operator() == start_op) {
                        equation_elements.Insert(i, new CalcScientificOperator(sci_op_elements.Item1));
                        equation_elements.Add(new CalcScientificOperator(sci_op_elements.Item2));
                    }
                }
            }
                
  
        
        }

        public void calculate() {

            // save equation in list, make a new list with result as first element
            List<CalcEquationElement> l = new List<CalcEquationElement>(equation_elements);

            double result = 0.0;

            bool braces_found = false;
            while (!braces_found)
            {
                braces_found = brace_lookup(ref l);
            }

            l = arithmetic_calc(l);

            bool sci_braces_found = false;
            while (!sci_braces_found)
            {
                sci_braces_found = sci_lookup(ref l);
            }

            l = arithmetic_calc(l);


            if (l.Count() > 0 && l[0] is CalcNumber)
            {
                result = (l[0] as CalcNumber).get_double();

                equation_elements.Clear();
                equation_elements.Add(new CalcNumber(result));
            }
        }

        public string get_result() {
            // calculates result of the equation
            // gets called after every input

            List<CalcEquationElement> l = new List<CalcEquationElement>(equation_elements);

            bool braces_found = false;
            while (!braces_found) {
                braces_found = brace_lookup(ref l);
            }
          
            l = arithmetic_calc(l);

            bool sci_braces_found = false;
            while (!sci_braces_found) {
                sci_braces_found = sci_lookup(ref l);
            }

            l = arithmetic_calc(l);


            if (l.Count()>0)
                calc_output = l[0].ToString();

            return calc_output;
        }
  
        public List<CalcEquationElement> arithmetic_calc(List<CalcEquationElement> equation) {
           // calculation of the equation
           // iterate over the equation, solving exponential, point and line arithmetic in order
            bool done = false;
            if (equation.Count > 2)
            {

                while (!done) {
                    done = exponential_lookup(equation);
                }
                done = false;

                while (!done)
                {
                    done = point_arithmetic_lookup(equation);
                }
                done = false;

                while (!done)
                {
                    done = line_arithmetic_lookup(equation);
                }
                
            }
            return equation;
        }

        private bool sci_lookup(ref List<CalcEquationElement> equation)
        {
            for (var i = 0; i < equation.Count; i++)
            {
                if (equation[i] is CalcScientificOperator)
                {
                    string current_sci_op = (equation[i] as CalcScientificOperator).get_operator();

                    if (current_sci_op.Contains(')'))
                    {

                        int end_idx = i;
                        string ending_op = current_sci_op;
                        char sci_op = ending_op[1];

                        for (var j = i - 1; j >-1; j--)
                        {

                            if (equation[j] is CalcScientificOperator)
                            {


                                if (sci_op == (equation[j] as CalcScientificOperator).get_operator()[0])
                                {
                                    // call function with value of idx, j is first index, end_idx the last
                                    if (j + 1 == i - 1)
                                    {
                                        //just one value, calculate
                                        double result = sci_calc(sci_op.ToString(), (equation[j + 1] as CalcNumber).get_double());
                                        equation.RemoveRange(j, 3);
                                        equation.Insert(j, new CalcNumber(result));

                                    }
                                    else {
                                        // calculate content of paranthesis first
                                        equation = arithmetic_calc(equation);
                                        //return true;
                                    }
                                        

                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public double sci_calc(string op, double value) {

            double return_value = 0;

            double trig_val = value;

            if (this.get_mode() == "DEG")
                trig_val = Math.PI * value / 180.0;

            switch (op) {

                case "s":
                    double sin = Math.Sin(trig_val);
                    return_value = (Math.Abs(sin) < helper_functions.get_epsilon()) ? 0.0 : sin;
                    break;
                case "c":
                    double cos = Math.Cos(trig_val);
                    return_value = (Math.Abs(cos) < helper_functions.get_epsilon()) ? 0.0 : cos;
                    break;
                case "t":
                    double tan = Math.Sin(trig_val);
                    return_value = (Math.Abs(tan) < helper_functions.get_epsilon()) ? 0.0 : tan;
                    break;
                case "n":
                    return_value = helper_functions.factorial((int)value);
                    break;
                case "l":
                    return_value = Math.Log(value, Math.E);
                    break;               
                default:
                    break;
            }

            return return_value;
        }

        private bool brace_lookup(ref List<CalcEquationElement> equation) {
            int brace_start_idx = -1;
            int brace_end_idx = -1;
            int current_idx = 0;

            // iterate through the list, looking for ending braces
            foreach (var op in equation)
            {
                if (op is CalcOperator)
                {
                    if ((op as CalcOperator).get_operator() == ")")
                    {
                        brace_end_idx = current_idx;

                        break;
                    }
                }
                current_idx++;
            }
            // found ending braces, search for starting braces
            current_idx = 0;
            if (brace_end_idx > 0)
            {

                for (var i = brace_end_idx; i+1 > 0; i--)
                {
                    if (equation[i] is CalcOperator)
                    {
                        if ((equation[i] as CalcOperator).get_operator() == "(")
                        {
                            brace_start_idx = i;

                            break;
                        }
                    }
                }

            }

            if (brace_start_idx > -1)
            {
                int brace_eq_length = brace_end_idx - brace_start_idx-1;

                var inner_equation = new List<CalcEquationElement>(equation.GetRange(brace_start_idx+1, brace_eq_length));

                var inner_result = arithmetic_calc(inner_equation);

                // substitue braces with result

                equation.RemoveRange(brace_start_idx, brace_eq_length + 2);
                equation.InsertRange(brace_start_idx, inner_result);

                return false;
            }
            return true;
        }
        
        private bool point_arithmetic_lookup(List<CalcEquationElement> equation){
            for (var i = equation.Count - 1; i >= 0; i--) {
                if (i + 1 < equation.Count && i > 0) {
                    // iterate from end to start, look for operator, check for numbers surrounding it, calculate it, remove it
                    if (equation[i] is CalcOperator) {
                        if ((equation[i] as CalcOperator).is_point_operator() &&
                            equation[i - 1] is CalcNumber && equation[i + 1] is CalcNumber) {

                            var result = apply_point_operation(equation[i - 1] as CalcNumber, equation[i + 1] as CalcNumber, equation[i] as CalcOperator);
                           // Console.WriteLine("calculating: " + equation[i - 1].ToString() + equation[i].ToString() + equation[i + 1].ToString());

                            equation.RemoveRange(i - 1, 3);
                            equation.Insert(i - 1, result);
                            //equation.Add(result);
                            return false;
                        } 

                    }
                    
                }
            }
            return true;
        }

        private bool line_arithmetic_lookup(List<CalcEquationElement> equation)
        {
            for (var i = 0; i< equation.Count - 1; i++)
            {
                if (i - 2 < equation.Count && i > 0)
                {
                    // iterate from end to start, look for operator, check for numbers surrounding it, calculate it, remove it
                    if (equation[i] is CalcOperator)
                    {
                        if ((equation[i] as CalcOperator).is_line_operator() &&
                            equation[i - 1] is CalcNumber && equation[i + 1] is CalcNumber)
                        {
                            var result = apply_line_operation(equation[i - 1] as CalcNumber, equation[i + 1] as CalcNumber, equation[i] as CalcOperator);
                            equation.RemoveRange(i - 1, 3);
                            equation.Insert(i - 1, result);

                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool exponential_lookup(List<CalcEquationElement> equation)
        {
            for (var i = 0; i < equation.Count - 1; i++)
            {
                if (i - 2 < equation.Count && i > 0)
                {
                    // iterate from end to start, look for operator, check for numbers surrounding it, calculate it, remove it
                    if (equation[i] is CalcOperator)
                    {
                        if ((equation[i] as CalcOperator).is_exp_operator() &&
                            equation[i - 1] is CalcNumber && equation[i + 1] is CalcNumber)
                        {

                            var result = apply_exp_operation(equation[i - 1] as CalcNumber, equation[i + 1] as CalcNumber, (equation[i] as CalcOperator));
                            equation.RemoveRange(i - 1, 3);
                            equation.Insert(i - 1, result);

                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public CalcNumber apply_point_operation(CalcNumber a, CalcNumber b, CalcOperator o)
        {
            CalcNumber returnvalue;

            switch (o.get_operator())
            {

                case "*":
                    returnvalue = new CalcNumber(a.get_double() * b.get_double());
                    break;
                case "/":
                    if (b.get_double() != 0)
                        returnvalue = new CalcNumber(a.get_double() / b.get_double());
                    else
                        returnvalue = new CalcNumber();
                    break;
                default:
                    returnvalue = new CalcNumber();
                    break;

            }

            return returnvalue;
        }

        public CalcNumber apply_line_operation(CalcNumber a, CalcNumber b, CalcOperator o)
        {
            CalcNumber returnvalue;

            switch (o.get_operator())
            {

                case "-":
                    returnvalue = new CalcNumber(a.get_double() - b.get_double());
                    break;
                case "+":                   
                    returnvalue = new CalcNumber(a.get_double() + b.get_double());                 
                    break;
                default:
                    returnvalue = new CalcNumber();
                    break;

            }
            return returnvalue;
        }

        public CalcNumber apply_exp_operation(CalcNumber a, CalcNumber b, CalcOperator op) {
            // application of exp, log and root operations

            CalcNumber rtn;

            switch (op.get_operator()) {

                case "^":
                    rtn = new CalcNumber(Math.Pow(a.get_double(), b.get_double()));
                    break;
                case "log":
                    rtn = new CalcNumber(Math.Log(a.get_double(), b.get_double()));
                    break;
                case "root":
                    rtn = new CalcNumber(Math.Pow(a.get_double(), 1.0 / b.get_double()));
                    break;
                default:
                    rtn = new CalcNumber();
                    break;
            }

            return rtn;
        }
    }

    class CalcDataInterface
    {   
        // interface or adapter class to create valid data structures from gui input
        // manages all entered equations:   - all states of an equation in the process in equation_history
        //                                  - all saved equations as list in saved_equations

        // Singleton 
        private static CalcDataInterface instance;
        private CalcDataInterface() {
            current_equation = new CalcInputEquation();
            equation_history = new List<CalcInputEquation>();
            saved_equations = new List<CalcInputEquation>();


            button_to_entry = new Dictionary<string, string> {
                {"fak","n!" },
                {"bs","(" },
                {"be",")" },
                {"op_div","/" },
                {"op_mul","*" },
                {"op_plu","+" },
                {"op_min","-" },
                {"comma", "."},
                {"op_res","=" },
                {"e", "e" },
                {"pi", "pi" },
                {"yroot", "root" },
                { "exp", "^"}

            };
            for (int i = 0; i < 10; i++) {
                button_to_entry.Add("b" + i.ToString(), i.ToString());
            }
        }
        public static CalcDataInterface Instance {
            get {
                if (instance == null) {
                    instance = new CalcDataInterface();
                }
                return instance;
            }
        }
        public void set_mode(string mod) {
            current_equation.set_mode(mod);
        }

        Dictionary<string, string> button_to_entry;

        CalcInputEquation current_equation;

        public string Name { get; set; }

        public string get_current_result() {
            return this.current_equation.get_result();
        }

        public void add_input_from_button(string entry) {
            if (button_to_entry.ContainsKey(entry))
            {
                string button_mapping = button_to_entry[entry];
                this.add_input(button_mapping);
            }
            else
                this.add_input(entry);
        }

        public void add_input(string entry)
        {
            // @param entry - user input from ui

            // read latest input from ui

            // cases: number input, point or comma, operator, punctuation clip, lg, ln, RAD/DEG change

            if (!(entry.Count() > 0))
                return;

            List<string> sci_ops = new List<string> { "sin", "cos", "tan", "n!", "ln" };
            //Console.WriteLine("add input call with : " + entry);


            if (sci_ops.Contains(entry))
                current_equation.add_sci_op(entry);

            if (helper_functions.is_operator(entry))
            {
                current_equation.add_operator(entry);
                // Console.WriteLine("op is : " + entry);
            }

            if (helper_functions.is_digit(entry))
            {
                current_equation.add_digit(entry[0]);
            }

            if (entry[0] == ',' || entry[0] == '.')
                current_equation.add_comma();

            if (entry == "Return" || entry[0] == '=')
            {
                current_equation.calculate();
            }

            if (entry == "(" || entry == ")")
            {
                current_equation.add_operator(entry);
            }

            if (entry == "pi") { current_equation.add_number(Math.PI); }
            if (entry == "e") { current_equation.add_number(Math.E); }

            if (entry == "back")
             {
                this.undo_input();
                return;
             }

            if (entry == "clear") {
                current_equation.clear_last_entry();
            }


            this.Name = entry;

            equation_history.Add(helper_functions.CopyObj<CalcInputEquation>(current_equation));
  
        }

        public string get_current_line() {
            return this.current_equation.to_string();

        }

        public void undo_input() {
            // delete last equation from the saved equation list, set current equation to the one before
            int current_idx = equation_history.Count()-1;
            if (equation_history.Count() > 1) {
                current_equation = equation_history[current_idx - 1];
                equation_history.RemoveAt(current_idx);
            }
        }

        List<CalcInputEquation> equation_history;
        List<CalcInputEquation> saved_equations;


        public List<String> get_all_equation_strings() {
            // returns all saved equations as strings
            List<string> equations = new List<String>();
            foreach (var i in saved_equations) {
                equations.Add(i.to_string());
            }
            return equations;
        }


        public void save_current_equation() {
            // deep copy of the object
            saved_equations.Add(helper_functions.CopyObj<CalcInputEquation>(current_equation));
            
        }

        public void load_equation(int idx) {
            if (saved_equations.Count() > idx - 1)
                current_equation = helper_functions.CopyObj<CalcInputEquation>(saved_equations[idx]);
        }

        public void delete_saved_equation(int idx) {
            if (idx <= saved_equations.Count - 1) { 
                saved_equations.RemoveAt(idx);
            }
        }

        

    }
}
