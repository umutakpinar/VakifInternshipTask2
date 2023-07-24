using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using VakifInternship_2.model;
using System.Text.RegularExpressions;

namespace VakifInternship_2.controller
{
    // .PRC OLDUĞUNU KONTROL ETMEYİ UNUTTUN ONU DA EKLE
    internal class FileController
    {
        private List<FileModel> _fileList;
        public FileController() 
        {
            _fileList = new List<FileModel>();
        }

        public static string GetDirectory() //Bu fonksiyonu çağıracağım view classında try catch kullacağım (sanırım try catch'i burada kullanmak daha mantıklı  aslında?)
        {
            string path = null;
            using (var fbd = new FolderBrowserDialog()) // Path'i almak için FolderBrowserDialog kullandım
            {
                DialogResult result = fbd.ShowDialog(); //Dialog'u gösterdim ve dialog sonucunu kaydettim
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath)) //Seçme işlemi tamamlandıysa ve seçilen yol null değil ise
                {
                    path = fbd.SelectedPath; //Seçilen al
                }
            }
            return path; //Son olarak seçilen pathi döndür
        }

        public List<FileModel> CheckFilesInsideDirectory(string directoryPath)
        {
            //BURAYA TRY CATCH EKLEYECEĞİM!!! SONUÇTA BİR FİLE OPERATİON YAPIYORUM
            DirectoryInfo dirInf = new DirectoryInfo(directoryPath); //Klasör hakkındaki bilgileri tutuyorum
            FileInfo[] fileInfos =  dirInf.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                string fileName = fileInfo.Name;
                string filePath = fileInfo.FullName;
                if (filePath.EndsWith(".prc")) //eğer dosya bir .prc dosyası ise listeye alabiliriz
                {
                    _fileList.Add(CheckFile(fileName, filePath));
                }
            }
            return _fileList;

        }

        public FileModel CheckFile(string fileName, string filePath)
        {
            FileModel file = new FileModel(fileName, filePath);
            string fileData = ReadFileData(file.FilePath);
            file.IsDynmaicSP = IsDynamic(fileData);

            if (file.IsDynmaicSP) //dynamicSP ise içinde varchar2 var mı diye kontrol et
            {
                string[] varchar2params = FindVarchar2Parameters(fileData); //içinde varchar2 parametreleri diziye at
                
                foreach(string i in varchar2params)
                {
                    Console.WriteLine("varchar2 param : "+i);
                }

                if (varchar2params.Length > 0) //varsa injection yapılaiblir mi onu kontrol edelim
                {
                    file.HasVarchar2 = true;
                    string[] uninjectableparams = FindUninjectableVarchar2Parameters(fileData);
                    file.InjectableParameters = string.Join(",",FindInjectableVarchar2Params(varchar2params, uninjectableparams));
                }
                else //yoksa daha fazla işleme gerek yok
                {
                    file.HasVarchar2 = false;
                }
            } //NULL HATASI ALIRSAM BURADA ELSE BLOĞU İÇERİSİNDE DYNAMİC OLMAYAN FİLE ÖĞESİNE BOŞ PARAMETRELER ATA!!!!!!!!!

            return file;
        }

        public static bool IsDynamic(string fileData)
        {
            string pattern = @"'''"; //dynamic olup olmadığını belirlemek
            return Regex.IsMatch(fileData, pattern); //Eğer bir tane bile eşleşme varsa dynamictir
        }

        public static string ReadFileData(string filePath)
        {
            return File.ReadAllText(filePath);
        }

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

        public static string[] FindUninjectableVarchar2Parameters(string fileData) //Burada SYS.DBMS_ASSERT.ENQUOTE_LITERAL metodu ile kullanılanları bulup bir listeye alır.
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

        public static string[] FindInjectableVarchar2Params(string[] varchar2params, string[] uninjvarchar2params)
        {
            return varchar2params.Except(uninjvarchar2params).ToArray();
        }
   
    }

}
