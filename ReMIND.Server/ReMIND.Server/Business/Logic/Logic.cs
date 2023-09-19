using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReMIND.Server.Logic
{
    public partial class Logic
    {
        //context

        //constructor with context
        public Logic()
        {
            //var optionsBuilder = new DbContextOptionsBuilder<CalendarContext>();
            //optionsBuilder.UseSqlite("Data Source=Data/iQCalendarDB.db;");
            //DbContext context = new DbContext(optionsBuilder.Options);
        }

        //auth and other utility methods here

        //db related methods into other files with the same partial class
    }
}
