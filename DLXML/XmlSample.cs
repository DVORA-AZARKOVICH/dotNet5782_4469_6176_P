using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dal
{
    class XmlSample
    {
        XElement customerRoot;
        string customerPath = @"CustomerXml.xml";
        public void SaveStudentList(List<Customer> customerList)
        {
            customerRoot = new XElement("customers");

            customerRoot.Save(customerPath);
        }
    }
}
