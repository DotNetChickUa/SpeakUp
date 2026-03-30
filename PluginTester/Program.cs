//using AbstractApiExtensions;

//Console.WriteLine(await PhoneValidation.IsPhoneNumberValid("", "", CancellationToken.None));
await OfficeExtensions.OfficeCommands.WordToPdfSpireDoc("1.docx", "4.pdf");
OfficeExtensions.OfficeCommands.WordToPdfCOM("C:\\Projects\\SpeakUp\\PluginTester\\bin\\Debug\\net10.0\\1.docx", "C:\\Projects\\SpeakUp\\PluginTester\\bin\\Debug\\net10.0\\5.pdf");
await OfficeExtensions.OfficeCommands.WordToPdfSyncfusion("1.docx", "6.pdf");
