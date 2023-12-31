﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using VakifInternship_2.model;

namespace VakifInternship_2.controller
{
    internal class UIController
    {
        private bool _initialStart = true;
        public UIController(RichTextBox tbxLog, ProgressBar progresbar, Label lblProcessInfo) {
            Logger.Build(tbxLog);
            utils.Progress.Build(progresbar, lblProcessInfo);
            _initialStart = false;
        }
        /// <summary>
        /// Bir tarama işlemi tamamlandıktan sonra yeni bir işlem başşlatılmadan önce kullanılır.
        /// </summary>
        public static void Reset()
        {
            utils.Progress.GetInstance().ResetProgress();
            Logger.GetInstance().ClearLogs();
        }

        /// <summary>
        /// Ekrandaki DataGridView ve TextBox componentlerinin instance'ını alır. Bunları LoadData fonksiyonuna gönderir.
        /// </summary>
        public void RefreshScreen(DataGridView dataGrid, TextBox tbx, Label lblProcessInfo)
        {
            LoadData(dataGrid,tbx, lblProcessInfo);
        }
        /// <summary>
        /// Kullanıcnın seçtiği pathi TextBox componentinde gösterir. Eğer path'te bir hata varsa string.Empty gösterir. MessageBox ile hatayı ekrana basar.
        /// Ayrıca işlem tamamlanıp yeniden bir işlem yapılmak istenirse gösterilen bilgileri reset eder.
        /// </summary>
        public void SelectPath(TextBox tbxInput)
        {
            if(!_initialStart)
            {
                Reset();
            }
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
        /// DataGrid'i doldurur. Sömürülebilir (injectinable) sp'lerin bulunduğu row'lar seçili halde DataGrid'i doldurur.
        /// Böylece başlangıçta  kullanıcı Ctrl + C yaparak tümünü kopyalayabilir.
        /// UI'ı kilitlememek için yeni bir thread'de işlemi yapıyor.
        /// (Öğrendiğim kadarıyla bir UI elementine erişmek için MainThread'e geri dönmek gerkiyormuş. Bu nedenle bazı yerlerde Form1'i yeniden invoke etmem gerekti.)
        /// </summary>
        public async static void LoadData(DataGridView dataGrid, TextBox tbx, Label lblProcessInfo)
        {
            FileController controller = new FileController();
            try
            {
                lblProcessInfo.ForeColor = Color.Orange;
                lblProcessInfo.Text = "PROCESSING";
                List<FileModel> data  = await Task.Run(() => controller.CheckFilesInsideDirectory(tbx.Text));
                dataGrid.DataSource = data;
                lblProcessInfo.ForeColor = Color.Green;
                lblProcessInfo.Text = "COMPLETED";
                if(dataGrid.RowCount > 0 )
                {
                    HighlightInjectableRows(dataGrid);
                }
            }
            catch (Exception ex)
            {
                lblProcessInfo.ForeColor = SystemColors.ActiveCaption;
                lblProcessInfo.Text = "WAITING";
                MessageBox.Show(ex.Message);
            }
            
        }
        /// <summary>
        /// Injectionable SP'leri temsil eden Row'ları başlangıçta seçili hale getirir. Eğer DataGrid boş ise seçemeyeceği için hata verecektir.
        /// Geliştirliebilir : Bu Row'ların arkaplanları farklı renge boyanabilir? (CellStyle)
        /// Geliştirilebilir : Bu rowları yalnızca başlangıçta seçili getirmek yerine bu metodu bir butoun onClick eventine atayabiliriz. (Ancak DataGrid'in dolu olup olmadığı durumunu göz önünde bulundurmalu o kısıma bi validation eklemedim..)
        /// </summary>
        public static void HighlightInjectableRows(DataGridView dataGrid)
        {
            dataGrid.Rows[0].Cells[0].Selected = false;
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                if (!(dataGrid.Rows[i].Cells["InjectableParameters"].Value == null || dataGrid.Rows[i].Cells["InjectableParameters"].Value.ToString() == ""))
                {
                    dataGrid.Rows[i].Selected = true;
                }
            }
        }
    }
}
