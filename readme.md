# Bootcamp DTT - Jornada C# & .NET

Bem-vindo ao repositório do meu aprendizado no **Bootcamp DTT**. Aqui documento toda a minha evolução, desde a lógica básica até a construção de APIs robustas utilizando o ecossistema .NET.

> Status: Em progresso 🚀 (Atualizado conforme as aulas avançam)

---

## 🛠️ Tecnologias e Ferramentas 
* **Linguagem:** C#
* **Framework:** .NET (Console & Web API)
* **Ferramentas:** Git, GitHub, VS Code/Visual Studio, Dockers, Dbeaver

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
| **Dia 07** | Dockers e Dbeaver | Continuação do projeto do dia 06, por isso não teve nova pasta com o dia 07 |

---

## 💡 Destaques do Projeto

### Conteúdo da classe `Visitante`
Focada em **Modelagem de Dados**, esta classe gerencia:
- Atributos como `Nome`, `Id`, e horários.
- Métodos para formatação de data/hora no padrão `HH:mm`.
- Geração de tabelas de informações para o cliente.

### Conteúdo da classe `Program`
O "coração" das aplicações console, onde implementei:
- Menus interativos com `while` e `switch`.
- Persistência em listas para busca e filtragem.
- Tratamento de erros com `try-catch` para uma melhor experiência do usuário.

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



