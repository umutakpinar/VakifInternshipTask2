using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakifInternship_2.model
{
    internal class FileModel
    {
        private string _fileName;
        private string _filePath;
        private bool _IsDynamicSP;
        private bool _hasVarchar2;
        private string _injectableParameters;
        
        public FileModel(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        public string FileName {
            get
            {
                return _fileName;
            } 
            set
            {
                _fileName = value;
            }
        }
        public string FilePath {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
            } 
        }
        public bool IsDynmaicSP
        {
            get
            {
                return _IsDynamicSP;
            }
            set
            {
                _IsDynamicSP = value;
            }
        }
        public bool HasVarchar2 {
            get
            {
                return _hasVarchar2;
            }
            set
            {
                _hasVarchar2 = value;
            }
        }
        public string InjectableParameters {
            get
            {
                return _injectableParameters;
            }
            set
            {
                _injectableParameters = value;
            }
        }
    }
}


