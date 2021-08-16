using System.ComponentModel.DataAnnotations.Schema;

namespace PegarCrmMedicos
{
    [Table("Dados")]
    public class Dados
    {
        public long ID { get; set; }
        public string ID_SOLICITANTE { get; set; }
        public string NOME { get; set; }
        public string CRM { get; set; }
        public string UF_CRM { get; set; }
        public string SITUACAO { get; set; }
        public string ENDERECO { get; set; }
        public string TELEFONE { get; set; }
        public string INSCRICAO { get; set; }
        public string AUTORIZACAO_IMAGEM { get; set; }
        public string AUTORIZACAO_ENDERECO { get; set; }
        public string HASH { get; set; }
    }
}