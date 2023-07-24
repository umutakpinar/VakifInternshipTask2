using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VakifInternship_2.controller
{
    internal class UIController
    {
        public void RefreshScreen(DataGridView dataGrid, TextBox tbx)
        {
            LoadData(dataGrid,tbx);
        }

        public void SelectPath(TextBox tbxInput) //Kullanıcı tarafından seçilen pathi ekranda göstermek için için bu metodu kullandım
        {
            string selectedPath = string.Empty;

            try
            {
                selectedPath = FileController.GetDirectory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + selectedPath);
            }
            tbxInput.Text = selectedPath;
        }

        public static void LoadData(DataGridView dataGrid, TextBox tbx)
        {
            FileController controller = new FileController();
            dataGrid.DataSource = controller.CheckFilesInsideDirectory(tbx.Text);
        }
    }
}
