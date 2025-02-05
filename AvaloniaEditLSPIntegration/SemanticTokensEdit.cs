namespace AvaloniaEditLSPIntegration;

public record SemanticTokensEdit(int Start, int DeleteCount, IList<int> Data);