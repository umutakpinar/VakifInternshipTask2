using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace VakifInternship_2.controller
{
    internal class UIController
    {
        public UIController(RichTextBox tbxLog, ProgressBar progresbar) {
            Logger.Build(tbxLog);
            utils.Progress.Build(progresbar);
        }

        /// <summary>
        /// Ekrandaki DataGridView ve TextBox componentlerinin instance'ını alır. Bunları LoadData fonksiyonuna gönderir.
        /// </summary>
        public void RefreshScreen(DataGridView dataGrid, TextBox tbx)
        {
            LoadData(dataGrid,tbx);
        }
        /// <summary>
        /// Kullanıcnın seçtiği pathi TextBox componentinde gösterir. Eğer path'te bir hata varsa string.Empty gösterir. MessageBox ile hatayı ekrana basar.
        /// </summary>
        public void SelectPath(TextBox tbxInput)
        {
            string selectedPath = string.Empty;

            try
            {
                selectedPath = FileController.GetDirectory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message);
            }
            tbxInput.Text = selectedPath;
        }
        /// <summary>
        /// DataGrid'i doldurur. Sömürülebilir (injectinable) sp'lerin bulunduğu row'lar seçili halde DataGrid'i doldurur. Böylece başlangıçta  kullanıcı Ctrl + C yaparak tümünü kopyalayabilir.
        /// </summary>
        public static void LoadData(DataGridView dataGrid, TextBox tbx)
        {
            FileController controller = new FileController();
            dataGrid.DataSource = controller.CheckFilesInsideDirectory(tbx.Text);
            HighlightInjectableRows(dataGrid);
        }
        /// <summary>
        /// Injectionable SP'leri temsil eden Row'ları başlangıçta seçili hale getirir. Eğer DataGrid boş ise seçemeyeceği için hata verecektir.
        /// Geliştirliebilir : Bu Row'ların arkaplanları farklı renge boyanabilir? (CellStyle)
        /// Geliştirilebilir : Bu rowları yalnızca başlangıçta seçili getirmek yerine bu metodu bir butoun onClick eventine atayabiliriz. (Ancak DataGrid'in dolu olup olmadığı durumunu göz önünde bulundurmalu o kısıma bi validation eklemedim..)
        /// </summary>
        public static void HighlightInjectableRows(DataGridView dataGrid)
        {
            try
            {
                dataGrid.Rows[0].Cells[0].Selected = false;
                for (int i = 0; i < dataGrid.Rows.Count; i++)
                {
                    if (!(dataGrid.Rows[i].Cells[4].Value == null || dataGrid.Rows[i].Cells[4].Value == ""))
                    {
                        dataGrid.Rows[i].Selected = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Selection cancelled.");
            }
        }
    }
}
