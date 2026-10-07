# ASP.NET Core CI/CD Pipeline – Azure DevOps + Azure App Service

This project demonstrates a complete CI/CD pipeline using:

- ASP.NET Core Web App
- GitHub (source control)
- Azure DevOps Pipelines (CI)
- Azure DevOps Release Pipeline (CD)
- Azure App Service (deployment)

It is based on my real deployment from my Azure DevOps organization **webap1** and project **DevWeb**.

## 🚀 Architecture Overview
Developer → GitHub → Azure DevOps CI → Artifact → Azure DevOps CD → Azure App Service → Live Website

## 🧩 CI Pipeline
See: `azure-devops/ci-pipeline.yml`

## 🔄 CD Pipeline
See: `azure-devops/release-pipeline.md`

## 🌐 Live Deployment
`https://myfirstw-abgmgnbydqaga9dg.westus3-01.azurewebsites.net`

## 📸 Screenshots
See `azure-devops/screenshots/`

## 🎯 Skills Demonstrated
- CI/CD pipeline design
- Azure DevOps YAML authoring
- Artifact management
- Azure App Service deployment
- GitHub integration
