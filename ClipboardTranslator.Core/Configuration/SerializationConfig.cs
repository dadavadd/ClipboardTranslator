using System.Text.Json.Serialization;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.AITranslator.Models.AiRequest;
using ClipboardTranslator.Core.AITranslator.Models.AiResponse;

namespace ClipboardTranslator.Core.Models;

// Config
[JsonSerializable(typeof(TranslatorConfig))]
[JsonSerializable(typeof(GeminiOptions))]
[JsonSerializable(typeof(LanguagePair))]

// Request
[JsonSerializable(typeof(RequestBody))]
[JsonSerializable(typeof(GenerationConfig))]
[JsonSerializable(typeof(RequestContent))]
[JsonSerializable(typeof(RequstPart))]

// Response
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(Candidate))]
[JsonSerializable(typeof(ResponseContent))]
[JsonSerializable(typeof(ResponsePart))]
[JsonSerializable(typeof(UsageMetadata))]
[JsonSerializable(typeof(PromptTokensDetail))]
internal partial class SerializationConfig : JsonSerializerContext;
