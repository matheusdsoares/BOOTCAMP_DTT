using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniServiceDesk
{
    #region Enums e Exceções
    public enum Prioridade { Baixa, Media, Alta }
    public enum Status { Aberto, EmAndamento, Resolvido, Fechado }

    // Exceção de Domínio Personalizada
    public class ServiceDeskException : Exception 
    { 
        public ServiceDeskException(string message) : base(message) { } 
    }

    public class TransicaoStatusInvalidaException : ServiceDeskException 
    {
        public TransicaoStatusInvalidaException(Status atual, Status novo) 
            : base($"Transição inválida: Não é permitido mudar de {atual} para {novo}.") { }
    }
    #endregion

    #region Modelagem (Entidades)
    public class HistoricoStatus
    {
        public Status Status { get; set; }
        public DateTime DataMudanca { get; set; }
    }

    public abstract class Chamado
    {
        public int Id { get; private set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Solicitante { get; set; }
        public Prioridade Prioridade { get; set; }
        public Status Status { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public List<HistoricoStatus> Historico { get; } = new List<HistoricoStatus>();

        protected Chamado(int id, string titulo, string solicitante, string descricao, Prioridade prioridade = Prioridade.Media)
        {
            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(solicitante))
                throw new ServiceDeskException("Título e Solicitante são obrigatórios!");

            Id = id;
            Titulo = titulo;
            Solicitante = solicitante;
            Descricao = descricao;
            Prioridade = prioridade;
            Status = Status.Aberto;
            DataAbertura = DateTime.Now;
            RegistrarHistorico(Status.Aberto);
        }

        public void AtualizarStatus(Status novoStatus)
        {
            // Regra de Negócio: Validação de Fluxo
            bool transicaoValida = (Status == Status.Aberto && novoStatus == Status.EmAndamento) ||
                                   (Status == Status.EmAndamento && novoStatus == Status.Resolvido) ||
                                   (Status == Status.Resolvido && novoStatus == Status.Fechado);

            if (!transicaoValida)
                throw new TransicaoStatusInvalidaException(Status, novoStatus);

            Status = novoStatus;
            RegistrarHistorico(novoStatus);
        }

        private void RegistrarHistorico(Status s) => 
            Historico.Add(new HistoricoStatus { Status = s, DataMudanca = DateTime.Now });

        // Polimorfismo: Cada tipo de chamado terá seu SLA
        public abstract string GetSLA();
    }

    // Herança: Especializações
    public class ChamadoInfra : Chamado
    {
        public ChamadoInfra(int id, string t, string s, string d, Prioridade p) : base(id, t, s, d, p) { }
        public override string GetSLA() => "SLA de Infra: 24h úteis.";
    }

    public class ChamadoAplicacao : Chamado
    {
        public ChamadoAplicacao(int id, string t, string s, string d, Prioridade p) : base(id, t, s, d, p) { }
        public override string GetSLA() => "SLA de Aplicação: 48h úteis.";
    }
    #endregion

    #region Abstração (Interface e Repositório)
    public interface IRepositorioChamados
    {
        void Criar(Chamado chamado);
        Chamado ObterPorId(int id);
        List<Chamado> Listar(Status? status = null, Prioridade? prioridade = null);
        void Excluir(int id);
    }

    public class RepositorioChamadosMemoria : IRepositorioChamados
    {
        private readonly List<Chamado> _chamados = new List<Chamado>();

        public void Criar(Chamado chamado) => _chamados.Add(chamado);
        public Chamado ObterPorId(int id) => _chamados.FirstOrDefault(c => c.Id == id);
        public void Excluir(int id)
        {
            var c = ObterPorId(id);
            if (c == null) throw new ServiceDeskException("Chamado não encontrado.");
            if (c.Status != Status.Aberto) throw new ServiceDeskException("Só é permitido excluir chamados com status 'Aberto'.");
            _chamados.Remove(c);
        }

        public List<Chamado> Listar(Status? status = null, Prioridade? prioridade = null)
        {
            var query = _chamados.AsQueryable();
            if (status.HasValue) query = query.Where(c => c.Status == status);
            if (prioridade.HasValue) query = query.Where(c => c.Prioridade == prioridade);
            return query.ToList();
        }
    }
    #endregion

    #region Programa Principal (Console)
    class Program
    {
        static IRepositorioChamados repo = new RepositorioChamadosMemoria();
        static int contadorId = 1;

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("=== MINI SERVICE DESK POO ===");
                    Console.WriteLine("1. Abrir Chamado (Infra)\n2. Abrir Chamado (App)\n3. Listar/Filtrar\n4. Mudar Status\n5. Detalhar\n6. Excluir\n0. Sair");
                    Console.Write("Opção: ");
                    string op = Console.ReadLine();

                    if (op == "0") break;

                    switch (op)
                    {
                        case "1": CriarChamado(true); break;
                        case "2": CriarChamado(false); break;
                        case "3": ListarChamados(); break;
                        case "4": AlterarStatus(); break;
                        case "5": Detalhar(); break;
                        case "6": Excluir(); break;
                    }
                }
                catch (ServiceDeskException ex) { Console.WriteLine($"\n[ERRO DE NEGÓCIO]: {ex.Message}"); }
                catch (Exception ex) { Console.WriteLine($"\n[ERRO CRÍTICO]: {ex.Message}"); }
                
                Console.WriteLine("\nPressione qualquer tecla...");
                Console.ReadKey();
            }
        }

        static void CriarChamado(bool isInfra)
        {
            Console.Write("Título: "); string t = Console.ReadLine();
            Console.Write("Solicitante: "); string s = Console.ReadLine();
            Console.Write("Descrição: "); string d = Console.ReadLine();
            Console.Write("Prioridade (0:Baixa, 1:Media, 2:Alta): ");
            Prioridade p = (Prioridade)int.Parse(Console.ReadLine());

            Chamado novo = isInfra ? new ChamadoInfra(contadorId++, t, s, d, p) : new ChamadoAplicacao(contadorId++, t, s, d, p);
            repo.Criar(novo);
            Console.WriteLine("Chamado criado com sucesso!");
        }

        static void ListarChamados()
        {
            Console.WriteLine("Filtrar por Status? (Deixe vazio para todos ou digite: Aberto, EmAndamento, Resolvido, Fechado)");
            string input = Console.ReadLine();
            
            List<Chamado> lista;
            if (Enum.TryParse(input, out Status s)) lista = repo.Listar(status: s);
            else lista = repo.Listar();

            foreach (var c in lista)
                Console.WriteLine($"ID: {c.Id} | [{c.Status}] | {c.Titulo} | Prioridade: {c.Prioridade}");
        }

        static void AlterarStatus()
        {
            Console.Write("ID do Chamado: "); int id = int.Parse(Console.ReadLine());
            var c = repo.ObterPorId(id);
            if (c == null) throw new ServiceDeskException("Chamado não encontrado.");

            Console.WriteLine($"Status Atual: {c.Status}");
            Console.WriteLine("Para qual status deseja mudar? (1: EmAndamento, 2: Resolvido, 3: Fechado)");
            Status novo = (Status)int.Parse(Console.ReadLine());

            c.AtualizarStatus(novo);
            Console.WriteLine("Status atualizado!");
        }

        static void Detalhar()
        {
            Console.Write("ID do Chamado: "); int id = int.Parse(Console.ReadLine());
            var c = repo.ObterPorId(id);
            if (c == null) return;

            Console.WriteLine($"\n--- DETALHES DO CHAMADO #{c.Id} ---");
            Console.WriteLine($"Título: {c.Titulo}\nSolicitante: {c.Solicitante}\nSLA: {c.GetSLA()}");
            Console.WriteLine("HISTÓRICO DE STATUS:");
            foreach (var h in c.Historico) Console.WriteLine($"- {h.Status} em {h.DataMudanca}");
        }

        static void Excluir()
        {
            Console.Write("ID para excluir: "); int id = int.Parse(Console.ReadLine());
            repo.Excluir(id);
            Console.WriteLine("Chamado excluído.");
        }
    }
    #endregion
}