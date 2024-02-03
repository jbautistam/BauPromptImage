using Bau.Libraries.LibHelper.Extensors;

namespace BauPromptImage.Application.Parser;

/// <summary>
///		Intérprete de un prompt
/// </summary>
internal class PromptParser
{
	/// <summary>
	///		Interpreta un prompt
	/// </summary>
	internal string Parse(string prompt)
	{
		string result = string.Empty;

			// Interpreta las líneas
			if (!string.IsNullOrWhiteSpace(prompt))
			{
				List<string> blocks = ExtractBlocks(prompt);

					// Interpreta los bloques
					foreach (string block in blocks)
						result = result.AddWithSeparator(ParseBlock(block), ",");
			}
			// Devuelve la cadena generada
			return result;
	}

	/// <summary>
	///		Interpreta los bloques
	/// </summary>
	private List<string> ExtractBlocks(string prompt)
	{
		List<string> blocks = new();
		string actual = string.Empty;

			// Añade los bloques
			foreach (string line in prompt.Split('\r', '\n'))
				if (line.TrimIgnoreNull().StartsWith('#'))
				{
					// Cierra el bloque
					if (!string.IsNullOrWhiteSpace(actual))
						blocks.Add(actual);
					// Vacía el contenido actual
					actual = string.Empty;
				}
				else
					actual += Environment.NewLine + line;
			// Añade el último bloque
			if (!string.IsNullOrWhiteSpace(actual))
				blocks.Add(actual);
			// Devuelve los bloques generados
			return blocks;
	}

	/// <summary>
	///		Interpreta un bloque
	/// </summary>
	private string ParseBlock(string block)
	{
		string result = string.Empty;

			// Interpreta el bloque
			if (!string.IsNullOrEmpty(block))
			{
				int previousIndent = -1;

					// Procesa las líneas
					foreach (string line in block.Split('\r', '\n'))
						if (!string.IsNullOrWhiteSpace(line))
						{
							(int indent, string part) = GetIndent(line);

								if (indent >= previousIndent)
									result = result.AddWithSeparator(part, " ", false);
								else
									result = result.AddWithSeparator(part, ",");
								// Guarda la indentación actual
								previousIndent = indent;
						}
			}
			// Devuelve la cadena
			return result;
	}

	/// <summary>
	///		Obtiene la indentación
	/// </summary>
	private (int indent, string value) GetIndent(string line)
	{
		int indent = 0;
		string content = line;

			// Si realmente hay algo
			if (!string.IsNullOrWhiteSpace(line))
			{
				// Cuenta el número de caracteres vacíos iniciales
				while (indent < line.Length && (line[indent] == ' ' || line[indent] == '*' || line[indent] == '\t'))
					indent++;
				// Obtiene el contenido
				if (indent > 0 && indent < line.Length - 1)
					content = line[indent..];
				// Quita las comas finales
				content = content.TrimIgnoreNull();
				while (content.Length > 0 && (content.EndsWith(',') || content.EndsWith(';') || content.EndsWith('.')))
					content = content[..^1];
			}
			// Devuelve la indentación y la cadena cortada
			return (indent, content);
	}
}
