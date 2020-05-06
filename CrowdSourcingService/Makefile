IMAGE_NAME=
GIT_COMMIT=$(shell git rev-parse --short=8 HEAD)
DOCKER_NETWORK_NAME=providenceV1Network
MYSQL_CONTAINER_NAME=providence_data
SQL_DUMP_FILE=/path/to/your/dump.sql
DB_NAME=providence_comparison_class2

.PHONY: all
all: build_image create_docker_network run_image

.PHONY: build_image 
build_image:
	docker build . -f CrowdSourcingService/Dockerfile -t $(IMAGE_NAME):$(GIT_COMMIT)

.PHONY: push_image
push_image:
	docker push $(IMAGE_NAME):$(GIT_COMMIT)

.PHONY: format_sql_dump
format_sql_dump:
	@if [ ! -z "${SQL_DUMP_FILE}" ]; then \
		sed -i 's/COLLATE=utf8mb4_0900_ai_ci//' "${SQL_DUMP_FILE}"; \
	fi

.PHONY: run_sql_image
run_sql_image:
	docker run \
		--mount type=bind,source="${SQL_DUMP_FILE}",target=/docker-entrypoint-initdb.d/dump.sql \
		--rm \
		--network ${DOCKER_NETWORK_NAME} \
		--name ${MYSQL_CONTAINER_NAME} \
		-e MYSQL_ROOT_PASSWORD=$(shell cat data/db_password_root.env) \
		-e MYSQL_DATABASE=${DB_NAME} \
		-e MYSQL_USER=$(shell cat data/db_user_name.env) \
		-e MYSQL_PASSWORD=$(shell cat data/db_user_password.env) \
		-p 48256:3306 \
		mysql:5.7 \
			--max-allowed-packet=12884616 \
			--ssl=TRUE

.PHONY: run_image
run_image:
	docker run \
		--rm \
		-it \
		--network=annotationNetwork \
		--mount type=bind,src=$(shell pwd)/data/certificate.pfx,target=/https/certificate.pfx \
		-p 8000:80 \
		-p 8001:443 \
		-e ASPNETCORE_URLS="https://+;http://+" \
		-e ASPNETCORE_HTTPS_PORT=8001 \
		-e ASPNETCORE_Kestrel__Certificates__Default__Password="$(shell cat data/cert_passphrase.env)" \
		-e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/certificate.pfx \
		-e CHILDES_SQL_DB_CONNECTION_STRING="$(shell cat data/db_connection_string.env)" \
		$(IMAGE_NAME):$(GIT_COMMIT)
		

.PHONY: create_docker_network
create_docker_network:
	docker network create -d bridge ${DOCKER_NETWORK_NAME}

cert.pem:
	openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365

certificate.pfx:
	openssl pkcs12 -export -out certificate.pfx -inkey key.pem -in cert.pem