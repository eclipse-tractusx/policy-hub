# Helm chart for Policy Hub

This helm chart installs the Catena-X Policy Hub application.

For further information please refer to [Technical Documentation](../../docs/technical-documentation/).

The referenced container images are for demonstration purposes only.

## Prerequisites

- [Kubernetes](https://kubernetes.io) 1.25.11+
- [Helm](https://helm.sh) 3.9.3+
- PV provisioner support in the underlying infrastructure

## Installation

To install the chart with the release name `policy-hub`:

```shell
$ helm repo add tractusx-dev https://eclipse-tractusx.github.io/charts/dev
$ helm install policy-hub tractusx-dev/policy-hub
```

To install the helm chart into your cluster with your values:

```shell
$ helm install -f your-values.yaml policy-hub tractusx-dev/policy-hub
```

To use the helm chart as a dependency:

```yaml
dependencies:
  - name: policy-hub
    repository: https://eclipse-tractusx.github.io/charts/dev
    version: 1.0.0
```

## Requirements

| Repository | Name | Version |
|------------|------|---------|
| https://charts.bitnami.com/bitnami | postgresql | 12.12.x |

## Values

| Key | Type | Default | Description |
|-----|------|---------|-------------|
| service.image.name | string | `"docker.io/tractusx/policy-hub-service"` |  |
| service.image.tag | string | `""` |  |
| service.imagePullPolicy | string | `"IfNotPresent"` |  |
| service.resources | object | `{"limits":{"cpu":"45m","memory":"300M"},"requests":{"cpu":"15m","memory":"300M"}}` | We recommend to review the default resource limits as this should a conscious choice. |
| service.logging.businessLogic | string | `"Information"` |  |
| service.logging.default | string | `"Information"` |  |
| service.healthChecks.startup.path | string | `"/health/startup"` |  |
| service.healthChecks.startup.tags[0].name | string | `"HEALTHCHECKS__0__TAGS__1"` |  |
| service.healthChecks.startup.tags[0].value | string | `"policyhubdb"` |  |
| service.healthChecks.liveness.path | string | `"/healthz"` |  |
| service.healthChecks.readyness.path | string | `"/ready"` |  |
| service.swaggerEnabled | bool | `false` |  |
| migrations.image.name | string | `"docker.io/tractusx/policy-hub-migrations"` |  |
| migrations.image.tag | string | `""` |  |
| migrations.imagePullPolicy | string | `"IfNotPresent"` |  |
| migrations.resources | object | `{"limits":{"cpu":"45m","memory":"105M"},"requests":{"cpu":"15m","memory":"105M"}}` | We recommend to review the default resource limits as this should a conscious choice. |
| migrations.seeding.testDataEnvironments | string | `""` |  |
| migrations.seeding.testDataPaths | string | `"Seeder/Data"` |  |
| migrations.logging.default | string | `"Information"` |  |
| dotnetEnvironment | string | `"Production"` |  |
| dbConnection.schema | string | `"hub"` |  |
| dbConnection.sslMode | string | `"Disable"` |  |
| postgresql.enabled | bool | `true` | PostgreSQL chart configuration; default configurations: host: "policy-hub-postgresql-primary", port: 5432; Switch to enable or disable the PostgreSQL helm chart. |
| postgresql.image | object | `{"tag":"15-debian-11"}` | Setting image tag to major to get latest minor updates |
| postgresql.commonLabels."app.kubernetes.io/version" | string | `"15"` |  |
| postgresql.auth.username | string | `"hub"` | Non-root username. |
| postgresql.auth.database | string | `"policy-hub"` | Database name. |
| postgresql.auth.existingSecret | string | `"{{ .Release.Name }}-phub-postgres"` | Secret containing the passwords for root usernames postgres and non-root username hub. Should not be changed without changing the "phub-postgresSecretName" template as well. |
| postgresql.auth.postgrespassword | string | `""` | Password for the root username 'postgres'. Secret-key 'postgres-password'. |
| postgresql.auth.password | string | `""` | Password for the non-root username 'hub'. Secret-key 'password'. |
| postgresql.auth.replicationPassword | string | `""` | Password for the non-root username 'repl_user'. Secret-key 'replication-password'. |
| postgresql.architecture | string | `"replication"` |  |
| postgresql.audit.pgAuditLog | string | `"write, ddl"` |  |
| postgresql.audit.logLinePrefix | string | `"%m %u %d "` |  |
| postgresql.primary.extendedConfiguration | string | `""` | Extended PostgreSQL Primary configuration (increase of max_connections recommended - default is 100) |
| postgresql.primary.initdb.scriptsConfigMap | string | `"{{ .Release.Name }}-phub-cm-postgres"` |  |
| postgresql.readReplicas.extendedConfiguration | string | `""` | Extended PostgreSQL read only replicas configuration (increase of max_connections recommended - default is 100) |
| externalDatabase.host | string | `"phub-postgres-ext"` | External PostgreSQL configuration IMPORTANT: non-root db user needs to be created beforehand on external database. And the init script (02-init-db.sql) available in templates/configmap-postgres-init.yaml needs to be executed beforehand. Database host ('-primary' is added as postfix). |
| externalDatabase.port | int | `5432` | Database port number. |
| externalDatabase.username | string | `"hub"` | Non-root username for policy-hub. |
| externalDatabase.database | string | `"policy-hub"` | Database name. |
| externalDatabase.password | string | `""` | Password for the non-root username (default 'hub'). Secret-key 'password'. |
| externalDatabase.existingSecret | string | `"policy-hub-external-db"` | Secret containing the password non-root username, (default 'hub'). |
| centralidp | object | `{"address":"https://centralidp.example.org","authRealm":"CX-Central","jwtBearerOptions":{"metadataPath":"/auth/realms/CX-Central/.well-known/openid-configuration","refreshInterval":"00:00:30","requireHttpsMetadata":"true","tokenValidationParameters":{"validAudience":"Cl23-CX-Policy-Hub","validIssuerPath":"/auth/realms/CX-Central"}},"tokenPath":"/auth/realms/CX-Central/protocol/openid-connect/token","useAuthTrail":true}` | Provide details about centralidp (CX IAM) Keycloak instance. |
| centralidp.address | string | `"https://centralidp.example.org"` | Provide centralidp base address (CX IAM), without trailing '/auth'. |
| centralidp.useAuthTrail | bool | `true` | Flag if the api should be used with an leading /auth path |
| ingress.enabled | bool | `false` | Policy Hub ingress parameters, enable ingress record generation for policy-hub. |
| ingress.tls | list | `[]` | Ingress TLS configuration |
| ingress.hosts[0] | object | `{"host":"","paths":[{"path":"/api/policy-hub","pathType":"Prefix"}]}` | Provide default path for the ingress record. |
| portContainer | int | `8080` |  |
| portService | int | `8080` |  |
| replicaCount | int | `3` |  |
| nodeSelector | object | `{}` | Node labels for pod assignment |
| tolerations | list | `[]` | Tolerations for pod assignment |
| affinity.podAntiAffinity | object | `{"preferredDuringSchedulingIgnoredDuringExecution":[{"podAffinityTerm":{"labelSelector":{"matchExpressions":[{"key":"app.kubernetes.io/name","operator":"DoesNotExist"}]},"topologyKey":"kubernetes.io/hostname"},"weight":100}]}` | Following Catena-X Helm Best Practices, [reference](https://kubernetes.io/docs/concepts/scheduling-eviction/assign-pod-node/#affinity-and-anti-affinity). |
| updateStrategy.type | string | `"RollingUpdate"` | Update strategy type, rolling update configuration parameters, [reference](https://kubernetes.io/docs/concepts/workloads/controllers/statefulset/#update-strategies). |
| updateStrategy.rollingUpdate.maxSurge | int | `1` |  |
| updateStrategy.rollingUpdate.maxUnavailable | int | `0` |  |
| startupProbe | object | `{"failureThreshold":30,"initialDelaySeconds":10,"periodSeconds":10,"successThreshold":1,"timeoutSeconds":1}` | Following Catena-X Helm Best Practices, [reference](https://github.com/helm/charts/blob/master/stable/nginx-ingress/values.yaml#L210). |
| livenessProbe.failureThreshold | int | `3` |  |
| livenessProbe.initialDelaySeconds | int | `10` |  |
| livenessProbe.periodSeconds | int | `10` |  |
| livenessProbe.successThreshold | int | `1` |  |
| livenessProbe.timeoutSeconds | int | `10` |  |
| readinessProbe.failureThreshold | int | `3` |  |
| readinessProbe.initialDelaySeconds | int | `10` |  |
| readinessProbe.periodSeconds | int | `10` |  |
| readinessProbe.successThreshold | int | `1` |  |
| readinessProbe.timeoutSeconds | int | `1` |  |

Autogenerated with [helm docs](https://github.com/norwoodj/helm-docs)
