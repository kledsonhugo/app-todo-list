using System.Text.RegularExpressions;

namespace TodoListApp.Services
{
    public interface IIconService
    {
        string GetIconForTask(string title, string? description = null);
    }

    public class IconService : IIconService
    {
        private readonly Dictionary<string, string[]> _categoryKeywords = new()
        {
            { "fas fa-book", new[] { "estudar", "estudo", "aprender", "curso", "educação", "lição", "aula", "escola", "universidade", "faculdade" } },
            { "fas fa-dumbbell", new[] { "exercício", "exercícios", "treino", "academia", "ginástica", "caminhada", "corrida", "esporte", "saúde", "fitness" } },
            { "fas fa-file-alt", new[] { "documentação", "ler", "leitura", "documento", "manual", "guia", "artigo", "livro", "texto" } },
            { "fas fa-briefcase", new[] { "trabalho", "reunião", "meeting", "cliente", "negócio", "empresa", "profissional", "carreira", "orçamento", "contrato" } },
            { "fas fa-code", new[] { "programar", "desenvolvimento", "código", "api", "sistema", "software", "aplicação", "git", "deploy", ".net", "javascript", "python", "java" } },
            { "fas fa-shopping-cart", new[] { "comprar", "compra", "shopping", "mercado", "loja", "produto", "item", "lista de compras" } },
            { "fas fa-home", new[] { "casa", "doméstico", "limpar", "limpeza", "organizar", "cozinhar", "família", "pessoal" } },
            { "fas fa-car", new[] { "carro", "viagem", "viajar", "transporte", "dirigir", "combustível", "mecânico", "revisão" } },
            { "fas fa-medkit", new[] { "médico", "consulta", "saúde", "hospital", "dentista", "exame", "medicamento", "tratamento" } },
            { "fas fa-utensils", new[] { "comer", "almoço", "jantar", "café", "comida", "restaurante", "receita", "cozinhar" } },
            { "fas fa-phone", new[] { "ligar", "telefonar", "contato", "telefone", "chamada", "comunicação" } },
            { "fas fa-envelope", new[] { "email", "carta", "correspondência", "mensagem", "enviar", "responder" } },
            { "fas fa-calendar", new[] { "agendar", "agenda", "compromisso", "evento", "data", "prazo", "deadline" } },
            { "fas fa-dollar-sign", new[] { "dinheiro", "pagamento", "conta", "financeiro", "banco", "cartão", "fatura", "imposto" } },
            { "fas fa-gift", new[] { "presente", "aniversário", "festa", "celebração", "comemoração", "natal", "páscoa" } }
        };

        public string GetIconForTask(string title, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "fas fa-tasks"; // Default icon

            var combinedText = $"{title?.ToLowerInvariant()} {description?.ToLowerInvariant()}".Trim();

            // Try to find matching keywords
            foreach (var category in _categoryKeywords)
            {
                var icon = category.Key;
                var keywords = category.Value;

                foreach (var keyword in keywords)
                {
                    if (ContainsWord(combinedText, keyword))
                    {
                        return icon;
                    }
                }
            }

            // Default icon if no match found
            return "fas fa-tasks";
        }

        private static bool ContainsWord(string text, string word)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(word))
                return false;

            // Use word boundaries to ensure we match complete words
            var pattern = $@"\b{Regex.Escape(word)}\b";
            return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
        }
    }
}