# Simple banking app

A very simple banking application built for gaining experience in asp.net microservices. There are few basic concepts
like identity, bank account, personal data, kyc approving process, withdraw/deposit operations. All microservices are
under one common solution for simplicity.

### Project structure:

* Chassis - microservices chassis pattern for sharing codebase between all microservices
* Chassis.Gateway - library for sharing codebase between grpc microservices
* Chassis.Gateway - library for sharing codebase between gateways (http api)
* Gateway.Root - http api for access to all grpc microservices
* Service.BankAccount - bank account operations (create, take funds, add funds etc)
* Service.Currency - service that is responsible for syncing currencies exchange rates
* Service.Identity - user operations (register, login). Access token pattern
* Service.IdentityNotifier - user notifications operations (sending registration confirmation letter)
* Service.PersonalData - banking users operations (saving kyc, creation personal data, approving/declining approval
  requests). Personal data is based on Identity and bank accounts are based on PersonalData
* Shared.Abstractions - library for sharing contracts
* Shared.Grpc - library for sharing grpc contracts
* Vladerlord.Banking.Deploy - helm package

### Infrastructure:

* Used tools: Linkerd service mesh, EFK for logging, Jaeger for tracing(opentelemetry), Prometheus for metrics(
  opentelemetry), CertManager, godaddy webhook for creating certificates based on godaddy domains, helmfile for helm
  deployment simplicity
* Security: dev tools without internal authentication page are hidden behind oauth2 protection
* Automation: github actions ci/cd(run unit tests, build and push docker images)

### Installation

1. Create Vladerlord.Banking.Deploy/environment/production/secrets.yaml from secrets.yaml.dist and values.yaml.dist
2. Run `make createCerts` to create linkerd certificates
3. Run `make createLinkerdNamespaces` to create linkerd namespaces (some parts of helmfile installation require linkerd
   namespaces before linkerd is installed)
4. Run `make sync`
