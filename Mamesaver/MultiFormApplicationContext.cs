using System.Collections.Generic;
using System.Windows.Forms;

namespace Mamesaver
{
    public class MultiFormApplicationContext : ApplicationContext
    {
        public MultiFormApplicationContext(List<Form> forms) => forms.ForEach(form => form.Show());
    }
}