# 🏢 Sistema de Gestão (Estoque e Faturamento) - Full Stack

## 🎯 Sobre o Projeto
Este repositório contém a solução desenvolvida para um teste técnico prático focado em arquitetura Full Stack. O projeto é composto por um Front-end responsivo em Angular e duas APIs independentes no Back-end, simulando um ambiente de microsserviços para gestão de Estoque e Faturamento.

## 🛠️ Tecnologias Utilizadas
*   **Front-end:** Angular, TypeScript, RxJS, HTML/CSS.
*   **Back-end:** C# .NET (APIs REST).

## ⚙️ Detalhamento Técnico - Front-end (Angular)

*   **Ciclos de vida (Lifecycle Hooks):** Utilização do `ngOnInit` no componente principal para disparar as requisições HTTP iniciais e carregar as listas de Produtos e Notas na inicialização da tela.
*   **Reatividade com RxJS:** Consumo das APIs REST via `HttpClient` (`.subscribe()`). Aplicação avançada dos operadores `pipe` e `catchError` para interceptar erros vindos do backend (ex: falhas de comunicação entre os microsserviços) e tratá-los de forma reativa no front-end, alertando o usuário de maneira amigável.
*   **Módulos Nativos:** Utilização do `CommonModule` (para diretivas estruturais como `*ngIf` e `*ngFor`) e do `FormsModule` (para o Two-Way Data Binding através do `[(ngModel)]`).
*   **Componentes Visuais e Performance:** Para garantir leveza e rápida renderização, optei por não utilizar bibliotecas de componentes externas pesadas. A interface foi desenhada de forma limpa e responsiva utilizando puramente HTML e CSS nativos.

## ⚙️ Detalhamento Técnico - Back-end (C# .NET)

*   **Arquitetura:** O backend foi estruturado dividindo as responsabilidades de negócio, resultando em duas aplicações independentes:
    *   `EstoqueAPI`: Gerenciamento e persistência dos dados de produtos.
    *   `FaturamentoAPI`: Processamento de notas e regras de faturamento.
*   *Nota: O código foi refatorado visando padronização e limpeza de comentários, garantindo um Clean Code.*

## 🚀 Como Executar o Projeto Localmente

**1. Back-end (APIs em C#):**
*   Navegue até as pastas `EstoqueAPI` e `FaturamentoAPI`.
*   Restaure as dependências e execute cada projeto utilizando a sua IDE de preferência (Visual Studio / VS Code) ou via terminal com o comando `dotnet run`.

**2. Front-end (Angular):**
*   Navegue até a pasta `frontend`.
*   Instale as dependências executando: `npm install`
*   Inicie o servidor de desenvolvimento: `ng serve`
*   Acesse no navegador: `http://localhost:4200`
