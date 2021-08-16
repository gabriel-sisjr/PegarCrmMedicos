using System.ComponentModel.DataAnnotations.Schema;

namespace PegarCrmMedicos
{
    [Table("Logs")]
    public class Logs
    {
        public long Id { get; set; }
        public string CRM { get; set; }
        public string UF_CRM { get; set; }
    }
}
