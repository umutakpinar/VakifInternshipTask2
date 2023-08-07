using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VakifInternship_2.utils;

namespace VakifInternship_2.controller
{
    internal class Logger
    {
        private static List<string> logs;
        private static RichTextBox tbxLog;

        private static Logger _logger;

        /// <summary>
        /// Burada singleton design pattern tarzı bir yapı kurdum.
        /// Constructor private olduğu için erişemezsin bunun yerine Logger.Build() metodunu dene.
        /// </summary>
        /// <param name="richTextBoxLog">Evet Logger'ı UI'da hangi richtexbox''ta göstereceksen onun nesnesi gerekli</param>
        private Logger(RichTextBox richTextBoxLog)
        {
            Logs = new List<string>();
            tbxLog = richTextBoxLog;
        }
        /// <summary>
        /// Logger sınıfını kurmak için bunuun bir kez başlatılması gereklidir. Daha sonra bu sınıftan nesne almak için GetInstance() metodu kullanılabilir.
        /// </summary>
        /// <param name="richTextBoxLog"></param>
        public static void Build(RichTextBox richTextBoxLog) { 
            
            if(_logger == null)
            {
                _logger = new Logger(richTextBoxLog);
            }
        }
        /// <summary>
        /// Logger classından bir nesne verir.
        /// </summary>
        /// <returns> Dikkat! Eğer bu metodu kullanamdan önce Logger.Build() çağırılmadıysa GetInstance() metodu null döner.</returns>
        public static Logger GetInstance()
        {
            return _logger;
        }

        private static List<string> Logs 
        {
            get => logs;
            set => logs = value; 
        }

        /// <summary>
        /// Log kayıtlarını temizler. Ekranda gösterilen Log kayıtlarını da temizler.
        /// </summary>
        public void ClearLogs()
        {
            Application.OpenForms[0].Invoke(new Action(() => {
                Logs.Clear();
                tbxLog.Clear();
            }));
        }

        /// <summary>
        /// Ekranda log kaydı göstermeye yarar. Dosya adı, mesaj türü, ve mesaj adında 3 parametre alır. Son iki parametreyi gitmek zorunlu değildir.
        /// Girilen dosya adı ve mesaj türüne göre özel hazırlanmış text ekranda gösterilir.
        /// Buna ek olarak bir mesaj/açıklama eklemek isterseniz message parametresine değer verebilirsiniz.
        /// hazır mesaj tipleri dışında sadece bir log basmak ya da kullanıcıya bir uyarı göndermek için dosya mesaj tipini Unknown seçin, dosya adına bir değer verirseniz başlık yerine geçer, message ise ekranda gösterilecek log kaydınızdır.
        /// Unknown türündeki mesajlar ---- ile ayrılır.
        /// </summary>
        /// <param name="filename">Eğer yalnızca ekrana bir şey yazdıracaksan "" empty string gönderiniz.</param>
        /// <param name="messageType">Eğer herhangi bir log kaydı basacaksan lütfen Unknown seçin.</param>
        /// <param name="message">Zorunlu değil. boş bırakılabilir.</param>
        /// <returns>The sum of a and b.</returns>
        public void Log(EMessageType.MessageType messageType, string filename = null, string message = "")
        {
            string log = "";
            switch (messageType)
            {
                case EMessageType.MessageType.Started:
                    log = $"{DateTime.Now} - {filename ?? "Unknown"} adlı dosya işlenmeye başlanıyor.\n{message}";
                    break;

                case EMessageType.MessageType.Process:
                    log = $"{DateTime.Now} - {filename ?? "Unknown"} adlı dosya işleniyor.\n{message}";
                    break;

                case EMessageType.MessageType.End:
                    log = $"{DateTime.Now} - {filename ?? "Unknown"} adlı dosyanın işlenmesi tamamlandı.\n{message}";
                    break;

                case EMessageType.MessageType.Unknown:
                    log = $"\n\n___________________________\n{DateTime.Now} : \n{filename ?? "*MESAJ"}:\n{message}\n___________________________\n";
                    break;

                default:
                    log = $"{message}";
                    break;
            }
            Logs.Add(log);
            NotifyTextBox();
        }

        /// <summary>
        /// Log'a yeni bir değer eklendiğinde textBox'u uyarmak için.
        /// </summary>
        private static void NotifyTextBox()
        {
            Application.OpenForms[0].Invoke(new Action(() => {
                tbxLog.HideSelection = false;
                tbxLog.AppendText($"\n{Logs[Logs.Count - 1]}");
            }));
            
        }
    }
}
