using System.Text.Json.Serialization;
using ClipboardTranslator.Core.Translators.Ai.Models.AiRequest;
using ClipboardTranslator.Core.Translators.Ai.Models.AiResponse;
using System.Text.Json;

namespace ClipboardTranslator.Core.Configuration;


[JsonSerializable(typeof(TranslatorConfig))]
[JsonSerializable(typeof(AiRequestBody))]
[JsonSerializable(typeof(AiResponseBody))]
[JsonSerializable(typeof(JsonElement))]

internal partial class SerializationConfig : JsonSerializerContext;
