using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class CustomerForList
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string PhoneNumber { set; get; }
        public int SentParcels { set; get; }//חבילות שהלקוח שלח
        public int Delivered { set; get; }// חבילות שנשלחו ונמסרו
        public int Ordered { set; get; }// חבילות שהלקוח הזמין
        public int Arrived { set; get; }//חבילות שהוזמנו והגיעו

        public override string ToString()
        {
            return this.ToStringProperty();
        }

    }
}
