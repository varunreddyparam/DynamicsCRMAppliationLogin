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
using System.Windows.Shapes;
using UpdateCRMRecords;

namespace CrudOperations
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : Window
    {
        private Service xrmService;
        private ConnecttoCRM connecttoCRM = new ConnecttoCRM();
        public LoginControl()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            //varun@paramworkshop7.onmicrosoft.com
            xrmService = GetService();
            if (xrmService.OrganizationService != null)
            {
                lblMessage.Content = "Connection Established Successfully...";
                lblMessage.Foreground = Brushes.Green;
                using (var context = Context.Instance())
                {
                    context.Service = xrmService;
                }
            }
            else
            {
                lblMessage.Content = "Failed to Established Connection!!!";
                lblMessage.Foreground = Brushes.Red;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public Service GetService()
        {
            xrmService = connecttoCRM.connection(txturl.Text, txtUserName.Text, txtPassword.Text);
            return xrmService;
        }

    }
}
