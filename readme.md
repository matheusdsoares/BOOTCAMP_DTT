# Bootcamp DTT - Jornada C# & .NET

Bem-vindo ao repositório do meu aprendizado no **Bootcamp DTT**. Aqui documento toda a minha evolução, desde a lógica básica até a construção de APIs robustas utilizando o ecossistema .NET.

> Status: Em progresso 🚀 (Atualizado conforme as aulas avançam)

---

## 🛠️ Tecnologias e Ferramentas 
* **Linguagem:** C#
* **Framework:** .NET (Console & Web API)
* **Ferramentas:** Git, GitHub, VS Code/Visual Studio, Dockers, Dbeaver, Insomnia, Redis

---

## 📅 Diário de Bordo
Acompanhe o que foi desenvolvido em cada etapa:

| Dia | Foco do Aprendizado | Principais Entregas |
| :--- | :--- | :--- |
| **Dia 01** | Organização | Estrutura de pastas e setup do ambiente. |
| **Dia 02** | POO Básica | Criação da classe `Visitante` e lógica de CRUD. |
| **Dia 03** | Refatoração | Melhorias na lógica e organização de código. |
| **Dia 04** | Estruturação | Gestão de pastas para múltiplos casos de uso. |
| **Dia 05** | APIs | Desenvolvimento da primeira `MinhaApi`. | 
| **Dia 06** | Dockers e Dbeaver | Desenvolvimento da API, com aplicação no Dorckers e Dbeaver |
| **Dia 07** | Dockers e Dbeaver | Continuação do projeto do dia 06, por isso não teve nova pasta com dia 07 |
| **Dia 08** | Tests e Proteção | Utilização do XUnit para testar todo o código do CRUD Api. |
| **Dia 09** | Tests e Proteção | Não foi criado uma nova pasta porque continuou os testes unitarios na pasta com o dia 08 |
| **Dia 10** | Classes Fila | Foi criada uma conexão com o Redis, para quando o POST fosse utilizado, aparecesse o processamento no Redis. Não foi criado uma nova pasta, pois foi utilizada a MinhaApi do dia 06|
| **Dia 11** | Minha Api Melhoras |Desenvolvido novos metodos para melhoria da MinhaApi e criado nova pasta dia11|


## 💡 Destaques do Projeto

### Conteúdo do Controller `LotesMinerio`
Responsável pela exposição dos endpoints REST da aplicação:
- Criação, consulta, atualização e remoção de lotes de minério.
- Validações de negócio (campos obrigatórios, limites de teor, umidade, etc.).
- Persistência das informações no PostgreSQL via Entity Framework.
- Publicação de eventos no Redis sempre que um novo lote é criado.
Ao realizar um POST, além de salvar no banco, a API envia uma mensagem para processamento assíncrono.

### Integração com Redis (Mensageria)

Implementei um Producer responsável por enviar eventos para um Redis Stream, permitindo comunicação desacoplada entre serviços.

Cada novo lote gera uma mensagem contendo:

- Id do lote
- Código
- Teor de ferro
- Umidade
- Data de produção
- Ação executada (Processamento)

Essa abordagem permite que workers ou outros sistemas consumam os dados de forma independente da API.

### Classe LoteQueueProducer
Camada responsável por:

- Conectar ao Redis usando ConnectionMultiplexer.
- Serializar a mensagem em JSON.
- Publicar o evento no stream configurado.
- Garantir um modelo de arquitetura assíncrona utilizado em ambientes distribuídos.

### Configuração via Program.cs
Aplicação estruturada com Injeção de Dependência, registrando:

- DbContext com PostgreSQL.
- Conexão com Redis.
- Opções de fila usando IOptions.
- Producer de mensageria.

Esse padrão facilita testes, manutenção e escalabilidade.


---

## ⌨️ Comandos Git/Terminal Utilizados
No dia a dia, utilizei os seguintes comandos para versionamento:

```bash
# Criar diretórios
mkdir [nome_da_pasta]

# Comandos .NET
dotnet new console  # Inicia o projeto
dotnet run          # Executa a aplicação
dotnet new webapi -n MinhaApi #Criação da APIWeb

# Versionamento Git
git add .
git commit -m "Explicação da alteração"
git push origin [branch]
git pull origin [branch]
git branch



