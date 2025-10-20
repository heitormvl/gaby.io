# Sistema de Avaliação de Livros

## Resumo da Implementação

Implementamos um sistema completo de avaliação de livros onde cada usuário pode avaliar os livros que leu e ver a nota média na tela de detalhes do livro.

## Alterações Realizadas

### 1. Models

#### ReadingModel.cs
### 1. Models

#### ReadingModel.cs
- Campo `Rating` alterado para `int?` com `[Range(0, 5)]` (anteriormente `decimal(2,1)`)
- Adicionados campos: `StartDate` (DateTime?), `EndDate` (DateTime?), `Status` (string), `PagesRead` (int)
- Campo `UserId` alterado para `string` (Identity padrão)
- Adicionados campos: `StartDate`, `EndDate`, `Status`, `PagesRead`
- Campo `UserId` alterado para `string` (Identity padrão)

### 2. ViewModels

#### ReadingFormViewModel.cs
- Adicionado campo `Rating` com validação de 0 a 5 para permitir ao usuário avaliar o livro
- Adicionados campos: `StartDate`, `EndDate`, `Status`, `PagesRead`

#### ReadingDetailsViewModel.cs
- Adicionado campo `Rating` para exibir a avaliação na tela de detalhes
- Adicionados campos: `StartDate`, `EndDate`, `Status`, `PagesRead`, `TotalPages`

#### BookDetailsViewModel.cs
- Adicionado campo `AverageRating` (decimal?) para mostrar a nota média
- Adicionado campo `TotalRatings` (int) para mostrar o número total de avaliações

### 3. Controllers

#### ReadingController.cs
- **Create**: Atualizado para salvar o valor da avaliação (`Rating`) e novos campos (`StartDate`, `EndDate`, `Status`, `PagesRead`)
- **Edit**: Atualizado para carregar e salvar o valor da avaliação e novos campos
- **Details**: Atualizado para exibir a avaliação do usuário e novos campos

#### BookController.cs
- **Details**: Implementada lógica para calcular a nota média das avaliações:
  - Filtra apenas avaliações com valor maior que 0
  - Calcula a média arredondada para 1 casa decimal
  - Conta o total de avaliações

### 4. Views

#### Views/Reading/Create.cshtml
- Adicionado campo de avaliação com input numérico (0-5, step 1)
- Adicionados campos: Data de início, Data de término, Status, Páginas lidas
- Campo opcional (pode ser deixado em branco)

#### Views/Reading/Edit.cshtml
- Adicionado campo de avaliação com input numérico (0-5, step 1)
- Adicionados campos para editar os novos atributos
- Permite editar a avaliação existente

#### Views/Reading/Details.cshtml
- Adicionada seção para exibir a avaliação do usuário
- Exibição visual com estrelas (completas, metade e vazias)
- Mostra o valor numérico da avaliação
- Exibe os novos campos: datas, status, páginas lidas

#### Views/Book/Details.cshtml
- Adicionada seção para exibir a **avaliação média** do livro
- Exibição visual com estrelas
- Mostra o valor numérico e o total de avaliações
- Ex: "★★★★☆ 4.2 (15 avaliações)"
- Também adicionada a editora que estava faltando

## Como Funciona

### Para o Usuário Avaliar um Livro:

1. Usuário acessa a tela de **Leituras** (`/Reading`)
2. Cria uma nova leitura ou edita uma existente
3. No formulário, pode informar a avaliação de 0 a 5 (com incrementos de 0.5)
4. Pode deixar em branco se não quiser avaliar ainda
5. A avaliação fica salva junto com o registro de leitura

### Visualização da Nota Média:

1. Acesse a tela de **Detalhes do Livro** (`/Book/Details/{id}`)
2. A seção "Avaliação Média" mostrará:
   - Estrelas visuais (★★★★☆)
   - Nota numérica (ex: 4.2)
   - Quantidade de avaliações (ex: "15 avaliações")
3. Se não houver avaliações, exibe "Sem avaliações"

## Características do Sistema

- ✅ Cada usuário pode avaliar um livro de 0 a 5 estrelas (inteiro)
- ✅ Avaliações são opcionais (podem ficar em branco)
- ✅ Cálculo automático da média das avaliações
- ✅ Contagem do total de avaliações
- ✅ Exibição visual com ícones de estrelas
- ✅ Filtra apenas avaliações válidas (maiores que 0)
- ✅ Nota média arredondada para 1 casa decimal
- ✅ Novos campos para rastreamento detalhado de leituras

## Próximos Passos Sugeridos

1. Criar uma migration para garantir que o campo está correto no banco:
   ```bash
   dotnet ef migrations add AddRatingValidation
   dotnet ef database update
   ```

2. Melhorias futuras (opcionais):
   - Adicionar input de estrelas clicáveis no formulário (em vez de campo numérico)
   - Mostrar histograma de avaliações (quantas 5 estrelas, 4 estrelas, etc)
   - Adicionar comentários/reviews junto com as avaliações
   - Exibir as avaliações individuais na página do livro
   - Permitir que outros usuários vejam as avaliações da comunidade

## Validações Implementadas

- `Range(0, 5)` no Model para garantir valores válidos (inteiro)
- Campo opcional (aceita `null`)
- Tipo `int` no banco de dados
- Filtro de avaliações > 0 no cálculo da média (ignora valores 0 ou null)
