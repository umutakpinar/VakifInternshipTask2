using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace VakifInternship_2.utils
{
    internal class Progress
    {
        private static Progress _progress = new Progress();
        private static double _processPercentage = 0.0;
        private static int _totalProcessAmount = 0;
        private static int _completedProcesses = 0;
        private static Label _lblProcessInfo;
        private static ProgressBar _progressBar;

        private Progress() { }

        /// <summary>
        /// Buil burayı kurmak için UI controllerda çağırılmalı. Daha sonra taikp edilecek process tanımlanmadan önce SetNewProcess çağırılmalı!
        /// </summary>
        /// <param name="progressBar"></param>
        /// <param name="totalProcessAmount"></param>
        public static void Build(ProgressBar progressBar, Label lblProcessInfo) {
            _progress = new Progress();
            _progressBar = progressBar;
            _lblProcessInfo = lblProcessInfo;
        }
        /// <summary>
        /// Bu fonksiyon process başlamadan önce çağğırılmalı. Yapılacak işlemin hangi adımları takip edilmek isteniyorsa ve bu adım sayısı (işlem sayısı) atanmalı.
        /// </summary>
        /// <param name="totalProcessAmount">Toplam yapılacak işlem (adım) sayısı</param>
        public static void SetNewProcess(int totalProcessAmount)
        {
            _totalProcessAmount = totalProcessAmount;
        }

        public static Progress GetInstance()
        {
            return _progress;
        }

        public void IncreaseProgess()
        {
            _completedProcesses++;
            _processPercentage = CalculateProgress();
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                _progressBar.Value = (int)_processPercentage;
                if(_progressBar.Value > 95) {
                    _lblProcessInfo.ForeColor = Color.Blue;
                    _lblProcessInfo.Text = "LOADING";
                }
            }));

        }

        private double CalculateProgress()
        {
            return _completedProcesses * 100 / _totalProcessAmount;
        }

        public void ResetProgress()
        {
            _completedProcesses = 0;
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                _progressBar.Value = 0;
            }));
            _progress = null;
            _completedProcesses = 0;
        }
    }
}
