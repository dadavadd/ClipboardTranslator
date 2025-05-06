using System.Text.Json.Serialization;
using ClipboardTranslator.Core.Translators;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.Translators.Ai.Models.AiRequest;
using ClipboardTranslator.Core.Translators.Ai.Models.AiResponse;
using System.Text.Json;

namespace ClipboardTranslator.Core.Models;

// Config
[JsonSerializable(typeof(TranslatorConfig))]
[JsonSerializable(typeof(GeminiOptions))]
[JsonSerializable(typeof(LanguagePair))]

// AiRequest
[JsonSerializable(typeof(RequestBody))]
[JsonSerializable(typeof(GenerationConfig))]
[JsonSerializable(typeof(RequestContent))]
[JsonSerializable(typeof(RequstPart))]

// AiResponse
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(Candidate))]
[JsonSerializable(typeof(ResponseContent))]
[JsonSerializable(typeof(ResponsePart))]
[JsonSerializable(typeof(UsageMetadata))]
[JsonSerializable(typeof(PromptTokensDetail))]

[JsonSerializable(typeof(JsonElement))]

internal partial class SerializationConfig : JsonSerializerContext;
