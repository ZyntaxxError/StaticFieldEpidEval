using StaticFieldEpidEval;
using StaticFieldEpidEval.ViewModels;
using System.Windows;

namespace VMS.DV.PD.Scripting
{
    public class Script
    {
        public Script()
        {
        }

        public void Execute(ScriptContext context, Window window)
        {
            if (context.DoseImage == null)
            {
                MessageBox.Show("No portal dose image in context window!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                window.Content = new MainView()
                {
                    DataContext = new MainViewModel(context)
                };
                // Set minimum size of the window, want to cover the default window that 
                // always pops up when running a script in Aria to make it more user friendly
                window.MinWidth = 600;
                window.MinHeight = 300;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "StaticFieldEpidEval: Portal dosimetry evaluation of static fields";
            }
        }
    }
}