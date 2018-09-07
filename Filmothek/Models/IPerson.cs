using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public interface IPerson
    {
        int Id;
        string Nachname;
        string Vorname;
        string Addresse;
        string Login;
        string Pw;
        short Berechtigung;     
    }
}
