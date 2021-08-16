using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PegarCrmMedicos
{
    class Program
    {
        private readonly static string[] _siglasEstados =
            new string[] { "AC", "AL", "AP", "AM", "BA",
                 "CE", "DF", "ES", "GO", "MA", "MT",
                 "MS", "MG", "PA", "PB", "PR", "PE",
                 "PI", "RJ", "RN", "RS", "RO", "RR",
                 "SC", "SP", "SE", "TO" };

        static async Task Main(string[] args)
        {
            using (var _context = new ContextDB())
            {
                using (var wc = new WebClient())
                {
                    var _logCsv = new StringBuilder();
                    foreach (var uf in _siglasEstados)
                    {
                        var haDados = true;
                        var retries = 0;
                        var crm = 1;

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
                                _logCsv.AppendLine($"{crm},{uf}");
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
                            await Esperar(500); // Espera meio segundo.

                            // A cada 100 registros salva e pausa (pra api dos caras respirar).
                            if (crm % 100 == 0)
                            {
                                _context.SaveChanges();
                                await Esperar(); // Espera 3s
                            }
                        }
                        Console.WriteLine($"\n ======== {uf} ======== ");
                    }
                    var p = Path.GetFullPath(@"csvMedicos");
                    File.AppendAllText(p, _logCsv.ToString());
                }
            }
        }
        public static async Task Esperar(int msEspera = 3000) => await Task.Delay(msEspera);
    }
}