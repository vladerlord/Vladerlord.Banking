# Installation

1. Create environment/production/secrets.yaml from secrets.yaml.dist and values.yaml.dist
2. make createCerts to create linkerd certificates
3. make createLinkerdNamespaces to create linkerd namespaces (some parts of helmfile installation require linkerd
   namespaces before linkerd is installed)

# Used tools

* Logging - EFK stack
* Tracing - jaeger
* Metrics - prometheus
* Service mesh - linkerd
* Cluster security - dev tools are behind oauth2-proxy
