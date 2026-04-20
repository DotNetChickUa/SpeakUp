# SpeakUp

**AI-Powered Voice Assistant with Extensible Plugin Architecture**

A .NET MAUI cross-platform voice-controlled assistant that leverages AI agents to execute commands across multiple services and platforms. Simply speak your commands, and SpeakUp will intelligently orchestrate actions through its plugin ecosystem.

## Overview

SpeakUp combines speech recognition (both online and offline) with Microsoft Agents AI to create a powerful, extensible automation platform. The modular plugin architecture allows for easy integration with various services, APIs, and system operations.

## Key Features

- **Voice Control**: Both online and offline speech-to-text recognition
- **AI Agent Execution**: Intelligent command interpretation and execution using Microsoft.Agents.AI
- **Plugin Architecture**: 100+ extensible plugins for different services
- **Cross-Platform**: Built with .NET MAUI for Windows, macOS, iOS, and Android
- **Real-time Feedback**: Live activity logs and status updates
- **Context-Aware**: AI agent maintains command context and workflow understanding

## Available Plugins

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

and more...

## Getting Started

### Prerequisites

- .NET 10 SDK
- OpenAI API key or compatible AI service

### Usage

1. Launch the application
1. Setup your AI agent with the desired plugins and configurations in Settings
2. Choose between online and offline speech recognition
3. Click **Start** to begin listening
4. Speak your command (e.g., "Start notepad")
5. Click **Stop**
6. The AI agent will interpret and execute your command
7. View real-time logs in the Activity Log section

## Video

[SpeakUp Demo](./Video%20Project.mp4)