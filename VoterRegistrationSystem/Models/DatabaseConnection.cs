using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterRegistrationSystem.Models
{
    class DatabaseConnection
    {
        public static string ConnectionString()
        {
            return "Data Source=(LocalDb)\\MSSQLLocalDb;Initial Catalog=Registration;Integrated Security=True";
        }
    }
}
