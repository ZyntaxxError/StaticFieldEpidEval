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
                MessageBox.Show("No dose image found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                window.Content = new MainView()
                {
                    DataContext = new MainViewModel(context)
                };
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "StaticFieldEpidEval: Portal dosimetry evaluation of static fields";
            }
        }
    }
}