Detalhamento Técnico - Front-end (Angular)

Ciclos de vida utilizados: Foi utilizado o ngOnInit no componente principal para disparar as requisições HTTP iniciais e carregar as listas de Produtos e Notas na inicialização da tela.

Uso da biblioteca RxJS: Utilizada para o consumo das APIs REST via HttpClient (.subscribe()). Além disso, apliquei os operadores pipe e catchError para interceptar os erros vindos do backend (ex: falha de comunicação entre microsserviços) e tratá-los de forma reativa no front-end, alertando o usuário.

Outras bibliotecas Angular: CommonModule (para diretivas como *ngIf e *ngFor) e FormsModule (para o Two-Way Data Binding com [(ngModel)]).

Componentes Visuais: Para garantir leveza, optei por não utilizar bibliotecas externas pesadas. A interface foi desenhada de forma limpa e responsiva utilizando HTML/CSS nativos.

Detalhamento Técnico - Back-end (C# .NET)

Frameworks e Arquitetura: Desenvolvido em C# (.NET 8) utilizando Minimal APIs. Foram criados dois microsserviços independentes (EstoqueAPI e FaturamentoAPI). Para a persistência de dados, utilizei o Entity Framework Core com bancos SQLite locais.

Tratamento de Falhas (Erros e Exceções): A comunicação com o Estoque está em um bloco try-catch. Caso ocorra uma HttpRequestException (simulando a queda do serviço de Estoque), o FaturamentoAPI captura a exceção e retorna um erro formatado com Status 503 Service Unavailable, garantindo a recuperação do sistema e o envio da mensagem ao front-end.

Uso de LINQ: Utilizado nas consultas do Entity Framework (ex: ToListAsync() e FindAsync()) para buscar produtos e listas de notas diretamente no banco de dados.
