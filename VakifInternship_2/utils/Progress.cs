using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VakifInternship_2.utils
{
    internal class Progress
    {
        private static Progress _progress = new Progress();
        private static double _processPercentage = 0.0;
        private static int _totalProcessAmount = 0;
        private static int _completedProcesses = 0;
        private static ProgressBar _progressBar;

        private Progress() { }

        public static void Build(ProgressBar progressBar) {
            _progress = new Progress();
            _progressBar = progressBar;
        }

        public static Progress GetInstance()
        {
            return _progress;
        }

        public void IncreaseProgess()
        {
            _completedProcesses++;
            _processPercentage = CalculateProgress();
            _progressBar.Value = (int)_processPercentage;

        }

        private double CalculateProgress()
        {
            return _completedProcesses * 100 / _totalProcessAmount;
        }

        public void ResetProgress()
        {
            _completedProcesses = 0;
            _progressBar.Value = 0;
            _progress = null;
            _completedProcesses = 0;
        }
    }
}
