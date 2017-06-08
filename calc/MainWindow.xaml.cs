using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using dk.calc.dto;

namespace calc
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        // Main Window Class
        // handling of Key Events - calcKeyHandler 
        // and text input events  - calcTextHandler for international keyboards


        private CalcDataInterface dts;

        private int selected = -1;

        List<TextBox> saved_eq;
        public MainWindow()
        {
            this.KeyDown += new KeyEventHandler(calcKeyHandler);
            this.TextInput += new TextCompositionEventHandler(calcTextHandler);

            dts = CalcDataInterface.Instance;
            InitializeComponent();

            saved_eq = new List<TextBox>() { saved_eq_1, saved_eq_2, saved_eq_3, saved_eq_4, saved_eq_5 };

        }

        private void calcTextHandler(object sender, TextCompositionEventArgs e) {

            this.dts.add_input(e.Text);
            this.input_textbox.Text = this.dts.get_current_result();
            this.equation_textbox.Text = this.dts.get_current_line();
        }

        private void calcKeyHandler(object sender, KeyEventArgs e) {

           //this.input_textbox.Text = e.Key.ToString();

            if (e.Key == Key.Return) {
                // return for end of equation

                this.dts.add_input("Return");
            }

            if (e.Key == Key.Delete) {

                if (selected < 0)
                    this.dts.add_input("clear");
                else {
                    this.dts.delete_saved_equation(selected);
                    selected = -1;
                }
            }

            if (e.Key == Key.Back)
            {
                this.dts.undo_input();
                return;
            }

            // arrow keys for save/load
            if (e.Key == Key.Left) {
                this.dts.save_current_equation();
            }

            if (e.Key == Key.Down) {
                // check for highlighted box
                var eqs = this.dts.get_all_equation_strings();

                if (selected + 1 < eqs.Count() ) {
                    this.selected += 1;
                }              
            }

            if (e.Key == Key.Up) {
                // check for highlighted box
                var eqs = this.dts.get_all_equation_strings();

                if (selected - 1 > -1)
                {
                    this.selected -= 1;
                }
            }

            if (e.Key == Key.Right) {
                // load a saved equation
                this.dts.load_equation(selected);
                this.selected = -1;
            }


            this.input_textbox.Text = this.dts.get_current_result();
            this.equation_textbox.Text = this.dts.get_current_line();
            show_saved_equations();

        }

        private void show_saved_equations() {
            var eqs = this.dts.get_all_equation_strings();
            //List<TextBox> eq_boxes = new List<TextBox> { this.saved_eq_1, this.saved_eq_2, this.saved_eq_3, this.saved_eq_4, this.saved_eq_5 };

            int range_begin = 0;
            int range_end = 4;

            if (selected > 4) {
                range_begin = selected - 4;
                range_end = selected;
            }

            for (var i = range_begin; i < range_end+1; i++) {
                if (i < eqs.Count())
                {
                    saved_eq[i - range_begin].Text = eqs[i];
                    if (selected == i)
                        saved_eq[i - range_begin].Background = Brushes.LightGray;
                    else
                        saved_eq[i - range_begin].Background = Brushes.White;
                }
                else
                {
                    saved_eq[i - range_begin].Text = "";
                    saved_eq[i - range_begin].Background = Brushes.White;

                }
            }
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
            // get the button name, send event to calculater object
        {
            string buttonText = ((Button)sender).Name;
            this.dts.add_input_from_button(buttonText);
            this.input_textbox.Text = this.dts.get_current_result();
            this.equation_textbox.Text = this.dts.get_current_line();
            ((Button)sender).Focusable = false;

        }

        private void Button_mod_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString() == "RAD")
            {
                ((Button)sender).Content = "DEG";
                this.dts.set_mode("DEG");
            }
            else {
                ((Button)sender).Content = "RAD";
                this.dts.set_mode("RAD");


            }
            this.input_textbox.Text = this.dts.get_current_result();

        }

        private void Button_Click_Up(object sender, RoutedEventArgs e)
        {
            // check for highlighted box
            var eqs = this.dts.get_all_equation_strings();

            if (selected - 1 > -1)
            {
                this.selected -= 1;
            }
            show_saved_equations();
        }

        private void Button_Click_Down(object sender, RoutedEventArgs e)
        {
            // check for highlighted box
            var eqs = this.dts.get_all_equation_strings();

            if (selected + 1 < eqs.Count())
            {
                this.selected += 1;
            }
            show_saved_equations();
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            this.dts.save_current_equation();
            show_saved_equations();
        }

        private void Button_Click_Load(object sender, RoutedEventArgs e)
        {
            // load a saved equation
            this.dts.load_equation(selected);
            this.selected = -1;
            show_saved_equations();

        }
    }
}
