using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineBookingApp.DataManagement
{
    public interface IDataValidator<T>
    {
        public abstract bool Validate(T data, out IEnumerable<string> validationErrors);
    }
}
