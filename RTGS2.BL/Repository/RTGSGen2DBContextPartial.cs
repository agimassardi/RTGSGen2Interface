using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTGS2.BL
{
    public partial class RTGSGen2DBContext : DbContext
    {
        public RTGSGen2DBContext(string connectionString)
            : base(connectionString)
        {

        }
    }
}
