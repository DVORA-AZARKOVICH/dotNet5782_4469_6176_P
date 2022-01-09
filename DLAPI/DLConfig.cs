using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DLAPI
{
    static class DLConfig
    {
        public class DLPackage
        {
            public string Name;
            public string PkgName;
            public string NameSpace;
            public string ClassName;
        }

        internal static string DalName;
        internal static Dictionary<string, string> DalPackages;
        static DLConfig()
        {
            XElement dalConfig = XElement.Load(@"xml\dal-config.xml");
            DalName = dalConfig.Element("dal").Value;
            DalPackages = (from pkg in dalConfig.Element("dal-packages").Elements()
                           select pkg
                          ).ToDictionary(p => "" + p.Name, p => p.Value);
        }
        
        /*
          * 
        internal static string DalName;
        internal static Dictionary<string, string> DalPackages;
        static DLConfig()
        {
            XElement dalConfig = XElement.Load(@"xml\dal-config.xml");
            DalName = dalConfig.Element("dal").Value;
            DalPackages = (from pkg in dalConfig.Element("dal-packages").Elements()
                           let tmp1 = pkg.Attribute("namespace")
                           let nameSpace = tmp1 == null ? "DL" : tmp1.Value
                           let temp2 = pkg.Attribute("class")
                           let className = temp2 == null ? pkg.Value : temp2.Value
                           select new DLPackage()
                           {
                               Name = "" + pkg.Name,
                               PkgName = pkg.Value,
                               NameSpace = nameSpace,
                               ClassName = className

                           })
                          .ToDictionary(p => "" + p.Name, p => p.Value);*/
    }
    public class DalConfigException : Exception
    {
        public DalConfigException(string msg) : base(msg) { }
        public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
    }

}

