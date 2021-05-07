using MemeSearch.Logic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemeSearch.Logic.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateSearch(string query, int results, int page, out string message)
        {
            var messages = new StringBuilder();
            ValidateQuery(query, messages);
            ValidatePagination(results, page, messages);
            message = messages.ToString();
            return messages.Length == 0;
        }

        public static bool ValidateAdvancedSearch(string query, SearchParameters parameters, int results, int page, out string message)
        {
            var messages = new StringBuilder();
            ValidateQuery(query, messages, true);
            ValidatePagination(results, page, messages);
            ValidateAdvancedSearchParams(parameters, messages, string.IsNullOrEmpty(query));
            message = messages.ToString();
            return message.Length == 0;
        }

        public static bool ValidateImageSearch(ImageSearchParameters parameters, int results, int page, out string message)
        {
            var messages = new StringBuilder();
            ValidatePagination(results, page, messages);
            ValidateImageSearchParams(parameters, messages);
            ValidateAdvancedSearchParams(parameters, messages, true);
            message = messages.ToString();
            return message.Length == 0;
        }


        private static void ValidateQuery(string query, StringBuilder message, bool allowEmpty = false)
        {
            if (string.IsNullOrWhiteSpace(query) && !allowEmpty) message.AppendLine("Empty query");
            
            query = query.Trim();

            var brackets = 0;
            var isInQuotes = false;

            for (int i=0; i<query.Length; i++)
            {
                if (query[i] == '(')
                {
                    brackets++;
                    if (i < query.Length - 1 && query[i + 1] != '(')
                    {
                        var substr = query.Substring(i + 1, query.Length-i-1).Trim();
                        if (substr.StartsWith("AND") || substr.StartsWith("OR"))
                        {
                            message.AppendLine("Invalid expression after opening brackets");
                            return;
                        }
                    }
                }
                if (query[i] == ')')
                {
                    if (brackets == 0)
                    {
                        message.AppendLine("Invalid order of brackets");
                        return;
                    }
                    brackets--;

                    if (i < query.Length - 1 && query[i+1] != ')'
                        && !CheckExpression(query, i))
                    {
                        message.AppendLine("Invalid expression after brackets");
                        return;
                    }

                    if (query[i - 1] != ')')
                    {
                        var substr = query.Substring(0, i).Trim();
                        if (substr.EndsWith("AND") || substr.EndsWith("OR"))
                        {
                            message.AppendLine("Invalid expression before closing brackets");
                            return;
                        }
                    }
                }

                if (query[i] == '"')
                {
                    if (!isInQuotes) isInQuotes = true;
                    else
                    {
                        isInQuotes = false;
                        if (i < query.Length - 1 && query[i+1] != ')' && !CheckExpression(query, i))
                        {
                            message.AppendLine("Invalid expression after quotes");
                            return;
                        }
                    }
                }

                var andIdx = query.IndexOf("AND");
                var orIdx = query.IndexOf("OR");

                if (andIdx == 0 || orIdx == 0)
                {
                    message.AppendLine("Operator cannot be first");
                    return;
                }

                if (andIdx == query.Length - 3 || orIdx == query.Length - 2)
                {
                    message.AppendLine("Operator cannot be last");
                    return;
                }

                andIdx = andIdx == -1 ? andIdx : andIdx + 3;
                orIdx = orIdx == -1 ? orIdx : orIdx + 2;

                var currentIdxEnd = orIdx == -1 ? andIdx : andIdx == -1 ? orIdx : andIdx < orIdx ? andIdx : orIdx;

                while(andIdx >= 0 || orIdx >= 0)
                {
                    andIdx = andIdx == -1 ? andIdx : query.IndexOf("AND", currentIdxEnd);
                    orIdx = orIdx == -1 ? orIdx : query.IndexOf("OR", currentIdxEnd);

                    var currentIdx = orIdx == -1 ? andIdx : andIdx == -1 ? orIdx : andIdx < orIdx ? andIdx : orIdx;
                    
                    if (currentIdx != -1 && query[currentIdxEnd..currentIdx].Trim().Length == 0)
                    {
                        message.AppendLine("No value between operators");
                        return;
                    }

                    andIdx = andIdx == -1 ? andIdx : andIdx + 3;
                    orIdx = orIdx == -1 ? orIdx : orIdx + 2;

                    currentIdxEnd = orIdx == -1 ? andIdx : andIdx == -1 ? orIdx : andIdx < orIdx ? andIdx : orIdx;
                }
            }

            if (brackets != 0)
            {
                message.AppendLine("Invalid number of brackets");
                return;
            }
        }

        private static bool CheckExpression(string query, int i)
        {
            // OR
            if (i < query.Length - 5
                && query.Substring(i+1, 4) == " OR ") return true;

            // AND
            if (i < query.Length - 6
                && query.Substring(i+1, 5) == " AND ") return true;

            return false;
        }

        private static void ValidatePagination(int results, int page, StringBuilder message)
        {
            if (page < 0) message.AppendLine("Page cannot be less than 0");
            if (results < 0) message.AppendLine("Results cannot be less than 0");
        }

        private static readonly List<string> _validSearchFields = new List<string>() { "Title", "Content", "Category", "Details", "Image" };
        private static readonly List<string> _validSortFields = new List<string>() { "Year" };

        private static void ValidateAdvancedSearchParams(SearchParameters parameters, StringBuilder message, bool hasQuery)
        {
            if (hasQuery && !parameters.Fields.Any()) message.AppendLine("Fields must contain at least 1 element");
            if (!parameters.Fields.All(f => _validSearchFields.Contains(f))) message.AppendLine("Invalid Fields list. Valid options are: " + string.Join(", ", _validSearchFields));
            if (parameters.Sort != null && !_validSortFields.Contains(parameters.Sort))message.AppendLine("Invalid Sort. Valid options are: " + string.Join(", ", _validSortFields));
            if (parameters.YearFrom.HasValue && parameters.YearTo.HasValue && parameters.YearFrom.Value > parameters.YearTo.Value) message.AppendLine("YearFrom cannot be greater than YearTo");
        }

        private static void ValidateImageSearchParams(ImageSearchParameters parameters, StringBuilder message)
        {
            if (string.IsNullOrWhiteSpace(parameters.Url)) message.AppendLine("Image url cannot be empty");
        }
    }
}
