using System.Collections.Generic;

namespace PegarCrmMedicos
{
    public class Request
    {
        public string status { get; set; }
        public List<Dados> dados { get; set; }
    }
}