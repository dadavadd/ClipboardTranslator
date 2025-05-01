using ClipboardTranslator.Core.ClipboardHandler;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.AITranslator;
using ClipboardTranslator.Core;

Console.Title = "https://github.com/dadavadd/ClipboardTranslator";


LogConfig.Configure(true);
var config = TranslatorConfig.Load();

using var translator = new Translator(
    new ClipboardMonitor(),
    new InputSimulator(),
    new AiTranslator(config)
);

Console.ReadLine();


