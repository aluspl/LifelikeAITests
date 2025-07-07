# AiAgent

## Project Description

AiAgent is a versatile API designed to interact with various AI models, including OpenAI and Google Gemini. It provides a unified interface for processing chat requests, managing chat history, and handling custom instructions for AI models. The project is built with .NET and leverages MongoDB for data persistence.

Key features include:
- Integration with OpenAI and Google Gemini APIs.
- Centralized chat history management.
- Dynamic instruction handling for AI models.
- Scalable and testable architecture.
- Containerization with Docker for easy deployment.
- Kubernetes manifests for production deployment.

## Tech Stack

- **Backend**: .NET 9.0 (C#)
- **Database**: MongoDB
- **AI Integration**: OpenAI API, Google Gemini API
- **Containerization**: Docker
- **Orchestration**: Kubernetes
- **API Documentation**: Scalar
- **Testing**: xUnit, Moq

## Configuration

To run this project, you need to configure several environment variables and settings.

### Environment Variables

Create a `.env` file in the project root directory based on `.env.example` and fill in your API keys and MongoDB credentials:

```
OPENAI_API_KEY=your_openai_api_key_here
GEMINI_API_KEY=your_gemini_api_key_here

MONGO_ROOT_USERNAME=root
MONGO_ROOT_PASSWORD=example
```

### Application Settings

Application settings are managed via `appsettings.json` and `appsettings.Development.json`. You can configure AI model endpoints, deployment names, and other application-specific settings here.

Example `appsettings.json` (relevant sections):

```json
{
  "AI": {
    "OpenAI": {
      "ApiKey": "",
      "Endpoint": "https://challenge-ai-openai.openai.azure.com/",
      "Deployment": "4oMini",
      "SystemChatMessage": "You are an assistant designed to write intriguing job descriptions."
    },
    "Gemini": {
      "ApiKey": "",
      "Model": "gemini-2.5-flash"
    }
  },
  "MongoDB":{
    "ConnectionString": "mongodb://root:example@mongodb:27017",
    "DatabaseName": "Chat"
  },
  "ShowScalar": true
}
```

### Running the Project

#### With Docker Compose (Recommended for local development)

Ensure Docker is installed and running. Navigate to the project root directory and run:

```bash
docker-compose up --build
```

This will build the Docker images, start the API service, and a MongoDB instance. The API will be accessible at `http://localhost:2137`.

#### Locally (without Docker)

1. **Install .NET SDK**: Make sure you have .NET 9.0 SDK installed.
2. **Restore dependencies**: Navigate to the `AiAgent.Api` directory and run:
   ```bash
   dotnet restore
   ```
3. **Run the application**: From the `AiAgent.Api` directory, run:
   ```bash
   dotnet run
   ```
   Ensure your environment variables are set correctly before running.

### Testing

To run unit and integration tests, navigate to the project root and execute:

```bash
dotnet test AiAgent.Api.Tests/
dotnet test Api.IntegrationTests/
```

### Deployment to Kubernetes

Kubernetes manifests are located in the `deployment/` directory. These files are configured for a basic deployment with a namespace, deployment, service, and ingress.

**Prerequisites:**
- A Kubernetes cluster (e.g., k3s, Minikube).
- `kubectl` configured to connect to your cluster.
- Cert-Manager installed and configured with a `ClusterIssuer` (e.g., `letsencrypt-prod`) for TLS certificates if you plan to use the provided Ingress configuration.

**Steps to Deploy:**

1. **Create Namespace**: (If not already created)
   ```bash
   kubectl apply -f deployment/00-namespace.yaml
   ```

2. **Create Secret for Google Application Credentials**: (If using Gemini with GSA)
   Create a Kubernetes secret named `gcp-key` from your Google Service Account JSON key file. Replace `path/to/your/gcp-key.json` with the actual path to your key file.
   ```bash
   kubectl create secret generic gcp-key --from-file=key.json=path/to/your/gcp-key.json -n ai-agent
   ```

3. **Create ConfigMap**: This will inject configuration values into your deployment.
   ```bash
   kubectl apply -f deployment/02-configmap.yaml
   ```

4. **Deploy Application**: This will create the Deployment and Service.
   ```bash
   kubectl apply -f deployment/03-deployment.yaml
   kubectl apply -f deployment/04-service.yaml
   ```

5. **Deploy Ingress**: (Optional, if you want external access via Ingress)
   ```bash
   kubectl apply -f deployment/05-ingress.yaml
   ```
   Ensure your DNS is configured to point `agent.lifelike.cloud` to your Ingress controller's external IP.

## Contributing

Feel free to fork the repository, open issues, and submit pull requests.