using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PegarCrmMedicos
{
    class Program
    {
        /*
        // Não tem SP aqui, pq a busca foi feita num array contendo apenas ele.
        private readonly static string[] _siglasEstados =
            new string[] { "AC", "AL", "AP", "AM", "BA",
                 "CE", "DF", "ES", "GO", "MA", "MT",
                 "MS", "MG", "PA", "PB", "PR", "PE",
                 "PI", "RJ", "RN", "RS", "RO", "RR",
                 "SC", "SE", "TO" };
        */

        private readonly static string[] _siglasEstados = new string[] {"SP"};

        static async Task Main(string[] args)
        {
            using var _context = new ContextDB();
            using var wc = new WebClient();
            foreach (var uf in _siglasEstados)
            {
                var haDados = true;
                var retries = 0;
                var crm = 3685;

                Console.WriteLine($"\n ======== {uf} ======== ");
                while (haDados)
                {
                    var dados = wc.DownloadString($"https://portal.cfm.org.br/api_rest_php/api/v1/medicos/buscar_foto/{crm}/{uf}");
                    var o = JsonSerializer.Deserialize<Request>(dados);

                    if (retries == 50) { haDados = false; break; }
                    if (o.dados == null)
                    {
                        retries++;
                        Console.WriteLine($"CRM sem dado: {crm}");
                        _context.Logs.Add(new Logs { CRM = crm.ToString(), UF_CRM = uf });
                        crm++;
                        await Esperar(500); // Espera meio segundo.
                        continue;
                    }

                    if (o.dados.Count > 1)
                    {
                        o.dados.ForEach(med =>
                        {
                            if (med.SITUACAO == "Ativo")
                                _context.Dados.Add(med);
                        });
                    }
                    else if (o.dados.Count != 0)
                    {
                        var med = o.dados.FirstOrDefault();
                        if (med.SITUACAO == "Ativo")
                            _context.Dados.Add(med);
                    }

                    //
                    crm++;
                    retries = 0; // Sempre zerado o retry, caso algum passe.
                    await Esperar(750); // Espera 0.75s

                    // A cada 100 registros salva e pausa (pra api dos caras respirar).
                    if (crm % 100 == 0)
                    {
                        _context.SaveChanges();
                        await Esperar(); // Espera 3s
                    }
                }
                Console.WriteLine($"\n ======== {uf} ======== ");
            }
        }
        public static async Task Esperar(int msEspera = 3000) => await Task.Delay(msEspera);
    }
}