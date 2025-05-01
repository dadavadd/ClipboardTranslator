using System.Text.Json.Serialization;
using ClipboardTranslator.Configuration;
using ClipboardTranslator.Translator.Models.AiRequest;
using ClipboardTranslator.Translator.Models.AiResponse;

namespace ClipboardTranslator.Models;

// Config
[JsonSerializable(typeof(Config))]
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
public partial class SerializationConfig : JsonSerializerContext;
