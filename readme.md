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
| **Dia 11** | Minha Api Melhorada |Desenvolvido novos metodos para melhoria da MinhaApi e criado nova pasta dia11|


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

### 🚀 Como Rodar o Projeto Desafio
### Pré-requisitos
* [.NET 9 SDK](https://dotnet.microsoft.com/download)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Passo a Passo

1. **Subir o Banco de Dados:**
   No diretório raiz do projeto (onde está o arquivo `docker-compose.yml`), execute:

   services:
  postgres:
    image: postgres:16
    container_name: pg_desafio
    restart: unless-stopped
    environment:
      POSTGRES_USER: ${POSTGRES_USER:-postgres}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-postgres}
      POSTGRES_DB: ${POSTGRES_DB:-api_db}
      PGDATA: /var/lib/postgresql/data/pgdata
    ports:
      - "${POSTGRES_PORT:-5431}:5432"
    volumes:
      - pg_desafio:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-postgres} -d ${POSTGRES_DB:-minhaapi_db}"]
      interval: 5s
      timeout: 5s
      retries: 10
      start_period: 10s
    labels:
      io.rancher.container.pull_image: always
      io.rancher.container.name: "PostgreSQL - Estudos"

volumes:
  pg_desafio:
    driver: local
  
   
docker-compose up -d

 ## Configurar o Banco (Script SQL):
    Abra o DBeaver, conecte-se ao Postgres (localhost:5431) e execute o script SQL de criação da tabela equipamentos fornecido na pasta /Database.
    Rodar a API:
    dotnet run

##  Exemplo de descrição do CRUD:


## 🛠️ API Endpoints - Equipamentos

| Método | Endpoint | Descrição |
| :--- | :--- | :--- |
| GET | `/api/equipamentos` | Lista todos os ativos |
| GET | `/api/equipamentos/{id}` | Busca um ativo por ID |
| POST | `/api/equipamentos` | Cadastra um novo ativo |
| PUT | `/api/equipamentos/{id}` | Atualiza dados (Valida horímetro) |
| DELETE | `/api/equipamentos/{id}` | Remove um ativo do sistema |


## Export do Insonmia

### Post Desafio (testando erros)
curl --request POST \
  --url http://localhost:5087/api/equipamentos \
  --header 'Content-Type: application/json' \
  --header 'User-Agent: insomnia/12.3.1' \
  --data '{
  "codigo": "",
  "tipo": "caminhao",
  "modelo": "",
  "horimetro": -10,
  "statusOperacional": "Operacional",
  "dataAquisicao": "2026-02-19T00:00:00Z"
}'

### Put Desafio 
curl --request PUT \
  --url http://localhost:5087/api/equipamentos/1 \
  --header 'Content-Type: application/json' \
  --header 'User-Agent: insomnia/12.3.1' \
  --data '{
	
	"codigo": "CAT-793F-002",
	"tipo": "Caminhao",
	"modelo": "Caterpillar 793F",
	"horimetro": 1250.50,
	"statusOperacional": "Operacional",
	"dataAquisicao": "2023-05-20T00:00:00",
	"localizacaoAtual": "Mina Carajás N4E"
}'

### Get Desafio
http://localhost:5087/api/equipamentos

### Post Desafio
curl --request POST \
  --url http://localhost:5087/api/equipamentos \
  --header 'Content-Type: application/json' \
  --header 'User-Agent: insomnia/12.3.1' \
  --data '{
  "codigo": "CAT-793F",
  "tipo": "Caminhao",
  "modelo": "Caterpillar 793F",
  "horimetro": 1500.50,
  "statusOperacional": "Operacional",
  "dataAquisicao": "2024-05-20T00:00:00Z",
  "localizacaoAtual": "Mina de Ferro - Setor Norte"
}'



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
 



