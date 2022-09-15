createCerts:
	step certificate create \
        root.linkerd.cluster.local \
        ./Vladerlord.Banking.Deploy/Utils/linkerdCerts/control-plane-ca.crt \
        ./Vladerlord.Banking.Deploy/Utils/linkerdCerts/control-plane-ca.key \
        --profile root-ca --no-password --insecure --not-after=87600h -f
	step certificate create webhook.linkerd.cluster.local \
        ./Vladerlord.Banking.Deploy/Utils/linkerdCerts/webhook-ca.crt \
        ./Vladerlord.Banking.Deploy/Utils/linkerdCerts/webhook-ca.key \
        --profile root-ca --no-password --insecure \
        --san webhook.linkerd.cluster.local --not-after=87600h -f

createLinkerdNamespaces:
	kubectl create namespace linkerd-viz
	kubectl label namespace linkerd-viz linkerd.io/extension=viz
	kubectl annotate namespace linkerd-viz linkerd.io/inject=enabled
	kubectl create namespace linkerd-jaeger
	kubectl label namespace linkerd-jaeger linkerd.io/extension=jaeger
	kubectl annotate namespace linkerd-jaeger linkerd.io/inject=enabled
	kubectl create namespace linkerd

sync:
	helmfile -f ./Vladerlord.Banking.Deploy/helmfile.yaml -e production sync --concurrency 1

buildAndPushImages:
	docker build -t vladerlord/vladerlord.banking.service.currency:latest -f Service.Currency/Dockerfile .
	docker build -t vladerlord/vladerlord.banking.service.identity:latest -f Service.Identity/Dockerfile .
	docker build -t vladerlord/vladerlord.banking.service.identitynotifier:latest -f Service.IdentityNotifier/Dockerfile .
	docker build -t vladerlord/vladerlord.banking.service.bankaccount:latest -f Service.BankAccount/Dockerfile .
	docker build -t vladerlord/vladerlord.banking.service.personaldata:latest -f Service.PersonalData/Dockerfile .
	docker build -t vladerlord/vladerlord.banking.gateway.root:latest -f Gateway.Root/Dockerfile .

	docker push vladerlord/vladerlord.banking.service.currency:latest
	docker push vladerlord/vladerlord.banking.service.identity:latest
	docker push vladerlord/vladerlord.banking.service.identitynotifier:latest
	docker push vladerlord/vladerlord.banking.service.bankaccount:latest
	docker push vladerlord/vladerlord.banking.service.personaldata:latest
	docker push vladerlord/vladerlord.banking.gateway.root:latest
