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
                MessageBox.Show("The current context does not contain a dose image.");
                return;
            }
            else
            {
                window.Content = new MainView()
                {
                    DataContext = new MainViewModel(context)
                };
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.Title = "StaticFieldEpidEval: Portal dosimetry evaluation of static fields";
            }
        }
    }
}