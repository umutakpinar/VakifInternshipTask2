using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using VakifInternship_2.model;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace VakifInternship_2.controller
{
    internal class FileController
    {
        private List<FileModel> _fileList;
        public FileController() 
        {
            _fileList = new List<FileModel>();
        }
        /// <summary>
        /// Kullanıcıdan bir klasör seçmesini ister ve klasör yolunu döndürür.
        /// </summary>
        /// <returns>Seçilmezse null döner.</returns>
        public static string GetDirectory() 
        {
            string path = null;
            using (var fbd = new FolderBrowserDialog()) 
            {
                DialogResult result = fbd.ShowDialog(); 
                if (result == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath)) 
                {
                    path = fbd.SelectedPath; 
                }
            }
            return path; 
        }
        /// <summary>
        /// parametre olarak bir klasör path'i alır. Klasördeki dosyaları tarar ve .prc uzantılı olanları bir listede tutar.
        /// </summary>
        /// <param name="directoryPath">Klasör'ün yolu</param>
        /// <returns>FileModel tipinde öğeler barındıran bir Liste döner.</returns>
        public List<FileModel> CheckFilesInsideDirectory(string directoryPath)
        {  
            if(directoryPath != "")
            {
                DirectoryInfo dirInf = new DirectoryInfo(directoryPath);
                Logger.GetInstance().Log(utils.EMessageType.MessageType.Started, "KLASÖR OKUNUYOR", $"Klasördeki tüm dosyalar okunuyor.");
                FileInfo[] fileInfos = dirInf.GetFiles();
                Logger.GetInstance().Log(utils.EMessageType.MessageType.End, "KLASÖR OKUMA İŞLEMİ TAMAMLANDI", $"Klasördeki tüm dosyalar okundu.");
                utils.Progress.SetNewProcess(fileInfos.Length); //Burada Progress'ı kurdum
                foreach (FileInfo fileInfo in fileInfos)
                {
                    string fileName = fileInfo.Name;
                    string filePath = fileInfo.FullName;
                    if (filePath.EndsWith(".prc"))
                    {
                        _fileList.Add(CheckFile(fileName, filePath));
                    }
                    utils.Progress.GetInstance().IncreaseProgess();
                }
                Logger.GetInstance().Log(utils.EMessageType.MessageType.Unknown, "TAMAMLANDI", $"Klasördeki {_fileList.Count} dosya tarandı.");
            }
            else
            {
                Logger.GetInstance().Log(utils.EMessageType.MessageType.End, "TAMAMLANDI", $"Seçim iptal edildi.");
            }
            
            return _fileList;

        }
        /// <summary>
        /// Parametre olarak gönderilen sp'yi analiz eder. Bulguları bir FileModel nesnesine yazar ve FileModel tipinde geri döndürür.
        /// Burada gelen dosyanın Dinamik olup olmadığına bakar Dynamic sp değilse dosyayı atlar. Dynmaic sp ise içerisinde varchar2 tipinde parametre alıp almadığını kontrol eder.
        /// Eğer varchar2 parametreleri var ise ve bu parametreler injectionable ise bu durumda innjectionable parametreleri FileModel nesnesine kaydeder.
        /// Son olarak bu FileModel nesnesini döndürür.
        /// </summary>
        /// <param name="fileName">Dosya adı</param>
        /// <param name="filePath">Dosya yolu</param>
        /// <returns>FileModel döner.</returns>
        public FileModel CheckFile(string fileName, string filePath)
        {
            FileModel file = new FileModel(fileName, filePath);
            Logger.GetInstance().Log(utils.EMessageType.MessageType.Started, fileName); //LOG KAYDI
            Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, "Dosya okunuyor.");
            string fileData = ReadFileData(file.FilePath);
            Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, "Dosya okundu.");
            file.IsDynmaicSP = IsDynamic(fileData);

            if (file.IsDynmaicSP) //dynamicSP ise içinde varchar2 var mı diye kontrol et
            {
                Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, "Dosya dinamik SP.");
                Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, "Dosyadaki varchar2 parametreleri aranıyor.");
                string[] varchar2params = FindVarchar2Parameters(fileData); //içinde varchar2 parametreleri diziye at
               
                if (varchar2params.Length > 0) //varsa injection yapılaiblir mi onu kontrol edelim
                {
                    Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, "Dosyada varchar2 parametreler var.");
                    file.HasVarchar2 = true;
                    Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, "Dosyadaki uninjectable parametreler aranıyor...");
                    string[] uninjectableparams = FindUninjectableVarchar2Parameters(fileData);
                    Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, $"Dosyada {uninjectableparams.Length} adet uninjectable parametre var.");
                    string[] injectablevarchar2params = FindInjectableVarchar2Params(varchar2params, uninjectableparams);
                    Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, $"Dosyada {injectablevarchar2params.Length} adet SQL Injectable parametre var.");
                    file.InjectableParameters = string.Join(",",injectablevarchar2params);
                }
                else
                {
                    Logger.GetInstance().Log(utils.EMessageType.MessageType.Process, fileName, "Dosyada varchar2 parametreler yok.");
                    file.HasVarchar2 = false;
                }
            }
            Logger.GetInstance().Log(utils.EMessageType.MessageType.End, fileName, "Dosya tarandı. İşlem tamamlandı.");
            return file;
        }
        /// <summary>
        /// Parametre olarak gelen sp dosyasının içerğini inceler. Eğer dinamik sp ise true döner. 
        /// </summary>
        /// <param name="fileData">Dosya içeriğini alır (string)</param>
        /// <returns>Boolean dönecek. Dynamic ise true değilse false. </returns>
        public static bool IsDynamic(string fileData)
        {
            string pattern = @"'''"; 
            return Regex.IsMatch(fileData, pattern);
        }
        /// <summary>
        /// Parametre olarak verilen dosya yolundaki dosyanın içeriğini okur ve string olarak geriye döner. 
        /// </summary>
        /// <param name="filePath">Dosya yolu (string)</param>
        /// <returns>Okuduğu dosya içeriğini string olarak döner. </returns>
        public static string ReadFileData(string filePath)
        {
            return File.ReadAllText(filePath);
        }
        /// <summary>
        /// Parametre olarak verilen dosya içeriğinde varchar2 parametre var mı diye kontrol eder. Varsa bu eşleşen parametreleri döner. 
        /// </summary>
        /// <param name="fileData">Dosya İçeriği</param>
        /// <returns>vaarchar2 tipindeki parametreleri bir string array olarak döner. Eğer hiç varchar2 parametre yok ise o halde boş bir string array döner.</returns>
        public static string[] FindVarchar2Parameters(string fileData)
        {
            string pattern = @"[\w]*(?=\s+IN\s+VARCHAR2)";
            MatchCollection matches = Regex.Matches(fileData, pattern, RegexOptions.IgnoreCase);
            string[] varchar2params = new string[matches.Count];

            if (matches.Count > 0) //eğer eşleşmelerin sayısı sıfırdan büyükse işlemi yap değilse yapmanın anlamı yok
            {
                for (int index = 0; index < matches.Count; index++)
                {
                    varchar2params[index] = matches[index].Value.Trim();
                }
            }
             
            return varchar2params;
        }
        /// <summary>
        /// Burada SYS.DBMS_ASSERT.ENQUOTE_LITERAL metodu ile kullanılan varchar2 parametreleri bulup bir diziye alır ve bu diziyi döndürür.
        /// </summary>
        /// <param name="fileData">Dosya İçeriği (string)</param>
        /// <returns>varchar2 parametrelerden injection açığı olmayan parametreleri bir dizi olarak geri döndürür. Eğer hiçbiri ENQUOTE_LITERAL ile kullanılmadıysa boş bir stirng array döner. </returns>
        public static string[] FindUninjectableVarchar2Parameters(string fileData) 
        {
            string pattern = @"(?<=SYS\.DBMS_ASSERT\.ENQUOTE_LITERAL\((\s+)?)[\w]+(?=(\s+)?\))";
            MatchCollection matches = Regex.Matches(fileData, pattern);
            string[] uninjectablevarchar2params = new string[matches.Count];
            
            if(matches.Count > 0) //Eğer ki 0'dan büyükse demek ki bazıları SYS.DBMS_ASSERT.ENQUOTE_LITERAL ile kullanılmış
            {
                for(int index =  0; index < matches.Count; index++)
                {
                    uninjectablevarchar2params[index] = matches[index].Value;
                }
            }
            //eğer hiç eşleşme yoksa sıfır dönsek yeter zaten

            return uninjectablevarchar2params;
        }
        /// <summary>
        /// Kendisine parametre olarak verilen iki string array arasındaki farkları yeni bir string array olarak geri döndürür. 
        /// (Aslında yaptığı işlem ilk array fark ikinci array.) Eğer ikinci array boş ise yani hiçbir parametre uninjectionable değilse bu durumda birinci diziyi komple dönmüş olur yani tüm varchar2 parametreler sömürülebililr demek bu.
        /// </summary>
        /// <param name="varchar2params">Injection edilmeye açık parametreler</param>
        /// <param name="uninjvarchar2params">Injection açığı olmayan parametreler</param>
        /// <returns>varchar2 parametrelerden injection açığı olmayan parametreleri bir dizi olarak geri döndürür. Eğer hiçbiri ENQUOTE_LITERAL ile kullanılmadıysa boş bir stirng array döner. </returns>
        public static string[] FindInjectableVarchar2Params(string[] varchar2params, string[] uninjvarchar2params)
        {
            return varchar2params.Except(uninjvarchar2params).ToArray();
        }
   
    }

}
