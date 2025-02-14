namespace AvaloniaEditLSPIntegration;

public record SemanticTokensEdit(uint Start, uint DeleteCount, IList<uint> Data);