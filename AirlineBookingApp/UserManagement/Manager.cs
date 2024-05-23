using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineBookingApp.UserManagement
{
    internal abstract class Manager : User
    {
        public string Deparment { get; set; }
        public Manager(string username, string password, string email, Role role, string deparment) : base(username, password, email, role)
        {
            Deparment = deparment;
        }
        public abstract void GenerateReports();


    }
}
