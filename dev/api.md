## 🗂 Estrutura de Rotas MVC (mínima)

| Método                       | Rota                                  | Função |
| ---------------------------- | ------------------------------------- | ------ |
| GET `/`                      | Dashboard com gráficos e estatísticas |        |
| GET `/Books`                 | Lista todos os livros                 |        |
| GET `/Books/Details/{id}`    | Detalhes de um livro                  |        |
| GET `/Books/Create`          | Formulário para criar livro           |        |
| POST `/Books/Create`         | Criar novo livro                      |        |
| GET `/Books/Edit/{id}`       | Formulário para editar livro          |        |
| POST `/Books/Edit/{id}`      | Editar livro                          |        |
| GET `/Books/Delete/{id}`     | Confirmação para deletar livro        |        |
| POST `/Books/Delete/{id}`    | Deletar livro                         |        |
| GET `/Readings`              | Lista leituras do usuário             |        |
| GET `/Readings/Details/{id}` | Detalhes de uma leitura               |        |
| GET `/Readings/Create`       | Formulário para criar leitura         |        |
| POST `/Readings/Create`      | Criar nova leitura                    |        |
| GET `/Readings/Edit/{id}`    | Formulário para editar leitura        |        |
| POST `/Readings/Edit/{id}`   | Editar leitura                        |        |
| GET `/Readings/Delete/{id}`  | Confirmação para deletar leitura      |        |
| POST `/Readings/Delete/{id}` | Deletar leitura                       |        |
