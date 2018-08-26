using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Mamesaver
{
    public class MultiFormApplicationContext : ApplicationContext
    {
        private readonly List<Form> _forms;
        private int _openForms;

        public MultiFormApplicationContext(List<Form> forms)
        {
            _forms = forms;
            
            foreach (var form in forms)
            {
                form.FormClosed += FormClosed;
                form.Show();
                _openForms++;
            }
        }

        private void FormClosed(object sender, FormClosedEventArgs args)
        {
            //When we have closed the last of the "starting" forms, 
            //end the program.
            if (Interlocked.Decrement(ref _openForms) == 0)
                ExitThread();
        }
    }
}