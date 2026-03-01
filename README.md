# SpeakUp

**AI-Powered Voice Assistant with Extensible Plugin Architecture**

A .NET MAUI cross-platform voice-controlled assistant that leverages AI agents to execute commands across multiple services and platforms. Simply speak your commands, and SpeakUp will intelligently orchestrate actions through its plugin ecosystem.

## 🎯 Overview

SpeakUp combines speech recognition (both online and offline) with Microsoft Agents AI to create a powerful, extensible automation platform. The modular plugin architecture allows for easy integration with various services, APIs, and system operations.

## ✨ Key Features

- **🎤 Voice Control**: Both online and offline speech-to-text recognition
- **🤖 AI Agent Execution**: Intelligent command interpretation and execution using Microsoft.Agents.AI
- **🔌 Plugin Architecture**: 20+ extensible plugins for different services
- **📱 Cross-Platform**: Built with .NET MAUI for Windows, macOS, iOS, and Android
- **🔄 Real-time Feedback**: Live activity logs and status updates
- **🧠 Context-Aware**: AI agent maintains command context and workflow understanding

## 📦 Available Plugins

### Communication
- **Telegram** - Send messages and manage Telegram interactions
- **Slack** - Post messages to Slack channels
- **Viber** - Send Viber messages
- **Twitter** - Publish and delete tweets
- **Facebook** - Create social media posts

### Email Services
- **SendGrid** - Send emails via SendGrid API
- **Mailgun** - Email delivery through Mailgun
- **ElasticEmail** - Email service integration
- **Twilio** - SMS messaging capabilities

### Cloud Storage
- **Amazon S3** - List and manage S3 files
- **Google Drive** - Access and list Google Drive files
- **Mega** - Download and list files from Mega.nz

### System Operations
- **File System** - Local file operations and listing
- **Process** - System process management
- **SSH** - Remote command execution
- **SFTP/SCP** - Secure file transfers
- **Device** - Geolocation and device capabilities

### Data & Development
- **Database** - Database operations and queries
- **Application Insights** - Telemetry and logging
- **HTTP** - REST API interactions
- **Random** - Generate random numbers and strings

### External APIs
- **AbstractAPI** - VAT validation, phone validation, IP geolocation, IBAN validation, holidays lookup, exchange rates, email validation

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio 2026 or later (for development)
- OpenAI API key or compatible AI service

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/SpeakUp.git
   cd SpeakUp
   ```

2. **Configure AI Key**
   
   Update `SpeakUp/appsettings.json`:
   ```json
   {
       "AIKey": "your-openai-api-key"
   }
   ```

3. **Build and Run**
   ```bash
   dotnet build
   dotnet run --project SpeakUp
   ```

### Usage

1. Launch the application
2. Choose between online or offline speech recognition
3. Click **Start** to begin listening
4. Speak your command (e.g., "Start notepad and type Hello World")
5. The AI agent will interpret and execute your command
6. View real-time logs in the Activity Log section

## 🏗️ Architecture

```
SpeakUp/
├── SpeakUp/              # Main MAUI application
│   ├── Executor/         # McpExecutor and AI agent logic
│   ├── MainPage.xaml     # UI
│   └── MainPageViewModel.cs
├── Plugins/              # Extensible plugin system
│   ├── Base.Extensions/  # Base classes and interfaces
│   ├── AIExtensions/
│   ├── DatabaseExtensions/
│   └── [20+ plugins]
├── Shared/               # Shared models and utilities
└── PluginTester/         # Plugin testing console app
```

## 🛣️ Roadmap & Planned Enhancements

### 🌟 High Priority

#### 1. **Plugin Marketplace & Management System**
- Plugin discovery and browsing UI
- Enable/disable plugins without restart
- Plugin versioning and dependency resolution
- Hot-reload capabilities
- Plugin templates and scaffolding tools
- **Value**: Creates ecosystem potential, community contributions

#### 2. **Conversation History & Context Awareness**
- SQLite-based session storage
- Save/load/export conversation history
- Multi-session support with timestamps
- Command usage analytics and patterns
- Search through past commands
- **Value**: Users can review interactions, improve AI accuracy over time

#### 3. **Custom Voice Commands & Workflows**
- Visual workflow builder for chaining commands
- User-defined voice trigger keywords
- Scheduled command execution
- Conditional logic and branching
- Macro recording and playback
- **Value**: Power users can automate complex multi-step tasks

#### 4. **Security & Authentication Layer**
- Biometric authentication for sensitive operations
- Encrypted credential vault (replace plain appsettings)
- OAuth 2.0 flow support for plugins
- Command approval system for destructive actions
- Role-based access control
- Audit logging for compliance
- **Value**: Enterprise-ready security for production environments

#### 5. **Comprehensive Testing Infrastructure**
- Unit tests for all plugins
- Integration tests for McpExecutor
- Mock speech-to-text for automated UI testing
- Performance benchmarks
- CI/CD pipeline with automated testing
- Code coverage reporting
- **Value**: Reliability, maintainability, contributor confidence

### 🎨 Medium Priority

#### 6. **Multi-Language Support**
- Localized resource files for all UI strings
- Support for multiple speech recognition languages
- AI response translation
- Regional format support (dates, numbers, currency)
- **Value**: Global audience reach

#### 7. **Performance Monitoring & Telemetry**
- Real-time command execution metrics dashboard
- Plugin performance profiling
- Error tracking and automatic retry logic
- Offline command queueing
- Health checks and status pages
- **Value**: Production readiness, improved debugging

#### 8. **Enhanced Desktop Experience**
- System tray/notification area integration
- Global hotkeys for voice activation
- Desktop notifications for command completion
- Windows/macOS menubar widget
- Always-on-top mini mode
- **Value**: Professional desktop productivity tool

#### 9. **Voice Feedback & Text-to-Speech**
- Audible confirmation of completed actions
- Error announcements
- Customizable voice personas
- Response tone and style settings
- **Value**: True hands-free, eyes-free experience

### 📚 Documentation & Community

#### 10. **Plugin Development SDK**
- Comprehensive plugin development guide
- API reference documentation
- Sample plugin projects with best practices
- Architecture diagrams and design patterns
- Video tutorials
- **Value**: Easier community contributions, faster onboarding

### 🔧 Quick Wins (Near-term)

- ✅ **Settings Page**: Configure AI provider, speech settings, plugin configurations **(IMPLEMENTED)**
- ✅ **Error Handling UI**: Display user-friendly error messages instead of logs only **(IMPLEMENTED)**
- ✅ **Command History View**: Scrollable list of past commands with timestamps **(IMPLEMENTED)**
- ✅ **Export Logs**: Save activity logs to file for debugging **(IMPLEMENTED)**
- ✅ **Plugin Status Indicators**: Visual feedback for which plugins are loaded/active **(IMPLEMENTED)**

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow .NET coding conventions and include XML comments
4. Write unit tests for new functionality
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

### Creating a New Plugin

1. Reference `Base.Extensions` project
2. Implement `MacroCommand` base class
3. Add proper `Description` attribute for AI discovery
4. Include XML documentation comments
5. Add unit tests in a corresponding test project

## 📄 License

[Specify your license here]

## 🙏 Acknowledgments

- Microsoft.Agents.AI for agent orchestration
- CommunityToolkit.Maui for MAUI extensions
- OpenAI for language model capabilities
- All plugin service providers

## 📞 Support & Contact

- **Issues**: [GitHub Issues](https://github.com/yourusername/SpeakUp/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/SpeakUp/discussions)

---

**Built with .NET 10 & .NET MAUI** 🚀